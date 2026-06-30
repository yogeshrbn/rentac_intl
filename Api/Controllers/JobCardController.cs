using BAL.DTO;
using BAL.Objects;
using BAL.Services;
using BAL.Services.Contracts;
using FarmaAPI.Helper;
using Microsoft.Ajax.Utilities;
using NLog;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class JobCardController : BaseApiController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly INotificationSenderService _notificationSenderService;

        public JobCardController(INotificationSenderService notificationSenderService)
        {
            _notificationSenderService = notificationSenderService;
        }

        [HttpPost]
        public async Task<ApiMessage> Save([FromBody] JobCardDto dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                var service = new JobCard();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;

                dto.GuId = Guid.NewGuid().ToString();

                msg.Data = await service.Save(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                logger.Error(ex);
            }
            return msg;
        }


        [HttpPost]
        public async Task<ApiMessage> GetList([FromBody] JobCardDto dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                var service = new JobCard();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;

                msg.Data = await service.GetList(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
                logger.Error(ex);
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> JobCardDelChallanItems([FromBody] JobCardDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                var jobCard = new JobCard();
                jobCard.InjectFrom(dto);

                dto.CompanyId = user.DefaultCompanyId;

                var data = await jobCard.JobCardChallanItems(dto.JobCardId, dto.CompanyId);

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

      


        [HttpPost]
        public async Task<ApiMessage> JobCardRetChallanItems([FromBody] JobCardDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                var jobCard = new JobCard();
                jobCard.InjectFrom(dto);

                dto.CompanyId = user.DefaultCompanyId;

                var data = await jobCard.JobCardReturnChallanItems(dto.JobCardId, dto.CompanyId);

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
       

        [HttpPost]
        public async Task<ApiMessage> UpdateStatus([FromBody] JobCardDto dto)
        {
            ApiMessage response = new ApiMessage();
            try
            {

                var user = new LoggedInUser();
                var jobCard = new JobCard();
                jobCard.InjectFrom(dto);

                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                var data = await jobCard.UpdateStatus(dto);
                var jb = new JobCard();
                var jbDetails = await jb.GetById(dto);

                if (data > 0)
                {
                    try
                    {
                        await TrySaveInstallActivityContractDocumentAsync(dto, jbDetails, user);
                    }
                    catch (Exception docEx)
                    {
                        logger.Error(docEx, "Failed to save contract document for install activity (JobCardId={0})", dto.JobCardId);
                    }
                }

                if (data > 0 && dto.StatusId == 2)
                {
                    var token = Request?.Headers?.Contains("Authorization") == true
                        ? Request.Headers.GetValues("Authorization").FirstOrDefault()
                        : null;
                    var xCompanyId = Request?.Headers?.Contains("x-companyId") == true
                        ? Request.Headers.GetValues("x-companyId").FirstOrDefault()
                        : dto.CompanyId.ToString();
                    string apiKey = Company.GetRentacApiKeyByCompanyId(dto.CompanyId);
                    try
                    {
                        if (!String.IsNullOrEmpty(apiKey))
                        {
                            var queuePublisher = new ServiceBusQueuePublisher();
                            await queuePublisher.PublishContractReminderAsync(new NotificationDto
                            {

                                CompanyId = dto.CompanyId,
                                StatusId = dto.StatusId,
                                ModifiedBy = dto.ModifiedBy,
                                ModifiedOn = dto.ModifiedOn,
                                Token = token,
                                RentacApiKey = apiKey,
                                Type = "whatsapp",
                                Receipients = "9811553130",
                                MetaData = "1104," + jbDetails.JobCardTypeKey


                            }, "contractreminders");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Failed to publish contract reminder queue message for JobCardId {0}", dto.JobCardId);
                    }
                }

                response.Data = data;
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        private const string InstallActivityImageDocType = "InstallActivityImage";

        private static bool LooksLikeImageContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return false;
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        private async Task TrySaveInstallActivityContractDocumentAsync(JobCardDto dto, JobCardDto jbDetails, LoggedInUser user)
        {
            var raw = dto.ActivityImageDocument;
            if (string.IsNullOrWhiteSpace(raw))
                return;
            if (!string.Equals(dto.Activity, "INSTALL", StringComparison.OrdinalIgnoreCase))
                return;
            if (jbDetails == null || !string.Equals(jbDetails.JobCardType, "contract", StringComparison.OrdinalIgnoreCase) || jbDetails.JobCardTypeKey <= 0)
                return;

            var file = new DataUrlHelper();
            if (!file.IsDataUrl(raw))
                return;
            if (!LooksLikeImageContentType(file.ContentType))
                throw new Exception("Only image files are allowed for install activity documents.");

            if (file.FileStream != null && file.FileStream.CanSeek)
                file.FileStream.Position = 0;

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await file.FileStream.CopyToAsync(ms).ConfigureAwait(false);
                bytes = ms.ToArray();
            }
            if (bytes == null || bytes.Length == 0)
                return;

            var ext = file.Extension;
            if (string.IsNullOrWhiteSpace(ext))
                ext = "bin";

            var blobRelativePath = user.FinYearId + "/" + user.DefaultCompanyId + "/contractdocs/" + Guid.NewGuid().ToString("N") + "." + ext;

            var azService = new AzureStorageService();
            if (!await azService.UploadFileAsync(blobRelativePath, bytes).ConfigureAwait(false))
                throw new Exception("Failed to upload install activity image to storage.");

            var contractSvc = new Contract();
            await contractSvc.InsertContractDocumentAsync(new ContractDocumentDto
            {
                CompanyId = dto.CompanyId,
                ContractId = jbDetails.JobCardTypeKey,
                JobCardId = dto.JobCardId,
                DocumentType = InstallActivityImageDocType,
                StoragePath = blobRelativePath,
                OriginalFileName = null,
                ContentType = file.ContentType,
                CreatedBy = user.UserId,
                CreatedOn = DateTime.UtcNow
            }).ConfigureAwait(false);
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;
using BAL.Common;
using Omu.ValueInjecter;
using System.Threading.Tasks;
using System.Windows.Interop;
using BAL.Exceptions;
using BAL.Services.Integrations;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class SettingsController : ApiController
    {

        /// <summary>
        /// Adds a new accountgroup
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage SetDefaultCompany([FromBody] UserDTO dto)
        {
            User objUser = new User();
            return Request.CreateResponse(HttpStatusCode.OK, objUser.SetDefaultCompany(new LoggedInUser().UserId, dto.DefaultCompanyId));
        }


        [HttpPost]
        public async Task<ApiMessage> UpdateIRPCredentials([FromBody] CompanyDTO obj)
        {
            Company objCompany = new Company();
            var message = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Invalid input";
                    return message;
                }
                var user = new LoggedInUser();
                obj.CompanyId = user.DefaultCompanyId;
                obj.IRPUpdatedOn = DateTime.Now;
                obj.IRPUpdatedBy = user.UserId;
                var result = objCompany.UpdateIRPCredentials(obj);
                if (result)
                {
                    var token = await GenerateIRPToken(obj);
                    message.Code = ApiMessageCodes.SUCCESS;
                }
                else
                {
                    message.Code = ApiMessageCodes.COULD_NOT_SUCCEED;

                }
                return message;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }

            return message;

        }
        [HttpPost]
        public async Task<ApiMessage> GenerateIRPToken([FromBody] CompanyDTO obj)
        {
            Company objCompany = new Company(obj.CompanyId);
            var compDto = objCompany.GetDetails();
            var message = new ApiMessage();
            try
            {
                if (obj == null)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Invalid input";
                    return message;
                }
                var user = new LoggedInUser();
                IRPRequestParams requestParams = new IRPRequestParams();
                requestParams.GSTIN = compDto.GSTNo;
                requestParams.Username = compDto.IRPUserName;
                requestParams.Password = compDto.IRPPassword;
                var eService = new EInvoiceService();

                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                var token = await eService.Authenticate(requestParams, userInfo);
                if (token != null)
                {
                    message.Data = token;
                    message.Code = ApiMessageCodes.SUCCESS;
                }
                else
                {
                    message.Code = ApiMessageCodes.COULD_NOT_SUCCEED;

                }
                return message;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }

            return message;

        }

        [HttpPost]
        public async Task<ApiMessage> UpdateEwayBillCreds([FromBody] CompanyDTO dto)
        {
            var message = new ApiMessage();

            //   leder.StoreId = 1;
            try
            {

                var user = new LoggedInUser();
                if (String.IsNullOrEmpty(dto.EwayUserName) || String.IsNullOrEmpty(dto.EwayPassword))
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Please provide username and password";
                    return message;
                }

                dto.CompanyId = user.DefaultCompanyId;

                var ewayBIllservice = new EwayBillService();
                var comp = new Company(dto.CompanyId);
                var cdto = comp.GetDetails();
                var userInfo = new LoggedInUserInfo();
                userInfo.InjectFrom(user);
                dto.GSTNo = cdto.GSTNo;
                var authenticated = await ewayBIllservice.Authenticate(dto, userInfo);

                if (!authenticated)
                {
                    message.Message = "Credentials not validated";
                    message.Code = ApiMessageCodes.ERROR;
                    return message;
                }
                //  Company comp = new Company(dto.CompanyId);
                bool credUpdated = comp.UpdateEwayBillCreds(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (UDFException ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #region WhatsAppIntegrations

        [HttpPost]
        public async Task<ApiMessage> CreateWhatsappApp([FromBody] WhatsappDTO dto)
        {
            var message = new ApiMessage();

            //   leder.StoreId = 1;
            try
            {

                var user = new LoggedInUser();
                var wsService = new WhatsappService();
                dto.CreatedBy = user.UserId;
                dto.CompanyId = user.DefaultCompanyId;
                dto.RbnClientId = user.RbnClientId;
                dto.CreatedOn = DateTime.Now;
                dto.GuId = Guid.NewGuid().ToString();
                message.Data = await wsService.CreateApp(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (UDFException ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> ListWhatsApps()
        {
            var message = new ApiMessage();

            try
            {

                var user = new LoggedInUser();
                var wsService = new WhatsappService();

                message.Data = await wsService.ListApps(user.DefaultCompanyId, user.RbnClientId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (UDFException ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> ActivateForCallback([FromBody] WhatsappDTO dto)
        {
            var message = new ApiMessage();

            try
            {

                var user = new LoggedInUser();
                var wsService = new WhatsappService();
                if (String.IsNullOrEmpty(dto.App_Id))
                {
                    message.Message = "App Id is invalid or incorrect";
                    message.Code = ApiMessageCodes.ERROR;
                    return message;
                }
                var result = await wsService.SetCallback(dto);

                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (UDFException ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> GenerateSignupLink([FromBody] WhatsappDTO dto)
        {
            var message = new ApiMessage();

            try
            {

                var user = new LoggedInUser();
                var wsService = new WhatsappService();
                if (String.IsNullOrEmpty(dto.App_Id) || String.IsNullOrEmpty(dto.Name))
                {
                    message.Message = "App Id or Name is invalid or incorrect";
                    message.Code = ApiMessageCodes.ERROR;
                    return message;
                }
                var result = await wsService.UpdateEmbedLink(dto);

                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (UDFException ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> RefreshAppDetails([FromBody] WhatsappDTO dto)
        {
            var message = new ApiMessage();

            try
            {

                var user = new LoggedInUser();
                var wsService = new WhatsappService();
                if (String.IsNullOrEmpty(dto.App_Id) || String.IsNullOrEmpty(dto.Name))
                {
                    message.Message = "App Id or Name is invalid or incorrect";
                    message.Code = ApiMessageCodes.ERROR;
                    return message;
                }
                var result = await wsService.RefreshAppDetails(dto);

                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (UDFException ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class TaxController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public HttpResponseMessage GetChallanTaxes([FromBody] ChallanDTO obj)
        {
            Tax objTax = new Tax();
            List<TaxDTO> taxes = objTax.GetApplicableTaxes(BAL.Enums.TaxItem.Product, obj.ChallanId);
            return Request.CreateResponse(HttpStatusCode.OK, taxes);
        }



        [HttpPost]
        public HttpResponseMessage GetApplicableTaxes([FromBody] TaxDTO dto)
        {
            Tax objTax = new Tax();
            List<TaxDTO> taxes = objTax.GetApplicableTaxes(dto.ItemId, dto.ItemValue);
            return Request.CreateResponse(HttpStatusCode.OK, taxes);
        }

        [HttpPost]
        public HttpResponseMessage SaveTax()
        {
            string dto = System.Web.HttpContext.Current.Request["dto"];


            JObject jsonObject = new JObject();

            BillingDTO billingDTO = new BillingDTO();
            var jArray = JArray.Parse(dto);
            List<TaxDTO> lstTaxes = new List<TaxDTO>();
            foreach (var item in jArray)
            {
                TaxDTO taxDTO = new TaxDTO();
                taxDTO.ItemId = (BAL.Enums.TaxItem)Enum.ToObject(typeof(BAL.Enums.TaxItem), Convert.ToInt16(item["ItemId"]));
                taxDTO.ItemValue = Convert.ToInt16(item["ItemValue"]);
                taxDTO.TaxId = Convert.ToInt16(item["TaxId"]);
                taxDTO.Rate = Convert.ToDouble(item["Rate"]);
                taxDTO.ApplicableTaxId = Convert.ToInt16(item["ApplicableTaxId"]);
                taxDTO.ModifiedBy = new LoggedInUser().UserId;

                lstTaxes.Add(taxDTO);

            }

            Tax objTax = new Tax();
            bool result = objTax.Save(lstTaxes);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        [HttpPost]
        public HttpResponseMessage GetAllTaxes([FromBody] TaxDTO obj)
        {
            Tax objTax = new Tax();

            List<TaxDTO> lst = objTax.GetAllTaxes(new LoggedInUser().DefaultCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, lst);
        }
        [HttpGet]
        public async Task<ApiMessage> TaxCategories()
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Tax objTax = new Tax();

                msg.Data = await objTax.GetAllTaxCategories();
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;

        }

        [HttpPost]
        public async Task<ApiMessage> SaveTaxCategory([FromBody] TaxCategoryDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;

                if (dto.TaxCategoryId == 0)
                {
                    dto.CreatedBy = user.UserId;
                    dto.CreatedOn = DateTime.Now;
                }
                else
                {
                    dto.ModifiedBy = user.UserId;
                    dto.ModifiedOn = DateTime.Now;
                }

                Tax objTax = new Tax();
                msg.Data = await objTax.SaveTaxCategory(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> TaxCategoryById([FromBody] TaxCategoryDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Tax objTax = new Tax();
                msg.Data = await objTax.GetTaxCategoryById(dto.TaxCategoryId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> DeleteTaxCategory([FromBody] TaxCategoryDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                Tax objTax = new Tax();
                await objTax.DeleteTaxCategory(dto.TaxCategoryId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }

        [HttpGet]
        public async Task<ApiMessage> TaxMasters()
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                Tax objTax = new Tax();
                msg.Data = await objTax.GetAllTaxMasters(user.DefaultCompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> SaveTaxMaster([FromBody] TaxMasterDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                if (dto.Id == 0)
                {
                    dto.CreatedBy = user.FullName;
                    dto.CreatedDate = DateTime.Now;
                    if (dto.EffectiveFrom == DateTime.MinValue)
                        dto.EffectiveFrom = DateTime.Today;
                    if (string.IsNullOrEmpty(dto.RateType))
                        dto.RateType = "Percentage";
                }
                else
                {
                    dto.ModifiedBy = user.FullName;
                    dto.ModifiedDate = DateTime.Now;
                }
                dto.CompanyId = user.DefaultCompanyId;
              
                Tax objTax = new Tax();
                msg.Data = await objTax.SaveTaxMaster(dto);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }

        [HttpPost]
        public async Task<ApiMessage> TaxMasterById([FromBody] TaxMasterDTO dto)
        {
            ApiMessage msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                Tax objTax = new Tax();
                msg.Data = await objTax.GetTaxMasterById(dto.Id, user.DefaultCompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return msg;
        }
    }
}

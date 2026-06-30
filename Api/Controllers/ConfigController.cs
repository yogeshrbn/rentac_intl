using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
using FarmaAPI.Helper;
namespace FarmaAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Config")]
    public class ConfigController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage SetBillNo([FromBody] List<ConfigDTO> lst)
        {
            ApiMessage response = new ApiMessage();

            try
            {

                Config config = new Config();
                foreach (ConfigDTO dto in lst)
                {
                    dto.CompanyId = new LoggedInUser().DefaultCompanyId;
                    config.AddConfig(dto);
                }
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [HttpPost]
        public HttpResponseMessage GetBillingConfig()
        {
            Config config = new Config();
            ApiMessage message = new ApiMessage();
            try
            {
                message.Data = config.GetBillingConfig(new LoggedInUser().DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpGet]
        [Route("getConfig/{category}/{subCategory}")]
        public HttpResponseMessage GetConfig(string category, string subCategory)
        {
            Config config = new Config();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var configs = config.GetAll(user.DefaultCompanyId);
                if (configs != null)
                {
                    message.Data = configs.Where(o => o.Category.ToLower() == category.ToLower() && o.SubCategory == subCategory).ToList();
                }
                message.Code = ApiMessageCodes.SUCCESS;
                message.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpGet]
        public HttpResponseMessage GetConfig(string category)
        {
            Config config = new Config();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var configs = config.GetAll(user.DefaultCompanyId);
                if (configs != null)
                {

                    message.Data = configs.Where(o => o.Category.ToLower() == category.ToLower()).ToList();

                }
                message.Code = ApiMessageCodes.SUCCESS;
                message.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }


        [HttpGet]
        public HttpResponseMessage GetAllConfig()
        {
            Config config = new Config();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var configs = config.GetAll(user.DefaultCompanyId);

                message.Data = configs;

                message.Code = ApiMessageCodes.SUCCESS;
                message.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public async Task<HttpResponseMessage> SaveConfig([FromBody] List<ConfigDTO> configDto)
        {
            Config config = new Config();
            ApiMessage message = new ApiMessage();
            try
            {
                if (configDto == null)
                {
                    throw new Exception("Empty configuration data");
                }
                var user = new LoggedInUser();
                foreach (var c in configDto)
                {
                    c.CompanyId = user.DefaultCompanyId;                   
                }
                message.Data = await config.AddConfigAsync(configDto);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Message = ApiMessage.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
    }
}

using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class RoleCostingController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> List()
        {
            var message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var service = new RoleCosting();
                message.Data = await service.List(user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ById([FromBody] RoleCostingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var service = new RoleCosting();
                message.Data = await service.ById(dto.RoleCostingId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Save([FromBody] RoleCostingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var service = new RoleCosting();
                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                if (dto.RoleCostingId == 0)
                {
                    dto.CreatedBy = user.UserId;
                    dto.CreatedOn = DateTime.Now;
                    dto.GuId = Guid.NewGuid().ToString();
                }
                message.Data = await service.Save(dto);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Delete([FromBody] RoleCostingDTO dto)
        {
            var message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var service = new RoleCosting();
                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                message.Data = await service.Delete(dto);
                message.Code = ApiMessageCodes.SUCCESS;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using BAL.Enums;
using FarmaAPI.Helper;
using NLog;
using BAL.Exceptions;
using System.Threading.Tasks;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class EmployeeController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public HttpResponseMessage Add([FromBody] EmployeeDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Employee emp = new Employee();
                emp.Name = dto.Name;
                if (String.IsNullOrEmpty(dto.Phone))
                {
                    throw new Exception("Empty or invalid phone number supplied");
                }
                emp.Phone = dto.Phone;
                emp.Address = dto.Address;
                emp.Aadhar = dto.Aadhar;
                emp.RoleId = dto.RoleId;
                emp.EmployeeCode = dto.EmployeeCode;
                emp.EmployeeId = dto.EmployeeId;
                emp.CompanyId = new LoggedInUser().DefaultCompanyId;
                emp.Save();
                message.Code = ApiMessageCodes.SUCCESS;

                //logger.Error("message", "EmployeeController->Save");
            }
            catch (UDFException ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                return Request.CreateResponse(HttpStatusCode.OK, message);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "EmployeeController->Save");
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetAll()
        {

            ApiMessage message = new ApiMessage();
            try
            {
                logger.Debug("EmployeeController->GetAll");
                Employee emp = new Employee();
                message.Data = emp.GetAll(new LoggedInUser().DefaultCompanyId);
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
        public HttpResponseMessage GetById([FromBody] EmployeeDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Employee emp = new Employee(dto.EmployeeId);
                message.Data = emp;
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
        public async Task<HttpResponseMessage> SaveTeam([FromBody] TeamDto dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Employee emp = new Employee();
                var user = new LoggedInUser();
                dto.CreatedBy = user.UserId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.CompanyId = user.DefaultCompanyId;
                dto.GuId = Guid.NewGuid().ToString();
                message.Data = await emp.SaveTeam(dto);
                message.Code = ApiMessageCodes.SUCCESS;

            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);

        }

        [HttpGet]
        public async Task<HttpResponseMessage> TeamList()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Employee emp = new Employee();
                TeamDto dto = new TeamDto();
                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;

                message.Data = await emp.TeamList(dto);
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
        public async Task<HttpResponseMessage> DeleteTeam([FromBody] TeamDto dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Employee emp = new Employee();

                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                message.Data = await emp.DeleteTeam(dto);
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
        public async Task<HttpResponseMessage> TeamById([FromBody] TeamDto dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Employee emp = new Employee();

                var user = new LoggedInUser();

                dto.CompanyId = user.DefaultCompanyId;

                message.Data = await emp.TeamById(dto);
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

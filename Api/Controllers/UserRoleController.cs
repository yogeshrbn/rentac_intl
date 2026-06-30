using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;
namespace FarmaAPI.Controllers
{
     [Authorize]
    public class UserRoleController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage GetRoleFunctions([FromBody] RoleDTO dto)
        {
            ApiMessage mssage = new ApiMessage();
            try
            {
                UserRole objUser = new UserRole();
                mssage.Data = objUser.RoleFunctions(dto.RoleId, new LoggedInUser().DefaultCompanyId);
                mssage.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                mssage.Message = ex.Message;
                mssage.Description = ex.StackTrace;
                mssage.Code = ApiMessageCodes.ERROR;
            }
            return Request.CreateResponse(HttpStatusCode.OK, mssage);
        }
        [HttpPost]
        public HttpResponseMessage AddRoleFunction([FromBody] RoleDTO dto)
        {
            ApiMessage mssage = new ApiMessage();
            try
            {
                UserRole objUser = new UserRole();
                mssage.Data = objUser.AddRoleFunction(dto.RoleId, new LoggedInUser().DefaultCompanyId, dto.Functions.FirstOrDefault());
                mssage.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                mssage.Message = ex.Message;
                mssage.Description = ex.StackTrace;
                mssage.Code = ApiMessageCodes.ERROR;
            }
            return Request.CreateResponse(HttpStatusCode.OK, mssage);
        }

        
        
    }
}

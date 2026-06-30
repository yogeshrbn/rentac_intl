using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
using FarmaAPI.Helper;
using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace FarmaAPI.Controllers
{
     [Authorize]
    public class LookupController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage GetAllEmployeeRoles()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                LookupData data = new LookupData();
                message.Data = data.GetAllRoles(RoleType.Employee);
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
        public HttpResponseMessage GetAllSystemRoles()
        {
            ApiMessage message = new ApiMessage();
            try
            {
               
                LookupData data = new LookupData();
                message.Data = data.GetAllRoles(RoleType.User);
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
        public HttpResponseMessage GetOtherCharges()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                LookupData data = new LookupData();
                message.Data = data.GetOtherCharges();
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
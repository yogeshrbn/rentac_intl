using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;

using System.Web.Script.Serialization;
using BAL.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Runtime;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class TransporterController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Save([FromBody] TransporterDTO obj)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                TransporterService objService = new TransporterService();
                obj.CreatedBy = user.UserId;
                obj.CompanyId = user.DefaultCompanyId;
                obj.CreatedOn = DateTime.Now;
                msg.Code = ApiMessageCodes.SUCCESS;

                objService.Save(obj);

                return Request.CreateResponse(HttpStatusCode.OK, msg);
            }
            catch (UDFException ex)
            {
                msg.Message = ex.Message;
                msg.Code = ApiMessageCodes.ERROR;
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = "unknown error occurred, please try again in sometime";
                //msg.Code = ex.ErrorCode.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, msg);

            }
        }

        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            var user = new LoggedInUser();
            TransporterService objService = new TransporterService();
            var list = objService.GetAll(new TransporterDTO { CompanyId=user.DefaultCompanyId });


            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

    
    }
}

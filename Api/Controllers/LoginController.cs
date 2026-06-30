using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
namespace FarmaAPI.Controllers
{   
    public class LoginController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetFinYearList()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, Config.GetFinYearList());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage Register()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, Config.GetFinYearList());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

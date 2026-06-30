using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    [RoutePrefix("api/template")]
    public class TemplateController : ApiController
    {

       
        [Route("byGroup/{groupName}")]
        [HttpGet]
        public async Task<IHttpActionResult> ByGroup(string groupName)
        {
            ApiMessage response = new ApiMessage();
            try
            {
                var templateService = new TemplateService();
                var user = new LoggedInUser();


                var data = await templateService.GetByByGroup(groupName);

                response.Data = data.ToList();
                response.Code = ApiMessageCodes.SUCCESS;
                response.Message = ApiMessage.SUCCESS;

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Message = ex.Message;

            }
            return Ok(response);
        }
    }
}

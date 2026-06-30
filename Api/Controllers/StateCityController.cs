using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class StateCityController : ApiController
    {
        /// <summary>
        ///Select states
        /// </summary>
        /// <returns>States</returns>
        [HttpPost]
        public HttpResponseMessage GetAllStates()
        {
            return Request.CreateResponse(HttpStatusCode.OK, State.GetAllStates());
        }
        /// <summary>
        ///Select Cities
        /// </summary>
        /// <param name="dto">StateDTO</param>
        /// <returns>Cities</returns>
        [HttpPost]
        public HttpResponseMessage GetCities([FromBody] StateDTO dto)
        {
            return Request.CreateResponse(HttpStatusCode.OK, State.GetCites(dto.StateId));
        }
    }
}

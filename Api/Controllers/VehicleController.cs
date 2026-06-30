using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;
using BAL.Exceptions;
namespace FarmaAPI.Controllers
{
     [Authorize]
    public class VehicleController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Add([FromBody] VehicleDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Vehicle vehicle = new Vehicle();
                vehicle.ChachisNumber = dto.ChachisNumber;
                vehicle.CompanyId = new LoggedInUser().DefaultCompanyId;
                vehicle.EngineNumber = dto.EngineNumber;
                vehicle.RegNumber = dto.RegNumber;
                vehicle.Name = dto.Name;
                vehicle.VehicleId = dto.VehicleId;
                vehicle.Add();
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (UDFException ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                return Request.CreateResponse(HttpStatusCode.OK, message);

            }
            catch (Exception ex)
            {
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
                Vehicle vehicle = new Vehicle();
                message.Data = vehicle.GetAll(new LoggedInUser().DefaultCompanyId);
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
        public HttpResponseMessage GetById([FromBody] VehicleDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Vehicle vehicle = new Vehicle(dto.VehicleId);
                message.Data = vehicle;
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

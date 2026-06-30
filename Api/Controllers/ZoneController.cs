using BAL.DTO;
using BAL.Services;
using FarmaAPI.Helper;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class ZoneController : ApiController
    {
        private ZoneService zoneService;
        public ZoneController()
        {
            zoneService = new ZoneService();
        }
        [HttpPost]
        public async Task<IHttpActionResult> Save([FromBody] ZonesDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                dto.InjectFrom(user);
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedOn = DateTime.Now;
                dto.CreatedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.GuId = Guid.NewGuid().ToString();

                res.Data = await zoneService.Save(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ZoneList()
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                ZonesDTO dto = new ZonesDTO();
                dto.InjectFrom(user);
                dto.CompanyId = user.DefaultCompanyId;
                res.Data = await zoneService.ZonesList(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ZoneById([FromBody] ZonesDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                if (dto.ZoneId <= 0)
                {

                    res.Code = ApiMessageCodes.SUCCESS;
                    return Ok(res);
                }
                dto.InjectFrom(user);
                dto.CompanyId = user.DefaultCompanyId;
                res.Data = await zoneService.ZoneById(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteZone([FromBody] ZonesDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                dto.InjectFrom(user);
                res.Data = await zoneService.DeleteZone(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.Code = ApiMessageCodes.ERROR;
                res.Message = ex.Message;
                return Ok(res);
            }
        }
    }
}

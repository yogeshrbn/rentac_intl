using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class WarehouseController : ApiController
    {
        public WarehouseController() { }

        private static Logger logger = LogManager.GetCurrentClassLogger();


        [HttpPost]
        public async Task<IHttpActionResult> Save([FromBody] WarehouseDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var wh = new Warehouse();
                var user = new LoggedInUser();
                if (dto == null)
                {
                    throw new Exception("Input is null");
                }
                dto.CreatedBy = user.UserId;
                dto.RbnClientId = user.RbnClientId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;

                if (dto.WarehouseId == 0)
                {
                    res.Data = await wh.CreateWareHouse(dto);
                }
                if (dto.WarehouseId > 0)
                {
                    res.Data = await wh.UpdateWareHouse(dto);
                }
                res.Code = ApiMessageCodes.SUCCESS;

                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                WarehouseDTO dto = new WarehouseDTO();
                dto.RbnClientId = user.RbnClientId;

                var ws = new Warehouse();
                res.Data = await ws.GetAll(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }
        [HttpPost]
        public async Task<IHttpActionResult> GetById([FromBody] WarehouseDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                if (dto == null)
                {
                    throw new Exception("Input is null");
                }
                if(dto.WarehouseId == 0)
                {
                    throw new Exception("Warehouse Id is 0");

                }
                dto.RbnClientId = user.RbnClientId;

                var ws = new Warehouse();
                res.Data = await ws.GetById(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatus([FromBody] WarehouseDTO dto)
        {
            var res = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                if (dto == null)
                {
                    throw new Exception("Input is null");
                }
                if (dto.WarehouseId == 0)
                {
                    throw new Exception("Warehouse Id is 0");

                }
                dto.RbnClientId = user.RbnClientId;
                dto.ModifiedBy = user.UserId;
                dto.ModifiedOn = DateTime.Now;
                var ws = new Warehouse();
                res.Data = await ws.UpdateStatus(dto);
                res.Code = ApiMessageCodes.SUCCESS;
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }
    }
}

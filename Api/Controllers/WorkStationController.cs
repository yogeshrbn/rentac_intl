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
    [Authorize]
    public class WorkStationController : BaseApiController
    {
        #region WorkstationTypes
        [HttpPost]
        public async Task<ApiMessage> SaveWorkStationType([FromBody] WorkStationTypeDto dto)
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();
                var user = new LoggedInUser();
                dto.CreatedBy = user.UserId;
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.GuId = Guid.NewGuid().ToString();
                message.Data = await workStation.SaveWorkStationType(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> GetWorkStationTypeList()
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();

                var user = new LoggedInUser();
                message.Data = await workStation.GetTypeList(user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> GetWorkSationTypeDetails([FromBody] WorkStationTypeDto dto)
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();
                var user = new LoggedInUser();
                message.Data = await workStation.GetTypeById(dto.WorkStationTypeId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        #endregion
        #region Workstation
        [HttpPost]
        public async Task<ApiMessage> SaveWorkStation([FromBody] WorkStationDto dto)
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();
                var user = new LoggedInUser();
                dto.CreatedBy = user.UserId;
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.GuId = Guid.NewGuid().ToString();
                message.Data = await workStation.SaveWorkStation(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> GetWorkStationList()
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();

                var user = new LoggedInUser();
                message.Data = await workStation.GetWorkStationList(user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> GetWorkSationDetails([FromBody] WorkStationDto dto)
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();
                var user = new LoggedInUser();
                message.Data = await workStation.GetWorkStationById(dto.WorkStationId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        #endregion

        #region Operation
        [HttpPost]
        public async Task<ApiMessage> SaveOperation([FromBody] OperationDto dto)
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();
                var user = new LoggedInUser();
                dto.CreatedBy = user.UserId;
                dto.CompanyId = user.DefaultCompanyId;
                dto.CreatedOn = DateTime.Now;
                dto.ModifiedOn = DateTime.Now;
                dto.ModifiedBy = user.UserId;
                dto.GuId = Guid.NewGuid().ToString();
                message.Data = await workStation.SaveOperation(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ApiMessage> GetOperations()
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();

                var user = new LoggedInUser();
                message.Data = await workStation.GetOperations(user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> GetOperation([FromBody] OperationDto dto)
        {
            var message = new ApiMessage();
            try
            {
                var workStation = new WorkStationService();
                var user = new LoggedInUser();
                message.Data = await workStation.GetOperation(dto.OperationId, user.DefaultCompanyId);
                message.Code = ApiMessageCodes.SUCCESS;
                return message;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                throw ex;
            }
        }
        #endregion
    }
}

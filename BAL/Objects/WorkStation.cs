using BAL.DAL;
using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Objects
{
    public class WorkStationService
    {
        #region WorkStationType
        public async Task<bool> SaveWorkStationType(WorkStationTypeDto dto)
        {
            var dal = new WorkStationDAL();
            return await dal.SaveWorkStationType(dto);
        }
        public async Task<IEnumerable<WorkStationTypeDto>> GetTypeList(int companyId)
        {
            var dal = new WorkStationDAL();
            return await dal.GetTypeList(companyId);
        }
        public async Task<WorkStationTypeDto> GetTypeById(int workStationTypeId, int companyId)
        {
            var dal = new WorkStationDAL();
            return await dal.GetTypeById(workStationTypeId, companyId);
        }
        #endregion

        #region WorkStation
        public async Task<bool> SaveWorkStation(WorkStationDto dto)
        {
            var dal = new WorkStationDAL();
            return await dal.SaveWorkStation(dto);
        }
        public async Task<IEnumerable<WorkStationDto>> GetWorkStationList(int companyId)
        {
            var dal = new WorkStationDAL();
            return await dal.GetWorkStationList(companyId);
        }
        public async Task<WorkStationDto> GetWorkStationById(int workStationId, int companyId)
        {
            var dal = new WorkStationDAL();
            return await dal.GetWorkStationById(workStationId, companyId);
        }
        #endregion


        #region Operation
        public async Task<bool> SaveOperation(OperationDto dto)
        {
            var dal = new WorkStationDAL();
            return await dal.SaveOperation(dto);
        }
        public async Task<IEnumerable<OperationDto>> GetOperations(int companyId)
        {
            var dal = new WorkStationDAL();
            return await dal.GetOperations(companyId);
        }
        public async Task<OperationDto> GetOperation(int operationId, int companyId)
        {
            var dal = new WorkStationDAL();
            return await dal.GetOperation(operationId, companyId);
        }

        #endregion
    }
}

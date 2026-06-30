using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace BAL.DAL
{
    public class WorkStationDAL
    {

        #region WorkStationType
        public async Task<bool> SaveWorkStationType(WorkStationTypeDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@description", DbType.String, ParameterDirection.Input, 0, dto.Description);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                int result = 0;
                if (dto.WorkStationTypeId == 0)
                {
                    sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                    result = await sql.ExecuteNonQueryAsync(ADD_TYPE);

                }
                else
                {
                    sql.AddParameter("@workstationTypeId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkStationTypeId);

                    sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                    result = await sql.ExecuteNonQueryAsync(UPDATE_TYPE);

                }

                return result > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<WorkStationTypeDto>> GetTypeList(int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return await sql.QueryAsync<WorkStationTypeDto>(LIST_TYPE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<WorkStationTypeDto> GetTypeById(int workStationTypeId, int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@workStationTypeId", DbType.Int32, ParameterDirection.Input, 0, workStationTypeId);

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return (await sql.QueryAsync<WorkStationTypeDto>(SEL_TYPE)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region WorkStation
        public async Task<bool> SaveWorkStation(WorkStationDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@description", DbType.String, ParameterDirection.Input, 0, dto.Description);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@workstationTypeId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkStationTypeId);
                sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                int result = 0;
                if (dto.WorkStationId == 0)
                {
                    sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                    result = await sql.ExecuteNonQueryAsync(ADD_WORKSTATION);

                }
                else
                {
                    sql.AddParameter("@workstationId", DbType.Int32, ParameterDirection.Input, 0, dto.WorkStationId);


                    result = await sql.ExecuteNonQueryAsync(UPDATE_WORKSTATION);

                }

                return result > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<WorkStationDto>> GetWorkStationList(int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return await sql.QueryAsync<WorkStationDto>(LIST_WORKSTATION);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<WorkStationDto> GetWorkStationById(int workStationId, int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@workStationId", DbType.Int32, ParameterDirection.Input, 0, workStationId);

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return (await sql.QueryAsync<WorkStationDto>(SEL_WORKSTATION)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Operation
        public async Task<bool> SaveOperation(OperationDto dto)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@name", DbType.String, ParameterDirection.Input, 0, dto.Name);
                sql.AddParameter("@description", DbType.String, ParameterDirection.Input, 0, dto.Description);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
                sql.AddParameter("@parentOperationId", DbType.Int32, ParameterDirection.Input, 0, dto.ParentOperationId);
               
                int result = 0;
                if (dto.OperationId == 0)
                {
                    sql.AddParameter("@createdBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    sql.AddParameter("@guId", DbType.String, ParameterDirection.Input, 0, dto.GuId);
                    result = await sql.ExecuteNonQueryAsync(ADD_OPERATION);

                }
                else
                {
                    sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                    sql.AddParameter("@operationId", DbType.Int32, ParameterDirection.Input, 0, dto.OperationId);


                    result = await sql.ExecuteNonQueryAsync(UPDATE_OPERATION);

                }

                return result > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<OperationDto>> GetOperations(int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return await sql.QueryAsync<OperationDto>(LIST_OPERATION);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<OperationDto> GetOperation(int operationId, int companyId)
        {
            var sql = new SQL();
            try
            {
                sql.AddParameter("@operationId", DbType.Int32, ParameterDirection.Input, 0, operationId);

                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
                return (await sql.QueryAsync<OperationDto>(SEL_OPERATION)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region procedures

        const string ADD_TYPE = "p_workstationType_ins";
        const string UPDATE_TYPE = "p_workstationType_upd";
        const string LIST_TYPE = "p_workstationType_list";
        const string SEL_TYPE = "p_workstationType_sel";

        //WorkStation
        const string ADD_WORKSTATION = "p_workstation_ins";
        const string UPDATE_WORKSTATION = "p_workstation_upd";
        const string LIST_WORKSTATION = "p_workstation_list";
        const string SEL_WORKSTATION = "p_workstation_sel";

        //Operation
        const string ADD_OPERATION = "p_operation_ins";
        const string UPDATE_OPERATION = "p_operation_upd";
        const string LIST_OPERATION = "p_operation_list";
        const string SEL_OPERATION = "p_operation_sel";

        #endregion
    }
}

using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web.WebSockets;
using System.Data.SqlClient;
namespace BAL.DAL
{
    public class JobCardDAL
    {
        SQL sql;
        public async Task<bool> Save(JobCardDto dto)
        {
            bool local = false;
            try
            {
                if (sql == null)
                {
                    sql = new SQL();
                    local = true;
                }
                if (local)
                {

                    sql.BeginTransaction();
                    sql.NewCommand();
                }

                sql.AddParameter("@EmployeeId", DbType.Int32, ParameterDirection.Input, 0, dto.EmployeeId);

                sql.AddParameter("@ContactName", DbType.String, ParameterDirection.Input, 0, dto.ContactName);
                sql.AddParameter("@contactNo", DbType.String, ParameterDirection.Input, 0, dto.ContactNo);
                sql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, dto.JobNumber);


                sql.AddParameter("@days", DbType.Single, ParameterDirection.Input, 0, dto.Days);

                sql.AddParameter("@Quantity", DbType.Int16, ParameterDirection.Input, 0, dto.Quantity);
                sql.AddParameter("@EstimatedStartDate", DbType.Date, ParameterDirection.Input, 0, dto.EstimatedStartDate);
                sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

                sql.AddParameter("@typeId", DbType.Int32, ParameterDirection.Input, 0, dto.TypeId);
                sql.AddParameter("@areaCovered", DbType.Double, ParameterDirection.Input, 0, dto.AreaCovered);
                sql.AddParameter("@unitRate", DbType.Double, ParameterDirection.Input, 0, dto.UnitAreaRate);
                sql.AddParameter("@employees", DbType.String, ParameterDirection.Input, 0, dto.Employees);
                sql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, dto.Remarks);
                if (dto.EstimatedEndDate.Year > 2000)
                {
                    sql.AddParameter("@EstimatedEndDate", DbType.Date, ParameterDirection.Input, 0, dto.EstimatedEndDate);

                }
                int result = 0;
                if (dto.JobCardId == 0)
                {
                    sql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, dto.LedgerSiteId);
                    sql.AddParameter("@jobCardTypeKey", DbType.Int32, ParameterDirection.Input, 0, dto.JobCardTypeKey);
                    sql.AddParameter("@jobCardType", DbType.String, ParameterDirection.Input, 0, dto.JobCardType);
                    sql.AddParameter("@guid", DbType.String, ParameterDirection.Input, 0, dto.GuId);

                    sql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
                    sql.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.CreatedOn);
                    result = await sql.ExecuteNonQueryAsync(ADD_JOB_CARD);
                }
                if (dto.JobCardId > 0)
                {
                    sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
                    sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
                    sql.AddParameter("@JobCardId", DbType.Int32, ParameterDirection.Input, 0, dto.JobCardId);

                    result = await sql.ExecuteNonQueryAsync(UPDATE_JOB_CARD);
                }
                if (local)
                {
                    sql.Commit();
                }
                return result > 0;
            }
            catch (SqlException ex)
            {
                if (local)
                {
                    sql.Rollback();
                }
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                if (local)
                {
                    sql.Rollback();
                }
                throw new Exception("Could not create job card", ex);
            }
        }
        public async Task<IEnumerable<JobCardDto>> GetList(JobCardDto dto)
        {
            sql = new SQL();
            sql.AddParameter("@jobCardTypeKey", DbType.Int32, ParameterDirection.Input, 0, dto.JobCardTypeKey);
            sql.AddParameter("@jobCardType", DbType.String, ParameterDirection.Input, 0, dto.JobCardType);
            sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

            return await sql.QueryAsync<JobCardDto>(SEL_BY_TYPEKEY);
        }
        public async Task<JobCardDto> GetById(JobCardDto dto)
        {
            sql = new SQL();
 
            sql.AddParameter("@jobcardId", DbType.Int32, ParameterDirection.Input, 0, dto.JobCardId);
            sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);

            return await sql.QueryFirstAsync<JobCardDto>(GET_JOB_CARD_BYID);
        }
        public async Task<int> UpdateStatus(JobCardDto dto)
        {
            sql = new SQL();

            sql.AddParameter("@jobCardId", DbType.Int32, ParameterDirection.Input, 0, dto.JobCardId);
            sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, dto.CompanyId);
            sql.AddParameter("@status", DbType.String, ParameterDirection.Input, 0, dto.StatusId);
            sql.AddParameter("@remarks", DbType.String, ParameterDirection.Input, 0, dto.Remarks);
            sql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, dto.ModifiedBy);
            sql.AddParameter("@modifiedOn", DbType.DateTime, ParameterDirection.Input, 0, dto.ModifiedOn);
            sql.AddParameter("@employees", DbType.String, ParameterDirection.Input, 0, dto.Employees);
            sql.AddParameter("@areaCovered", DbType.Double, ParameterDirection.Input, 0, dto.AreaCovered);
            if (dto.CompletionDate.Year > 2000)
                sql.AddParameter("@completionDate", DbType.Date, ParameterDirection.Input, 0, dto.CompletionDate);

            return await sql.ExecuteNonQueryAsync(UPDATE_JOB_CARD_STATUS);
        }
    
        
        #region procedures
        const string ADD_JOB_CARD = "p_jobCard_ins";
        const string SEL_BY_TYPEKEY = "p_jobCard_sel";
        const string UPDATE_JOB_CARD = "p_jobCard_upd";
        const string UPDATE_JOB_CARD_STATUS = "p_jobCard_updStatus";
        const string GET_JOB_CARD_BYID = "p_jobCard_byId";
        #endregion
    }
}

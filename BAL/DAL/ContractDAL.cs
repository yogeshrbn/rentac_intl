using BAL.DTO;
using BAL.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using BAL.Objects;
using System.Windows.Controls.Primitives;
using System.ComponentModel.Design;
namespace BAL.DAL
{
    public class ContractDAL
    {
        public ContractDAL() { }
        SQL sql = new SQL();
        public async Task<bool> Save(ContractDTO contractDTO)
        {

            try
            {
                sql.BeginTransaction();

                sql.NewCommand();
                sql.AddParameter("@title", DbType.String, ParameterDirection.Input, 0, contractDTO.Title);
                sql.AddParameter("@ledgerId", DbType.Int64, ParameterDirection.Input, 0, contractDTO.LedgerId);
                sql.AddParameter("@LedgerSiteId", DbType.Int64, ParameterDirection.Input, 0, contractDTO.LedgerSiteId);
                sql.AddParameter("@contractType", DbType.Byte, ParameterDirection.Input, 0, contractDTO.ContractType);
                sql.AddParameter("@effectiveFrom", DbType.Date, ParameterDirection.Input, 0, contractDTO.EffectiveFrom);
                sql.AddParameter("@validTill", DbType.Date, ParameterDirection.Input, 0, contractDTO.ValidTill);

                sql.AddParameter("@duration", DbType.Int16, ParameterDirection.Input, 0, contractDTO.Duration);
                sql.AddParameter("@rate", DbType.Currency, ParameterDirection.Input, 0, contractDTO.Rate);
                sql.AddParameter("@measureType", DbType.Byte, ParameterDirection.Input, 0, contractDTO.MeasureType);
                sql.AddParameter("@ContractValue", DbType.Currency, ParameterDirection.Input, 0, contractDTO.ContractValue);
                sql.AddParameter("@Area", DbType.Currency, ParameterDirection.Input, 0, contractDTO.Area);
                sql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, contractDTO.CompanyId);
                sql.AddParameter("@taxCategoryId", DbType.Int32, ParameterDirection.Input, 0, contractDTO.TaxCategoryId);

                sql.AddParameter("@billCycle", DbType.String, ParameterDirection.Input, 0, contractDTO.BillCycle);
                sql.AddParameter("@BillDay", DbType.Int16, ParameterDirection.Input, 0, contractDTO.BillDay);
                sql.AddParameter("@projectOwner", DbType.Int32, ParameterDirection.Input, 0, contractDTO.ProjectOwner);
                sql.AddParameter("@clientRefNo", DbType.String, ParameterDirection.Input, 0, contractDTO.ClientRefNo);
                sql.AddParameter("@height", DbType.Single, ParameterDirection.Input, 0, contractDTO.Height);
                sql.AddParameter("@width", DbType.Single, ParameterDirection.Input, 0, contractDTO.Width);
                sql.AddParameter("@approximateWeight", DbType.Single, ParameterDirection.Input, 0, contractDTO.ApproximateWeight);
                sql.AddParameter("@poNumber", DbType.String, ParameterDirection.Input, 0, contractDTO.PONumber);
                sql.AddParameter("@poDate", DbType.Date, ParameterDirection.Input, 0, contractDTO.PODate.HasValue ? (object)contractDTO.PODate.Value : DBNull.Value);


                sql.AddParameter("@Remarks", DbType.String, ParameterDirection.Input, 0, contractDTO.Remarks);

                sql.AddParameter("@Doc1", DbType.String, ParameterDirection.Input, 0, contractDTO.Doc1);
                sql.AddParameter("@Doc2", DbType.String, ParameterDirection.Input, 0, contractDTO.Doc2);
                sql.AddParameter("@Doc3", DbType.String, ParameterDirection.Input, 0, contractDTO.Doc3);
                sql.AddParameter("@sizeDescription", DbType.String, ParameterDirection.Input, 0, contractDTO.SizeDescription);
                sql.AddParameter("@zoneId", DbType.Int32, ParameterDirection.Input, 0, contractDTO.ZoneId);
                sql.AddParameter("@localityId", DbType.Int32, ParameterDirection.Input, 0, contractDTO.LocalityId);

                if (contractDTO.ContractId > 0)
                {
                    sql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, contractDTO.ContractId);
                    var result = Convert.ToInt16(sql.ExecuteNonQuery(UPDATE_CONTRACT)) > 0;
                }
                else
                {
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, contractDTO.CreatedOn);
                    sql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, contractDTO.CreatedBy);
                    sql.AddParameter("@GuId", DbType.String, ParameterDirection.Input, 0, contractDTO.GuId);
                    sql.AddParameter("@StatusId", DbType.Byte, ParameterDirection.Input, 0, 1);
                    sql.AddParameter("@QuotationId", DbType.Int32, ParameterDirection.Input, 0, contractDTO.QuotationId);
                    contractDTO.ContractId = Convert.ToInt32(await sql.ExecuteScalarAsync(ADD_CONTRACT));
                }

                if (contractDTO.ContractId > 0)
                {
                    await AddDetails(contractDTO.Details, contractDTO);
                    await AddCondition(contractDTO.Conditions, contractDTO);
                }
                sql.Commit();
                return true;
            }
            catch (UDFException ex)
            {
                sql.Rollback();
                throw ex;
            }
            catch (SqlException ex)
            {
                sql.Rollback();
                throw ex;
            }
            catch (Exception ex)
            {
                sql.Rollback();
                throw ex;
            }

        }
        public async Task<bool> AddDetails(List<ContractDetailDTO> dto, ContractDTO cdto)
        {
            //SQL sql = new SQL();
            try
            {
                foreach (var item in dto)
                {
                    item.GuId = Guid.NewGuid().ToString();
                    item.CreatedOn = DateTime.Now;
                    item.CreatedBy = cdto.CreatedBy;

                    sql.NewCommand();
                    sql.AddParameter("@ContractId", DbType.Int64, ParameterDirection.Input, 0, cdto.ContractId);

                    if (item.Deleted == 1)
                    {
                        sql.AddParameter("@productId", DbType.Int64, ParameterDirection.Input, 0, item.ProductId);
                        Convert.ToInt64(await sql.ExecuteScalarAsync(DEL_CONTRACT_DETAIL));
                        continue;
                    }
                    sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, cdto.CompanyId);
                    sql.AddParameter("@ProductId", DbType.Int64, ParameterDirection.Input, 0, item.ProductId);
                    sql.AddParameter("@Quantity", DbType.Currency, ParameterDirection.Input, 0, item.Quantity);
                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, item.CreatedOn);
                    sql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, item.CreatedBy);
                    sql.AddParameter("@GuId", DbType.String, ParameterDirection.Input, 0, item.GuId);
                    sql.AddParameter("@contractDetailId", DbType.Int64, ParameterDirection.Input, 0, item.ContractDetailId);

                    item.ContractDetailId = Convert.ToInt32(await sql.ExecuteScalarAsync(ADD_CONTRACT_DETAIL));

                }
                return true;

            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<bool> AddCondition(List<ContractConditionDTO> dto, ContractDTO cdto)
        {
            //    SQL sql = new SQL();
            try
            {
                foreach (var item in dto)
                {
                    if (String.IsNullOrEmpty(item.Condition))
                    {
                        continue;
                    }
                    item.GuId = Guid.NewGuid().ToString();
                    item.CreatedOn = DateTime.Now;
                    item.CreatedBy = cdto.CreatedBy;

                    sql.NewCommand();
                    sql.AddParameter("@ContractId", DbType.Int64, ParameterDirection.Input, 0, cdto.ContractId);

                    if (item.Deleted == 1)
                    {
                        sql.AddParameter("@ConditionId", DbType.Int64, ParameterDirection.Input, 0, item.ContractConditionId);
                        Convert.ToInt64(await sql.ExecuteScalarAsync(DEL_CONTRACT_CONDITION));
                        continue;
                    }
                    sql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, cdto.CompanyId);
                    sql.AddParameter("@Condition", DbType.String, ParameterDirection.Input, 0, item.Condition);

                    sql.AddParameter("@createdOn", DbType.DateTime, ParameterDirection.Input, 0, item.CreatedOn);
                    sql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, item.CreatedBy);
                    sql.AddParameter("@GuId", DbType.String, ParameterDirection.Input, 0, item.GuId);
                    sql.AddParameter("@ConditionId", DbType.Int64, ParameterDirection.Input, 0, item.ContractConditionId);
                    item.ContractConditionId = Convert.ToInt32(await sql.ExecuteScalarAsync(ADD_CONTRACT_CONDITION));
                }
                return true;

            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<ContractViewDto>> GetAll(ContractFilterDto filter)
        {
            sql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
            sql.AddParameter("@ledgerId", DbType.Int64, ParameterDirection.Input, 0, filter.LedgerId);
            sql.AddParameter("@siteId", DbType.Int64, ParameterDirection.Input, 0, filter.LedgerSiteId);
            sql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
            sql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
            sql.AddParameter("@status", DbType.Byte, ParameterDirection.Input, 0, filter.StatusId);
            sql.AddParameter("@expiryOn", DbType.Byte, ParameterDirection.Input, 0, filter.ExpringOn);
            //  sql.AddParameter("@dueBillOn", DbType.Byte, ParameterDirection.Input, 0, filter.DueBillOn);

            sql.AddParameter("@extended", DbType.Boolean, ParameterDirection.Input, 0, filter.Extended);
            if (!String.IsNullOrEmpty(filter.QuotationNumber))
            {
                sql.AddParameter("@quotationNumber", DbType.String, ParameterDirection.Input, 0, 
                    filter.QuotationNumber);
            }
            if (filter.FilterFor.ToLower() == "ops")
            {
                sql.AddParameter("@activityType", DbType.Byte, ParameterDirection.Input, 0, filter.ActivityType);

                sql.AddParameter("@activityStatusId", DbType.Byte, ParameterDirection.Input, 0, filter.ActivityStatusId);
                return await sql.QueryAsync<ContractViewDto>(GET_ALL_OPS);
            }
            if (filter.FilterFor.ToLower() == "sales")
            {
                sql.AddParameter("@activityType", DbType.Byte, ParameterDirection.Input, 0, filter.ActivityType);

                sql.AddParameter("@activityStatusId", DbType.Byte, ParameterDirection.Input, 0, filter.ActivityStatusId);
                return await sql.QueryAsync<ContractViewDto>("p_contracts_allV1");
            }
            else
                return await sql.QueryAsync<ContractViewDto>(GET_ALL);

        }

        public async Task<IEnumerable<ContractViewDto>> GetById(ContractFilterDto filter)
        {
            sql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
            sql.AddParameter("@contractId", DbType.Int64, ParameterDirection.Input, 0, filter.ContractId);

            return await sql.QueryAsync<ContractViewDto>(GET_CONTRACT_BY_ID);

        }
        public async Task<IEnumerable<ContractDetailViewDto>> GetContractDetails(ContractFilterDto filter)
        {
            sql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
            sql.AddParameter("@contractId", DbType.Int64, ParameterDirection.Input, 0, filter.ContractId);

            return await sql.QueryAsync<ContractDetailViewDto>(GET_CONTRACT_DETAILS);

        }
        public async Task<IEnumerable<ContractConditionViewDto>> GetContractConditions(ContractFilterDto filter)
        {
            sql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
            sql.AddParameter("@contractId", DbType.Int64, ParameterDirection.Input, 0, filter.ContractId);

            return await sql.QueryAsync<ContractConditionViewDto>(GET_CONTRACT_CONDITIONS);

        }

        public async Task<bool> UpdateContractStatus(ContractDTO cdto)
        {
            //SQL sql = new SQL();
            try
            {
                using (var objSql = new SQL())
                {
                    objSql.AddParameter("@ContractId", DbType.Int64, ParameterDirection.Input, 0, cdto.ContractId);
                    objSql.AddParameter("@StatusId", DbType.Int32, ParameterDirection.Input, 0, cdto.StatusId);
                    objSql.AddParameter("@UpdatedOn", DbType.DateTime, ParameterDirection.Input, 0, cdto.UpdatedOn);
                    objSql.AddParameter("@UpdatedBy", DbType.Int32, ParameterDirection.Input, 0, cdto.UpdatedBy);
                    objSql.AddParameter("@UpdateRemarks", DbType.String, ParameterDirection.Input, 0, cdto.Remarks);
                    return await objSql.ExecuteNonQueryAsync(UPDATE_CONTRACT_STATUS) > 0;
                }
            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<WorkOrderItemDTO>> ContractInventory(ContractFilterDto filter)
        {
            sql.AddParameter("@companyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
            sql.AddParameter("@contractId", DbType.Int64, ParameterDirection.Input, 0, filter.ContractId);
            return await sql.QueryAsync<WorkOrderItemDTO>(CONTRACT_INVENTORY);

        }
        public async Task<bool> ExtendContract(ContractDTO cdto)
        {
            //SQL sql = new SQL();
            try
            {
                using (var objSql = new SQL())
                {
                    objSql.AddParameter("@ContractId", DbType.Int64, ParameterDirection.Input, 0, cdto.ContractId);
                    objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, cdto.CompanyId);
                    objSql.AddParameter("@modifiedDate", DbType.DateTime, ParameterDirection.Input, 0, cdto.UpdatedOn);
                    objSql.AddParameter("@validTill", DbType.DateTime, ParameterDirection.Input, 0, cdto.ValidTill);
                    objSql.AddParameter("@modifiedBy", DbType.Int32, ParameterDirection.Input, 0, cdto.UpdatedBy);
                    objSql.AddParameter("@extensionRemarks", DbType.String, ParameterDirection.Input, 0, cdto.Remarks);
                    return await objSql.ExecuteNonQueryAsync(EXTEND_CONTRACT) > 0;
                }
            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<int> InsertActivityAsync(ContractActivity activity)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, activity.CompanyId);
                objSql.AddParameter("@ContractId", DbType.Int32, ParameterDirection.Input, 0, activity.ContractId);
                objSql.AddParameter("@ContractStatus", DbType.Byte, ParameterDirection.Input, 0, activity.ContractStatus);
                objSql.AddParameter("@TeamId", DbType.Int32, ParameterDirection.Input, 0, activity.TeamId);
                objSql.AddParameter("@Employees", DbType.String, ParameterDirection.Input, 0, activity.Employees ?? string.Empty);
                objSql.AddParameter("@ActivityStatus", DbType.String, ParameterDirection.Input, 50, activity.ActivityStatus);
                objSql.AddParameter("@ActivityType", DbType.Byte, ParameterDirection.Input, 0, activity.ActivityType);
                objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, activity.CreatedBy);
                objSql.AddParameter("@CreatedOn", DbType.DateTime, ParameterDirection.Input, 0, activity.CreatedOn);
                objSql.AddParameter("@ModifiedBy", DbType.Int32, ParameterDirection.Input, 0, activity.ModifiedBy);
                objSql.AddParameter("@ModifiedOn", DbType.DateTime, ParameterDirection.Input, 0, activity.ModifiedOn);

                // Output parameter for ActivityId
                objSql.AddParameter("@ActivityId", DbType.Int32, ParameterDirection.Output, 0, 0);

                var result = await objSql.ExecuteNonQueryAsync(SP_INSERT_ACTIVITY);

                return result;
            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateActivityAsync(ContractActivity activity)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@ActivityId", DbType.Int32, ParameterDirection.Input, 0, activity.ActivityId);
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, activity.CompanyId);
                objSql.AddParameter("@ContractId", DbType.Int32, ParameterDirection.Input, 0, activity.ContractId);
                objSql.AddParameter("@ContractStatus", DbType.Byte, ParameterDirection.Input, 0, activity.ContractStatus);
                objSql.AddParameter("@TeamId", DbType.Int32, ParameterDirection.Input, 0, activity.TeamId);
                objSql.AddParameter("@Employees", DbType.String, ParameterDirection.Input, 0, activity.Employees ?? string.Empty);
                objSql.AddParameter("@ActivityStatus", DbType.String, ParameterDirection.Input, 50, activity.ActivityStatus);
                objSql.AddParameter("@ActivityType", DbType.Byte, ParameterDirection.Input, 0, activity.ActivityType);
                objSql.AddParameter("@ModifiedBy", DbType.Int32, ParameterDirection.Input, 0, activity.ModifiedBy);
                objSql.AddParameter("@ModifiedOn", DbType.DateTime, ParameterDirection.Input, 0, activity.ModifiedOn);

                var rowsAffected = await objSql.ExecuteNonQueryAsync(SP_UPDATE_ACTIVITY);
                return rowsAffected > 0;
            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteActivityAsync(int activityId, int companyId)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@ActivityId", DbType.Int32, ParameterDirection.Input, 0, activityId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

                var rowsAffected = await objSql.ExecuteNonQueryAsync(SP_DELETE_ACTIVITY);
                return rowsAffected > 0;
            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ContractActivity> GetByIdAsync(int activityId, int companyId)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@ActivityId", DbType.Int32, ParameterDirection.Input, 0, activityId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

                return await objSql.QueryFirstAsync<ContractActivity>(SP_GET_BY_ID_ACTIVITY);



            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ContractActivity>> GetAllAsync(int contractId, int companyId)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@contractId", DbType.Int32, ParameterDirection.Input, 0, contractId);
                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);

                return await objSql.QueryAsync<ContractActivity>(SP_GET_ALL_ACTIVITY);

            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<ContractActivityTrackerDTO>> ActivityTracker(FilterCriteria filter)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);
                if (Convert.ToDateTime(filter.From).Year > 2000)
                {
                    objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                }
                if (Convert.ToDateTime(filter.To).Year > 2000)
                {
                    objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                }
                return await objSql.QueryAsync<ContractActivityTrackerDTO>(CONTRACT_ACTIVITY_TRACKER);

            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<EmployeeDPRDTO>> EmployeeDPR(FilterCriteria filter)
        {
            try
            {
                var objSql = new SQL();

                objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);
                objSql.AddParameter("@employeeId", DbType.Int32, ParameterDirection.Input, 0, filter.EmployeeId);
                if (Convert.ToDateTime(filter.From).Year > 2000)
                {
                    objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                }
                if (Convert.ToDateTime(filter.To).Year > 2000)
                {
                    objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                }
                return await objSql.QueryAsync<EmployeeDPRDTO>(EMPLOYEE_DPR);

            }
            catch (UDFException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Procedures
        const string ADD_CONTRACT = "p_contract_ins";
        const string UPDATE_CONTRACT = "p_contract_upd";
        const string ADD_CONTRACT_DETAIL = "p_contractDetails_ins";
        const string DEL_CONTRACT_DETAIL = "p_contractDetails_del";
        const string ADD_CONTRACT_CONDITION = "p_contractConditions_ins";
        const string DEL_CONTRACT_CONDITION = "p_contractConditions_del";
        const string GET_ALL = "p_contracts_all";
        const string GET_ALL_OPS = "p_contracts_ops_all";

        const string GET_CONTRACT_BY_ID = "p_contract_byId";
        const string GET_CONTRACT_DETAILS = "p_contractDetails_sel";
        const string GET_CONTRACT_CONDITIONS = "p_contractConditions_sel";
        const string UPDATE_CONTRACT_STATUS = "p_contract_updStatus";
        const string CONTRACT_INVENTORY = "p_contractInventory";
        const string EXTEND_CONTRACT = "p_extendContract";
        //   const string CONTRACTS_FOR_DUE_BILLS = "p_extendContract";


        private const string SP_INSERT_ACTIVITY = "p_ContractActivity_Ins";
        private const string SP_UPDATE_ACTIVITY = "p_ContractActivity_Upd";
        private const string SP_DELETE_ACTIVITY = "p_ContractActivity_Del";
        private const string SP_GET_BY_ID_ACTIVITY = "p_ContractActivity_GetById";
        private const string SP_GET_ALL_ACTIVITY = "p_ContractActivity_GetAll";
        private const string CONTRACT_ACTIVITY_TRACKER = "p_contractActivityTracker";
        private const string EMPLOYEE_DPR = "p_employeeDPR";


        #endregion

    }
}

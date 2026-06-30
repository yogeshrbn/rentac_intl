using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
namespace BAL.DAL
{
    internal class AccountGroupDAL
    {
        /// <summary>
        /// Creates a new account group
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>AccountGroup Id</returns>
        public int Add(AccountGroupDTO dto)
        {
            SQL objSql = new SQL();

            objSql.AddParameter("@Name", DbType.String, ParameterDirection.Input, 0, dto.Name);
            objSql.AddParameter("@ParentGroup", DbType.Int32, ParameterDirection.Input, 0, dto.ParentGroup);
            objSql.AddParameter("@GroupCode", DbType.String, ParameterDirection.Input, 0, dto.GroupCode);

            //Add account group
            if (dto.AccountGroupId == 0)
            {
                objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, dto.StoreId);
                return objSql.ExecuteNonQuery(ADD);
            }
            else
            {
                //Update existing account group
                objSql.AddParameter("@AccountGroupId", DbType.Int32, ParameterDirection.Input, 0, dto.AccountGroupId);
                return objSql.ExecuteNonQuery(UPDATE);
            }
        }

        /// <summary>
        /// Selects all account groups 
        /// </summary>
        /// <param name="storeId">Store to the account groups from</param>
        /// <returns></returns>
        public List<AccountGroupDTO> GetAll(int storeId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@StoreId", DbType.Int32, ParameterDirection.Input, 0, storeId);
            return objSql.ContructList<AccountGroupDTO>(objSql.ExecuteDataSet(GETALL));
        }

        /// <summary>
        /// Gets the AccountGroup Details
        /// </summary>
        /// <param name="accountGroupId">AccountGroupId</param>
        /// <returns>AccountGroup object</returns>
        public AccountGroupDTO GetInfo(int accountGroupId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@AccountGroupId", DbType.Int32, ParameterDirection.Input, 0, accountGroupId);
            return objSql.ContructList<AccountGroupDTO>(objSql.ExecuteDataSet(GETDETAIL)).FirstOrDefault();
        }

        public bool UpdateStatus(int accountGroupId, bool active)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@AccountGroupId", DbType.Int32, ParameterDirection.Input, 0, accountGroupId);
            objSql.AddParameter("@IsActive", DbType.Boolean, ParameterDirection.Input, 0, active);
            return Convert.ToInt16(objSql.ExecuteNonQuery(UPDATESTATUS)) == 0;
        }

        /// <summary>
        /// Selects entry types for supplied group
        /// </summary>
        /// <param name="accountGroupId">AccountGroupId</param>
        /// <returns>List of Entry types</returns>
        public List<EntryTypeDTO> GetEntryTypes(int accountGroupId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@AccountGroupId", DbType.Int32, ParameterDirection.Input, 0, accountGroupId);
            return objSql.ContructList<EntryTypeDTO>(objSql.ExecuteDataSet(ENTRY_TYPE_SEL));
        }
        /// <summary>
        /// Get account/ledger of a group
        /// </summary>
        /// <param name="accountGroupId">AccountGroupId</param>
        /// <returns>List of account/ledger</returns>
        public List<LedgerDTO> GetAccountsByGroup(int accountGroupId, int companyId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@AccountGroupId", DbType.Int32, ParameterDirection.Input, 0, accountGroupId);
            objSql.AddParameter("@companyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<LedgerDTO>(objSql.ExecuteDataSet(ACCOUNT_BY_GRUOP));
        }

        public List<LedgerTransactionDTO> GetBankEntries(int bankId, int ledgerId, int ledgerSiteId, string from, string to,
            int entryType, string tranRefNumber, int companyId)
        {
            SQL objSql = new SQL();
            if (bankId > 0)
                objSql.AddParameter("@BankId", DbType.Int32, ParameterDirection.Input, 0, bankId);
            if (ledgerId > 0)
                objSql.AddParameter("@PartyId", DbType.Int32, ParameterDirection.Input, 0, ledgerId);
            if (ledgerSiteId > 0)
                objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            if (from != null)
                objSql.AddParameter("@From", DbType.Date, ParameterDirection.Input, 0, from);
            if (to != null)
                objSql.AddParameter("@To", DbType.Date, ParameterDirection.Input, 0, to);
            if (entryType > 0)
                objSql.AddParameter("@EntryType", DbType.Int32, ParameterDirection.Input, 0, entryType);
            if (tranRefNumber != null)
                objSql.AddParameter("@TranRefNumber", DbType.Int32, ParameterDirection.Input, 0, tranRefNumber);
            objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<LedgerTransactionDTO>(objSql.ExecuteDataSet(GET_BANK_ENTRIES));
        }
        #region Procedures
        const string ADD = "p_accountGroup_ins";
        const string UPDATE = "p_accountGroup_upd";
        const string GETALL = "p_accountGroup_all";
        const string GETDETAIL = "p_accountGroup_sel";
        const string UPDATESTATUS = "p_accountGroupStatus_upd";
        const string ENTRY_TYPE_SEL = "p_AccountGroupEntryType_sel";
        const string ACCOUNT_BY_GRUOP = "p_AccountByGroup_sel";
        const string GET_BANK_ENTRIES = "p_BankEntryregister_sel";
        #endregion

    }
}

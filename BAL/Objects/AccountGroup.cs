using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class AccountGroup : AccountGroupDTO
    {

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public AccountGroup()
        {

        }
        /// <summary>
        /// Initializes the current instance with the values from the database.
        /// </summary>
        /// <param name="accountGrouPId"></param>
        public AccountGroup(int accountGrouPId)
        {
            this.AccountGroupId = accountGrouPId;
            if (this.AccountGroupId > 0)
            {
                GetInfo();
            }
        }
        /// <summary>
        /// Gets the group info from the database.
        /// </summary>
        void GetInfo()
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            AccountGroupDTO infoObject = objDal.GetInfo(this.AccountGroupId);
            this.Name = infoObject.Name;
            this.PrimaryGroup = infoObject.PrimaryGroup;
            this.SubGroup = infoObject.SubGroup;
            this.ParentGroup = infoObject.ParentGroup;
            this.Editable = infoObject.Editable;
            this.StoreId = infoObject.StoreId;
            this.GroupCode = infoObject.GroupCode;
            this.IsActive = infoObject.IsActive;
        }

        /// <summary>
        /// select all groups of a store
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<AccountGroupDTO> GetAll(int storeId)
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            return objDal.GetAll(storeId);
        }

        /// <summary>
        /// Saves a new account group or edit an existing group
        /// </summary>
        /// <returns>true/false</returns>
        public bool Save()
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            int accountGroupId = objDal.Add(this);
            if (this.AccountGroupId == 0)
            {
                this.AccountGroupId = accountGroupId;
            }
            return accountGroupId > 0;
        }


        public bool UpdateStatus()
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            return objDal.UpdateStatus(this.AccountGroupId, this.IsActive);
        }
        /// <summary>
        /// Selects entry types for supplied group
        /// </summary>
        /// <param name="accountGroupId">AccountGroupId</param>
        /// <returns>List of Entry types</returns>
        public List<EntryTypeDTO> GetEntryTypes(int accountGroupId)
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            return objDal.GetEntryTypes(accountGroupId);
        }
        /// <summary>
        /// Get account/ledger of a group
        /// </summary>
        /// <param name="accountGroupId">AccountGroupId</param>
        /// <returns>List of account/ledger</returns>
        public List<LedgerDTO> GetAccountsByGroup(int accountGroupId, int companyId)
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            
            return objDal.GetAccountsByGroup(accountGroupId, companyId);
        }
        public List<LedgerTransactionDTO> GetBankEntries(int bankId,int ledgerId,int ledgerSiteId,string from ,string to,
            int entryType,string transactionReferenceNumber,int companyId)
        {
            AccountGroupDAL objDal = new AccountGroupDAL();
            return objDal.GetBankEntries(bankId,ledgerId,ledgerSiteId,from,to,entryType,transactionReferenceNumber,companyId);
        }
    }
}

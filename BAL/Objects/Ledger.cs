using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using System.Data;
using System.Collections;
using BAL.Common;
namespace BAL.Objects
{
    public class Ledger : LedgerDTO
    {
        public Ledger(int ledgerId)
            : base(ledgerId)
        {

        }
        public Ledger() { }
        public int Save()
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.Save(this);

        }

        public List<LedgerDTO> GetAll(int companyId, bool isActive, string query = "")
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetAll(companyId, query, isActive);
        }

        public async Task<IEnumerable<LedgerDTO>> GetAll(LedgerDTO dto)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.GetAll(dto);
        }

        public async Task<IEnumerable<LedgerDTO>> GetAllByGroups(int companyId, string groupIds)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.GetAllByGroups(companyId, groupIds);

        }
        public LedgerDTO GetDetails()
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetDetails(this.LedgerId);
        }
        public bool DeActivate(bool activate)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.ActivateDeActivate(activate, this.LedgerId);

        }

        #region Transactions
        public int CreateTransactions(LedgerTransactionDTO dto)
        {
            if (dto.LedgerId == dto.RefLedgerId)
            {
                throw new Exception("Can not post a transaction with same debit and credit ledger");
            }
            int t = 0;
            int drLedgerId = dto.DrLedgerId;
            LedgerDAL dal = new LedgerDAL();
            //if (dto.CrLedgerId > 0)
            //{
            //    dto.RefLedgerId = dto.DrLedgerId;
            //    dto.DrLedgerId = 0;

            //    dto.TransactionType = 2;

            t = dal.CreateTransactions(dto);
            //}
            //if (drLedgerId > 0)
            //{
            //    dto.RefLedgerId = dto.CrLedgerId;
            //    dto.DrLedgerId = drLedgerId;
            //    dto.CrLedgerId = 0;
            //    dto.TransactionType = 1;

            //    dto.Narration = "paid to ";
            //    t = dal.CreateTransactions(dto);
            //}

            return t;


        }
        //added by ram
        public Boolean VerifyChequeNumber(LedgerTransactionDTO dto)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.VerifyChequeNumber(dto);
        }
        //added by ram
        public Boolean VerifySameDayAmount(LedgerTransactionDTO dto)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.VerifySameDayAmount(dto);
        }
        public List<LedgerTransactionDTO> GetReceiptRegister(int ledgerId, int ledgerSiteId, DateTime from, DateTime to, int entryType, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetReceiptRegister(ledgerId, ledgerSiteId, from, to, entryType, companyId);
        }

        public List<LedgerTransactionDTO> GetContractReceiptPayments(int contractId, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetContractReceiptPayments(contractId, companyId);
        }
        public List<LedgerTransactionDTO> LedgerTransactionsAll(int ledgerId, string from, string to, int entyType, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.LedgerTransactionsAll(ledgerId, from, to, entyType, companyId);
        }



        public List<BillingItemDTO> StockRegister(int ledgerId, int lederSiteId, int companyId, string fromDate, string toDate)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.StockRegister(ledgerId, lederSiteId, companyId, fromDate, toDate);
        }
        public DataSet StockRegister_rpt(int ledgerId, int companyId, string fromDate, string toDate)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.StockRegister_rpt(ledgerId, companyId, fromDate, toDate);
        }
        public List<BillingItemDTO> PartyOpeningBalance(int ledgerId, string fromDate)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.PartyOpeningBalance(ledgerId, fromDate);
        }
        public List<TransactionLookupDTO> GetLedgerTransactionLookup(int ledgerId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetLedgerTransactionLookup(ledgerId);
        }
        public List<LedgerTransactionDTO> GetTransactionDetails(int ledgerId, string date)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetTransactionDetails(ledgerId, date);
        }
        public async Task<bool> DeleteLedgerTransaction(int ledgerTransactionId, LoggedInUserInfo user)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.DeleteLedgerTransaction(ledgerTransactionId, user);
        }

        public async Task<bool> ClearQuickReceiptDocument(int ledgerTransactionId, LoggedInUserInfo user)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.ClearQuickReceiptDocument(ledgerTransactionId, user);
        }
        /// <summary>
        /// Gets bank entry transactions register
        /// </summary>
        /// <param name="bankId">Bank Id</param>
        /// <returns></returns>
        public List<LedgerTransactionDTO> BankEntryRegister(int bankId, int partyId, int ledgerSiteId, string fromDate, string toDate, Int16 entryTypeId, string cheque, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.BankEntryRegister(bankId, partyId, ledgerSiteId, fromDate, toDate, entryTypeId, cheque, companyId);
        }
        /// <summary>
        /// Gets bank entry transactions register
        /// </summary>
        /// <param name="bankId">Bank Id</param>
        /// <returns></returns>
        public DataSet BankEntryRegister_rpt(int bankId, int partyId, string fromDate, string toDate, Int16 entryTypeId, string cheque, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.BankEntryRegister_rpt(bankId, partyId, fromDate, toDate, entryTypeId, cheque, companyId);
        }

        public List<InvoiceItemDTO> PartyStockBalance(int ledgerId, int companyId, string from, string to, int ledgerSiteId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.PartyStockBalance(ledgerId, companyId, from, to, ledgerSiteId);
        }
        public List<InvoiceItemDTO> PartyStockBalanceBySize(int ledgerId, int companyId, string from, string to, int ledgerSiteId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.PartyStockBalanceBySize(ledgerId, companyId, from, to, ledgerSiteId);
        }
        public DataSet PartyStockBalance_REPORT(int ledgerId, int companyId, string from, string to)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.PartyStockBalance_REPORT(ledgerId, companyId, from, to);
        }
        public DataSet PartyStockBalance_DashBoard(int ledgerId, int companyId, string from, string to)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.PartyStockBalance_DashBoard(ledgerId, companyId, from, to);
        }
        public List<AccountLedgerDTO> GetAccountLedger(int ledgerId, string fromDate, String toDate, int ledgerSiteId
            , int companyId, int finyearId)
        {
            LedgerDAL dal = new LedgerDAL();
            var d = dal.GetAccountLedger(ledgerId, fromDate, toDate, ledgerSiteId, companyId, finyearId);
            return d;
        }

        public List<LedgerTransactionDTO> GetLedgerTransactions(int ledgerId, string fromDate, String toDate, int ledgerSiteId, int companyId, int finyearId)
        {
            LedgerDAL dal = new LedgerDAL();
            var d = dal.GetLedgerTransactions(ledgerId, fromDate, toDate, ledgerSiteId, companyId, finyearId);
            return d;
        }
        public List<LedgerTransactionDTO> GetAccountGroupLedgerTransactions(int accountGroupId, string fromDate,
           String toDate, int ledgerSiteId, int companyId, int finyearId)
        {
            LedgerDAL dal = new LedgerDAL();
            var d = dal.GetAccountGroupLedgerTransactions(accountGroupId, fromDate, toDate, ledgerSiteId, companyId, finyearId);
            return d;
        }
        public LedgerTransactionDTO GetAccountBalanceForBill(int ledgerId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetAccountBalanceForBill(ledgerId);
        }
        public DataSet GetAccountLedger_rpt(int ledgerId, string fromDate, String toDate)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetAccountLedger_rpt(ledgerId, fromDate, toDate);
        }
        public DataSet GetReceiptRegisterPRT(int ledgerId, string fromDate, String toDate, int entryType, string receiptNumber)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetReceiptRegisterPRT(ledgerId, fromDate, toDate, entryType, receiptNumber);
        }
        /// <summary>
        /// Gets site ledger and balance as on date
        /// </summary>
        /// <param name="ledgerId">LedgerId</param>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public List<LedgerTransactionDTO> GetFixedSiteLedger(int ledgerId, string fromDate, String toDate, int workOrderId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetFixedSiteLedger(ledgerId, fromDate, toDate, workOrderId);

        }
        //  public DataSet GetReceiptRegisterPRT(int ledgerId, string fromDate, String toDate, int entryType, string receiptNumber)
        //{
        //    LedgerDAL dal = new LedgerDAL();
        //    return dal.GetReceiptRegisterPRT(ledgerId, fromDate, toDate, entryType, receiptNumber);
        //}
        public LedgerDTO GetByPhone(string phone)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetByPhone(phone);
        }
        public List<InvoiceDTO> BillsByPhone(string phone)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.BillsByPhone(phone);
        }
        #endregion

        #region ProductRates
        public List<ProductRateDTO> GetProductRates(int ledgerId, int ledgerSiteId, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetProductRates(ledgerId, ledgerSiteId, companyId);
        }
        public bool AddUpdateProductRates(int ledgerId, List<ProductRateDTO> list)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.AddUpdateProductRates(ledgerId, list);
        }

        /// <summary>
        /// Selects products rates.
        /// </summary>
        /// <param name="ledgerId"></param>
        /// <returns></returns>
        public List<ProductRateDTO> ProductRates(int ledgerId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.ProductRates(ledgerId);
        }

        #endregion

        #region Sites

        public bool AddSite(ClientSiteDTO dto)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.AddSite(dto);
        }

        public bool UpdateClientSiteDocuments(ClientSiteDTO dto)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.UpdateClientSiteDocuments(dto);
        }

        public List<ClientSiteDTO> GetSites(FilterCriteria filter)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetSites(filter.LedgerId, filter.Closed);
        }
        public DataSet GetTransactionById(int ledgerTransactionId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetTransactionById(ledgerTransactionId);
        }
        public async Task<LedgerTransactionDTO> GetTransaction(int ledgerTransactionId, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            var txn = await dal.GetTransaction(ledgerTransactionId, companyId);
            return txn;
        }
        public ClientSiteDTO GetSiteById(int ledgerSiteId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetSiteById(ledgerSiteId);
        }
        public List<LedgerTaxDTO> GetSiteTaxes(int ledgerSiteId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetSiteTaxes(ledgerSiteId);
        }
        public InvoiceDTO GetLastBill(int ledgerId, int ledgerSiteId, int finYearId, int invoiceId = 0)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetLastBill(ledgerId, ledgerSiteId, finYearId, invoiceId);
        }
        public bool RemoveLedger(int ledgerId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.RemoveLedger(ledgerId);
        }
        public List<LedgerTransactionDTO> EstimatedRentPerDay(int ledgerId, string fromDate, string toDate, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.EstimatedRentPerDay(ledgerId, fromDate, toDate, companyId);
        }

        public List<LedgerTransactionDTO> GetDrCrNotes(int ledgerId, int companyId, int finYearId, int entryType)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetDrCrNotes(ledgerId, companyId, finYearId, entryType);
        }
        public DataSet GetPartyBalance(int ledgerId, int finYearId, int companyId, int ledgerSiteId, string from, string to)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetPartyBalance(ledgerId, finYearId, companyId, ledgerSiteId, from, to);
        }
        public DataSet UnbilledSites(int finYearId, int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.UnbilledSites(finYearId, companyId);
        }
        public List<ClientSiteDTO> GetAllClientSites(string jobNumber, string site, string client, string siteEng)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetAllClientSites(jobNumber, site, client, siteEng, this.CompanyId);

        }
        public List<SiteDTO> GetClientJobs(int ledgerId)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.GetClientJobs(ledgerId);
        }
        public DataSet ClientWiseItems(int ledgerId, int ledgerSiteId, int companyId, string fromDate, string toDate,
           string poNumber = "", string balanceType = "rent", byte forBill = 0)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.ClientWiseItems(ledgerId, ledgerSiteId, companyId, fromDate, toDate, poNumber, balanceType, forBill);
        }
        public DataSet ItemWiseClients(int companyId, int productId, string fromDate, string toDate, string balanceType = "rent")
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.ItemWiseClients(companyId, productId, fromDate, toDate, balanceType);
        }
        public DataSet Cashbook(int accountGroupId, int companyId, string fromDate, string toDate)
        {
            LedgerDAL dal = new LedgerDAL();
            return dal.Cashbook(accountGroupId, companyId, fromDate, toDate);
        }
        public async Task<IEnumerable<ClientSiteDTO>> AllClientsWithSites(int companyId)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.AllClientsWithSites(companyId);
        }
        public async Task<IEnumerable<AccountLedgerDTO>> TrialBalance(FilterCriteria dto)
        {
            LedgerDAL dal = new LedgerDAL();
            var data = await dal.TrialBalance(dto);

            return data;
        }



        #endregion

        public async Task<int> CloseSite(ClientSiteDTO dto, LoggedInUserInfo user)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.CloseSite(dto, user);
        }
        public async Task<int> CopyRates(
       CopyRatesDTO copy,
          LoggedInUserInfo user)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.CopyRates(copy, user);
        }

        public async Task<IEnumerable<LedgerTransactionDTO>> GetAdvancereceipts(FilterCriteria filter)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.GetAdvancereceipts(filter);
        }

        public async Task<decimal> getOpeningBalacneAsOnDate(FilterCriteria filter)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.getOpeningBalacneAsOnDate(filter);
        }
    }
}

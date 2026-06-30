using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ReportDTO
    {
        public int Days { get; set; }
        public double Amount { get; set; }
        public DateTime StartDate { get; set; }
        public int DaysLeft { get; set; }
        public string Site { get; set; }
        public string Client { get; set; }
        public string Compnay { get; set; }
        public string Company { get; set; }
        public string WorkOrderNumber { get; set; }
        public string JobNumber { get; set; }
        public string ChallanNumber { get; set; }

        public float AmountReceived { get; set; }
        public string Product { get; set; }
        public string GRN { get; set; }
        public DateTime ReceivingDate { get; set; }
        public Int32 ReceivingProductId { get; set; }
        public Double ReceivingQty { get; set; }
        public Double BalanceQty { get; set; }
        public Int32 SiteId { get; set; }
        public Double SentQty { get; set; }
        public Int32 WorkOrderId { get; set; }
        public Int32 SentProductId { get; set; }
        public DateTime SentDate { get; set; }
        public String FromDate { get; set; }
        public string ToDate { get; set; }
        public short OpenSites { get; set; }
        public short ClosedSites { get; set; }
        public short OpenJobNumbers { get; set; }
        public short ClosedJobNumbers { get; set; }
        public Double Quantity { get; set; }
        public short Mode { get; set; }
        public int RbnClientId { get; set; }
        public int LedgerId { get; set; }
        public int Breakage { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthYear { get; set; }

        public decimal TotalRentBill { get; set; }
        public decimal TotalSaleBill { get; set; }

        public decimal TotalContractBill { get; set; }
    }

    public class BillOverDueSummary
    {
        public string ClientName { get; set; }
        public decimal Amount { get; set; }
        public int DueFrom { get; set; }
        public string BalanceType { get; set; }
    }
    public class BillOverDueSummaryFilter
    {
        public int DueFromDays { get; set; }
        public int CompanyId { get; set; }
    }

    public class NewCustomersDTO
    {
        public int Customers { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthYear { get; set; }
    }
    public class NewCustomersReportsFilterDTO
    {
        public String DateFrom { get; set; }
        public String DateTo { get; set; }
    }
    public class FundSummaryFilter
    {
        public String DateFrom { get; set; }
        public String DateTo { get; set; }
    }
    public class ReportFilter
    {
        public int CompanyId { get; set; }
        public int FinYearId { get; set; }
        public String DateFrom { get; set; }
        public String DateTo { get; set; }
    }

    public class ClientDashBoardStatsDTO
    {
        public int Challans { get; set; }
        public int ReturnChallans { get; set; }        
        public int Sites { get; set; }
        public long Sales { get; set; }
        public long RentalSale { get; set; }
        public long ContractSale { get; set; }
        public int ContractDelieryChallans { get; set; }
        public int ContractReturnChallans { get; set; }
        public int ActiveContracts { get; set; }
        public int DelayedInstall { get; set; }
        public int DelayedDismantle{ get; set; }

    }
    public class FundSummaryVWDTO
    {

        public decimal BankBalance { get; set; }
        public decimal BankPayment { get; set; }
        public decimal CashPayment { get; set; }
        public decimal CashBalance { get; set; }
    }
    public class FundSummaryDTO
    {
        public int EntryType { get; set; }
        public decimal Amount { get; set; }
    }
    public class StockSummaryDTO
    {
        public string Product { get; set; }
        public decimal OnSite { get; set; }
        public decimal StockInHand { get; set; }
    }

    public class TopItemsOnRent
    {
        public string Product { get; set; }
        public int Qty { get; set; }

    }
    public class GSTRFilterDto
    {
        public int CompanyId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

    }
    public class PnlStatementDto
    {
        public decimal OpeningStock { get; set; }
        public decimal Purchase { get; set; }
        //   public decimal PurchaseTaxPaid { get; set; }

        //public List<PnlStatmentDtoDetail> DirectExpenses { get; set; }
        //public List<PnlStatmentDtoDetail> InDirectExpenses { get; set; }
        //public List<PnlStatmentDtoDetail> Sale { get; set; }
        //public List<PnlStatmentDtoDetail> IndirectIncome { get; set; }

        public decimal DERoundOff { get; set; }
        public decimal DELabourAc { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }

        public decimal GrossLoss { get; set; }
        public decimal NetLoss { get; set; }
        public decimal IESalary { get; set; }
        public decimal IEOfficeExpense { get; set; }

        public decimal Sales { get; set; }

        public decimal ClosingStock { get; set; }
        public decimal IIInterestAc { get; set; }


        public decimal InDirectExpenses { get; set; }
        public decimal DirectExpenses { get; set; }
        public decimal DirectIncome { get; set; }
        public decimal InDirectIncome { get; set; }


        //other than Opening Stock/Purchase/Sale/Closing Stock
        public List<PnlStatmentDtoDetail> Details { get; set; }

        public List<PnlStatmentDtoDetail> Groups { get; set; }

    }
    public class PnlStatementDto_old
    {
        public decimal OpeningStock { get; set; }
        public List<PnlStatmentDtoDetail> Purchase { get; set; }
        //   public decimal PurchaseTaxPaid { get; set; }

        public List<PnlStatmentDtoDetail> DirectExpenses { get; set; }
        public List<PnlStatmentDtoDetail> InDirectExpenses { get; set; }
        public List<PnlStatmentDtoDetail> Sale { get; set; }
        public List<PnlStatmentDtoDetail> IndirectIncome { get; set; }

        //public decimal DERoundOff { get; set; }
        //public decimal DELabourAc { get; set; }
        public decimal GrossProfit { get; set; }
        //public decimal IESalary { get; set; }
        //public decimal IEOfficeExpense { get; set; }
        public decimal NetProfit { get; set; }
        // public decimal Sales { get; set; }
        public decimal DirectIncome { get; set; }
        public decimal ClosingStock { get; set; }
        //  public decimal IIInterestAc { get; set; }

    }

    public class PnlStatmentDtoDetail
    {
        public string Group { get; set; }
        public string AccType { get; set; }
        public int ParentGroupId { get; set; }

        public int AccountGroupId { get; set; }
        public int LedgerId { get; set; }
        public string LedgerName { get; set; }
        public byte TransactionType { get; set; }


        public decimal Amount { get; set; }

    }

    public class BalanceSheetDto
    {

        public decimal InternalLiability { get; set; }
        public decimal Loans { get; set; }
        public decimal CurrentLiability { get; set; }
        public decimal Branch { get; set; }
        public decimal ProfitnLoss { get; set; }

        public decimal Suspense { get; set; }
        public decimal FixedAssets { get; set; }
        public decimal Investments { get; set; }
        public decimal CurrentAssets { get; set; }
        public decimal MiscExpenses { get; set; }

        ///
    }

    public class BillPaymentReportDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double Total { get; set; }
        public string TranRefNumber { get; set; }
        public DateTime TransactionDate { get; set; }

        public string Narration { get; set; }

        public double PaidAmount { get; set; }


    }
    public class AmountReceiveables
    {
        public int LedgerId { get; set; }
        public string Client { get; set; }
        public String Site { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal DueAmount { get; set; }
         

    }

    public class MaterialPikupReminderDto
    {

        public int WorkOrderId { get; set; }
        public string SiteId { get; set; }
        public string Client { get; set; }
        public string Site { get; set; }
        public string DeliveryDate { get; set; }
        public string RecoveryDate { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }

    }

    public class TransporterChargesDto
    {

        public int WorkOrderId { get; set; }
        public string SiteId { get; set; }
        public string Transporter { get; set; }
        public int TransporterId { get; set; }
        public decimal Freight { get; set; }
        public string Vehicle { get; set; }

        public string Driver { get; set; }
        public string ChallanNumber { get; set; }
        public string Client { get; set; }
        public string PicLocation { get; set; }
        public string DropLocation { get; set; }
        public DateTime WorkOrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public string TransporterPhone { get; set; }
        public string TransporterEmail { get; set; }

    }


}
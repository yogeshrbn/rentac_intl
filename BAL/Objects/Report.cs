using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using System.Data;
using System.Windows.Forms;
using System.Windows;
namespace BAL.Objects
{
    public class Report
    {
        public List<ReportDTO> PendingPayments(int daysLeft, string site, string jobNumber)
        {
            ReportDAL dal = new ReportDAL();
            return dal.PendingPayments(daysLeft, site, jobNumber);
        }
        //public static List<ReportDTO> PaymentReceived()
        //{
        //    ReportDAL dal = new ReportDAL();
        //    return dal.PaymentReceived();
        //}

        //public List<ReportDTO> SiteWiseInventory(String site, String jobNumber)
        //{
        //    ReportDAL dal = new ReportDAL();
        //    return dal.SiteWiseInventory(site, jobNumber);

        //}
        //public System.Data.DataTable SiteWiseInventoryExcel(String site)
        //{
        //    ReportDAL dal = new ReportDAL();
        //    return dal.SiteWiseInventoryExcel(site);

        //}
        //public List<ReportDTO> SiteWiseInventorySummary(String site, String jobNumber)
        //{
        //    ReportDAL dal = new ReportDAL();
        //    return dal.SiteWiseInventorySummary(site, jobNumber);

        //}
        //public List<ReportDTO> ClosedSites(String from, String to)
        //{
        //    ReportDAL dal = new ReportDAL();
        //    return dal.ClosedSites(from, to);

        //}
        public List<ReportDTO> DailyInOutTransactions(int ledgerId, int ledgerSiteId, int companyId, string from, string to)
        {
            ReportDAL dal = new ReportDAL();
            return dal.DailyInOutTransactions(ledgerId, ledgerSiteId, companyId, from, to);
        }
        //public List<ReportDTO> DashboardSummary()
        //{

        //    ReportDAL dal = new ReportDAL();
        //    return dal.DashboardSummary();
        //}
        public async Task<IEnumerable<ReportDTO>> MonthlyBillingSummaryForDashBoard(ReportFilter filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.MonthlyBillingSummaryForDashBoard(filter);
        }

        public async Task<IEnumerable<BillPaymentReportDto>> BillPaymentSummary(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.BillPaymentSummary(filter);
        }
        public async Task<IEnumerable<AmountReceiveables>> AmountReceiveable(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.AmountReceiveable(filter);
        }
        public async Task<IEnumerable<VehicleTravelReport>> VehicleTravelReport(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.VehicleTravelReport(filter);
        }
        public DataSet GetReportHeader(int ledgerId, int companyId, int challanHeaderType = 0)
        {
            ReportDAL dal = new ReportDAL();
            return dal.GetReportHeader(ledgerId, companyId, challanHeaderType);
        }
        public DataSet GetReportHeader_Bill(int ledgerId, int companyId, int invoiceId, int challanHeaderType = 0)
        {
            ReportDAL dal = new ReportDAL();
            return dal.GetReportHeader_Bill(ledgerId, companyId, invoiceId, challanHeaderType);
        }

        public List<BillOverDueSummary> BillOverDueSummary(BillOverDueSummaryFilter filter)
        {
            ReportDAL dal = new ReportDAL();
            return dal.BillOverDueSummary(filter);
        }
        public List<NewCustomersDTO> NewCustomers(ReportFilter filter)
        {
            ReportDAL dal = new ReportDAL();
            return dal.NewCustomers(filter);
        }

        public ClientDashBoardStatsDTO ClientDashboardDTO(ReportFilter filter)
        {
            ReportDAL dal = new ReportDAL();
            return dal.ClientDashboardDTO(filter);
        }
        public async Task<RentalVsContractComparisonAnalyticsDto> RentalVsContractComparisonAnalytics(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.RentalVsContractComparisonAnalytics(filter);
        }
        public async Task<CeoForecastingAnalyticsDto> CeoForecastingAnalytics(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.CeoForecastingAnalytics(filter);
        }
        public async Task<CeoEarlyWarningAlertsDto> CeoEarlyWarningAlerts(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.CeoEarlyWarningAlerts(filter);
        }
        public async Task<IEnumerable<CeoRiskProjectDto>> CeoTopRiskProjects(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.CeoTopRiskProjects(filter);
        }
        public async Task<IEnumerable<CeoSalesPipelineDto>> CeoSalesPipeline(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.CeoSalesPipeline(filter);
        }
        public async Task<OperationsSiteKpisDto> OperationsSiteKpis(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.OperationsSiteKpis(filter);
        }
        public async Task<IEnumerable<OperationsDailyActivityDto>> OperationsDailyActivity(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.OperationsDailyActivity(filter);
        }
        public async Task<IEnumerable<OperationsInstallationTeamDailyDto>> OperationsInstallationTeamDaily(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.OperationsInstallationTeamDaily(filter);
        }
        public async Task<AccountsRevenueKpisDto> AccountsRevenueKpis(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.AccountsRevenueKpis(filter);
        }
        public async Task<AccountsAgeingSummaryDto> AccountsAgeingSummary(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.AccountsAgeingSummary(filter);
        }
        public async Task<IEnumerable<AccountsSiteOutstandingDto>> AccountsSiteOutstanding(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.AccountsSiteOutstanding(filter);
        }
        public FundSummaryVWDTO FundSummary(ReportFilter filter)
        {
            var summary = new FundSummaryVWDTO();
            ReportDAL dal = new ReportDAL();
            var s = dal.FundSummary(filter);
            int[] paymentTypes = { 1, 2, 5, 7 };
            var lst = new FundSummaryVWDTO();

            var payment = new FundSummaryVWDTO();
            lst.BankPayment = s.Where(o => paymentTypes.Contains(o.EntryType) == true).Sum(o => o.Amount);


            int[] bankReceived = { 3, 4 };
            var bankBalance = new FundSummaryVWDTO();
            lst.BankBalance = s.Where(o => bankReceived.Contains(o.EntryType) == true).Sum(o => o.Amount) - lst.BankBalance;


            int[] cashPaymentTypes = { 9 };
            var cashPayment = new FundSummaryVWDTO();
            lst.CashPayment = s.Where(o => cashPaymentTypes.Contains(o.EntryType) == true).Sum(o => o.Amount);


            int[] cashReceived = { 8 };
            var cashBalance = new FundSummaryVWDTO();
            lst.CashBalance = s.Where(o => cashReceived.Contains(o.EntryType) == true).Sum(o => o.Amount) - lst.CashPayment;


            return lst;
        }
        public List<TopItemsOnRent> TopItemsOnRent(ReportFilter filter)
        {
            ReportDAL dal = new ReportDAL();
            return dal.TopItemsOnRent(filter);
        }

        public List<StockSummaryDTO> StockSummary(ReportFilter filter)
        {
            ReportDAL dal = new ReportDAL();
            return dal.StockSummary(filter);
        }

        #region GSTrReports
        public async Task<GSTR1SummaryDTO> Gstr11(GSTRFilterDto filter)
        {
            ReportDAL dal = new ReportDAL();
            var _detailedData = await dal.Gstr11(filter);


            var gstrSummary = new GSTR1SummaryDTO();
            gstrSummary.hsnSummary = await dal.GSTR1_HSNSummary(filter);
            gstrSummary.b2b = new GSTR1DTO();
            var b2bData = _detailedData.Where(o => !String.IsNullOrEmpty(o.PartyGST)
                && o.VoucherType != 7
            );
            if (b2bData != null)
            {
                gstrSummary.b2b.TaxAbleAmount = b2bData.Sum(o => o.TaxAbleAmount);
                gstrSummary.b2b.Vouchers = b2bData.Count();
                gstrSummary.b2b.IGSTAmount = b2bData.Sum(o => o.IGSTAmount);
                gstrSummary.b2b.SGSTAmount = b2bData.Sum(o => o.SGSTAmount);
                gstrSummary.b2b.CGSTAmount = b2bData.Sum(o => o.CGSTAmount);
                gstrSummary.b2b.Details = b2bData;
                gstrSummary.b2b.TaxAmount = b2bData.Sum(o => o.IGSTAmount + o.CGSTAmount + o.SGSTAmount);
            }
            //if consumer is of same sate or if cosnumer is from other state and bill amount is less than 25000
            //then it will come under B2c small invoices
            //also includes debit/credit notes (sales returns) of same state
            gstrSummary.b2c_smallInvoice = new GSTR1DTO();
            gstrSummary.b2c_smallInvoice.Details = _detailedData.Where(o => String.IsNullOrEmpty(o.PartyGST)

                                    && ((o.CompanyStateId == o.PartyStateId) ||
                                    (o.VoucherType != 7 && o.CompanyStateId != o.PartyStateId && o.TaxAbleAmount <= 250000)));

            gstrSummary.b2c_smallInvoice.TaxAbleAmount = gstrSummary.b2c_smallInvoice.Details.Sum(o => o.TaxAbleAmount);
            gstrSummary.b2c_smallInvoice.Vouchers = gstrSummary.b2c_smallInvoice.Details.Count();
            gstrSummary.b2c_smallInvoice.IGSTAmount = gstrSummary.b2c_smallInvoice.Details.Sum(o => o.IGSTAmount);
            gstrSummary.b2c_smallInvoice.SGSTAmount = gstrSummary.b2c_smallInvoice.Details.Sum(o => o.SGSTAmount);
            gstrSummary.b2c_smallInvoice.CGSTAmount = gstrSummary.b2c_smallInvoice.Details.Sum(o => o.CGSTAmount);
            gstrSummary.b2c_smallInvoice.TaxAmount = gstrSummary.b2c_smallInvoice.Details.Sum(o => o.IGSTAmount + o.CGSTAmount + o.SGSTAmount);

            //if consumer is from other state and invoice amount is greater than 250000 then 
            //it will come under b2c large invoices
            gstrSummary.b2c_largeInvoice = new GSTR1DTO();
            gstrSummary.b2c_largeInvoice.Details = _detailedData.Where(o => String.IsNullOrEmpty(o.PartyGST)
                                     && o.VoucherType != 7
                                    && (o.CompanyStateId != o.PartyStateId && o.TaxAbleAmount > 250000));

            gstrSummary.b2c_largeInvoice.TaxAbleAmount = gstrSummary.b2c_largeInvoice.Details.Sum(o => o.TaxAbleAmount);
            gstrSummary.b2c_largeInvoice.Vouchers = gstrSummary.b2c_largeInvoice.Details.Count();
            gstrSummary.b2c_largeInvoice.IGSTAmount = gstrSummary.b2c_largeInvoice.Details.Sum(o => o.IGSTAmount);
            gstrSummary.b2c_largeInvoice.SGSTAmount = gstrSummary.b2c_largeInvoice.Details.Sum(o => o.SGSTAmount);
            gstrSummary.b2c_largeInvoice.CGSTAmount = gstrSummary.b2c_largeInvoice.Details.Sum(o => o.CGSTAmount);
            gstrSummary.b2c_largeInvoice.TaxAmount = gstrSummary.b2c_largeInvoice.Details.Sum(o => o.IGSTAmount + o.CGSTAmount + o.SGSTAmount);


            gstrSummary.notesRegistered = new GSTR1DTO();
            gstrSummary.notesRegistered.Details = _detailedData.Where(o => !String.IsNullOrEmpty(o.PartyGST)
                                   && o.VoucherType == 7);
            gstrSummary.notesRegistered.TaxAbleAmount = gstrSummary.notesRegistered.Details.Sum(o => o.TaxAbleAmount);
            gstrSummary.notesRegistered.Vouchers = gstrSummary.notesRegistered.Details.Count();
            gstrSummary.notesRegistered.IGSTAmount = gstrSummary.notesRegistered.Details.Sum(o => o.IGSTAmount);
            gstrSummary.notesRegistered.SGSTAmount = gstrSummary.notesRegistered.Details.Sum(o => o.SGSTAmount);
            gstrSummary.notesRegistered.CGSTAmount = gstrSummary.notesRegistered.Details.Sum(o => o.CGSTAmount);
            gstrSummary.notesRegistered.TaxAmount = gstrSummary.notesRegistered.Details.Sum(o => o.IGSTAmount + o.CGSTAmount + o.SGSTAmount);


            gstrSummary.notesUnRegistered = new GSTR1DTO();
            gstrSummary.notesUnRegistered.Details = _detailedData.Where(o => String.IsNullOrEmpty(o.PartyGST)
                                   && o.VoucherType == 7
                                    && o.PartyStateId != o.CompanyStateId);

            gstrSummary.notesUnRegistered.TaxAbleAmount = gstrSummary.notesUnRegistered.Details.Sum(o => o.TaxAbleAmount);
            gstrSummary.notesUnRegistered.Vouchers = gstrSummary.notesUnRegistered.Details.Count();
            gstrSummary.notesUnRegistered.IGSTAmount = gstrSummary.notesUnRegistered.Details.Sum(o => o.IGSTAmount);
            gstrSummary.notesUnRegistered.SGSTAmount = gstrSummary.notesUnRegistered.Details.Sum(o => o.SGSTAmount);
            gstrSummary.notesUnRegistered.CGSTAmount = gstrSummary.notesUnRegistered.Details.Sum(o => o.CGSTAmount);
            gstrSummary.notesUnRegistered.TaxAmount = gstrSummary.notesUnRegistered.Details.Sum(o => o.IGSTAmount + o.CGSTAmount + o.SGSTAmount);

            gstrSummary.exportInvoices = new GSTR1DTO();
            gstrSummary.nillRated = new GSTR1DTO();
            gstrSummary.advanceAdjustment = new GSTR1DTO();
            gstrSummary.advanceReceived = new GSTR1DTO();
            gstrSummary.advanceRefund = new GSTR1DTO();

            gstrSummary.total = new GSTR1DTO();

            gstrSummary.total.Vouchers = _detailedData.Count();
            gstrSummary.total.TaxAbleAmount = _detailedData.Sum(o => o.TaxAbleAmount);

            gstrSummary.total.TaxAmount = _detailedData.Sum(o => o.IGSTAmount + o.CGSTAmount + o.SGSTAmount);


            return gstrSummary;
        }

        public async Task<IEnumerable<GSTTaxReport>> GstTaxSales(GSTRFilterDto filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.GstTaxSales(filter);
        }
        public async Task<IEnumerable<GSTTaxReport>> GstTaxPurchase(GSTRFilterDto filter)
        {
            ReportDAL dal = new ReportDAL();
            return await dal.GstTaxPurchase(filter);
        }
        public async Task<PnlStatementDto> PnlStatement(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            var data = await dal.PnlStatement(filter);

            data.Groups = (from d in data.Details
                           where d.ParentGroupId == 0
                           group d by d.Group into g

                           select new PnlStatmentDtoDetail
                           {
                               AccountGroupId = g.First().AccountGroupId,
                               Group = g.First().Group,
                               AccType = g.First().AccType,
                               Amount = g.Sum(o => o.Amount)

                           }).ToList();

            foreach (var d in data.Groups)
            {
                d.Amount += getTotalAmountOFAllChildren(data.Details, d.AccountGroupId);
                if (d.AccountGroupId == 23)
                {
                    d.Amount = data.Details.Where(o => o.AccountGroupId == 23 && o.TransactionType == 1).Sum(o => o.Amount);
                    d.Amount -= data.Details.Where(o => o.AccountGroupId == 23 && o.TransactionType == 2).Sum(o => o.Amount);
                }
                if (d.AccountGroupId == 22)
                {
                    d.Amount = data.Details.Where(o => o.AccountGroupId == 22 && o.TransactionType == 2).Sum(o => o.Amount);
                    d.Amount -= data.Details.Where(o => o.AccountGroupId == 22 && o.TransactionType == 1).Sum(o => o.Amount);
                }
            }

            data.DirectExpenses = data.Details.Where(o => o.AccountGroupId == 26).Sum(o => o.Amount);
            data.InDirectExpenses = data.Details.Where(o => o.AccountGroupId == 27).Sum(o => o.Amount);
            data.DirectIncome = data.Details.Where(o => o.AccountGroupId == 24).Sum(o => o.Amount);
            data.InDirectIncome = data.Details.Where(o => o.AccountGroupId == 25).Sum(o => o.Amount);
            data.Sales = data.Groups.FirstOrDefault(o => o.AccountGroupId == 22).Amount;
            data.Purchase = data.Groups.FirstOrDefault(o => o.AccountGroupId == 23).Amount;

            data.GrossProfit = data.Sales + data.ClosingStock - (data.OpeningStock + data.Purchase + data.DirectExpenses);

            if (data.GrossProfit < 0)
            {
                data.GrossLoss = Math.Abs(data.GrossProfit);
                data.NetLoss = data.GrossLoss + data.InDirectExpenses + data.InDirectIncome;

                data.GrossProfit = 0;
                data.NetProfit = 0;



            }
            else
                data.NetProfit = (data.GrossProfit - data.InDirectExpenses) + data.InDirectIncome;


            return data;
        }
        decimal getTotalAmountOFAllChildren(List<PnlStatmentDtoDetail> list, int groupId)
        {
            decimal totalAmount = 0;
            var children = list.Where(o => o.ParentGroupId == groupId);
            totalAmount = children.Sum(o => o.Amount);
            foreach (var child in children)
            {
                totalAmount += getTotalAmountOFAllChildren(list, child.AccountGroupId);
            }
            return totalAmount;


        }



        public async Task<PnlStatementDto> BalanceSheet(FilterCriteria filter)
        {
            ReportDAL dal = new ReportDAL();
            var _sheet = new PnlStatementDto();
            _sheet.Details = (await dal.BalanceSheet(filter)).ToList();

            _sheet.Groups = (from d in _sheet.Details
                             where d.ParentGroupId == 0
                             group d by d.Group into g

                             select new PnlStatmentDtoDetail
                             {
                                 AccountGroupId = g.First().AccountGroupId,
                                 Group = g.First().Group,
                                 AccType = g.First().AccType,
                                 Amount = g.Sum(o => o.Amount)

                             }).ToList();

            foreach (var d in _sheet.Groups)
            {
                d.Amount += getTotalAmountOFAllChildren(_sheet.Details, d.AccountGroupId);
            }


            var pnl = await PnlStatement(filter);

            if (pnl.GrossProfit > 0)
            {
                var profit = new PnlStatmentDtoDetail();
                profit.Group = "Gross Profit";
                profit.AccType = "Liability";
                profit.Amount = pnl.GrossProfit;
                _sheet.Groups.Add(profit);
            }
            else if (pnl.GrossLoss > 0)
            {
                var profit = new PnlStatmentDtoDetail();
                profit.Group = "Gross Loss";
                profit.AccType = "Asset";
                profit.Amount = pnl.GrossLoss;
                _sheet.Groups.Add(profit);
            }

            return _sheet;
        }
        #endregion
        public async Task<IEnumerable<WorkOrderItemDTO>> DeliveryChallans(FilterCriteria filter)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.DeliveryChallans(filter.From, filter.To, filter.ChallanType, filter.LedgerId, filter.LedgerSiteId, filter.CompanyId);
        }
        public async Task<IEnumerable<GRNDTO>> ReturnChallans(FilterCriteria filter)
        {
            WorkorderDAL dal = new WorkorderDAL();

            return await dal.ReturnChallans(filter.From, filter.To, filter.ChallanType, filter.LedgerId, filter.LedgerSiteId, filter.CompanyId);

        }

        public async Task<IEnumerable<GRNDTO>> BreakageReport(FilterCriteria filter)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.PartyBreakageReport(filter.From, filter.To, filter.ChallanType, filter.LedgerId, filter.LedgerSiteId, filter.CompanyId);
        }

        public async Task<IEnumerable<GRNDTO>> LostReport(FilterCriteria filter)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.PartyLostReport(filter.From, filter.To, filter.ChallanType, filter.LedgerId, filter.LedgerSiteId, filter.CompanyId);
        }

        public async Task<IEnumerable<GRNDTO>> ExcessReport(FilterCriteria filter)
        {
            WorkorderDAL dal = new WorkorderDAL();
            return await dal.PartyExcessReport(filter.From, filter.To, filter.ChallanType, filter.LedgerId, filter.LedgerSiteId, filter.CompanyId);
        }

        public async Task<IEnumerable<LedgerTransactionDTO>> PartySiteWisePayments(FilterCriteria filter)
        {
            LedgerDAL dal = new LedgerDAL();
            return await dal.PartySiteWisePayments(filter);
        }

        /// <summary>
        /// Gets party payment transactions (EntryType 9 - payment vouchers) for the Party Payments report.
        /// </summary>
        public List<LedgerTransactionDTO> PartyPayments(FilterCriteria filter)
        {
            var ledger = new Ledger();
            return ledger.GetReceiptRegister(filter.LedgerId, filter.LedgerSiteId, Convert.ToDateTime(filter.From),
               Convert.ToDateTime(filter.To), filter.EntryType, filter.CompanyId);
        }
        public async Task<IEnumerable<MaterialPikupReminderDto>> MaterialPickupReminder(FilterCriteria filter)
        {
            var reportDal = new ReportDAL();
            return await reportDal.MaterialPickupReminder(filter);
        }
        public async Task<IEnumerable<TransporterChargesDto>> TransporterReport(FilterCriteria filter)
        {
            var reportDal = new ReportDAL();
            return await reportDal.TransporterReport(filter);
        }
    }

}
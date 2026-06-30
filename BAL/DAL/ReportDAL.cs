using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using System.Data;
using System.Globalization;
using Omu.ValueInjecter;
using System.CodeDom;

namespace BAL.DAL
{
    public class ReportDAL
    {

        public List<ReportDTO> PendingPayments(int daysLeft, string site, string jobNumber)
        {
            SQL objSql = new SQL();

            if (daysLeft > 0)
                objSql.AddParameter("@DaysLeft", DbType.Int16, ParameterDirection.Input, 0, daysLeft);
            if (!String.IsNullOrEmpty(site))
                objSql.AddParameter("@Site", DbType.String, ParameterDirection.Input, 0, site);
            if (!String.IsNullOrEmpty(jobNumber))
                objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, jobNumber);
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(UPCOMING_PAYMENTS));
        }
        public List<ReportDTO> SiteWiseInventory(String site, string jobNumber)
        {
            SQL objSql = new SQL();

            if (!String.IsNullOrEmpty(site))
                objSql.AddParameter("@SiteName", DbType.String, ParameterDirection.Input, 0, site);
            if (!String.IsNullOrEmpty(jobNumber))
                objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, jobNumber);
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(SITE_WISE_INVENTORY));
        }
        public DataTable SiteWiseInventoryExcel(String site)
        {
            SQL objSql = new SQL();

            if (!String.IsNullOrEmpty(site))
                objSql.AddParameter("@SiteName", DbType.String, ParameterDirection.Input, 0, site);
            //if (!String.IsNullOrEmpty(jobNumber))
            //    objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, jobNumber);
            DataTable tbl = new DataTable();
            DataSet ds = objSql.ExecuteDataSet(SITE_WISE_INVENTORY_EXCEL);
            if (ds.Tables.Count > 0)
            {
                tbl = ds.Tables[0];
            }
            return tbl;
        }
        public List<ReportDTO> SiteWiseInventorySummary(String site, string jobNumber)
        {
            SQL objSql = new SQL();

            if (!String.IsNullOrEmpty(site))
                objSql.AddParameter("@SiteName", DbType.String, ParameterDirection.Input, 0, site);
            if (!String.IsNullOrEmpty(jobNumber))
                objSql.AddParameter("@JobNumber", DbType.String, ParameterDirection.Input, 0, jobNumber);
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(SITE_WISE_INVENTORY_SUMMARY));
        }
        public List<ReportDTO> ClosedSites(String from, String to)
        {
            SQL objSql = new SQL();

            if (!String.IsNullOrEmpty(from) && DateTime.Parse(from).Year > 1970)
                objSql.AddParameter("@from", DbType.String, ParameterDirection.Input, 0, from);
            if (!String.IsNullOrEmpty(to) && DateTime.Parse(to).Year > 1970)
                objSql.AddParameter("@to", DbType.String, ParameterDirection.Input, 0, to);
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(CLOSED_SITES));
        }
        public List<ReportDTO> PaymentReceived()
        {
            SQL objSql = new SQL();
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(PAYMENTS_RECEIVED));
        }
        public List<ReportDTO> DashboardSummary()
        {
            SQL objSql = new SQL();
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(DASHBOARD_SUMMARY));
        }
        public List<ReportDTO> DailyInOutTransactions(int ledgerId, int ledgerSiteId, int companyId, string from, string to)
        {
            SQL objSql = new SQL();
            // objSql.AddParameter("@rbnClientId", DbType.Int16, ParameterDirection.Input, 0, rbnClientId);
            if (!String.IsNullOrEmpty(from) && DateTime.Parse(from).Year > 1970)
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, from);
            if (!String.IsNullOrEmpty(to) && DateTime.Parse(to).Year > 1970)
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, to);
            if (ledgerId > 0)
            {
                objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            }
            if (ledgerSiteId > 0)
            {
                objSql.AddParameter("@LedgerSiteId", DbType.Int16, ParameterDirection.Input, 0, ledgerSiteId);

            }
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(DAILY_INOUT_TRANSACTIONS));
        }
        public DataSet GetReportHeader(int ledgerId, int companyId, int challanHeaderType = 0)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            return objSql.ExecuteDataSet(REPORT_HEADER);

        }
        public DataSet GetReportHeader_Bill(int ledgerId, int companyId, int invoiceId, int challanHeaderType = 0)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, companyId);
            objSql.AddParameter("@CopyType", DbType.Int16, ParameterDirection.Input, 0, challanHeaderType);
            objSql.AddParameter("@invoiceId", DbType.Int16, ParameterDirection.Input, 0, invoiceId);


            return objSql.ExecuteDataSet(REPORT_HEADER_V2);

        }

        public List<BillOverDueSummary> BillOverDueSummary(BillOverDueSummaryFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@dueFrom", DbType.Int16, ParameterDirection.Input, 0, filter.DueFromDays);
                var ds = objSql.ExecuteDataSet(BILL_OVER_DUESUMMARY);

                var ret = (from d in ds.Tables[0].AsEnumerable()
                           select new BillOverDueSummary
                           {
                               Amount = Math.Abs(d.Field<decimal>("amountDue")),
                               ClientName = d.Field<string>("Name"),
                               BalanceType = d["BalanceType"] == DBNull.Value ? null : d.Field<string>("BalanceType"),
                               DueFrom = Math.Abs(d.Field<int>("dueFrom")),

                           }
                         ).ToList();
                return ret;
            }
        }

        public List<NewCustomersDTO> NewCustomers(ReportFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateFrom));
                objSql.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateTo, new CultureInfo("en-GB")));
                objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, filter.FinYearId);
                var ds = objSql.ExecuteDataSet(NEW_CUSTOMERS);

                var ret = (from d in ds.Tables[0].AsEnumerable()
                           select new NewCustomersDTO
                           {
                               Customers = d.Field<Int32>("customers"),
                               Month = d.Field<Int32>("month"),
                               Year = d.Field<Int32>("year"),
                           }
                         ).ToList();

                foreach (var d in ret)
                {
                    d.MonthYear = new DateTime(d.Year, d.Month, 1).ToString("MMM yy");
                }

                return ret;
            }
        }
        public ClientDashBoardStatsDTO ClientDashboardDTO(ReportFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateFrom, new CultureInfo("en-GB")));
                objSql.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateTo, new CultureInfo("en-GB")));
                objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, filter.FinYearId);
                var ds = objSql.ExecuteDataSet(CLIENT_DASHBOARD_STATS);

                var ret = (from d in ds.Tables[0].AsEnumerable()
                           select new ClientDashBoardStatsDTO
                           {
                               Challans = d.Field<Int32>("challans"),
                               Sites = d.Field<Int32>("sites"),
                               Sales = d.Field<Int64>("totalSales"),
                               ReturnChallans = d.Field<Int32>("returnChallans"),
                               RentalSale = d.Field<Int64>("rentalSale"),
                               ContractSale = d.Field<Int64>("contractSale"),
                               ContractDelieryChallans = d.Field<Int32>("contractDelieryChallans"),
                               ContractReturnChallans = d.Field<Int32>("contractReturnChallans"),
                               ActiveContracts = d.Field<Int32>("activeContracts"),
                               DelayedInstall = d.Field<Int32>("delayedInstall"),
                               DelayedDismantle = d.Field<Int32>("delayedDismantle"),
                           }
                         ).FirstOrDefault();



                return ret;
            }
        }
        public List<FundSummaryDTO> FundSummary(ReportFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateFrom, new CultureInfo("en-GB")));
                objSql.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateTo, new CultureInfo("en-GB")));

                var ds = objSql.ExecuteDataSet(FUND_SUMMARY);

                var ret = (from d in ds.Tables[0].AsEnumerable()
                           select new FundSummaryDTO
                           {
                               EntryType = d.Field<Int32>("entryType"),
                               Amount = d.Field<decimal>("amount"),

                           }
                         ).ToList();



                return ret;
            }
        }
        public List<StockSummaryDTO> StockSummary(ReportFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, filter.FinYearId);


                var ds = objSql.ExecuteDataSet(STOCK_SUMMARY_ASOFTODAY);

                var ret = (from d in ds.Tables[0].AsEnumerable()
                           select new StockSummaryDTO
                           {
                               Product = d.Field<string>("product"),
                               OnSite = Convert.ToInt32(d["onsite"]),
                               StockInHand = Convert.ToInt32(d["stockinhand"]),
                           }
                         ).ToList();



                return ret;
            }
        }
        public List<TopItemsOnRent> TopItemsOnRent(ReportFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@finYearId", DbType.Int16, ParameterDirection.Input, 0, filter.FinYearId);
                objSql.AddParameter("@fromDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateFrom, new CultureInfo("en-GB")));
                objSql.AddParameter("@toDate", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateTo, new CultureInfo("en-GB")));
                var ds = objSql.ExecuteDataSet(TOP_ITEMS_ON_RENT);
                var ret = (from d in ds.Tables[0].AsEnumerable()
                           select new TopItemsOnRent
                           {
                               Product = d.Field<string>("product"),
                               Qty = d.Field<int>("SentQty")
                           }
                         ).ToList();
                return ret;
            }
        }
        public async Task<IEnumerable<GSTR1DetailDTO>> Gstr11(GSTRFilterDto filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@month", DbType.Int16, ParameterDirection.Input, 0, filter.Month);
                objSql.AddParameter("@year", DbType.Int16, ParameterDirection.Input, 0, filter.Year);
                return await objSql.QueryAsync<GSTR1DetailDTO>(GSTR1);

            }
        }
        public async Task<IEnumerable<HSNSummaryDTO>> GSTR1_HSNSummary(GSTRFilterDto filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int16, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@month", DbType.Int16, ParameterDirection.Input, 0, filter.Month);
                objSql.AddParameter("@year", DbType.Int16, ParameterDirection.Input, 0, filter.Year);
                return await objSql.QueryAsync<HSNSummaryDTO>("p_gstr1_hsnSummary");

            }
        }
        public async Task<IEnumerable<GSTTaxReport>> GstTaxSales(GSTRFilterDto filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryAsync<GSTTaxReport>(p_gsttax_sales);
            }
        }
        public async Task<IEnumerable<GSTTaxReport>> GstTaxPurchase(GSTRFilterDto filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryAsync<GSTTaxReport>(p_gsttax_purchase);
            }
        }
        public async Task<PnlStatementDto> PnlStatement(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                //   return await objSql.QueryAsync<LedgerTransactionDTO>(PROFIT_AND_LOSS_STATEMENT);
                var pnl = await objSql.QueryFirstAsync<PnlStatementDto>(PROFIT_AND_LOSS_STATEMENT);

                var details = await objSql.QueryAsync<PnlStatmentDtoDetail>(PROFIT_AND_LOSS_STATEMENT_DETAIL);
                if (pnl != null)
                {

                    if (details != null)
                    {
                        pnl.Details = details.ToList();
                    }
                }
                return pnl;
            }

        }
        public async Task<IEnumerable<PnlStatmentDtoDetail>> BalanceSheet(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);

                var pnl = await objSql.QueryAsync<PnlStatmentDtoDetail>(BALANCE_SHEET);


                return pnl;
            }

        }
        public async Task<IEnumerable<ReportDTO>> MonthlyBillingSummaryForDashBoard(ReportFilter filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateFrom, new CultureInfo("en-GB")));
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, Convert.ToDateTime(filter.DateTo, new CultureInfo("en-GB")));

                var pnl = await objSql.QueryAsync<ReportDTO>(DASH_BOARD_BILLING_CHART);


                return pnl;
            }

        }
        public async Task<IEnumerable<BillPaymentReportDto>> BillPaymentSummary(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);

                var pnl = await objSql.QueryAsync<BillPaymentReportDto>(BILL_PAYMENT_SUMMARY);


                return pnl;
            }

        }
        public async Task<IEnumerable<AmountReceiveables>> AmountReceiveable(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@ledgerId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerId);
                objSql.AddParameter("@ledgerSiteId", DbType.Int32, ParameterDirection.Input, 0, filter.LedgerSiteId);

                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);

                var pnl = await objSql.QueryAsync<AmountReceiveables>("p_amountReceiveable");


                return pnl;
            }

        }
        public async Task<IEnumerable<VehicleTravelReport>> VehicleTravelReport(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                objSql.AddParameter("@vehicleNumber", DbType.String, ParameterDirection.Input, 0, filter.VehicleNo);

                return await objSql.QueryAsync<VehicleTravelReport>(VEHICLE_TRAVEL_DETAILS);


            }

        }
        public async Task<IEnumerable<MaterialPikupReminderDto>> MaterialPickupReminder(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                //objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                //objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                //objSql.AddParameter("@vehicleNumber", DbType.String, ParameterDirection.Input, 0, filter.VehicleNo);

                return await objSql.QueryAsync<MaterialPikupReminderDto>(MATERIAL_PICKUP_REMINDER);


            }

        }
        public async Task<IEnumerable<TransporterChargesDto>> TransporterReport(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                objSql.AddParameter("@transporterId", DbType.String, ParameterDirection.Input, 0, filter.TransporterId);

                return await objSql.QueryAsync<TransporterChargesDto>(TRANSPORTER_REPORT);


            }

        }
        public async Task<RentalVsContractComparisonAnalyticsDto> RentalVsContractComparisonAnalytics(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryFirstAsync<RentalVsContractComparisonAnalyticsDto>(CEO_RENTAL_CONTRACT_COMPARISON);
            }
        }
        public async Task<CeoForecastingAnalyticsDto> CeoForecastingAnalytics(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryFirstAsync<CeoForecastingAnalyticsDto>(CEO_FORECASTING_ANALYTICS);
            }
        }
        public async Task<CeoEarlyWarningAlertsDto> CeoEarlyWarningAlerts(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryFirstAsync<CeoEarlyWarningAlertsDto>(CEO_EARLY_WARNING_ALERTS);
            }
        }
        public async Task<IEnumerable<CeoRiskProjectDto>> CeoTopRiskProjects(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryAsync<CeoRiskProjectDto>(CEO_TOP_RISK_PROJECTS);
            }
        }
        public async Task<IEnumerable<CeoSalesPipelineDto>> CeoSalesPipeline(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryAsync<CeoSalesPipelineDto>(CEO_SALES_PIPELINE);
            }
        }
        public async Task<OperationsSiteKpisDto> OperationsSiteKpis(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                return await objSql.QueryFirstAsync<OperationsSiteKpisDto>(OPERATIONS_SITE_KPIS);
            }
        }
        public async Task<IEnumerable<OperationsDailyActivityDto>> OperationsDailyActivity(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryAsync<OperationsDailyActivityDto>(OPERATIONS_DAILY_ACTIVITY);
            }
        }
        public async Task<IEnumerable<OperationsInstallationTeamDailyDto>> OperationsInstallationTeamDaily(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryAsync<OperationsInstallationTeamDailyDto>(OPERATIONS_INSTALLATION_TEAM_DAILY);
            }
        }
        public async Task<AccountsRevenueKpisDto> AccountsRevenueKpis(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                objSql.AddParameter("@from", DbType.Date, ParameterDirection.Input, 0, filter.From);
                objSql.AddParameter("@to", DbType.Date, ParameterDirection.Input, 0, filter.To);
                return await objSql.QueryFirstAsync<AccountsRevenueKpisDto>(ACCOUNTS_REVENUE_KPIS);
            }
        }
        public async Task<AccountsAgeingSummaryDto> AccountsAgeingSummary(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                return await objSql.QueryFirstAsync<AccountsAgeingSummaryDto>(ACCOUNTS_AGEING_SUMMARY);
            }
        }
        public async Task<IEnumerable<AccountsSiteOutstandingDto>> AccountsSiteOutstanding(FilterCriteria filter)
        {
            using (SQL objSql = new SQL())
            {
                objSql.AddParameter("@CompanyId", DbType.Int32, ParameterDirection.Input, 0, filter.CompanyId);
                return await objSql.QueryAsync<AccountsSiteOutstandingDto>(ACCOUNTS_SITE_OUTSTANDING);
            }
        }
        #region Procedures
        const string UPCOMING_PAYMENTS = "p_pendingPayments";
        const string PAYMENTS_RECEIVED = "p_sitePayments_AsofDate";
        const string SITE_WISE_INVENTORY = "p_SiteWise_Inentory";
        const string SITE_WISE_INVENTORY_SUMMARY = "p_SiteWise_Inentory_Summary";
        const string SITE_WISE_INVENTORY_EXCEL = "p_download_SiteInventory";
        const string CLOSED_SITES = "p_ClosedSites_sel";
        const string DASHBOARD_SUMMARY = "p_DashBoardSummary_sel";
        const string DAILY_INOUT_TRANSACTIONS = "p_datewiseInOutTransactions";
        const string REPORT_HEADER = "p_ReportHeader_rpt";
        const string REPORT_HEADER_V2 = "p_ReportHeaderv2";
        const string BILL_OVER_DUESUMMARY = "p_overdueAmount";
        const string NEW_CUSTOMERS = "p_newCustomers";
        const string CLIENT_DASHBOARD_STATS = "p_clientStats";
        const string FUND_SUMMARY = "p_fundSummary";
        const string STOCK_SUMMARY_ASOFTODAY = "p_StockAsOnToday";
        const string TOP_ITEMS_ON_RENT = "p_topItemsOnRent";
        const string GSTR1 = "p_gstr1";
        const string GSTR3B = "p_topItemsOnRent";
        const string p_gsttax_sales = "p_gsttax_sales";
        const string p_gsttax_purchase = "p_gsttax_purchase";
        const string PROFIT_AND_LOSS_STATEMENT = "p_pnlStatement";
        const string PROFIT_AND_LOSS_STATEMENT_DETAIL = "p_pnlStatement_detail";
        const string BALANCE_SHEET = "p_balanceStatement";
        const string DASH_BOARD_BILLING_CHART = "p_dashboardBillingChart";
        const string BILL_PAYMENT_SUMMARY = "p_billPaymentReport";
        const string VEHICLE_TRAVEL_DETAILS = "p_vehicle_travelreport";
        const string MATERIAL_PICKUP_REMINDER = "p_materialPickupReminders";
        const string TRANSPORTER_REPORT = "p_transportReport";
        const string CEO_RENTAL_CONTRACT_COMPARISON = "p_ceo_rentalContractComparison";
        const string CEO_FORECASTING_ANALYTICS = "p_ceo_forecastingAnalytics";
        const string CEO_EARLY_WARNING_ALERTS = "p_ceo_earlyWarningAlerts";
        const string CEO_TOP_RISK_PROJECTS = "p_ceo_topRiskProjects";
        const string CEO_SALES_PIPELINE = "p_ceo_salesPipeline";
        const string OPERATIONS_SITE_KPIS = "p_operations_siteKpis";
        const string OPERATIONS_DAILY_ACTIVITY = "p_operations_dailyActivity";
        const string OPERATIONS_INSTALLATION_TEAM_DAILY = "p_operations_installationTeamDaily";
        const string ACCOUNTS_REVENUE_KPIS = "p_accounts_revenueKpis";
        const string ACCOUNTS_AGEING_SUMMARY = "p_accounts_ageingSummary";
        const string ACCOUNTS_SITE_OUTSTANDING = "p_accounts_siteWiseOutstanding";

        #endregion
    }
}

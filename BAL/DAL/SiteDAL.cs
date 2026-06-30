using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
using System.Data;
namespace BAL.DAL
{
    internal class SiteDAL
    {

        public List<PayReminderDTO> GetAllReminders(int _siteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, _siteId);
            return objSql.ContructList<PayReminderDTO>(objSql.ExecuteDataSet(SELECT));
        }
        public bool AddReminder(PayReminderDTO dto)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, dto.SiteId);
            objSql.AddParameter("@Days", DbType.Int32, ParameterDirection.Input, 0, dto.Days);
            objSql.AddParameter("@Amount", DbType.Int32, ParameterDirection.Input, 0, dto.Amount);
            objSql.AddParameter("@CreatedBy", DbType.Int32, ParameterDirection.Input, 0, dto.CreatedBy);
            return objSql.ExecuteNonQuery(ADD) > 0;

        }

        public bool DeletePayReminder(int _paymentRedminderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@PaymentReminderId", DbType.Int32, ParameterDirection.Input, 0, _paymentRedminderId);
            return objSql.ExecuteNonQuery(DELETE) > 0;

        }
        public SiteDTO GetInfo(int siteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@SiteId", DbType.Int32, ParameterDirection.Input, 0, siteId);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(GETSITEINFO)).FirstOrDefault();

        }
        public static List<SiteDTO> GetAllSiteNames()
        {
            SQL objSql = new SQL();
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(GET_ALL_SITENAMES));
        }
        public static List<WorkOrderItemDTO> GetSiteGRN(int workOrderId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@workOrderId", DbType.Int32, ParameterDirection.Input, 0, workOrderId);
            return objSql.ContructList<WorkOrderItemDTO>(objSql.ExecuteDataSet(GET_SITE_GRN));
        }
        public List<SiteDTO> GetSites(int ledgerId,string siteName)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@siteName", DbType.String, ParameterDirection.Input, 0, siteName);
            if (ledgerId > 0)
            {
                objSql.AddParameter("@LedgerId", DbType.Int16, ParameterDirection.Input, 0, ledgerId);
            }
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(GET_SITES));
        }
        public bool CloseSite(int siteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@siteId", DbType.String, ParameterDirection.Input, 0, siteId);
            return objSql.ExecuteNonQuery(CLOSE_SITE) > 0;
        }
        public bool ClosePayment(int siteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@siteId", DbType.String, ParameterDirection.Input, 0, siteId);
            return objSql.ExecuteNonQuery(CLOSE_PAYMENT) > 0;
        }
        public List<SiteDTO> ClosedSites(bool closed)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@closed", DbType.String, ParameterDirection.Input, 0, closed);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(CLOSED_SITE));
        }
        public List<SiteDTO> ClosedJobNumbers(bool closed)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@closed", DbType.String, ParameterDirection.Input, 0, closed);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(CLOSED_JOBNUMBERS));
        }
        public List<ReportDTO> SitePaymentSummary(string site)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@Site", DbType.String, ParameterDirection.Input, 0, site);
            return objSql.ContructList<ReportDTO>(objSql.ExecuteDataSet(SITE_PAYMENT_SUMMARY));
        }
        public List<SiteDTO> GetSiteJobs(int ledgerSiteId)
        {
            SQL objSql = new SQL();
            objSql.AddParameter("@LedgerSiteId", DbType.Int32, ParameterDirection.Input, 0, ledgerSiteId);
            return objSql.ContructList<SiteDTO>(objSql.ExecuteDataSet(LEDTER_SITE_JOBS));
        }
        #region Procedures
        const string ADD = "p_PaymentReminder_ins";
        const string SELECT = "p_PaymentReminder_sel";
        const string DELETE = "p_PaymentReminder_del";
        const string GETSITEINFO = "p_SiteInfo_selById";
        const string GET_ALL_SITENAMES = "p_SiteNames_sel";
        const string GET_SITE_GRN = "p_GetSiteGRN_sel";
        const string GET_SITES = "p_GetSites_sel";
        const string CLOSE_SITE = "p_CloseSite";
        const string CLOSE_PAYMENT = "p_SitePaymentClosed";
        const string CLOSED_SITE = "p_GetSiteClosed";
        const string CLOSED_JOBNUMBERS = "p_GetJobNumbersClosed";
        const string SITE_PAYMENT_SUMMARY = "p_SitePayment_Summary";
        const string LEDTER_SITE_JOBS = "p_LedgerSite_Jobs";
     

        #endregion
    }
}

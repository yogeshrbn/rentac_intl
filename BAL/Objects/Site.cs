using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;

namespace BAL.Objects
{
    public class Site : SiteDTO
    {

        public List<PayReminderDTO> GetAllPayReminders()
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.GetAllReminders(this.SiteId);

        }

        public bool DeletePayReminder(int _paymentRedminderId)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.DeletePayReminder(_paymentRedminderId);
        }

        public bool AddPayReminder(PayReminderDTO dto)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.AddReminder(dto);

        }
        public SiteDTO GetInfo()
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.GetInfo(this.SiteId);

        }

        public static List<SiteDTO> GetAllSiteNames()
        {
            return SiteDAL.GetAllSiteNames();
        }

        public static List<WorkOrderItemDTO> GetSiteGRN(int workOrderId)
        {
            return SiteDAL.GetSiteGRN(workOrderId);
        }

        public List<SiteDTO> GetSites(int ledgerId,String siteName)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.GetSites(ledgerId,siteName);

        }
        public bool CloseSite(int siteId)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.CloseSite(siteId);
        }
        public bool ClosePayment(int siteId)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.ClosePayment(siteId);
        }

        public List<SiteDTO> ClosedSites(bool closed)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.ClosedSites(closed);
        }
        public List<SiteDTO> ClosedJobNumbers(bool closed)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.ClosedJobNumbers(closed);
        }
        public List<ReportDTO> SitePaymentSummary(string site)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.SitePaymentSummary(site);
        }

        public List<SiteDTO> GetSiteJobs(int ledgerSiteId)
        {
            SiteDAL objDal = new SiteDAL();
            return objDal.GetSiteJobs(ledgerSiteId);
        }
    }

}

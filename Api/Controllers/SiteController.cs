using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class SiteController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetAllPayReminder([FromBody] SiteDTO dto)
        {
            try
            {
                Site payObj = new Site() { SiteId = dto.SiteId };
                return Request.CreateResponse(HttpStatusCode.OK, payObj.GetAllPayReminders());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage DeletePayReminder([FromBody] PayReminderDTO dto)
        {
            try
            {
                Site payObj = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, payObj.DeletePayReminder(dto.PayReminderId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage AddPayReminder([FromBody] PayReminderDTO dto)
        {
            try
            {
                Site payObj = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, payObj.AddPayReminder(dto));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetSiteInfo([FromBody] SiteDTO dto)
        {
            try
            {
                Site payObj = new Site() { SiteId = dto.SiteId };
                return Request.CreateResponse(HttpStatusCode.OK, payObj.GetInfo());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        public HttpResponseMessage GetAllSiteNames()
        {
            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, Site.GetAllSiteNames());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage GetSiteGRN([FromBody] SiteDTO dto)
        {
            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, Site.GetSiteGRN(dto.WorkOrderId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage CloseSite([FromBody] SiteDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, objSite.CloseSite(dto.SiteId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage CloseSitePayment([FromBody] SiteDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, objSite.ClosePayment(dto.SiteId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage GetJobNumbers([FromBody] SiteDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, (objSite.GetSites(dto.LedgerId, dto.JobNumber)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
		 [HttpPost]
        public HttpResponseMessage GetSiteJobs([FromBody] SiteDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, (objSite.GetSiteJobs(dto.LedgerSiteId)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage GetOpenJobNumbers([FromBody] SiteDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, (objSite.ClosedJobNumbers(dto.Closed)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage GetClosedSites([FromBody] SiteDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, (objSite.ClosedSites(dto.Closed)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        public HttpResponseMessage SitePaymentSummary([FromBody] ReportDTO dto)
        {
            try
            {
                Site objSite = new Site();
                return Request.CreateResponse(HttpStatusCode.OK, (objSite.SitePaymentSummary(dto.Site)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
    }
}

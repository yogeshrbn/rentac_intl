using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;

namespace FarmaAPI.Controllers
{
    public class AccountsDashboardController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> RevenueKpis(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.AccountsRevenueKpis(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> AgeingSummary()
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId
            };

            var data = await report.AccountsAgeingSummary(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> SiteWiseOutstanding()
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId
            };

            var data = await report.AccountsSiteOutstanding(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}

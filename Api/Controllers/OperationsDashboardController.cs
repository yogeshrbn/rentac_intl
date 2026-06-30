using System.Net;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;

namespace FarmaAPI.Controllers
{
    public class OperationsDashboardController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> SiteKpis()
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId
            };

            var data = await report.OperationsSiteKpis(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> DailyActivity(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.OperationsDailyActivity(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> InstallationTeamDaily(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.OperationsInstallationTeamDaily(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}

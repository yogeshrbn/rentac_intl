using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;

namespace FarmaAPI.Controllers
{
    public class CeoDashboardController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> RentalVsContractComparison(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.RentalVsContractComparisonAnalytics(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ForecastingAnalytics(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.CeoForecastingAnalytics(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> EarlyWarningAlerts(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.CeoEarlyWarningAlerts(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> TopRiskProjects(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.CeoTopRiskProjects(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> SalesPipeline(DateTime from, DateTime to)
        {
            var user = new LoggedInUser();
            var report = new Report();

            var filter = new FilterCriteria
            {
                CompanyId = user.DefaultCompanyId,
                From = Convert.ToString(from),
                To = Convert.ToString(to)
            };

            var data = await report.CeoSalesPipeline(filter);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}


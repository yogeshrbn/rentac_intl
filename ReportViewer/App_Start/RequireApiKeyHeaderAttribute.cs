using BAL.DTO;
using BAL.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ReportViewer
{
    public sealed class RequireApiKeyHeaderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.Contains("ApiKey"))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    "Missing required header: ApiKey");
                return;
            }

            var apiKey = actionContext.Request.Headers.GetValues("ApiKey").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    "Header ApiKey is empty.");
                return;
            }

            var company = GetCompanyByApiKey(apiKey);
            if (company == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    "Invalid ApiKey.");
                return;
            }

            HttpContext.Current.Items["DefaultCompanyId"] = company.CompanyId;
            base.OnActionExecuting(actionContext);
        }

        private static CompanyDTO GetCompanyByApiKey(string apiKey)
        {
            return new Company().ApiKeyExists(apiKey);
        }
    }
}

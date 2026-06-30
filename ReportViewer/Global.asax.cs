using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BAL.Exceptions;
using BAL.Common;

namespace ReportViewer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected void Application_Error()
        {
            Exception ex = Server.GetLastError();
            HttpContext context = HttpContext.Current;

            string controller = context?.Request?.RequestContext?.RouteData?.Values["controller"]?.ToString();
            string action = context?.Request?.RequestContext?.RouteData?.Values["action"]?.ToString();
            string url = context?.Request?.Url?.ToString();
            string user = context?.User?.Identity?.Name ?? "Anonymous";
            context.Response.StatusCode = 500;


            string exceptionMessage = string.Empty;
            if (ex.InnerException == null)
            {
                exceptionMessage = ex.Message;
            }
            else
            {
                exceptionMessage = ex.InnerException.Message;
            }
            var apiMessage = new ApiResponseMessage();
            apiMessage.Message = exceptionMessage;
            apiMessage.Code = ApiResponseMessageCodes.ERROR;
            if (ex.GetType() == typeof(UDFException))
            {
                var excep = (UDFException)ex;
                apiMessage.ErrorCode = excep.ErrorCode;
            }
            //We can log this exception message to the file or database.
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {

                Content = new StringContent(JsonConvert.SerializeObject(apiMessage)),
                ReasonPhrase = "Internal Server Error.Please Contact your Administrator.",
                StatusCode = HttpStatusCode.OK
            };

            logger.Error(ex, $"Unhandled exception in {controller}/{action} | User: {user} | URL: {url}");

            context.Response.Write(response);

        }
    }
}

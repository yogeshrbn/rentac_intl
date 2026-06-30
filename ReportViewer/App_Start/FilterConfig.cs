using BAL.Common;
using System;
using System.Net.Mime;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace ReportViewer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter());

        }
    }
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Log the exception (e.g., NLog, Serilog)
            var exception = context.Exception;

            var response = new
            {
                Message = "An unexpected error occurred.",
                Detail = exception.Message,
                Timestamp = DateTime.UtcNow
            };
            var res = new ApiResponseMessage();
            res.Data = response;
            var result = new JsonResult();
            result.Data = res;
            context.Result = result;
             


            context.ExceptionHandled = true;
        }
    }

}

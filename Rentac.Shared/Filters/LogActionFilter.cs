using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;
namespace Rentac.Shared.Filters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            logger.Info($"Starting execution of {controller}/{action}");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            logger.Info($"Finished execution of {controller}/{action}");
        }
    }
}

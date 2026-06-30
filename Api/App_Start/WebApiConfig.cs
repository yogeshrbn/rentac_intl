using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web.Http.Cors;
using System.Web.Http.WebHost;
using FarmaAPI.Providers;
using System.Web.SessionState;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web;
using Microsoft.AspNet.Identity;
using FarmaAPI.App_Start;
//using FarmaAPI.App_Start;
namespace FarmaAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional }

            );

            //To produce JSON format add this line of code
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            // var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

         

            //config.EnsureInitialized();
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api

            config.Filters.Add(new LogFilter());
            config.Filters.Add(new CustomExceptionFilter());
            config.EnableSystemDiagnosticsTracing();
       

        }


    }
    public class LogFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext filterContext)
        {
            if (filterContext.Request.Headers.Contains("Authorization"))
            {

                string token = filterContext.Request.Headers.GetValues("Authorization").FirstOrDefault();
                if (!String.IsNullOrEmpty(token))
                {
                    token = token.Replace("bearer", "").Trim();
                    System.Web.HttpContext.Current.Items.Add("token", token);
                    LoggedInUser user = new LoggedInUser();
                    if (user != null)
                    {
                        System.Web.HttpContext.Current.Items.Add("UserId", user.UserId);
                        System.Web.HttpContext.Current.Items.Add("DefaultCompanyId", user.DefaultCompanyId);
                    }
                }
            }
            // HttpContext.Current.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);

            base.OnActionExecuting(filterContext);
        }


        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //  System.Web.HttpContext.Current.SetSessionStateBehavior(
            //    SessionStateBehavior.Required);
            base.OnActionExecuted(actionExecutedContext);
        }


    }
}
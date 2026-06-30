using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using ActionFilterAttribute = System.Web.Mvc.ActionFilterAttribute;
using IActionFilter = System.Web.Mvc.IActionFilter;

namespace ReportViewer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
           


            //  config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            //config.Routes.MapHttpRoute(
            //name: "DefaultApi",
            //routeTemplate: "api/{controller}/{action}/{id}",
            //defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional }

            // );
             config.Formatters.JsonFormatter.SupportedMediaTypes
        .Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));

            config.Filters.Add(new LogFilter());


        }
    }

    public class CustomActionFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: Add your action filter's tasks here

            // Log Action Filter call
            if (filterContext.HttpContext.Request.Headers.AllKeys.Contains("Authorization"))
            {

                string token = filterContext.HttpContext.Request.Headers.Get("Authorization");
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

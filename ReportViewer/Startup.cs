using Microsoft.Owin;
using Owin;
using System;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
//using ReportViewer.Providers;
using System.Web;
using BAL.Services;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Text;
using Microsoft.Owin.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode;
[assembly: OwinStartup(typeof(ReportViewer.Startup))]

namespace ReportViewer
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //   GlobalConfiguration.Configure(WebApiConfig.Register);
 
            ConfigureOAuth(app);

            SimpleInjectorConfig.Register();

        }
        private string[] roles = new[] { "User", "Manager", "Administrator" };

        public void ConfigureOAuth(IAppBuilder app)
        {
            //OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            //{

            //    //AllowInsecureHttp = true,
            //    //TokenEndpointPath = new PathString("/api/token"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(120),
            //    Provider = new Providers.SimpleAuthorizationServerProvider(),
            //    RefreshTokenProvider = new SimpleRefreshTokenProvider()
            //};
         
            var secret = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["jwtSecret"]);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = ConfigurationManager.AppSettings["jwtIssuer"],
                    ValidAudience = ConfigurationManager.AppSettings["jwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                }
            });

            HttpConfiguration config = GlobalConfiguration.Configuration;
            //            var httpControllerRouteHandler = typeof(HttpControllerRouteHandler).GetField("_instance",
            //System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            //            if (httpControllerRouteHandler != null)
            //            {
            //                httpControllerRouteHandler.SetValue(null,
            //                    new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            //            }

            //   WebApiConfig.Register(config);
           // var config = new HttpConfiguration();
           
            
            //app.UseOAuthBearerTokens(OAuthServerOptions);
            //app.UseOAuthAuthorizationServer(OAuthServerOptions);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
          
            PDFGenerator.reportsPath = HttpContext.Current.Server.MapPath("~/Reports");
            PDFGenerator.docPath = ConfigurationManager.AppSettings["docsPath"];
            PDFGenerator.cssPath = HttpContext.Current.Server.MapPath("~/Content");
            AzureStorageService.ConnectionString = WebConfigurationManager.AppSettings["AzureStorageConnectionString"];
            AzureStorageService.ContainerBaseUrl = WebConfigurationManager.AppSettings["ContainerBaseUrl"];

             
            config.EnsureInitialized();

        }
    }
}

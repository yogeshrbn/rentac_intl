using BAL.Services;
using DbMigration;
using FarmaAPI.App_Start;
using FarmaAPI.Providers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
 
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using NLog;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.SessionState;
using AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode;
[assembly: OwinStartup(typeof(FarmaAPI.Startup))]
namespace FarmaAPI
{

    public class Startup
    {

        //public static void RequireAspNetSession(IAppBuilder app)
        //{
        //    app.Use((context, next) =>
        //    {
        //        var httpContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
        //        httpContext.SetSessionStateBehavior(SessionStateBehavior.Required);
        //        return next();
        //    });

        //    // To make sure the above `Use` is in the correct position:
        //    app.UseStageMarker(PipelineStage.MapHandler);
        //}
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            GlobalConfiguration.Configure(WebApiConfig.Register);
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
            //    Provider = new FarmaAPI.Providers.SimpleAuthorizationServerProvider(),
            //  //  RefreshTokenProvider = new SimpleRefreshTokenProvider()
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
                    ValidIssuer = ConfigurationManager.AppSettings["jwtIssuer"],
                    ValidAudience = ConfigurationManager.AppSettings["jwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                     ClockSkew = TimeSpan.Zero
                }
            });
            HttpConfiguration config = new HttpConfiguration();
            //var httpControllerRouteHandler = typeof(HttpControllerRouteHandler).GetField("_instance",
            //System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            //            if (httpControllerRouteHandler != null)
            //            {
            //                httpControllerRouteHandler.SetValue(null,
            //                    new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            //            }

            //   WebApiConfig.Register(config);
            //app.UseOAuthBearerTokens(OAuthServerOptions);
            //app.UseOAuthAuthorizationServer(OAuthServerOptions);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var connString = WebConfigurationManager.ConnectionStrings["sqlCon"].ConnectionString;
            IDbConnection conn = new SqlConnection(connString);
            var appDbName = "";
            if (conn != null)
            {
                appDbName = conn.Database;
            }
            var variables = new Dictionary<string, string>
            {
                { "AppDb", appDbName },
            };
            DbMigrationService.MigrateDatabase(connString, variables);
            AzureStorageService.ConnectionString = WebConfigurationManager.AppSettings["AzureStorageConnectionString"];
            AzureStorageService.ContainerBaseUrl = WebConfigurationManager.AppSettings["ContainerBaseUrl"];
        }
    }
}
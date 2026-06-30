using Microsoft.Owin;
using Owin;
using System;
using System.Web.Http;
using System.Threading.Tasks;

using Microsoft.Owin.Security.OAuth;
using Rentac.Providers;
using System.Text;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;

[assembly: OwinStartup(typeof(Rentac.Auth.Startup))]

namespace Rentac.Auth
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


        }
        private string[] roles = new[] { "User", "Manager", "Administrator" };

        public void ConfigureOAuth(IAppBuilder app)
        {
            //OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            //{
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/api/token"),
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
                    ValidIssuer = ConfigurationManager.AppSettings["jwtIssuer"],
                    ValidAudience = ConfigurationManager.AppSettings["jwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ClockSkew = TimeSpan.Zero
                }
            });








            HttpConfiguration config = new HttpConfiguration();
            //            var httpControllerRouteHandler = typeof(HttpControllerRouteHandler).GetField("_instance",
            //System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            //            if (httpControllerRouteHandler != null)
            //            {
            //                httpControllerRouteHandler.SetValue(null,
            //                    new Lazy<HttpControllerRouteHandler>(() => new SessionHttpControllerRouteHandler(), true));
            //            }

            //   WebApiConfig.Register(config);
            //app.UseOAuthBearerTokens(OAuthServerOptions);
            //app.UseOAuthAuthorizationServer(OAuthServerOptions);
            // app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());



        }
    }
}

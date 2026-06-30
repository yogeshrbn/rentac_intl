using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using BAL.Objects;
using BAL.DTO;
using System.Web.SessionState;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;
using NLog;
using Newtonsoft.Json;
using BAL.Services.Integrations;

namespace Rentac.Providers
{

    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {

            return base.AuthorizationEndpointResponse(context);
        }
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            //System.Web.HttpContext.Current.SetSessionStateBehavior(
            //    SessionStateBehavior.Required);
            //context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, PUT, DELETE, POST, OPTIONS" });
            //context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Accept, Authorization" });
            //context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });


            string token = context.AccessToken;
            var identity = context.Identity;

            Claim claim = context.Identity.Claims.FirstOrDefault();
            if (claim != null)
            {
                int finyearId = 0;
                if (!String.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
                {
                    finyearId = Convert.ToInt16(HttpContext.Current.Request["FinYear"]);
                }
                string userId = claim.Value;
                new Authentication().SaveToken(Convert.ToInt32(userId), token, finyearId);
                //UserDTO dto = new Authentication().GetToken(token);
                UserDTO dto = new User().GetById(Convert.ToInt32(userId), finyearId);

                context.AdditionalResponseParameters.Add("UserId", userId);
                context.AdditionalResponseParameters.Add("DefaultCompanyId", dto.DefaultCompanyId);
                context.AdditionalResponseParameters.Add("AllowSwitchCompany", dto.AllowSwitchCompany);
                context.AdditionalResponseParameters.Add("CompanyStateId", dto.CompanyStateId);

                context.AdditionalResponseParameters.Add("ClientId", dto.RbnClientId);
                context.AdditionalResponseParameters.Add("FullName", dto.FullName);
                context.AdditionalResponseParameters.Add("RoleId", dto.RoleId);
                context.AdditionalResponseParameters.Add("FinYearId", dto.FinYearId);
                context.AdditionalResponseParameters.Add("FinYearStart", dto.FinYearStart);
                context.AdditionalResponseParameters.Add("FinYearEnd", dto.FinYearEnd);
                context.AdditionalResponseParameters.Add("ProfilePic", dto.ProfilePic);
                context.AdditionalResponseParameters.Add("Phone", dto.Phone);

                var finyearCLaim = context.Identity.Claims.Where(o => o.Type == "FinYearId").FirstOrDefault();
                if (finyearCLaim != null)
                {
                    context.Identity.TryRemoveClaim(finyearCLaim);
                }
                context.Identity.AddClaim(new Claim("FinYearId", dto.FinYearId.ToString()));

                var packageService = new RentacPackageService();
                var package = packageService.ClientPackageSel(dto.RbnClientId);
                if (package != null)
                {

                    context.AdditionalResponseParameters.Add("lcd", JsonConvert.SerializeObject(package));
                }

                var userRole = new UserRole();
                var roleMenus = userRole.GetRoleMenus(dto.RoleId, dto.DefaultCompanyId);

                var comp = new Company(dto.DefaultCompanyId);
                var copDto = comp.GetDetails();
                if (copDto != null && String.IsNullOrEmpty(copDto.EwayUserName))
                {
                    var menu = roleMenus.Where(o => o.MenuId == 58).FirstOrDefault();
                    if (menu != null)
                    {
                        roleMenus.Remove(menu);
                    }
                }

                context.AdditionalResponseParameters.Add("menus", JsonConvert.SerializeObject(roleMenus));
                // HttpContext.Current.GetOwinContext().Authentication.SignIn(identity);
                //  string token = ticket.Identity.BootstrapContext.ToString();
                var wsService = new WhatsappService();
                var wsResult = Task.Run(async () => await wsService.ListApps(dto.DefaultCompanyId, dto.RbnClientId)).Result;
                if (wsResult != null)
                {
                    var ws = wsResult.Where(o => o.Live == true).FirstOrDefault();
                    if (ws != null)
                    {
                        context.AdditionalResponseParameters.Add("wsApp", 1);
                    }
                }
            }

            return base.TokenEndpointResponse(context);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            context.Validated(); //   
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {


            //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            // context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            UserDTO user = new Authentication().Authenticate(context.UserName, context.Password);
            if (user != null && user.Active)
            {
                //try
                //{

                //    SessionStateUtility.GetHttpSessionStateFromContext(HttpContext.Current)["user"] = user;
                //    // HttpContext.Current.Session["User"] = user;
                //}
                //catch (Exception ex)
                //{

                //}
                // new Authentication().SaveToken(Convert.ToInt32(userId), token, finyearId);
                //UserDTO dto = new Authentication().GetToken(token);
                UserDTO dto = new User().GetById(Convert.ToInt32(user.UserId), 0);

                //identity.AddClaim(new Claim("UserId", user.UserId.ToString()));
                //identity.AddClaim(new Claim("DefaultCompanyId", dto.DefaultCompanyId.ToString()));
                ////identity.AddClaim(new Claim("UserId", dto.UserId.ToString());
                //identity.AddClaim(new Claim("ClientId", dto.RbnClientId.ToString()));
                //identity.AddClaim(new Claim("FullName", dto.FullName));
                //identity.AddClaim(new Claim("RoleId", dto.RoleId.ToString()));

                //identity.AddClaim(new Claim("FinYearStart", dto.FinYearStart.ToString()));
                //identity.AddClaim(new Claim("FinYearEnd", dto.FinYearEnd.ToString()));
                //identity.AddClaim(new Claim("FinYearId", dto.FinYearId.ToString()));
                //context.AdditionalResponseParameters.Add("DefaultCompanyId", dto.DefaultCompanyId);
                //context.AdditionalResponseParameters.Add("AllowSwitchCompany", dto.AllowSwitchCompany);
                //context.AdditionalResponseParameters.Add("ClientId", dto.RbnClientId);
                //context.AdditionalResponseParameters.Add("FullName", dto.FullName);
                //context.AdditionalResponseParameters.Add("RoleId", dto.RoleId);
                //context.AdditionalResponseParameters.Add("FinYearId", dto.FinYearId);
                //context.AdditionalResponseParameters.Add("FinYearStart", dto.FinYearStart);
                //context.AdditionalResponseParameters.Add("FinYearEnd", dto.FinYearEnd);

                // identity.AddClaim(new Claim("UserId", user.UserId.ToString()));
                // System.Web.HttpContext.Current.Session["UserId"] = user.UserId;
                // HttpContext.Current.Items.Add("UserId", user.UserId);
                // HttpContext.Current.Items.Add("RbnClientId", user.RbnClientId);

                var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {
                                    "username", context.UserName
                                },
                                {
                                     "role", "admin"
                                }
                             });

                var ticket = new AuthenticationTicket(identity, props);

                dto.FullName = String.IsNullOrEmpty(dto.FullName) ? "" : dto.FullName;
                //    HttpContext.Current.GetOwinContext().Authentication.SignOut();
                identity = ticket.Identity;
                addClaim(identity, "UserId", user.UserId.ToString());
                addClaim(identity, "DefaultCompanyId", dto.DefaultCompanyId.ToString());
                addClaim(identity, "ClientId", dto.RbnClientId.ToString());
                addClaim(identity, "FullName", dto.FullName);
                addClaim(identity, "RoleId", dto.RoleId.ToString());
                addClaim(identity, "FinYearStart", dto.FinYearStart.ToString());
                addClaim(identity, "FinYearEnd", dto.FinYearEnd.ToString());
                addClaim(identity, "FinYearId", dto.FinYearId.ToString());
                addClaim(identity, "Phone", dto.Phone.ToString());

                HttpContext.Current.GetOwinContext().Authentication.SignIn(identity);

                //  string token = ticket.Identity.BootstrapContext.ToString();
                context.Validated(ticket);
            }
            else
            {

                if (user == null)
                {
                    context.SetError("invalid_grant", "Either username or password is invalid");
                }
                else if (!user.Active)
                {
                    context.SetError("invalid_grant", "Your account has been locked.");
                }

                return;
            }


            return;

        }

        void addClaim(ClaimsIdentity identity, string type, string value)
        {
            var x = identity.Claims.Where(o => o.Type == type).FirstOrDefault();
            if (x != null)
            {
                identity.RemoveClaim(x);
            }
            identity.AddClaim(new Claim(type, value));
        }
        //public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        //{
        //    if (context.IsTokenEndpoint && context.Request.Method == "OPTIONS")
        //    {
        //        context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
        //        context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, PUT, DELETE, POST, OPTIONS" });
        //        context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Accept, Authorization" });
        //        context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });
        //        context.RequestCompleted();
        //        return Task.FromResult(0);
        //    }

        //    return base.MatchEndpoint(context);
        //}

    }
    //public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    //{
    //    private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();
    //    void addClaim(ClaimsIdentity identity, string type, string value)
    //    {
    //        var x = identity.Claims.Where(o => o.Type == type).FirstOrDefault();
    //        if (x != null)
    //        {
    //            identity.RemoveClaim(x);
    //        }
    //        identity.AddClaim(new Claim(type, value));
    //    }
    //    public async Task CreateAsync(AuthenticationTokenCreateContext context)
    //    {
    //        var guid = Guid.NewGuid().ToString();

    //        // maybe only create a handle the first time, then re-use for same client
    //        // copy properties and set the desired lifetime of refresh token
    //        var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
    //        {
    //            IssuedUtc = context.Ticket.Properties.IssuedUtc,
    //            ExpiresUtc = DateTime.UtcNow.AddYears(1)
    //        };
    //        var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

    //        //_refreshTokens.TryAdd(guid, context.Ticket);
    //        _refreshTokens.TryAdd(guid, refreshTokenTicket);

    //        // consider storing only the hash of the handle
    //        context.SetToken(guid);
    //    }

    //    public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
    //    {
    //        AuthenticationTicket ticket;
    //        if (_refreshTokens.TryRemove(context.Token, out ticket))
    //        {
    //            var identity = ticket.Identity;
    //            int finyearId = 0;
    //            if (!String.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
    //            {
    //                finyearId = Convert.ToInt16(HttpContext.Current.Request["FinYear"]);
    //            }

    //            addClaim(identity, "FinYearId", finyearId.ToString());
    //            context.SetTicket(ticket);
    //        }
    //        //context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
    //        //context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, PUT, DELETE, POST, OPTIONS" });
    //        //context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Accept, Authorization" });
    //        //context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });
    //    }

    //    public void Create(AuthenticationTokenCreateContext context)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Receive(AuthenticationTokenReceiveContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();
        void addClaim(ClaimsIdentity identity, string type, string value)
        {
            var x = identity.Claims.Where(o => o.Type == type).FirstOrDefault();
            if (x != null)
            {
                identity.RemoveClaim(x);
            }
            identity.AddClaim(new Claim(type, value));
        }
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            // maybe only create a handle the first time, then re-use for same client
            // copy properties and set the desired lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = context.Ticket.Properties.IssuedUtc.Value.AddMinutes(60)
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            //_refreshTokens.TryAdd(guid, context.Ticket);
            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle
            context.SetToken(guid);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                var identity = ticket.Identity;
                int finyearId = 0;
                if (!String.IsNullOrEmpty(HttpContext.Current.Request["FinYear"]))
                {
                    finyearId = Convert.ToInt16(HttpContext.Current.Request["FinYear"]);
                }

                addClaim(identity, "FinYearId", finyearId.ToString());
                context.SetTicket(ticket);
            }
            //context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            //context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, PUT, DELETE, POST, OPTIONS" });
            //context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Accept, Authorization" });
            //context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}
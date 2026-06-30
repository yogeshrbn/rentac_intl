using BAL.DTO;
using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
 
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Security.Claims;
namespace ReportViewer
{
    public class LoggedInUser : UserDTO
    {
        public LoggedInUser()
        {
            if (System.Web.HttpContext.Current.Items["token"] != null)
            {
                var context = HttpContext.Current.GetOwinContext();
                //String token = Convert.ToString(System.Web.HttpContext.Current.Items["token"]);
                //    UserDTO user = new Authentication().GetToken(token);
                var claimsPrinciple = HttpContext.Current.User;
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;

                //var claims = ((System.Security.Claims.ClaimsIdentity)claimsPrinciple.Identity).Claims.ToList();
                var claims = context.Authentication.User.Claims.ToList();
                if (claims != null && claims.Count > 1)
                {
                    //this.FullName = user.FullName;
                    //this.RbnClientId = user.RbnClientId;
                    //this.UserId = user.UserId;
                    //this.DefaultCompanyId = user.DefaultCompanyId;
                    //this.FinYearId = user.FinYearId;


                    this.FullName = claims[3].Value;
                    this.RbnClientId = Convert.ToInt32(claims[2].Value);


                    var fyear = claims.Where(o => o.Type == "FinYearId").FirstOrDefault();
                    if (fyear != null)
                        this.FinYearId = Convert.ToInt32(fyear.Value);

                    this.UserId = Convert.ToInt32(claims[0].Value);

                    var user = new User().GetById(this.UserId, this.FinYearId);
                    this.DefaultCompanyId = user.DefaultCompanyId;// Convert.ToInt32(claims[1].Value);
                    // Override with x-companyId header when sent by client (e.g. company switch)
                    var xCompanyId = HttpContext.Current?.Request?.Headers["x-companyId"];
                    if (!string.IsNullOrEmpty(xCompanyId) && int.TryParse(xCompanyId, out int headerCompanyId) && headerCompanyId > 0)
                    {
                        this.DefaultCompanyId = headerCompanyId;
                    }
                    var phone = claims.Where(o => o.Type == "Phone").FirstOrDefault();
                    if (fyear != null)
                        this.Phone = Convert.ToString(phone.Value);
                }
            }
        }
    }
}
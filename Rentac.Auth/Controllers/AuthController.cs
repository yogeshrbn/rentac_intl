using BAL.Common;
using BAL.DTO;
using BAL.Objects;
using BAL.Services.Integrations;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace Rentac.Auth.Controllers
{
    public class AuthModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int FinYear { get; set; } = 0;
        public int UserId { get; set; } = 0;
    }
    public class AuthController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Login([FromBody] AuthModel model)
        {
            var apiMessage = new ApiResponseMessage();
            try
            {
                int finyearId = 0;

                var userObj = new LoggedInUserInfo();
                UserDTO user;
                var context = HttpContext.Current.GetOwinContext();
                var loggedInClamins = context.Authentication.User.Claims.ToList();

                if (loggedInClamins != null && loggedInClamins.Count > 1)
                {
                    finyearId = model.FinYear;
                    var _userId = Convert.ToInt32(loggedInClamins[0].Value);
                    if (model != null && model.FinYear > 0 && model.Password.ToLower() == "refresh_token")
                    {
                        user = new User().GetById(_userId, model.FinYear);
                    }
                    else
                        user = new Authentication().Authenticate(model.UserName, model.Password);
                }
                else
                    user = new Authentication().Authenticate(model.UserName, model.Password);
                // new Authentication().SaveToken(Convert.ToInt32(userId), token, finyearId);
                //UserDTO dto = new Authentication().GetToken(token);



                if (user != null && user.Active)
                {
                    UserDTO dto = new User().GetById(user.UserId, finyearId);

                    dto.FullName = String.IsNullOrEmpty(dto.FullName) ? "" : dto.FullName;

                    var claims = new List<Claim>
                {
               new Claim("UserId",user.UserId.ToString()),
                 new Claim("DefaultCompanyId",dto.DefaultCompanyId.ToString()),
                  new Claim("ClientId",dto.RbnClientId.ToString()),
               // new Claim("userName", dto.UserName),

                

                   new Claim("FullName",    dto.FullName),
               new Claim("RoleId",dto.RoleId.ToString()),
               // new Claim("AllowSwitchCompany",dto.RoleId.ToString()),
              //  new Claim("CompanyStateId",dto.RoleId.ToString()),
               
             //   new Claim("FullName",dto.FullName),

                new Claim("FinYearStart",dto.FinYearStart.ToString()),
                new Claim("FinYearEnd",dto.FinYearEnd.ToString()),
               // new Claim("ProfilePic",dto.RoleId.ToString()),
                 new Claim("FinYearId",dto.FinYearId.ToString()),
                new Claim("Phone",dto.Phone.ToString()),


                };

                    var packageService = new RentacPackageService();
                    var package = packageService.ClientPackageSel(dto.RbnClientId);
                    string lcd = "";
                    if (package != null)
                    {
                        lcd = JsonConvert.SerializeObject(package);
                        // claims.Add(new Claim("lcd", JsonConvert.SerializeObject(package)));
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

                    //  claims.Add(new Claim("menus", JsonConvert.SerializeObject(roleMenus)));
                    // HttpContext.Current.GetOwinContext().Authentication.SignIn(identity);
                    //  string token = ticket.Identity.BootstrapContext.ToString();
                    var wsService = new WhatsappService();
                    var wsResult = Task.Run(async () => await wsService.ListApps(dto.DefaultCompanyId, dto.RbnClientId)).Result;
                    byte wsActive = 0;
                    if (wsResult != null)
                    {
                        var ws = wsResult.Where(o => o.Live == true).FirstOrDefault();
                        if (ws != null)
                        {
                            wsActive = 1;
                            //    claims.Add(new Claim("wsApp", "1"));
                        }
                    }
                    var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["jwtSecret"]);
                    var token = new JwtSecurityToken(
                        issuer: ConfigurationManager.AppSettings["jwtIssuer"],
                        audience: ConfigurationManager.AppSettings["jwtAudience"],
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256)
                    );
                    string strToken = new JwtSecurityTokenHandler().WriteToken(token);

                    var allCompanies = Company.GetAll(dto.RbnClientId);
                    dto.DefaultWarehouseId = copDto.DefaultWarehouseId;
                    var _model = new
                    {
                        access_token = strToken,
                        _extra = dto,
                        wsApp = wsActive,
                        menus = JsonConvert.SerializeObject(roleMenus),
                        lcd = lcd,
                        companies = allCompanies
                    };
                    apiMessage.Data = _model;
                    apiMessage.Code = ApiResponseMessageCodes.SUCCESS;
                    return Ok(apiMessage);
                }
                else
                {

                    if (user == null)
                    {
                        apiMessage.Message = "Either username or password is invalid";
                        //context.SetError("invalid_grant", "Either username or password is invalid");
                    }
                    else if (!user.Active)
                    {
                        // context.SetError("invalid_grant", "Your account has been locked.");
                        apiMessage.Message = "Your account has been locked.";
                    }


                }
                apiMessage.Code = ApiResponseMessageCodes.ERROR;

                return Ok(apiMessage);
            }
            catch (Exception ex)
            {
                apiMessage.Code = ApiResponseMessageCodes.ERROR;
                apiMessage.Message = ex.Message;
                return Ok(apiMessage);
            }
        }





    }


}

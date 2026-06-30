using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage GetAllUsers([FromBody] FilterCriteria dto)
        {
            User objUser = new User();
            var _users = objUser.GetAllUsers(new LoggedInUser().RbnClientId);
            if (_users != null)
            {
                foreach (var user in _users)
                {
                    user.Password = string.Empty;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, _users);

        }
        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage CreateUser([FromBody] UserDTO dto)
        {
            string result = "FAIL";
            try
            {
                User objUser = new User();
                dto.RbnClientId = new LoggedInUser().RbnClientId;
                if (String.IsNullOrEmpty(dto.Companies))
                {
                    throw new Exception("Please select at least one company");
                }
                bool ret = false;
                if (dto.UserId == 0)
                {
                    ret = objUser.CreateUser(dto);
                }
                else
                {
                    ret = objUser.UpdateUser(dto);
                }
                result = ret ? "SUCCESS" : "FAIL";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage ResetPassword([FromBody] UserDTO dto)
        {
            string result = "FAIL";
            User objUser = new User();
            try
            {
                bool ret = objUser.ResetPassword(dto);
                result = ret ? "SUCCESS" : "FAIL";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }
        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage UpdateStatus([FromBody] UserDTO dto)
        {
            string result = "FAIL";
            User objUser = new User();
            try
            {

                bool ret = objUser.UpdateStatus(dto.UserId, dto.Active);
                result = ret ? "SUCCESS" : "FAIL";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        [HttpGet]
        public HttpResponseMessage GetUserId()
        {
            System.Web.HttpContext.Current.Session["UserId"] = 10;
            String userId = System.Web.HttpContext.Current.Session["UserId"].ToString();
            return Request.CreateResponse(HttpStatusCode.OK, userId);
        }
        [HttpPost]
        public HttpResponseMessage GetRouteAccess([FromBody] FunctionDTO dto)
        {
            int userId = new LoggedInUser().UserId;
            BAL.Objects.User user = new BAL.Objects.User();
            ApiMessage message = new ApiMessage();
            try
            {
                message.Data = user.GetRouteAccess(userId, dto.Route);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage ChangeProfilePicture()
        {
            int userId = new LoggedInUser().UserId;
            BAL.Objects.User user = new BAL.Objects.User();
            ApiMessage message = new ApiMessage();
            try
            {
                var files = System.Web.HttpContext.Current.Request.Files;

                if (files.Count == 0)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "No image found";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                var filepath = string.Format("images/{0}_profilePic.png", userId.ToString());
                files[0].SaveAs(HttpContext.Current.Server.MapPath("/") + filepath);
                message.Data = user.UpdateProfilePic(userId, filepath);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetLoggedInUserDetails()
        {
            int userId = new LoggedInUser().UserId;
            BAL.Objects.User user = new BAL.Objects.User();
            ApiMessage message = new ApiMessage();
            try
            {
                message.Data = new { userInfo = user.GetById(userId, 0), routes = user.GetAllowedRoutes(userId) };
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpGet]
        public HttpResponseMessage GetLicenseDetails()
        {
            var loggedInuser = new LoggedInUser();
            int userId = loggedInuser.UserId;
            var packageService = new RentacPackageService();
            var package = packageService.ClientPackageSel(loggedInuser.RbnClientId);
            ApiMessage message = new ApiMessage();
            try
            {
                message.Data = package;
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {

                if (String.IsNullOrEmpty(dto.NewPassword))
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "New password must be between 8 to 20 characters";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                if (String.IsNullOrEmpty(dto.CurrentPassword))
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Current password must not be empty";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                if (String.Equals(dto.CurrentPassword, dto.NewPassword, StringComparison.InvariantCulture))
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "New password must not be equal to current password";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }

                var loggedInuser = new LoggedInUser();
                int userId = loggedInuser.UserId;
                BAL.Objects.User user = new BAL.Objects.User();
                var package = user.ChangePassword(userId, dto.CurrentPassword, dto.NewPassword);


                message.Data = package;
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;

            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
    }
}

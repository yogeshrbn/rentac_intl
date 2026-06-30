using BAL.Common;
using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FarmaAPI.Controllers
{
    public class ForgotPasswordController : BaseApiController
    {

        public ForgotPasswordController() { }

        [HttpGet]
        public ApiMessage SendVerificationLink(string email)
        {
            var message = new ApiMessage();
            if (email == null)
            {
                message.Message = "Invalid email provided";
                message.Code = ApiMessageCodes.ERROR;
                return message; ;
            }
            var user = new BAL.Objects.User();

            var dto = new VerificationLinkDTO
            {

                Email = email,
                GuId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                ValidTill = DateTime.Now.AddMinutes(10)
            };
            var userObj = user.GetByEmail(email);
            if (userObj == null)
            {
                message.Message = "Invalid email provided";
                message.Code = ApiMessageCodes.ERROR;
                return message; ;
            }
            var created = user.CreateVarificationLink(dto);
            message.Data = created;
            if (created)
            {
                EmailParameters param = new EmailParameters();
                param.ForgotPasswordVerifyLink = Request.Headers.Referrer.AbsoluteUri + "index.html#/fgwdResetPwdLink/" + dto.GuId;
                 
                ReportUtility rep = new ReportUtility();
                rep.sendEmail(dto.Email, "Rentac", "Forgot Password", param, ReortFileName.FORGOT_PASSWORD_LINK);


            }
            message.Code = ApiMessageCodes.SUCCESS;
            return message;
        }
        [HttpGet]
        public ApiMessage ValidateLinkEmail(string guId)
        {
            var message = new ApiMessage();
            var user = new BAL.Objects.User();

            var link = user.GetVarificationLink(guId);
            if (link == null)
            {
                message.Data = "2";
                message.Code = ApiMessageCodes.ERROR;
            }
            else if (link.Used ==2)
            {
                message.Data = "3";
                message.Code = ApiMessageCodes.ERROR;
            }
            else if (link.ValidTill < DateTime.Now)
            {
                message.Data = "3";
                message.Code = ApiMessageCodes.ERROR;
            }
            else
            {
                message.Data = link;
                message.Code = ApiMessageCodes.SUCCESS;
            }



            return message;
        }
        [HttpPost]
        public ApiMessage UpdatePassword([FromBody] ResetPasswordDTO dto)
        {
            var message = new ApiMessage();
            var user = new BAL.Objects.User();

            var link = user.GetVarificationLink(dto.GuId);
            if (link == null)
            {
                message.Message = "Invalid link";
                message.Code = ApiMessageCodes.ERROR;
            }
            else if (link.ValidTill < DateTime.Now)
            {
                message.Message = "Link has been expired";
                message.Code = ApiMessageCodes.ERROR;
            }
            else if (link.Used == 2)
            {
                var userObj = user.GetByEmail(link.Email);
                if (userObj == null)
                {
                    message.Message = "User not found with the given email.";
                    message.Code = ApiMessageCodes.ERROR;
                }
                userObj.Password = dto.Password;
                message.Data = user.ResetPassword(userObj);
                message.Code = ApiMessageCodes.SUCCESS;
            }



            return message;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;
using BAL.Exceptions;
using NLog;
using BAL.DTO.View;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Net.Http.Formatting;
using System.Diagnostics;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Newtonsoft.Json.Linq;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class PartyController : ApiController

    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetOtp(string mobileNo)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                if (String.IsNullOrEmpty(mobileNo))
                {
                    throw new Exception("Invalid or empty mobile No");
                }

                string phone = mobileNo;
                var ledger = new Ledger();
                var ldto = ledger.GetByPhone(phone);
                if (ldto == null)
                {
                    throw new Exception("Record not found for give mobile no");
                }
                var otp = new OtpDTO();
                otp.MobileNo = mobileNo;
                otp.MobileOTP = (new Random()).Next(1000, 9999).ToString();
                otp.CreatedOn = DateTime.Now;
                otp.ValidTill = DateTime.Now.AddSeconds(60);
                otp.Guid = Guid.NewGuid().ToString();
                var otpService = new OTP();
                var added = otpService.Add(otp);

                if (added)
                {
                    CommHelpler com = new CommHelpler();
                    var param = new Dictionary<string, string>();
                    param.Add("otp", otp.MobileOTP);
                    com.sendSms(phone, SMSTemplates.PARTY_LOGIN, MessageEvent.PARTY_LOGIN, param);

                }
                message.Data = otp.Guid;
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (UDFException ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                //  return Request.CreateResponse(HttpStatusCode.OK, message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ResendOtp(string guId)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                if (String.IsNullOrEmpty(guId))
                {
                    throw new Exception("Invalid or empty Id");
                }
                var otpService = new OTP();
                var lastOtp = otpService.GetOTPByGuId(guId);
                if (lastOtp == null)
                {
                    throw new Exception("Invalid or empty Id");
                }
                var otp = new OtpDTO();
                otp.MobileNo = lastOtp.MobileNo;
                otp.MobileOTP = (new Random()).Next(1000, 9999).ToString();
                otp.CreatedOn = DateTime.Now;
                otp.ValidTill = DateTime.Now.AddSeconds(30);
                otp.Guid = Guid.NewGuid().ToString();
          
                var added = otpService.Add(otp);

                if (added)
                {
                    CommHelpler com = new CommHelpler();
                    var param = new Dictionary<string, string>();
                    param.Add("otp", otp.MobileOTP);
                    com.sendSms(otp.MobileNo, SMSTemplates.PARTY_LOGIN, MessageEvent.PARTY_LOGIN, param);

                }
                message.Data = otp.Guid;
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (UDFException ex)
            {
                message.Message = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                //  return Request.CreateResponse(HttpStatusCode.OK, message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }


        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult VerifyOtp([FromBody] OtpVerifyVMDto dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                var otpService = new OTP();
                var otpdto = otpService.GetByOtp(dto.Otp);
                if (otpdto == null)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Invalid OTP";
                    return Ok(message);
                }
                if (otpdto.ValidTill < DateTime.Now)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "OTP has been expired";
                    return Ok(message);
                }
                otpService.Update("M", otpdto.OtpId);
                message.Code = ApiMessageCodes.SUCCESS;

                var user = new User();

                var userDto = user.GetByPhone(otpdto.MobileNo);
                if (userDto == null)
                {
                    userDto = new UserDTO();
                    userDto.UserName = otpdto.MobileNo;
                    userDto.Password = otpdto.MobileNo;
                    userDto.RoleId = 7;
                    userDto.Phone = otpdto.MobileNo;
                    userDto.Active = true;
                    var userCreated = user.CreateUser(userDto);
                }
                var url = System.Web.HttpContext.Current.Request.Url.Authority;
                if (System.Web.HttpContext.Current.Request.IsSecureConnection)
                {
                    url = "https://" + url + "/api/token";
                }
                else
                {
                    url = "http://" + url + "/api/token";

                }
                using (var client = new HttpClient())
                {
                    var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username",  userDto.UserName},
                   {"password", userDto.Password},
               };
                    var tokenResponse = client.PostAsync(url, new FormUrlEncodedContent(form)).Result;
                    var token = tokenResponse.Content.ReadAsStringAsync().Result;
                    if (token != null)
                    {
                        var jtoken = JObject.Parse(token);
                        if (jtoken.GetValue("access_token") != null)
                        {
                            message.Data = new { token = jtoken.GetValue("access_token").ToString() };
                        }

                    }

                }
                return Ok(message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }

        [HttpPost]
        public IHttpActionResult Bills([FromBody] PartyFilter filter)
        {

            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                var ledger = new Ledger();
                var bills = ledger.BillsByPhone(user.Phone);
               
               
                message.Data = bills;
                message.Code = ApiMessageCodes.SUCCESS;
                return Ok(message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }

        [HttpGet]
        public IHttpActionResult Payments()
        {

            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                var payment = new PaymentService();
                message.Data = payment.PartyPayments(user.Phone);


                message.Code = ApiMessageCodes.SUCCESS;
                return Ok(message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }
        [HttpPost]
        public IHttpActionResult Returns([FromBody] FilterCriteria filter)
        {

            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                var grn = new Party();
                message.Data = grn.PartyReturns(user.Phone, Convert.ToDateTime(filter.From), Convert.ToDateTime(filter.To));


                message.Code = ApiMessageCodes.SUCCESS;
                return Ok(message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }

        [HttpPost]
        public IHttpActionResult Received([FromBody] FilterCriteria filter)
        {

            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                var grn = new Party();
                message.Data = grn.PartyReceived(user.Phone, Convert.ToDateTime(filter.From), Convert.ToDateTime(filter.To));


                message.Code = ApiMessageCodes.SUCCESS;
                return Ok(message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }
        [HttpPost]
        public IHttpActionResult StockBalance([FromBody] PartyFilter filter)
        {

            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();

                var grn = new Party();
                message.Data = grn.PartyStockBalance(user.Phone, filter.CompanyId);


                message.Code = ApiMessageCodes.SUCCESS;
                return Ok(message);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Ok(message);
        }
    }
}

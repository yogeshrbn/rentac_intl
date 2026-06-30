using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Windows.Forms;
using BAL.DTO;
using BAL.Objects;
using FarmaAPI.Helper;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using NLog;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace FarmaAPI.Controllers
{
    public class PaymentController : BaseApiController
    {
        RentacPackageService packageService = new RentacPackageService();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public PaymentController()
        {

        }

        [HttpPost]
        public async Task<ApiMessage> createPackagePurchaseOrder([FromBody] PackagePurchaseDTO dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var paymentService = new PaymentService();
                var payOrder = new PaymentOrderDTO();
                payOrder.ClientId = new LoggedInUser().RbnClientId;
                var packages = packageService.GetAllPackages();
                var selectedPackage = packages.Where(o => o.PackageId == dto.PackageId).FirstOrDefault();
                if (selectedPackage == null)
                {
                    msg.Message = "Invalid package selected";
                    msg.Code = ApiMessageCodes.ERROR;
                    return msg;
                }
                //if (dto.MonthlyYearly == "monthly")
                //{
                //    payOrder.Amount = selectedPackage.MonthlyPrice;
                //}
                //else if (dto.MonthlyYearly == "yearly")
                //{
                //    payOrder.Amount = selectedPackage.YearlyPrice;
                //}
                if(selectedPackage != null)
                {
                    payOrder.Amount = selectedPackage.Total ;
                }
                else
                {
                    msg.Message = "Invalid package selected";
                    msg.Code = ApiMessageCodes.ERROR;
                    return msg;
                }
                payOrder.PaymentType = "subscription";
                payOrder.PayerName = user.FullName;
                payOrder.PayerMobile = user.Phone;

                payOrder.PackageId = selectedPackage.PackageId;
                var x = await paymentService.CreatePaymentOrder(payOrder);
                msg.Data = new { order_id = x.orderId, key = payOrder.RazorPayKey };
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiMessage> CreatePaymentOrder([FromBody] CreatePaymentOrderDto dto)
        {
            var msg = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                var paymentService = new PaymentService();
                var payOrder = new PaymentOrderDTO();
                payOrder.ClientId = new LoggedInUser().RbnClientId;
                var packages = packageService.GetAllPackages();
                payOrder.PackageId = 0;
                payOrder.Amount = dto.Amount;
                payOrder.PaymentType = "party";
                payOrder.PayerMobile = user.Phone;
                payOrder.PayerName = user.FullName;
                payOrder.PayeeCompanyId = dto.CompanyId;
                payOrder.PayerLedgerId = dto.PayerLedgerId;
                var x = await paymentService.CreatePaymentOrder(payOrder);
                msg.Data = new { order_id = x.orderId, key = payOrder.RazorPayKey };
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        public async Task<ApiMessage> updateOrder([FromBody] PaymentOrderUpdateVMDTO dto)
        {
            var msg = new ApiMessage();
            try
            {
                var paymentService = new PaymentService();
                var payOrder = new PaymentOrderDTO();
                payOrder.ClientId = new LoggedInUser().RbnClientId;
                payOrder.orderId = dto.order_id;
                payOrder.payment_id = dto.payment_id;
                payOrder.payment_signature = dto.payment_signature;
                payOrder.PaymentDate = DateTime.Now;
                payOrder.Error = payOrder.code = dto.code;
                payOrder.description = dto.description;
                payOrder.source = dto.source;
                payOrder.reason = dto.reason;

                if (!String.IsNullOrEmpty(payOrder.payment_signature))
                {
                    payOrder.Status = "success";
                }
                else
                {
                    payOrder.Status = "failed";
                }


                payOrder.CompanyId = new LoggedInUser().DefaultCompanyId;
                var x = paymentService.UpdateOrderByOrderId(payOrder);
                if (payOrder.Status == "success" && payOrder.PackageId > 0)
                {
                    var packageService = new RentacPackageService();
                    var package = packageService.ClientPackageSel(payOrder.ClientId);
                    msg.Data = new { orderId = x, lcd = package };
                }
                else
                    msg.Data = new { orderId = x };
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<ApiMessage> ReconcilePayments()
        {
            var msg = new ApiMessage();
            try
            {
                var paymentService = new PaymentService();
                var payOrder = new PaymentOrderDTO();
                payOrder.ClientId = new LoggedInUser().RbnClientId;
                payOrder.CompanyId = new LoggedInUser().DefaultCompanyId;
                var x = paymentService.SettlePayments(payOrder.ClientId, payOrder.CompanyId);
                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiMessage> razorpayWebhook()
        {
            var msg = new ApiMessage();
            try
            {
                var jsonString = String.Empty;

                HttpContext.Current.Request.InputStream.Position = 0;
                using (var inputStream = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    jsonString = inputStream.ReadToEnd();
                }
                logger.Log(new LogEventInfo
                {
                    Message = jsonString,
                    Level = LogLevel.Info
                });

                var jsonObject = JObject.Parse(jsonString);
                var payloadEntity = jsonObject["payload"]["payment"]["entity"];
                if (payloadEntity != null)
                {
                    var paymentService = new PaymentService();
                    PaymentOrderDTO dto = new PaymentOrderDTO();

                    dto.Error = dto.code = Convert.ToString(payloadEntity["error_code"]);
                    dto.description = Convert.ToString(payloadEntity["error_description"]);
                    dto.source = Convert.ToString(payloadEntity["error_source"]);
                    dto.reason = Convert.ToString(payloadEntity["error_reason"]);
                    dto.orderId = Convert.ToString(payloadEntity["order_id"]);
                    dto.payment_id = Convert.ToString(payloadEntity["id"]);
                    dto.Status = Convert.ToString(payloadEntity["status"]);

                    if (dto.Status.ToLower() == "captured")
                    {
                        dto.Status = "success";
                        long ticks = Convert.ToInt64(payloadEntity["created_at"]);

                        dto.PaymentDate = DateTimeOffset.FromUnixTimeSeconds(ticks).DateTime;

                        ;
                    }
                    else if (!String.IsNullOrEmpty(dto.Error))
                    {
                        dto.Status = "failed";
                    }
                    if (dto.Status == "success" || dto.Status == "failed")
                    {
                        var x = paymentService.UpdateOrderByOrderId(dto);
                    }
                }


                msg.Code = ApiMessageCodes.SUCCESS;
                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

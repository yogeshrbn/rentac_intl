using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using BAL.DAL;
using BAL.DTO;
using BAL.Enums;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using Razorpay.Api;
namespace BAL.Objects
{
    public class PaymentService
    {
        public async Task<PaymentOrderResponseVM> CreatePaymentOrder(PaymentOrderDTO payOrder)
        {
            var pvmRes = new PaymentOrderResponseVM();
            var paymentDal = new PaymentDAL();
            //  var payOrder = new PaymentOrderDTO();

            //payOrder.UniqueId = payOrder.orderId;
            //payOrder.Amount = payOrder.Amount;
            //  payOrder.ClientId = payOrder.ClientId;
            payOrder.UniqueId = Utils.GetUniqueId();
            payOrder.CreationDate = DateTime.Now;
            payOrder.Status = "New";
            var created = paymentDal.CreateOrder(payOrder);
            if (created == 0)
            {
                pvmRes.Code = 1502;
                pvmRes.Message = "Could not create order";
                return pvmRes;
            }
            payOrder.RazorPayKey = ConfigurationManager.AppSettings["RazorPayKeyId"];

            var orderUrl = ConfigurationManager.AppSettings["RazorPayOrderUrl"];


            var order = new
            {
                amount = payOrder.Amount * 100,
                receipt = payOrder.UniqueId,
                currency = "INR"
            };
            var content = new StringContent(JsonConvert.SerializeObject(order));
            var res = await this.PostRequest(orderUrl, content);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                pvmRes.Code = 1501;
                pvmRes.Message = "Error while calling razor pay api";

                return pvmRes;
            }
            var str = await res.Content.ReadAsStringAsync();
            var orderRes = JsonConvert.DeserializeObject<PaymentOrderResponse>(str);
            if (orderRes.error != null)
            {
                pvmRes.Code = 1502;
                pvmRes.Message = orderRes.error.Description;
                payOrder.Status = "Failed";
                payOrder.Error = pvmRes.Message;
                paymentDal.UpdateOrder(payOrder);
                return pvmRes;
            }
            else
            {
                payOrder.Status = orderRes.status;
                payOrder.orderId = orderRes.id;
                paymentDal.UpdateOrder(payOrder);

            }
            pvmRes.Code = 200;
            pvmRes.orderId = orderRes.id;
            return pvmRes;
        }

        public int UpdateOrderByOrderId(PaymentOrderDTO payOrder)
        {
            var paymentDal = new PaymentDAL();

            var ret = paymentDal.UpdateOrderByOrderId(payOrder);
            if (ret != null && ret.Status.ToLower() == "success" && ret.PaymentType.ToLower() != "party")
            {
                payOrder.PackageId = ret.PackageId;
                payOrder.Amount = ret.Amount;

                //insert clientpackage
                var rentacPackageService = new RentacPackageService();
                var clientPackage = new ClientPackageDTO();
                clientPackage.InjectFrom(payOrder);
                clientPackage.RBNClientID = ret.ClientId;
                clientPackage.CompanyId = ret.CompanyId;
                clientPackage.Amount = payOrder.Amount;
                //  clientPackage.PurchasedBy = ret.
                var package = rentacPackageService.PackageById(payOrder.PackageId);
                if (package == null)
                {
                    throw new Exception("Invalid package");
                }
                clientPackage.Challans = package.Challans;
                clientPackage.Users = package.Users;
                clientPackage.Companies = package.Companies;
                clientPackage.GST = package.GST;
                
                clientPackage.PurchasedDate = payOrder.PaymentDate;
                if (payOrder.Amount == package.MonthlyPrice)
                {
                    clientPackage.MonthlyYearly = "monthly";
                    clientPackage.ValidTill = clientPackage.PurchasedDate.AddMonths(1).AddDays(-1);
                }
                else if (payOrder.Amount == package.YearlyPrice)
                {
                    clientPackage.MonthlyYearly = "yearly";
                    clientPackage.ValidTill = clientPackage.PurchasedDate.AddYears(1).AddDays(-1);
                }
                else
                {
                    clientPackage.MonthlyYearly = "yearly";
                    clientPackage.ValidTill = clientPackage.PurchasedDate.AddYears(1).AddDays(-1);
                }

                clientPackage.payment_id = payOrder.payment_id;
                var purchased = rentacPackageService.PurchasePackage(clientPackage);

                //create a billing record.
                if (purchased == 1)
                {
                    var billingDto = GetBillingDto(clientPackage);
                    if (billingDto != null)
                    {
                        billingDto.PackageId = clientPackage.PackageId;
                        billingDto.LiteItemDescription = "Rentac " + clientPackage.MonthlyYearly + "Package (" + package.Name + ")";
                        billingDto.payment_id = payOrder.payment_id;
                        var _dal = new SubscriptionDAL();
                        _dal.SaveBillig(billingDto);
                    }
                }

                /*
                 // this is for pay as you go, we will implement this in future.

                 var billing = new SubscriptionService();
                 var order = paymentDal.GetOrderByPaymentId(payOrder.payment_id);
                 if (order != null)
                 {
                     var filter = new RentacBillingFilterDTO();
                     filter.ClientId = payOrder.ClientId;
                     filter.CompanyId = payOrder.CompanyId;
                     var bills = billing.GetPendingInvoice(filter);
                     var balance = order.Amount;
                     foreach (var bill in bills.OrderBy(o => o.Balance))
                     {
                         balance = balance - bill.Balance;
                         if (balance <= 0)
                         {
                             bill.Balance = Math.Abs(balance);
                         }
                         else
                         {
                             bill.Balance = 0;
                         }

                         bill.PaymentDate = order.PaymentDate;
                         bill.PaymentRefId = order.payment_id;
                         if (balance <= 0)
                             break;
                     }

                     var modifiedBills = bills.Where(o => !String.IsNullOrEmpty(o.PaymentRefId)).ToList();
                     billing.updateInvoice(modifiedBills);
                 }
             */

            }
            else if (ret != null && ret.Status.ToLower() == "success" && ret.PaymentType.ToLower() == "party")
            {


                LedgerDAL objLedger = new LedgerDAL();
                int txnId = objLedger.CreateTransactions(new LedgerTransactionDTO
                {
                    TransactionAmount = Convert.ToDouble(ret.Amount),
                    TransactionDate = ret.PaymentDate,//.ToShortDateString(),
                    LedgerId = ret.PayerLedgerId,
                    CrLedgerId = ret.PayerLedgerId,
                    Description = "paid via party portal",
                    CreatedBy = ret.PayerLedgerId,
                    TransactionType = 1, //debit
                    TransactionMode = TransactionModes.Cash,
                    EntryType = 12,
                    Narration = "paid via party portal",
                    TranRefNumber = ret.payment_id.ToString(),
                    FinYearId = 0,
                    CompanyId = ret.PayeeCompanyId,
                    LedgerSiteId = 0


                });

            }
            return 1;
        }
        RentacBillingDTO GetBillingDto(ClientPackageDTO clientPackage)
        {
            var billingDTO = new RentacBillingDTO();
            var objClient = new Ledger(clientPackage.RBNClientID);

            var client = new RBNClient();
            var clientInfo = client.GetInfo(clientPackage.RBNClientID);

            Address objAddress = new Address();

            //List<AddressDTO> addressDTO = objAddress.GetAddresses(BAL.Enums.AddressRoleType.Ledger, dto.LedgerId);
            //clientInfo.BillingAddress = addressDTO.Where(o => o.AddressTypeId == Convert.ToInt16(BAL.Enums.AddressType.Billing)).FirstOrDefault();
            //clientInfo.ShippingAddress = addressDTO.Where(o => o.AddressTypeId == Convert.ToInt16(BAL.Enums.AddressType.Shipping)).FirstOrDefault();

            if (clientInfo.Address1 != null)
            {
                billingDTO.BillingAddress = clientInfo.Address1 + " " + clientInfo.Address2;
                billingDTO.BillingCity = String.IsNullOrEmpty(clientInfo.City) ? "" : clientInfo.City;
                billingDTO.BillingPinCode = Convert.ToInt32(clientInfo.PinCode);
            }
            else
            {
                billingDTO.BillingAddress = "";
                billingDTO.BillingCity = "";
                billingDTO.BillingPinCode = 0;

            }
            //if (clientInfo.StateName != null && clientInfo.StateName.ToLower().Contains("delhi"))
            //{
            //    billingDTO.SGSTRate = 9;
            //    billingDTO.CGSTRate = 9;
            //}
            //else
            //{
            //    billingDTO.IGSTRate = 18;
            //}
            billingDTO.SGSTRate = 9;
            billingDTO.CGSTRate = 9;
            billingDTO.SubTotal = Utils.CalculateReverseGST(clientPackage.Amount, 18);
            //if (billingDTO.IGSTRate > 0)
            //    billingDTO.IGST = Utils.CalculateReverseGST(clientPackage.Amount, billingDTO.IGSTRate);
            //  if (billingDTO.SGSTRate > 0)
            billingDTO.SGST = (clientPackage.Amount - billingDTO.SubTotal) / 2;
            // if (billingDTO.CGSTRate > 0)
            billingDTO.CGST = (clientPackage.Amount - billingDTO.SubTotal) / 2;

            // billingDTO.SubTotal = clientPackage.Amount - (billingDTO.IGST + billingDTO.SGST + billingDTO.CGST);

            //billingDTO.BaseAmount = clientPackage.Amount - (billingDTO.IGST + billingDTO.SGST + billingDTO.CGST);

            //billingDTO.SGST = billingDTO.SGSTRate * billingDTO.SubTotal / 100;
            //billingDTO.CGST = billingDTO.CGSTRate * billingDTO.SubTotal / 100;

            billingDTO.Tax = billingDTO.IGST + billingDTO.CGST + billingDTO.SGST;
            billingDTO.TotalItems = 1;

            billingDTO.TotalAmount = clientPackage.Amount;
            billingDTO.Month = clientPackage.PurchasedDate.Month;
            billingDTO.Year = clientPackage.PurchasedDate.Year;
            billingDTO.CreationDate = DateTime.Now;
            billingDTO.DueDate = clientPackage.PurchasedDate;
            billingDTO.ClientId = clientPackage.RBNClientID;
            billingDTO.CompanyId = clientPackage.CompanyId;


            billingDTO.Remarks = "";
            return billingDTO;
        }
        public async Task<HttpResponseMessage> PostRequest(string url, HttpContent content)
        {
            var key = ConfigurationManager.AppSettings["RazorPayKeyId"];
            var secret = ConfigurationManager.AppSettings["RazorPayKeySecret"];
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(key + ":" + secret));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", auth);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return await client.PostAsync(url, content);
            }
        }

        public int SettlePayments(int clientId, int companyId)
        {
            var paymentDal = new PaymentDAL();
            var today = DateTime.Today;
            var firstOfMonth = new DateTime(today.Year, today.Month, 1);
            var ret = paymentDal.GetUnsettledPayments(clientId, firstOfMonth, firstOfMonth.AddMonths(1).AddDays(-1));
            if (ret != null)
            {
                var billing = new SubscriptionService();

                var filter = new RentacBillingFilterDTO();
                filter.ClientId = clientId;
                filter.CompanyId = companyId;
                /*
                 * this is when we use payas you go, currently we are using prepaid packages so not required
                var bills = billing.GetPendingInvoice(filter);

                foreach (var order in ret)
                {
                    var balance = order.Amount;
                    foreach (var bill in bills.OrderBy(o => o.Balance))
                    {
                        balance = balance - bill.Balance;
                        if (balance <= 0)
                        {
                            bill.Balance = Math.Abs(balance);
                        }
                        else
                        {
                            bill.Balance = 0;
                        }

                        bill.PaymentDate = order.PaymentDate;
                        bill.PaymentRefId = order.payment_id;
                        if (balance <= 0)
                            break;
                    }
                }
                var modifiedBills = bills.Where(o => !String.IsNullOrEmpty(o.PaymentRefId)
                && ret.Select(m => m.payment_id).Contains(o.PaymentRefId)
                ).ToList();
                billing.updateInvoice(modifiedBills);
                */
            }
            return 0;
        }
        public List<PaymentOrderDTO> AllPayments(int clientId, DateTime fromDate, DateTime endDate)
        {
            var paymentDal = new PaymentDAL();

            return paymentDal.AllPayments(clientId, fromDate, endDate);
        }

        public List<PaymentOrderDTO> PartyPayments(string mobile)
        {
            var paymentDal = new PaymentDAL();
            return paymentDal.PartyPayments(mobile);
        }

    }
}

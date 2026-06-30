using BAL.DAL;
using BAL.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Policy;
using System.Web.UI.WebControls;
using BAL.Common;
using Omu.ValueInjecter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace BAL.Objects
{
    public class EInvoiceService
    {

        string email = ConfigurationManager.AppSettings["masterGSTEmail"];
        public async Task<IRPToken> Authenticate(IRPRequestParams requestParams, LoggedInUserInfo user)
        {

            var base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/einvoice/authenticate?email=" + email;
            var response = new HttpResponseMessage();

            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                requestParams.GSTIN = eInvoiceGSTIN;
            }
            using (var client = new HttpClient())
            {
                AddDefaultHeaders(client);
                client.DefaultRequestHeaders.Add("username", requestParams.Username);
                client.DefaultRequestHeaders.Add("password", requestParams.Password);
                client.DefaultRequestHeaders.Add("gstin", requestParams.GSTIN);

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.GetAsync(_url);
            }
            var cDto = new IRPToken();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var dataObject = data["data"];
                    var error = data["error"];
                    if (error != null)
                    {
                        throw new Exception(Convert.ToString(error["message"]));
                    }
                    cDto.Username = Convert.ToString(dataObject["UserName"]);
                    cDto.TokenExpiry = Convert.ToDateTime(dataObject["TokenExpiry"]);
                    cDto.Sek = Convert.ToString(dataObject["Sek"]);
                    cDto.ClientId = Convert.ToString(dataObject["ClientId"]);
                    cDto.AuthToken = Convert.ToString(dataObject["AuthToken"]);
                    cDto.CreatedOn = DateTime.Now;
                    cDto.CreatedBy = user.UserId;
                    cDto.CompanyId = user.DefaultCompanyId;


                    var dal = new IRPDAL();
                    var result = dal.Save(cDto);
                    if (result)
                    {
                        var company = new Company();
                        company.CompanyId = cDto.CompanyId;
                        company.IRPToken = cDto.AuthToken;
                        company.IRPTokenExpiry = cDto.TokenExpiry;
                        result = company.UpdateIPRToken(company);

                        return cDto;
                    }
                    throw new Exception("Error while saving token");

                }
            }
            else
            {
                throw new Exception("");
            }
            return null;
        }
        /*
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
        */
        public async Task<HttpResponseMessage> PostRequest(string url, HttpContent content)
        {
            var clientId = ConfigurationManager.AppSettings["masterGST_IRP_ClientId"];
            var secret = ConfigurationManager.AppSettings["masterGST_IRP_Secret"];
            var ipAddress = ConfigurationManager.AppSettings["ipAddress"];


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", auth);
                content.Headers.Add("client_id", clientId);
                content.Headers.Add("client_secret", secret);

                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return await client.PostAsync(url, content);
            }
        }

        public JObject PrepareIRNData(BillingDTO billDto)
        {
            var jobject = new JObject();
            jobject.Add("Version", "1.1");
            var tranDtls = new JObject();
            tranDtls.Add("TaxSch", "GST");
            tranDtls.Add("SupTyp", "B2B");

            //  var billSumary = items.First();

            int buyerId = billDto.LedgerId;
            int sellerId = billDto.CompanyId;

            var buyer = new Ledger(buyerId).GetDetails();
            var seller = new Company(sellerId).GetDetails();


            tranDtls.Add("RegRev", "Y");
            //   tranDtls.Add("EcmGstin", "null");
            tranDtls.Add("IgstOnIntra", "N");

            jobject.Add("TranDtls", tranDtls);
            var docDtls = new JObject();
            docDtls.Add("Typ", "INV");
            docDtls.Add("No", billDto.InvoiceNumber);
            docDtls.Add("Dt", billDto.InvoiceDate.ToString("dd/MM/yyyy").Replace("-", "/"));
            jobject.Add("DocDtls", docDtls);

            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                seller.GSTNo = eInvoiceGSTIN;
            }
            var sellerDtls = new JObject();
            sellerDtls.Add("Gstin", seller.GSTNo);
            sellerDtls.Add("LglNm", seller.LegalName);
            sellerDtls.Add("TrdNm", seller.TradeName);
            sellerDtls.Add("Addr1", seller.Address1);
            sellerDtls.Add("Addr2", seller.Address2);
            sellerDtls.Add("Loc", seller.City);
            sellerDtls.Add("Pin", seller.ZipCode);
            sellerDtls.Add("Stcd", seller.StateCode.ToString());
            sellerDtls.Add("ph", seller.Phone1);
            sellerDtls.Add("em", seller.Email);

            jobject.Add("SellerDtls", sellerDtls);

            var buyerDtls = new JObject();
            buyerDtls.Add("Gstin", buyer.GSTNo);
            buyerDtls.Add("LglNm", buyer.Name);
            buyerDtls.Add("TrdNm", buyer.TradeName);
            buyerDtls.Add("Pos", buyer.StateCode.ToString());

            buyerDtls.Add("Addr1", buyer.Address1);
            buyerDtls.Add("Addr2", buyer.Address2);
            buyerDtls.Add("Loc", buyer.City);
            buyerDtls.Add("Pin", buyer.ZipCode);
            buyerDtls.Add("Stcd", buyer.StateCode.ToString());
            buyerDtls.Add("ph", buyer.Phone1);
            buyerDtls.Add("em", buyer.Email);
            jobject.Add("BuyerDtls", buyerDtls);

            var itemDetails = new JArray();
            short serialNo = 1;
            double totalAssValueOfAllItems = 0;
            foreach (var item in billDto.BillableItems)
            {
                var itemMaster = item.ItemMaster;
                var itemObject = new JObject();
                itemObject.Add("SlNo", serialNo.ToString());

                itemObject.Add("IsServc", itemMaster.ProductType == 1 ? "N" : "Y");

                itemObject.Add("HsnCd", itemMaster.HSNCode);
                itemObject.Add("UnitPrice", item.Rate);
                itemObject.Add("TotAmt", item.SubTotal);

                var assAmount = item.SubTotal - item.Discount;
                totalAssValueOfAllItems += assAmount;
                itemObject.Add("AssAmt", assAmount);
                double gstRate = 0;
                if (item.IGST > 0)
                {
                    gstRate = item.IGST * 100 / item.SubTotal;
                }
                else
                {
                    gstRate = item.SGST * 100 / item.SubTotal;
                }

                itemObject.Add("GstRt", gstRate);

                var totalItemVal = 0d;
                totalItemVal = assAmount + item.CGST + item.SGST + item.IGST;
                itemObject.Add("TotItemVal", totalItemVal);
                itemObject.Add("PrdDesc", itemMaster.Description);
                itemObject.Add("Qty", item.Quantity);
                itemObject.Add("Unit", itemMaster.Unit);

                itemObject.Add("IgstAmt", item.IGST);
                itemObject.Add("CgstAmt", item.CGST);
                itemObject.Add("SgstAmt", item.SGST);


                itemDetails.Add(itemObject);
                serialNo++;
            }
            jobject.Add("ItemList", itemDetails);

            var valDtls = new JObject();
            var totalSGST = billDto.BillableItems.Sum(o => o.SGST);
            var totalCGST = billDto.BillableItems.Sum(o => o.CGST);
            var totalIGST = billDto.BillableItems.Sum(o => o.IGST);
            var totalInvoiceValue = totalAssValueOfAllItems + totalCGST + totalIGST + totalSGST;
            valDtls.Add("AssVal", totalAssValueOfAllItems);
            valDtls.Add("CgstVal", totalCGST);
            valDtls.Add("SgstVal", totalSGST);
            valDtls.Add("IgstVal", totalIGST);
            valDtls.Add("TotInvVal", totalInvoiceValue);

            jobject.Add("ValDtls", valDtls);

            return jobject;

        }

        public async Task<bool> CreateIRN(BillingDTO billDto)
        {
            var _data = PrepareIRNData(billDto);
            if (_data != null)
            {
                var comp = new Company(billDto.CompanyId);
                var compDto = comp.GetDetails();
                IRPRequestParams requestParams = new IRPRequestParams();
                requestParams.GSTIN = compDto.GSTNo;
                requestParams.Username = compDto.IRPUserName;
                requestParams.Password = compDto.IRPPassword;

                var userInfo = new LoggedInUserInfo();
                userInfo.UserId = billDto.UserId;
                userInfo.DefaultCompanyId = billDto.CompanyId;

                var irpToken = new IRPToken();
                if (String.IsNullOrEmpty(compDto.IRPToken) || compDto.IRPTokenExpiry < DateTime.Now)
                {
                    irpToken = await Authenticate(requestParams, userInfo);
                }
                else
                {
                    irpToken.AuthToken = compDto.IRPToken;
                }
                var env = ConfigurationManager.AppSettings["Environment"];
                var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
                if (env.ToLower() != "prod")
                {
                    requestParams.GSTIN = eInvoiceGSTIN;
                }
                string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
                var _url = base_url + "/einvoice/type/GENERATE/version/V1_03?email=" + email;
                var response = new HttpResponseMessage();
                var dataString = JsonConvert.SerializeObject(_data);
                var content = new StringContent(dataString);

                using (var client = new HttpClient())
                {
                    var clientId = ConfigurationManager.AppSettings["masterGST_IRP_ClientId"];
                    var secret = ConfigurationManager.AppSettings["masterGST_IRP_Secret"];
                    var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

                    content.Headers.Add("client_id", clientId);
                    content.Headers.Add("client_secret", secret);

                    content.Headers.Add("ip_address", ipAddress);

                    content.Headers.Add("username", requestParams.Username);

                    content.Headers.Add("gstin", requestParams.GSTIN);
                    content.Headers.Add("auth-token", irpToken.AuthToken);

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(_url, content);

                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var dataStr = await response.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(dataStr))
                    {
                        throw new Exception("Generate IRN response is empty");
                    }
                    var data = JObject.Parse(dataStr);
                    if (data != null)
                    {
                        var dataObject = data["data"];
                        var status_cd = Convert.ToString(data["status_cd"]);
                        var status_desc = data["status_desc"];
                        if (status_cd == "0" && status_desc != null)
                        {
                            var discArray = JArray.Parse(status_desc.ToString());
                            var msg = discArray.First();
                            if (msg != null)
                            {
                                var erroMessage = Convert.ToString(msg["ErrorMessage"]);
                                var errorCode = Convert.ToString(msg["ErrorCode"]);
                                //duplicate IRN
                                if (errorCode == "2150")
                                {
                                    return await GetIRNByDocNumber(billDto);
                                }
                                throw new Exception(erroMessage);
                            }
                        }


                        var irnDetails = new InvoiceIRNDTO();
                        irnDetails.CompanyId = billDto.CompanyId;
                        irnDetails.IRN = Convert.ToString(dataObject["Irn"]);
                        irnDetails.IRNStatus = Convert.ToString(dataObject["Status"]);
                        irnDetails.AckNo = Convert.ToString(dataObject["AckNo"]);
                        irnDetails.AckDate = Convert.ToDateTime(dataObject["AckDt"]);
                        irnDetails.SingedInvoice = Convert.ToString(dataObject["SignedInvoice"]);
                        irnDetails.SingedQrCode = Convert.ToString(dataObject["SignedQRCode"]);
                        irnDetails.InvoiceId = billDto.InvoiceId;
                        irnDetails.CreatedBy = billDto.UserId;
                        irnDetails.CreatedOn = DateTime.Now;

                        var dal = new IRPDAL();
                        var result = dal.UpdateInvoiceIRNDetails(irnDetails);
                        return result;
                    }
                }
            }
            return false;
        }

        public async Task<bool> GetIRNByDocNumber(BillingDTO billDto)
        {
            //var _data = PrepareIRNData(billDto);

            var comp = new Company(billDto.CompanyId);
            var compDto = comp.GetDetails();
            IRPRequestParams requestParams = new IRPRequestParams();
            requestParams.GSTIN = compDto.GSTNo;
            requestParams.Username = compDto.IRPUserName;
            requestParams.Password = compDto.IRPPassword;

            var userInfo = new LoggedInUserInfo();
            userInfo.UserId = billDto.UserId;
            userInfo.DefaultCompanyId = billDto.CompanyId;

            var irpToken = new IRPToken();
            if (String.IsNullOrEmpty(compDto.IRPToken) || compDto.IRPTokenExpiry < DateTime.Now)
            {
                irpToken = await Authenticate(requestParams, userInfo);
            }
            else
            {
                irpToken.AuthToken = compDto.IRPToken;
            }
            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                requestParams.GSTIN = eInvoiceGSTIN;
            }
            string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/einvoice/type/GETIRNBYDOCDETAILS/version/V1_03?param1=INV&email=" + email;
            var response = new HttpResponseMessage();
            //var dataString = JsonConvert.SerializeObject(_data);
          //  var content = new StringContent(dataString);

            using (var client = new HttpClient())
            {
                var clientId = ConfigurationManager.AppSettings["masterGST_IRP_ClientId"];
                var secret = ConfigurationManager.AppSettings["masterGST_IRP_Secret"];
                var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
                AddDefaultHeaders(client);


                client.DefaultRequestHeaders.Add("username", requestParams.Username);

                client.DefaultRequestHeaders.Add("gstin", requestParams.GSTIN);
                client.DefaultRequestHeaders.Add("auth-token", irpToken.AuthToken);
                client.DefaultRequestHeaders.Add("docnum", billDto.InvoiceNumber);
                client.DefaultRequestHeaders.Add("docdate", billDto.InvoiceDate.ToString("dd/MM/yyyy").Replace("-", "/"));

                //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(_url);

            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Generate IRN response is empty");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var dataObject = data["data"];
                    var status_cd = Convert.ToString(data["status_cd"]);
                    var status_desc = data["status_desc"];
                    if (status_cd == "0" && status_desc != null)
                    {
                        var discArray = (JArray)status_desc;
                        var msg = discArray.First();
                        if (msg != null)
                        {
                            var erroMessage = Convert.ToString(msg["ErrorMessage"]);
                            throw new Exception(erroMessage);
                        }
                    }


                    var irnDetails = new InvoiceIRNDTO();
                    irnDetails.CompanyId = billDto.CompanyId;
                    irnDetails.IRN = Convert.ToString(dataObject["Irn"]);
                    irnDetails.IRNStatus = Convert.ToString(dataObject["Status"]);
                    irnDetails.AckNo = Convert.ToString(dataObject["AckNo"]);
                    irnDetails.AckDate = Convert.ToDateTime(dataObject["AckDt"]);
                    irnDetails.SingedInvoice = Convert.ToString(dataObject["SignedInvoice"]);
                    irnDetails.SingedQrCode = Convert.ToString(dataObject["SignedQRCode"]);
                    irnDetails.InvoiceId = billDto.InvoiceId;
                    irnDetails.CreatedBy = billDto.UserId;
                    irnDetails.CreatedOn = DateTime.Now;

                    var dal = new IRPDAL();
                    var result = dal.UpdateInvoiceIRNDetails(irnDetails);
                    return result;
                }
            }

            return false;
        }

        void AddDefaultHeaders(HttpClient client)
        {
            var clientId = ConfigurationManager.AppSettings["masterGST_IRP_ClientId"];
            var secret = ConfigurationManager.AppSettings["masterGST_IRP_Secret"];
            var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

            client.DefaultRequestHeaders.Add("client_id", clientId);
            client.DefaultRequestHeaders.Add("client_secret", secret);

            client.DefaultRequestHeaders.Add("ip_address", ipAddress);

        }

    }
}

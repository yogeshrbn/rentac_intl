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

namespace BAL.Objects
{
    public class GSTService
    {
        public async Task<CompanyDTO> GetTaxPayerDetails(string gstIn)
        {
            var email = ConfigurationManager.AppSettings["masterGSTEmail"];
            var base_url = ConfigurationManager.AppSettings["masterGST_GST_baseUrl"];
            var _url = base_url + "/public/search?email=" + email + "&gstin=" + gstIn;
            var response = await GetRequest(_url);
            var cDto = new CompanyDTO();
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
                    cDto.LegalName = Convert.ToString(dataObject["lgnm"]);
                    cDto.GSTRegistrationDate = Convert.ToString(dataObject["rgdt"]);
                    cDto.GSTStatus = Convert.ToString(dataObject["sts"]);
                    cDto.TradeName = Convert.ToString(dataObject["tradeNam"]);
                    cDto.GSTNo = Convert.ToString(dataObject["gstin"]);
                    var states = State.GetAllStates();
                    if (dataObject["pradr"] != null)
                    {
                        var pradr = dataObject["pradr"];
                        var addr = pradr["addr"];
                        if (addr != null)
                        {
                            cDto.Address1 = Convert.ToString(addr["bno"]) + " " + Convert.ToString(addr["bnm"]) + " " + Convert.ToString(addr["st"]);

                            cDto.Address2 = Convert.ToString(addr["loc"]) + " " + Convert.ToString(addr["locality"])
                                + " " + Convert.ToString(addr["landMark"]);

                            cDto.ZipCode = Convert.ToString(addr["pncd"]);
                            cDto.City = Convert.ToString(addr["dst"]);
                            if (states != null)
                            {
                                var st = Convert.ToString(addr["stcd"]);
                                if (!String.IsNullOrEmpty(st))
                                {
                                    var state = states.Find(o => o.StateName == st);
                                    if (state != null)
                                    {
                                        cDto.StateId = state.StateId;
                                    }
                                }
                            }
                        }
                    }



                    return cDto;

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
            var clientId = ConfigurationManager.AppSettings["masterGST_GST_ClientId"];
            var secret = ConfigurationManager.AppSettings["masterGST_GST_Secret"];
            var email = ConfigurationManager.AppSettings["masterGSTEmail"];


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


        public async Task<HttpResponseMessage> GetRequest(string url)
        {
            var clientId = ConfigurationManager.AppSettings["masterGST_GST_ClientId"];
            var secret = ConfigurationManager.AppSettings["masterGST_GST_Secret"];



            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", auth);
                client.DefaultRequestHeaders.Add("client_id", clientId);
                client.DefaultRequestHeaders.Add("client_secret", secret);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                return await client.GetAsync(url);
            }
        }
    }
}

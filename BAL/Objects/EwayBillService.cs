using BAL.Common;
using BAL.DAL;
using BAL.DTO;
using BAL.Exceptions;
using BAL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Omu.ValueInjecter;
using PdfSharp.Pdf.Content.Objects;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Xml.Linq;


namespace BAL.Objects
{
    public class EwayBillService
    {
        string email = ConfigurationManager.AppSettings["masterGSTEmail"];

        Logger logger = LogManager.GetCurrentClassLogger();
        public async Task<bool> CreateEwayBill(BillingDTO billDto, LoggedInUserInfo user, EwayBillDTO ewayBill)
        {
            try
            {
                int buyerId = billDto.LedgerId;
                int sellerId = billDto.CompanyId;
                var seller = new Company(sellerId).GetDetails();

                if (!String.IsNullOrEmpty(seller.EwayUserName))
                {
                    if (seller.EwayLastAuthenticatedOn.AddHours(6) < DateTime.Now)
                    {
                        var auth = await Authenticate(seller, user);
                        if (!auth)
                        {
                            throw new Exception("Could not authenticate from E-waybill portal");
                        }
                    }
                }
                var env = ConfigurationManager.AppSettings["Environment"];
                var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
                if (env.ToLower() != "prod")
                {
                    seller.GSTNo = eInvoiceGSTIN;
                }

                var jData = PrepareData(billDto, user, ewayBill);
                string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
                var _url = base_url + "/ewaybillapi/v1.03/ewayapi/genewaybill?email=" + email;
                var response = new HttpResponseMessage();
                var dataString = JsonConvert.SerializeObject(jData);
                var content = new StringContent(dataString);

                logger.Error(dataString);
                using (var client = new HttpClient())
                {
                    var clientId = ConfigurationManager.AppSettings["masterGST_Eway_ClientId"];
                    var secret = ConfigurationManager.AppSettings["masterGST_Eway_Secret"];
                    var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

                    content.Headers.Add("client_id", clientId);
                    content.Headers.Add("client_secret", secret);
                    content.Headers.Add("ip_address", ipAddress);

                    content.Headers.Add("gstin", seller.GSTNo);


                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(_url, content);

                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //  var responseStr = await response.Content.ReadAsStringAsync();
                    var dataStr = await response.Content.ReadAsStringAsync();
                    logger.Error(dataStr);
                    if (String.IsNullOrEmpty(dataStr))
                    {
                        throw new Exception("Generate IRN response is empty");
                    }
                    var data = JObject.Parse(dataStr);
                    if (data != null)
                    {
                        var dataObject = data["data"];
                        var status_cd = Convert.ToString(data["status_cd"]);
                        var status_desc = Convert.ToString(data["status_desc"]);
                        if (status_cd == "1")
                        {
                            var ewayDto = new EwayBillDTO();
                            ewayDto.InjectFrom(ewayBill);
                            ewayDto.EwayBillId = ewayBill.EwayBillId;
                            ewayDto.EwayBillNo = Convert.ToString(dataObject["ewayBillNo"]);
                            ewayDto.EwayBillDate = Utils.FormatDate(Convert.ToString(dataObject["ewayBillDate"]));
                            ewayDto.EwayBillValidUpTo = Utils.FormatDate(Convert.ToString(dataObject["validUpto"]));
                            ewayDto.EwayBillAlert = Convert.ToString(dataObject["alert"]);
                            ewayDto.EwayBillCreatedBy = user.UserId;
                            ewayDto.InvoiceId = ewayBill.InvoiceId;
                            var bill = new Billing();

                            bool result = this.UpdateEwayBillInfo(ewayDto);

                            return result;

                        }
                        else if (status_cd == "0")
                        {

                            var errorObj = data["error"];
                            var msgStr = Convert.ToString(errorObj["message"]);
                            var isValidJson = Utils.IsValidJson(msgStr);
                            if (!isValidJson)
                            {
                                throw new Exception(msgStr);
                            }

                            var msg = JObject.Parse(msgStr);

                            if (msg != null)
                            {
                                var errorCode = Convert.ToString(msg["errorCodes"]);
                                if (!String.IsNullOrEmpty(errorCode))
                                {
                                    var c = ThrowErroCodes(errorCode);
                                    if (c == 0)
                                    {
                                        throw new Exception(status_desc);
                                    }

                                }
                                else
                                {
                                    throw new Exception(status_desc);
                                }
                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                logger.Error(ex, ex.Message);

                logger.Debug(ex.Message);
                throw ex;
            }
        }

        public bool UpdateEwayBillInfo(EwayBillDTO ewayDto)
        {
            var bill = new Billing();

            bool result = bill.UpdateEwayBillPortalInfo(ewayDto);
            if (!result)
            {
                throw new UDFException("Could not update e-way bill in ewayBill", ErrorCodes.FAILED_UPDATE_INFO_ON_EWAYBILL);
            }

            if (ewayDto.DocType.ToLower() == "inv")
            {
                result = bill.UpdateEwayBillInfo(ewayDto);
                if (!result)
                {
                    throw new UDFException("Could not update e-way bill in invoice", ErrorCodes.FAILED_UPDATE_INFO_ON_BILL);
                }
            }
            else if (ewayDto.DocType.ToLower() == "chl" && ewayDto.DocSubType.ToLower() == "del")
            {
                var wo = new WorkOrder(ewayDto.InvoiceId);
                result = wo.UpdateEwayBIllNo(ewayDto.InvoiceId, ewayDto.EwayBillNo);
                if (!result)
                {
                    throw new UDFException("Could not update ewaybill no in challan", ErrorCodes.ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN);
                }
            }
            else if (ewayDto.DocType.ToLower() == "chl" && ewayDto.DocSubType.ToLower() == "ret")
            {
                var wo = new GRN();
                result = wo.UpdateEwayBIllNo(ewayDto.InvoiceId, ewayDto.CompanyId, ewayDto.EwayBillNo);
                if (!result)
                {
                    throw new UDFException("Could not update ewaybill no in challan", ErrorCodes.ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN);
                }
            }
            return true;
        }

        public async Task<bool> UpdateVehileInfo(LoggedInUserInfo user, UdpateVehicleDto ewayBill)
        {

            var seller = new Company(user.DefaultCompanyId).GetDetails();

            if (!String.IsNullOrEmpty(seller.EwayUserName))
            {
                if (seller.EwayLastAuthenticatedOn.AddHours(6) < DateTime.Now)
                {
                    var auth = await Authenticate(seller, user);
                    if (!auth)
                    {
                        throw new Exception("Could not authenticate from E-waybill portal");
                    }
                }
            }
            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                seller.GSTNo = eInvoiceGSTIN;
            }
            var allSates = State.GetAllStates();

            var selectedState = allSates.Where(o => o.StateId == Convert.ToInt16(ewayBill.FromStateId)).FirstOrDefault();
            if (selectedState == null)
            {
                throw new UDFException("State code not found.", Exceptions.ErrorCodes.STATE_GST_CODE_NOT_FOUND);
            }

            var jData = new JObject();
            jData.Add("fromPlace", ewayBill.FromPlace);
            jData.Add("fromState", Convert.ToInt16(selectedState.GSTCode));
            jData.Add("reasonCode", ewayBill.ReasonCode);
            jData.Add("reasonRem", ewayBill.Remarks);
            jData.Add("transMode", ewayBill.TransportationMode.ToString());
            jData.Add("ewbNo", Convert.ToUInt64(ewayBill.EwayBillNo));
            jData.Add("vehicleNo", ewayBill.VehicleNo);


            if (ewayBill.TransportationMode == 2 || ewayBill.TransportationMode == 3 || ewayBill.TransportationMode == 4)
            {

                jData.Add("transDocNo", "");
                jData.Add("transDocDate", "");
            }


            string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/ewaybillapi/v1.03/ewayapi/vehewb?email=" + email;
            var response = new HttpResponseMessage();
            var dataString = JsonConvert.SerializeObject(jData);
            var content = new StringContent(dataString);


            using (var client = new HttpClient())
            {
                var clientId = ConfigurationManager.AppSettings["masterGST_Eway_ClientId"];
                var secret = ConfigurationManager.AppSettings["masterGST_Eway_Secret"];
                var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

                content.Headers.Add("client_id", clientId);
                content.Headers.Add("client_secret", secret);
                content.Headers.Add("ip_address", ipAddress);

                content.Headers.Add("gstin", seller.GSTNo);


                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(_url, content);

            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status_cd = Convert.ToString(data["status_cd"]);
                    var status_desc = Convert.ToString(data["status_desc"]);
                    if (status_cd == "1")
                    {
                        return true;
                    }
                    else
                    {
                        var statusObj = JObject.Parse(status_desc);
                        if (statusObj != null)
                        {
                            var errorCode = Convert.ToString(statusObj["errorCodes"]);
                            if (!String.IsNullOrEmpty(errorCode))
                            {
                                var c = ThrowErroCodes(errorCode);
                                if (c == 0)
                                {
                                    throw new Exception(status_desc);
                                }

                            }
                            else
                            {
                                throw new Exception(status_desc);
                            }
                        }
                    }

                    throw new Exception(status_cd);

                }
            }
            else
            {
                throw new Exception("Error while calling update vehicle service");
            }
            return true;
        }

        public async Task<bool> UpdateTransporter(LoggedInUserInfo user, UdpateVehicleDto ewayBill)
        {

            var seller = new Company(user.DefaultCompanyId).GetDetails();

            if (!String.IsNullOrEmpty(seller.EwayUserName))
            {
                if (seller.EwayLastAuthenticatedOn.AddHours(6) < DateTime.Now)
                {
                    var auth = await Authenticate(seller, user);
                    if (!auth)
                    {
                        throw new Exception("Could not authenticate from E-waybill portal");
                    }
                }
            }
            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                seller.GSTNo = eInvoiceGSTIN;
            }



            var jData = new JObject();
            jData.Add("ewbNo", Convert.ToInt64(ewayBill.EwayBillNo));
            jData.Add("transporterId", ewayBill.TransporterGST);


            string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/ewaybillapi/v1.03/ewayapi/updatetransporter?email=" + email;
            var response = new HttpResponseMessage();
            var dataString = JsonConvert.SerializeObject(jData);
            var content = new StringContent(dataString);


            using (var client = new HttpClient())
            {
                var clientId = ConfigurationManager.AppSettings["masterGST_Eway_ClientId"];
                var secret = ConfigurationManager.AppSettings["masterGST_Eway_Secret"];
                var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

                content.Headers.Add("client_id", clientId);
                content.Headers.Add("client_secret", secret);
                content.Headers.Add("ip_address", ipAddress);

                content.Headers.Add("gstin", seller.GSTNo);


                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(_url, content);

            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status_cd = Convert.ToString(data["status_cd"]);
                    var status_desc = Convert.ToString(data["status_desc"]);
                    if (status_cd == "1")
                    {
                        return true;
                    }
                    else
                    {
                        var errorObj = data["error"];
                        var msg = JObject.Parse(Convert.ToString(errorObj["message"]));


                        if (msg != null)
                        {
                            var errorCode = Convert.ToString(msg["errorCodes"]);
                            if (!String.IsNullOrEmpty(errorCode))
                            {
                                var c = ThrowErroCodes(errorCode);
                                if (c == 0)
                                {
                                    throw new Exception(status_desc);
                                }

                            }
                            else
                            {
                                throw new Exception(status_desc);
                            }
                        }
                    }

                    throw new Exception(status_cd);

                }
            }
            else
            {
                throw new Exception("Error while calling update vehicle service");
            }
            return true;
        }


        public async Task<bool> CancelEwayBill(LoggedInUserInfo user, CancelEwayBillDto ewayBill)
        {

            var seller = new Company(user.DefaultCompanyId).GetDetails();

            if (!String.IsNullOrEmpty(seller.EwayUserName))
            {
                if (seller.EwayLastAuthenticatedOn.AddHours(6) < DateTime.Now)
                {
                    var auth = await Authenticate(seller, user);
                    if (!auth)
                    {
                        throw new Exception("Could not authenticate from E-waybill portal");
                    }
                }
            }
            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                seller.GSTNo = eInvoiceGSTIN;
            }



            var jData = new JObject();
            jData.Add("ewbNo", Convert.ToInt64(ewayBill.EwayBillNo));
            jData.Add("cancelRsnCode", ewayBill.CancelReasonCode);
            jData.Add("cancelRmrk", ewayBill.CancelRemarks);



            string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/ewaybillapi/v1.03/ewayapi/canewb?email=" + email;
            var response = new HttpResponseMessage();
            var dataString = JsonConvert.SerializeObject(jData);
            var content = new StringContent(dataString);


            using (var client = new HttpClient())
            {
                var clientId = ConfigurationManager.AppSettings["masterGST_Eway_ClientId"];
                var secret = ConfigurationManager.AppSettings["masterGST_Eway_Secret"];
                var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

                content.Headers.Add("client_id", clientId);
                content.Headers.Add("client_secret", secret);
                content.Headers.Add("ip_address", ipAddress);

                content.Headers.Add("gstin", seller.GSTNo);


                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(_url, content);

            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status_cd = Convert.ToString(data["status_cd"]);
                    var status_desc = Convert.ToString(data["status_desc"]);
                    if (status_cd == "1")
                    {
                        return true;
                    }
                    else
                    {
                        var errorObj = data["error"];
                        var msg = JObject.Parse(Convert.ToString(errorObj["message"]));


                        if (msg != null)
                        {
                            var errorCode = Convert.ToString(msg["errorCodes"]);
                            if (!String.IsNullOrEmpty(errorCode))
                            {
                                var c = ThrowErroCodes(errorCode);
                                if (c == 0)
                                {
                                    throw new Exception(status_desc);
                                }

                            }
                            else
                            {
                                throw new Exception(status_desc);
                            }
                        }
                    }

                    throw new Exception(status_cd);

                }
            }
            else
            {
                throw new Exception("Error while calling update vehicle service");
            }
            return true;
        }


        public async Task<List<EwayBillDTO>> FetchEwayBillsByDate(LoggedInUserInfo user, EwayBillDTO ewayBill)
        {

            var seller = new Company(user.DefaultCompanyId).GetDetails();

            if (!String.IsNullOrEmpty(seller.EwayUserName))
            {
                if (seller.EwayLastAuthenticatedOn.AddHours(6) < DateTime.Now)
                {
                    var auth = await Authenticate(seller, user);
                    if (!auth)
                    {
                        throw new Exception("Could not authenticate from E-waybill portal");
                    }
                }
            }
            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                seller.GSTNo = eInvoiceGSTIN;
            }



            //var jData = new JObject();
            //jData.Add("ewbNo", Convert.ToInt64(ewayBill.EwayBillNo));
            //jData.Add("transporterId", ewayBill.TransporterGST);


            string base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/ewaybillapi/v1.03/ewayapi/getewaybillsbydate?email=" + email + "&date=" + ewayBill.CreatedOn.ToString("dd/MM/yyyy").Replace("-", "/");
            var response = new HttpResponseMessage();
            // var dataString = JsonConvert.SerializeObject(jData);
            // var content = new StringContent(dataString);


            using (var client = new HttpClient())
            {

                AddDefaultHeaders(client);
                client.DefaultRequestHeaders.Add("gstin", seller.GSTNo);

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.GetAsync(_url);

            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status_cd = Convert.ToString(data["status_cd"]);
                    var status_desc = Convert.ToString(data["status_desc"]);
                    if (status_cd == "1")
                    {
                        var dataObj = JArray.Parse(Convert.ToString(data["data"]));
                        var lstEwayBills = new List<EwayBillDTO>();
                        foreach (JObject jo in dataObj.AsEnumerable())
                        {
                            var ew = new EwayBillDTO();
                            ew.EwayBillNo = Convert.ToString(jo["ewbNo"]);
                            ew.EwayBillDate = Utils.FormatDate(Convert.ToString(jo["ewbDate"]));
                            ew.Status = Convert.ToString(jo["status"]);
                            ew.GenGstin = Convert.ToString(jo["genGstin"]);
                            if (!String.IsNullOrEmpty(Convert.ToString(jo["docDate"])))
                                ew.DocDate = Utils.FormatDate(Convert.ToString(jo["docDate"]));
                            ew.DocNumber = Convert.ToString(jo["docNo"]);

                            ew.DelPinCode = Convert.ToString(jo["delPinCode"]);
                            ew.DelStateCode = Convert.ToString(jo["delStateCode"]);
                            ew.DelPlace = Convert.ToString(jo["delPlace"]);
                            if (!String.IsNullOrEmpty(Convert.ToString(jo["validUpto"])))
                                ew.EwayBillValidUpTo = Utils.FormatDate(Convert.ToString(jo["validUpto"]));
                            lstEwayBills.Add(ew);

                        }
                        return lstEwayBills;

                    }
                    else
                    {
                        var errorObj = data["error"];
                        var msg = JObject.Parse(Convert.ToString(errorObj["message"]));


                        if (msg != null)
                        {
                            var errorCode = Convert.ToString(msg["errorCodes"]);
                            if (!String.IsNullOrEmpty(errorCode))
                            {
                                var c = ThrowErroCodes(errorCode);
                                if (c == 0)
                                {
                                    throw new Exception(status_desc);
                                }

                            }
                            else
                            {
                                throw new Exception(status_desc);
                            }
                        }
                    }

                    throw new Exception(status_cd);

                }
                else
                {
                    throw new Exception("Empty data returned from portal");
                }
            }
            else
            {
                throw new Exception("Error while calling update vehicle service");
            }

        }

        public JObject PrepareData(BillingDTO billDto, LoggedInUserInfo user, EwayBillDTO ewayBill)
        {
            var jobject = new JObject();

            try
            {
                int buyerId = billDto.LedgerId;
                int sellerId = billDto.CompanyId;
                //var sellerDto = new CompanyDTO();
                //var buyerDto = new CompanyDTO();

                var sellerDto = ewayBill.Seller;
                var buyerDto = ewayBill.Buyer;

                //if (ewayBill.DocSubType.ToLower() == "ret")
                //{
                //    var seller = new   Ledger(buyerId).GetDetails();
                //    sellerDto.InjectFrom(seller);
                //    var buyer = new Company(sellerId).GetDetails();
                //    buyerDto.InjectFrom(buyer);

                //}
                //else
                //{
                //    var buyer = new Ledger(buyerId).GetDetails();
                //    var seller = new Company(sellerId).GetDetails();
                //    sellerDto.InjectFrom(seller);
                //    buyerDto.InjectFrom(buyer);
                //}

                var env = ConfigurationManager.AppSettings["Environment"];
                var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
                if (env.ToLower() != "prod")
                {
                    sellerDto.GST = eInvoiceGSTIN;
                }


                jobject.Add("actFromStateCode", Convert.ToInt16(sellerDto.StateCode));
                jobject.Add("actToStateCode", Convert.ToInt16(buyerDto.StateCode));
                jobject.Add("docDate", billDto.InvoiceDate.ToString("dd/MM/yyyy").Replace("-", "/"));
                jobject.Add("docNo", billDto.InvoiceNumber);
                jobject.Add("docType", ewayBill.DocType.ToUpper());
                jobject.Add("fromGstin", sellerDto.GST);
                jobject.Add("fromStateCode", Convert.ToInt16(sellerDto.StateCode));
                jobject.Add("fromPincode", Convert.ToInt32(sellerDto.ZipCode));

                //insert items details
                var itemDetails = new JArray();
                short serialNo = 1;
                var totalSGST = billDto.BillableItems.Sum(o => o.SGST);
                var totalCGST = billDto.BillableItems.Sum(o => o.CGST);
                var totalIGST = billDto.BillableItems.Sum(o => o.IGST);
                decimal totalTax = Convert.ToDecimal(totalCGST + totalSGST + totalIGST);
                decimal totInvValue = Convert.ToDecimal(billDto.Total);
                decimal totalValue = Convert.ToDecimal(billDto.SubTotal);
                if (ewayBill.ApproximateValue.HasValue && ewayBill.ApproximateValue.Value > 0)
                {
                    totalValue = totInvValue = Math.Round(ewayBill.ApproximateValue.Value, 2, MidpointRounding.AwayFromZero);
                    //if approximate value provided then taxes will be 0;

                    totalTax = 0;
                    totalSGST = 0;
                    totalIGST = 0;
                    totalCGST = 0;
                    billDto.BillableItems.ForEach(o =>
                    {
                        o.CGSTRate = 0;
                        o.IGSTRate = 0;
                        o.SGSTRate = 0;
                        o.CGST = 0;
                        o.SGST = 0;
                        o.IGST = 0;
                    });
                    //var derivedTaxable = totInvValue - totalTax;
                    //totalValue = derivedTaxable >= 0
                    //? Math.Round(derivedTaxable, 2, MidpointRounding.AwayFromZero)
                    //: Convert.ToDecimal(billDto.SubTotal);
                }
                foreach (var item in billDto.BillableItems)
                {
                    var itemMaster = item.ItemMaster;
                    var jItem = new JObject();
                    //if (ewayBill.DocType == "inv")
                    //{
                    //    jItem.Add("productName", itemMaster.Name);
                    //    jItem.Add("productDesc", itemMaster.Description);
                    //    jItem.Add("hsnCode", itemMaster.HSNCode);
                    //}
                    //else
                    //{
                    jItem.Add("productName", item.Product);
                    jItem.Add("productDesc", item.Product);
                    jItem.Add("hsnCode", item.HSNCode);
                    //}

                    //jItem.Add("quantity", item.Quantity);
                    //jItem.Add("qtyUnit", itemMaster.Unit);


                    jItem.Add("quantity", item.Quantity);
                    jItem.Add("qtyUnit", item.Unit);

                    jItem.Add("taxableAmount", item.SubTotal);
                    // double cstRate = 0, igstRate = 0, sgstRate = 0;
                    //if (item.IGST > 0)
                    //{
                    //    igstRate = item.IGST * 100 / item.SubTotal;
                    //}
                    //else
                    //{
                    //    cstRate = item.CGST * 100 / item.SubTotal;
                    //    sgstRate = item.CGST * 100 / item.SubTotal;

                    //}

                    jItem.Add("sgstRate", item.SGSTRate);
                    jItem.Add("cgstRate", item.CGSTRate);
                    jItem.Add("igstRate", item.IGSTRate);
                    jItem.Add("cessRate", 0);

                    itemDetails.Add(jItem);
                }

                jobject.Add("itemList", itemDetails);

                if (String.IsNullOrEmpty(buyerDto.Address))
                {
                    throw new UDFException("Buyer address must not be empty", ErrorCodes.ERROR_WHILE_PREPARING_DATA);
                }
                if (String.IsNullOrEmpty(buyerDto.City))
                {
                    throw new UDFException("Buyer city must not be empty", ErrorCodes.ERROR_WHILE_PREPARING_DATA);
                }
                if (String.IsNullOrEmpty(buyerDto.ZipCode))
                {
                    throw new UDFException("Buyer postal code must not be empty", ErrorCodes.ERROR_WHILE_PREPARING_DATA);
                }
                if (String.IsNullOrEmpty(buyerDto.StateCode))
                {
                    throw new UDFException("Buyer state code must not be empty", ErrorCodes.ERROR_WHILE_PREPARING_DATA);
                }
                if (String.IsNullOrEmpty(buyerDto.GST))
                {
                    throw new UDFException("Buyer must have a valid GST number", ErrorCodes.ERROR_WHILE_PREPARING_DATA);
                }

                jobject.Add("subSupplyType", ewayBill.SubTypeId.ToString());
                if (ewayBill.DocSubType.ToLower() == "ret")
                {
                    jobject.Add("supplyType", "I");
                }
                else
                {
                    jobject.Add("supplyType", "O");

                }

                jobject.Add("toGstin", buyerDto.GST);
                jobject.Add("toPincode", Convert.ToInt32(buyerDto.ZipCode));
                jobject.Add("toStateCode", Convert.ToInt16(buyerDto.StateCode));



                jobject.Add("totInvValue", totInvValue);
                jobject.Add("transDistance", ewayBill.Distance.ToString());
                jobject.Add("transMode", ewayBill.TransportationMode.ToString());
                jobject.Add("transactionType", ewayBill.TransactionType);
                //if sub subpply type is others then pass its description as others
                if (ewayBill.SubTypeId == 8)
                {
                    if (!String.IsNullOrEmpty(ewayBill.OtherTypeDesc))
                    {
                        jobject.Add("subSupplyDesc", ewayBill.OtherTypeDesc);
                    }
                }
                jobject.Add("fromTrdName", sellerDto.TradeName);
                if (!String.IsNullOrEmpty(ewayBill.ShipFromAddress))
                {
                    jobject.Add("fromAddr1", ewayBill.ShipFromAddress);
                }
                else
                    jobject.Add("fromAddr1", sellerDto.Address);
                // jobject.Add("fromAddr2", sellerDto.Address2);


                jobject.Add("fromPlace", sellerDto.City);
                jobject.Add("toTrdName", buyerDto.TradeName);


                //buyerDto.Address2 = String.IsNullOrEmpty(buyerDto.Address2) ? "" : buyerDto.Address2;

                if (!String.IsNullOrEmpty(ewayBill.ShipFromAddress))
                {
                    jobject.Add("toAddr1", ewayBill.ShipToAddress);
                }
                else
                    jobject.Add("toAddr1", buyerDto.Address);

                //   jobject.Add("toAddr2", buyerDto.Address2);
                jobject.Add("toPlace", buyerDto.City);

                //jobject.Add("dispatchFromGSTIN", seller.GSTNo);
                //jobject.Add("dispatchFromTradeName", seller.TradeName);
                if (!String.IsNullOrEmpty(ewayBill.ShipToGST) && buyerDto.GST != ewayBill.ShipToGST)
                    jobject.Add("shipToGSTIN", buyerDto.GST);

                //jobject.Add("shipToTradeName", buyer.TradeName);


                jobject.Add("totalValue", totalValue);

                jobject.Add("cgstValue", totalCGST);
                jobject.Add("sgstValue", totalSGST);
                jobject.Add("igstValue", totalIGST);
                jobject.Add("cessValue", 0);
                jobject.Add("cessNonAdvolValue", 0);
                if (!String.IsNullOrEmpty(ewayBill.TransporterName))
                {
                    jobject.Add("transporterName", ewayBill.TransporterName);

                    jobject.Add("transporterId", ewayBill.TransporterGST);
                }
                //2=Rail,3=Air,4=Ship
                if (ewayBill.TransportationMode == 2 || ewayBill.TransportationMode == 3 || ewayBill.TransportationMode == 4)
                {
                    jobject.Add("transDocDate", ewayBill.TransporterDocDate.ToString("dd/MM/yyyy").Replace("-", "/"));
                    jobject.Add("transDocNo", ewayBill.TransporterDocNo);

                }

                //1=Rail
                if (ewayBill.TransportationMode == 1)
                {
                    jobject.Add("vehicleNo", ewayBill.VehicleNo);
                    jobject.Add("vehicleType", ewayBill.VehicleType);
                }
                return jobject;
            }
            catch (Exception ex)
            {
                throw new UDFException(ex.Message, ErrorCodes.ERROR_WHILE_PREPARING_DATA);
            }




        }
        public async Task<bool> Authenticate(CompanyDTO dto, LoggedInUserInfo user)
        {

            var base_url = ConfigurationManager.AppSettings["masterGST_IRP_baseUrl"];
            var _url = base_url + "/ewaybillapi/v1.03/authenticate?email=" + email + "&username=" + dto.EwayUserName + "&password=" + dto.EwayPassword;
            var response = new HttpResponseMessage();

            var env = ConfigurationManager.AppSettings["Environment"];
            var eInvoiceGSTIN = ConfigurationManager.AppSettings["eInvoiceGSTIN"];
            if (env.ToLower() != "prod")
            {
                dto.GSTNo = eInvoiceGSTIN;
            }
            using (var client = new HttpClient())
            {
                AddDefaultHeaders(client);
                //client.DefaultRequestHeaders.Add("username", requestParams.Username);
                //client.DefaultRequestHeaders.Add("password", requestParams.Password);
                client.DefaultRequestHeaders.Add("gstin", dto.GSTNo);

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                response = await client.GetAsync(_url);
            }

            var cDto = new IRPToken();
            var dataStr = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                // var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status_cd = Convert.ToString(data["status_cd"]);
                    var status_desc = Convert.ToString(data["status_desc"]);
                    if (status_cd == "1")
                    {
                        return true;
                    }
                    else
                    {
                        var statusObj = JObject.Parse(status_desc);
                        if (statusObj != null)
                        {
                            var errorCode = Convert.ToString(statusObj["errorCodes"]);
                            if (!String.IsNullOrEmpty(errorCode))
                            {
                                var c = ThrowErroCodes(errorCode);
                                if (c == 0)
                                {
                                    throw new Exception(status_desc);
                                }

                            }
                            else
                            {
                                throw new Exception(status_desc);
                            }
                        }
                    }

                    throw new Exception(status_cd);

                }
            }
            else
            {
                throw new Exception("");
            }
            return false;
        }

        short ThrowErroCodes(string errorCode)
        {
            var code = Convert.ToInt16(errorCode.Split(',').First());
            if (code > 0)
            {
                var codeMsg = ErrorCodeService.GetMessage(code);
                if (codeMsg != null)
                {
                    throw new UDFException(codeMsg.Message, code);
                }

            }
            return 0;

        }


        void AddDefaultHeaders(HttpClient client)
        {
            var clientId = ConfigurationManager.AppSettings["masterGST_Eway_ClientId"];
            var secret = ConfigurationManager.AppSettings["masterGST_Eway_Secret"];
            var ipAddress = ConfigurationManager.AppSettings["ipAddress"];

            client.DefaultRequestHeaders.Add("client_id", clientId);
            client.DefaultRequestHeaders.Add("client_secret", secret);

            client.DefaultRequestHeaders.Add("ip_address", ipAddress);

        }



        public DataSet PrintEwayBill(int ewayBillid)
        {
            Billing billing = new Billing();
            DataSet ds = billing.PrintEwayBill(new EwayBillDTO { EwayBillId = ewayBillid });
            int partyId = 0;

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    partyId = Convert.ToInt32(ds.Tables[0].Rows[0]["LedgerId"]);


                    var qrCodeStr = Convert.ToString(ds.Tables[0].Rows[0]["ewayBillNo"]);

                    var codeGen = new QRCodeGenerator();

                    var qrCodeData = codeGen.CreateQrCode(Encoding.UTF8.GetBytes(qrCodeStr), QRCodeGenerator.ECCLevel.Q);
                    using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
                    {


                        ds.Tables[0].Rows[0]["qrCode"] = qrCode.GetGraphic(50);// Convert.ToBase64String(qrCodeImage);
                    }


                }
            }
            return ds;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using BAL.Objects;
using BAL.DTO;
using FarmaAPI.Helper;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using System.IO;
using PdfSharp.Pdf;
using Microsoft.Reporting.WebForms;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FarmaAPI;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Drawing;
using System.Drawing;
using System.Text;
using BAL.Exceptions;
using System.Threading.Tasks;
using System.Web.Razor.Generator;
using NLog;
using Spire.Xls.Core.Interfaces;
using iTextSharp.text.xml;
using BAL.Common;
using iTextSharp.xmp.impl.xpath;
using System.Web;
using Omu.ValueInjecter;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace FarmaAPI.Controllers
{
    [Authorize]
    public class LedgerController : BaseApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string mapPath = Path.Combine(HttpRuntime.AppDomainAppPath, "docs");
        [HttpPost]
        public HttpResponseMessage Save()
        {

            Ledger objClient = new Ledger();
            ApiMessage response = new ApiMessage();


            try
            {
                string dto = System.Web.HttpContext.Current.Request["dto"];
                System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                JObject jsonObject = new JObject();

                var usr = new LoggedInUser();

                jsonObject = JObject.Parse(dto);
                var wObj = jsonObject["Props"];
                Ledger objLedger = new Ledger();
                var billingAddress = wObj["BillingAddress"];
                var shippingAddress = wObj["ShippingAddress"];
                objClient.FinYearId = usr.FinYearId;
                objClient.LedgerId = Convert.ToInt32(wObj["LedgerId"]);
                objClient.Name = Convert.ToString(wObj["Name"]);
                objClient.TradeName = Convert.ToString(wObj["TradeName"]);
                objClient.UseTradeNameForBilling = (wObj["UseTradeNameForBilling"] != null && Convert.ToInt32(wObj["UseTradeNameForBilling"]) == 1) ? (byte)1 : (byte)0;

                objClient.ForQuotation = 0;
                if (wObj["forQuotation"] != null)
                {
                    objClient.ForQuotation = Convert.ToByte(wObj["forQuotation"]);
                }
                objClient.Code = Convert.ToString(wObj["Code"]);

                objClient.TIN = Convert.ToString(wObj["TIN"]);
                objClient.TAN = Convert.ToString(wObj["TAN"]);
                if (!String.IsNullOrEmpty(Convert.ToString(wObj["AccountGroup"])))
                {
                    objClient.AccountGroup = Convert.ToInt16(wObj["AccountGroup"]);
                }

                objClient.AadharCard = Convert.ToString(wObj["AadharCard"]);
                objClient.GSTNo = Convert.ToString(wObj["GSTNo"]);
                if (!String.IsNullOrEmpty(Convert.ToString(wObj["OpeningBal"])))
                {
                    objClient.OpeningBal = Convert.ToDouble(wObj["OpeningBal"]);
                }
                if (!String.IsNullOrEmpty(Convert.ToString(wObj["TransType"])))
                {
                    objClient.TransType = Convert.ToInt16(wObj["TransType"]);
                }
                objClient.PAN = Convert.ToString(wObj["PAN"]);

                objClient.ServiceTaxNumber = Convert.ToString(wObj["ServiceTaxNumber"]);
                objClient.ContactPersonName = Convert.ToString(wObj["ContactPersonName"]);
                objClient.ContactPersonDesignation = Convert.ToString(wObj["ContactPersonDesignation"]);
                objClient.ContactPersonMobile = objClient.Phone1 = Convert.ToString(wObj["ContactPersonMobile"]);
                objClient.ContactPersonOffPhone = objClient.Phone2 = Convert.ToString(wObj["ContactPersonOffPhone"]);
                if (!String.IsNullOrEmpty(Convert.ToString(wObj["CreditDays"])))
                {
                    objClient.CreditDays = Convert.ToInt32(wObj["CreditDays"]);
                }

                List<ProductRateDTO> lstItems = new List<ProductRateDTO>();
                List<AddressDTO> bi = new List<AddressDTO>();
                objClient.Addresses = new List<AddressDTO>();
                //foreach (var b in billingAddress)
                //{
                if (billingAddress != null)
                {
                    AddressDTO ad = GetAddressDTO(billingAddress);
                    ad.AddressTypeId = Convert.ToInt16(BAL.Enums.AddressType.Billing);
                    ad.RoleId = Convert.ToInt16(BAL.Enums.AddressRoleType.Ledger);
                    // objClient.BillingAddress = ad; // billing address will be same for a client
                    objClient.Addresses.Add(ad);
                    objClient.Address1 = ad.Address1;
                    objClient.Address2 = ad.Address2;
                    objClient.City = ad.City;
                    objClient.State = ad.State;
                    objClient.Email = ad.Email;
                    objClient.ZipCode = ad.ZipCode;
                    //objClient.Phone1 = ad.Phone1;
                    //objClient.Phone2 = ad.Phone2;
                    objClient.Web = ad.Web;
                    objClient.Fax = ad.Fax;
                    objClient.StateId = ad.StateId;

                }
                //  }
                //foreach (var s in shippingAddress)
                //{
                if (shippingAddress != null)
                {
                    AddressDTO shipA = GetAddressDTO(shippingAddress);
                    shipA.AddressTypeId = Convert.ToInt16(BAL.Enums.AddressType.Shipping);
                    shipA.RoleId = Convert.ToInt16(BAL.Enums.AddressRoleType.Ledger);
                    objClient.Addresses.Add(shipA);

                }
                // }

                objClient.RbnClientId = usr.RbnClientId;
                objClient.CompanyId = usr.DefaultCompanyId;
                objClient.CreatedBy = usr.UserId;
                objClient.CreationDate = DateTime.Now;



                int result = objClient.Save();
                response.Code = ApiMessageCodes.SUCCESS;
                response.Data = result.ToString();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (UDFException ex)
            {
                response.Message = ex.Message;
                response.Code = ApiMessageCodes.ERROR;
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                response.Code = ApiMessageCodes.ERROR;
                response.Description = response.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        AddressDTO GetAddressDTO(JToken b)
        {
            AddressDTO ad = new AddressDTO();
            if (b != null)
            {
                ad.AddressId = Convert.ToInt32(b["AddressId"]);
                ad.Address1 = Convert.ToString(b["Address1"]);
                ad.Address2 = Convert.ToString(b["Address2"]);
                ad.Phone1 = Convert.ToString(b["Phone1"]);
                ad.Phone2 = Convert.ToString(b["Phone2"]);
                ad.Email = Convert.ToString(b["Email"]);
                ad.City = Convert.ToString(b["City"]);
                ad.State = Convert.ToString(b["State"]);
                ad.Fax = Convert.ToString(b["Fax"]);
                ad.Web = Convert.ToString(b["Web"]);
                ad.StateId = Convert.ToInt16(b["StateId"]);
                ad.ZipCode = Convert.ToString(b["ZipCode"]);

            }
            return ad;
        }

        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new Ledger().GetAll(new LoggedInUser().DefaultCompanyId, true));
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllByGroups(string groups)
        {
            var msg = new ApiMessage();
            try
            {
                Ledger leder = new Ledger(0);
                leder.StoreId = 1;
                msg.Data = await leder.GetAllByGroups(new LoggedInUser().DefaultCompanyId, groups);
                msg.Code = ApiMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        [HttpPost]
        public HttpResponseMessage ActivateDeActivate([FromBody] LedgerDTO obj)
        {
            Ledger objClient = new Ledger(obj.LedgerId);
            objClient.DeActivate(obj.IsActive);
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }
        [HttpPost]
        public HttpResponseMessage Remove([FromBody] LedgerDTO obj)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger objClient = new Ledger();
                message.Data = objClient.RemoveLedger(obj.LedgerId);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Description = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetDetails([FromBody] LedgerDTO obj)
        {
            Ledger objClient = new Ledger(obj.LedgerId);
            LedgerDTO dto = objClient.GetDetails();
            if (dto != null)
            {
                Address objAddress = new Address();

                List<AddressDTO> addressDTO = objAddress.GetAddresses(BAL.Enums.AddressRoleType.Ledger, obj.LedgerId);
                dto.BillingAddress = addressDTO.Where(o => o.AddressTypeId == Convert.ToInt16(BAL.Enums.AddressType.Billing)).FirstOrDefault();
                dto.ShippingAddress = addressDTO.Where(o => o.AddressTypeId == Convert.ToInt16(BAL.Enums.AddressType.Shipping)).FirstOrDefault();

                if (dto.BillingAddress != null)
                {
                    dto.BillingAddress.ZipCode = dto.ZipCode;
                    if (dto.ShippingAddress != null)
                        dto.ShippingAddress.ZipCode = dto.ZipCode;

                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }
        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpGet]
        public HttpResponseMessage GetAllLedger(string name)
        {
            Ledger leder = new Ledger(0);
            leder.StoreId = 1;
            List<LedgerDTO> allLedgers = leder.GetAll(new LoggedInUser().DefaultCompanyId, true, name);
            SearchResult<LedgerDTO> objResult = new SearchResult<LedgerDTO> { total_count = allLedgers.Count, incomplete_results = false, items = allLedgers };

            return Request.CreateResponse(HttpStatusCode.OK, allLedgers);
        }

        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SearchClient([FromBody] ClientDTO dto)
        {
            Ledger leder = new Ledger();

            var user = new LoggedInUser();
            var ldto = new LedgerDTO();
            //   leder.StoreId = 1;
            if (dto == null)
            {
                throw new Exception("Please provide valid search parameters");
            }
            ldto.InjectFrom(dto);
            ldto.CompanyId = user.DefaultCompanyId;
            ldto.AccountGroup = Convert.ToInt16(dto.AccountGroup);
            ldto.Phone1 = dto.Phone1;
            ldto.Code = dto.Code;
            ldto.Name = dto.Name;
            List<LedgerDTO> allLedgers = (await leder.GetAll(ldto)).ToList();
            if (!String.IsNullOrEmpty(System.Web.HttpContext.Current.Request["all"]))
            {
                allLedgers.Insert(0, new LedgerDTO { Name = "All", LedgerId = 0 });
            }
            var config = new Config();
            var configs = config.GetConfig(user.DefaultCompanyId, "general", "clients");

            if ((dto.Page == null || dto.Page.ToLower() != "quotation"))
            {
                if (configs != null)
                {
                    var hideClients = configs.Where(o => o.Key == "hidQuotationClients").FirstOrDefault();
                    if (hideClients != null)
                    {
                        if (Convert.ToBoolean(hideClients.Value))
                        {
                            allLedgers = allLedgers.Where(o => o.ForQuotation != 1).ToList();
                        }
                    }
                }
            }

            SearchResult<LedgerDTO> objResult = new SearchResult<LedgerDTO> { total_count = allLedgers.Count, incomplete_results = false, items = allLedgers };
            return Request.CreateResponse(HttpStatusCode.OK, objResult);
        }


        #region Transactions
        [HttpPost]
        public HttpResponseMessage CreateTransactions([FromBody] LedgerTransactionDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger objLedger = new Ledger();
                //added by ram
                if (dto.TransactionMode == BAL.Enums.TransactionModes.Cash)
                {
                    //if (!objLedger.VerifySameDayAmount(dto))
                    //{
                    //    message.Description = "Same Amount entry on this date already exists.";
                    //    message.Code = ApiMessageCodes.ERROR;
                    //    return Request.CreateResponse(HttpStatusCode.OK, message);
                    //}
                }
                else
                {
                    if (!objLedger.VerifyChequeNumber(dto))
                    {
                        message.Description = "Cheque Number with this bank already exists.";
                        message.Code = ApiMessageCodes.ERROR;
                        return Request.CreateResponse(HttpStatusCode.OK, message);
                    }
                    //-----------------------
                }
                dto.CreatedBy = new LoggedInUser().UserId;
                dto.CreationDate = DateTime.Now;
                dto.FinYearId = new LoggedInUser().FinYearId;
                //   dto.TransactionDate = Utils.FormatDate(dto.TransactionDate).ToShortDateString();
                if (dto.ChequeDate != null)
                {
                    //dto.ChequeDate = Utils.FormatDate(dto.ChequeDate).ToShortDateString();
                }

                //dto.TransactionMode = BAL.Enums.TransactionModes.Cash;
                dto.CompanyId = new LoggedInUser().DefaultCompanyId;
                CommHelpler com = new CommHelpler();



                //  int tId = objLedger.CreateTransactions(dto);
                message.Data = objLedger.CreateTransactions(dto);
                message.Code = ApiMessageCodes.SUCCESS;

            }
            catch (Exception ex)
            {
                message.Description = ex.Message;
                message.Code = ApiMessageCodes.ERROR;
                logger.Error(ex, ex.Message);
                //return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        /// <summary>Multipart upload: stores image under temp/quick-receipt; returns staging path in message.Data for CreateTransactions.ReceiptStagingPath.</summary>
        [HttpPost]
        public HttpResponseMessage StageQuickReceiptImage()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                var files = HttpContext.Current?.Request?.Files;
                if (files == null || files.Count == 0)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Description = "No file uploaded.";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                var f = files[0];
                const int maxBytes = 5 * 1024 * 1024;
                if (f.ContentLength > maxBytes)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Description = "Image exceeds maximum size (5 MB).";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                var user = new LoggedInUser();
                var path = QuickReceiptDocumentHelper.SaveStagingFile(f, user.DefaultCompanyId);
                message.Data = path;
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Description = ex.Message;
                logger.Error(ex, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        /// <summary>Clears saved quick-receipt path on the transaction and deletes the file from disk.</summary>
        [HttpPost]
        public async Task<HttpResponseMessage> ClearQuickReceiptDocument([FromBody] FilterCriteria dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                if (dto == null || dto.LedgerTransactionId <= 0)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Description = "Invalid transaction.";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                var user = new LoggedInUserInfo();
                var objLedger = new Ledger();
                var ok = await objLedger.ClearQuickReceiptDocument(dto.LedgerTransactionId, user);
                if (!ok)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Description = "Transaction not found or could not be updated.";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Description = ex.Message;
                logger.Error(ex, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage ReceiptTransRegister([FromBody] FilterDto dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                //dto.From = Utils.FormatDate(dto.From).ToShortDateString();
                //dto.To = Utils.FormatDate(dto.To).ToShortDateString();
                //string from = dto.From.ToShortDateString();
                //string to = dto.To.ToShortDateString();

                var user = new LoggedInUser();
                return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetReceiptRegister(dto.LedgerId, dto.LedgerSiteId,
                   dto.From, dto.To, dto.EntryType, user.DefaultCompanyId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage GetContractReceiptPayments([FromBody] FilterCriteria dto)
        {
            try
            {
                if (dto == null || dto.ContractId <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new List<LedgerTransactionDTO>());
                }
                var user = new LoggedInUser();
                Ledger objLedger = new Ledger();
                return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetContractReceiptPayments(dto.ContractId, user.DefaultCompanyId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage LedgerTransactionsAll([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                //dto.From = Utils.FormatDate(dto.From).ToShortDateString();
                //dto.To = Utils.FormatDate(dto.To).ToShortDateString();
                var user = new LoggedInUser();
                return Request.CreateResponse(HttpStatusCode.OK, objLedger.LedgerTransactionsAll(dto.LedgerId, dto.From, dto.To, dto.EntryType, user.DefaultCompanyId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage StockRegister([FromBody] FilterCriteria dto)
        {
            try
            {
                var msg = new ApiMessage();
                Ledger objLedger = new Ledger();
                var data = objLedger.StockRegister(dto.LedgerId, dto.LedgerSiteId, new LoggedInUser().DefaultCompanyId, dto.From, dto.To);
                var items = (from d in data
                             group d by d.ProductId into g
                             select new BillingItemDTO
                             {
                                 ProductId = g.First().ProductId,
                                 Size = g.First().Size,
                                 Weight = g.First().Weight
                             }
                             ).Distinct().ToList();
                foreach (var item in items)
                {
                    item.Balance = data.Where(o => o.ProductId == item.ProductId).Sum(o => o.TranType == 1 ? o.Quantity : -o.Quantity);

                    item.SizeBalance = item.Balance * Convert.ToDouble(item.Size);
                    item.WeightBalance = item.Balance * Convert.ToDouble(item.Weight);

                }
                var modifiedData = new { balance = items, allData = data };
                return Request.CreateResponse(HttpStatusCode.OK, modifiedData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        [HttpPost]
        public HttpResponseMessage GetLedgerTransactionLookup([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                //dto.From = Utils.FormatDate(dto.From).ToShortDateString();
                //dto.To = Utils.FormatDate(dto.To).ToShortDateString();
                return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetLedgerTransactionLookup(dto.LedgerId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage GetTransactionDetails([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                dto.TransactionDate = Utils.FormatDate(dto.TransactionDate).ToShortDateString();
                return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetTransactionDetails(dto.LedgerId, dto.TransactionDate));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage GetTransactionById([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();

                return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetTransactionById(dto.LedgerTransactionId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> GetTransaction([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                var usr = new LoggedInUser();
                var txn = await objLedger.GetTransaction(dto.LedgerTransactionId, usr.DefaultCompanyId);
                var vendorBills = new List<PurchaseDTO>();
                //if payment
                if (txn.TransactionType == 1)
                {

                    var purchase = new Purchase();
                    var unPaidBills = await purchase.GetUnpaidBills(txn.LedgerId, txn.CompanyId);
                    vendorBills = unPaidBills.ToList();
                    if (txn.TxnDetails != null)
                    {
                        var billIds = String.Join(",", txn.TxnDetails.Select(o => o.BillId));
                        if (!String.IsNullOrEmpty(billIds))
                        {
                            var existingBills = await purchase.GetBillsByIds(billIds, txn.CompanyId);
                            var newBills = unPaidBills.Where(o => !existingBills.Any(y => y.PurchaseId == o.PurchaseId));

                            foreach (var b in newBills)
                            {
                                existingBills = existingBills.Append(b);
                            }

                            foreach (var b in existingBills)
                            {
                                var exist = txn.TxnDetails.Where(o => o.BillId == b.PurchaseId).FirstOrDefault();
                                if (exist != null)
                                {
                                    b.AppliedAmount = exist.AppliedAmount;
                                    b.TdsAmount = exist.TdsAmount;
                                    b.Balance += b.AppliedAmount;
                                }
                            }
                            vendorBills = existingBills.ToList();
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { txn = txn, vendorBills = vendorBills });
                }
                if (txn.TransactionType == 2)
                {
                    var clientBills = new List<InvoiceDTO>();
                    var billing = new Billing();
                    var unPaidBills = await billing.GetUnpaidBills(txn.LedgerId, txn.CompanyId, 0);
                    clientBills = unPaidBills.ToList();
                    if (txn.TxnDetails != null)
                    {
                        var billIds = String.Join(",", txn.TxnDetails.Select(o => o.BillId));
                        if (!String.IsNullOrEmpty(billIds))
                        {
                            var existingBills = await billing.GetBillsByIds(billIds, txn.CompanyId);

                            var newBills = unPaidBills.Where(o => !existingBills.Any(y => y.Invoiceid == o.Invoiceid));

                            foreach (var b in newBills)
                            {
                                existingBills = existingBills.Append(b);
                            }

                            foreach (var b in existingBills)
                            {
                                var exist = txn.TxnDetails.Where(o => o.BillId == b.InvoiceId).FirstOrDefault();
                                if (exist != null)
                                {
                                    b.AppliedAmount = exist.AppliedAmount;
                                    b.TdsAmount = exist.TdsAmount;
                                    b.Balance += b.AppliedAmount;
                                }
                            }
                            clientBills = existingBills.ToList();
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new { txn = txn, invoices = clientBills });
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK, new { txn = txn });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// gets the advance receipt vouchers whose balance is > 0
        /// It does not fetch which are already adjusted in any bill
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> getAdvanceReceipts([FromBody] FilterCriteria filter)
        {
            var msg = new ApiMessage();

            try
            {
                if (filter == null)
                {
                    throw new Exception("Please provide valid input payload");
                }
                if (filter.LedgerId == 0 || filter.LedgerSiteId == 0)
                {
                    throw new Exception("Party and Site must be provided");
                }
                var user = new LoggedInUser();
                filter.CompanyId = user.DefaultCompanyId;

                var ledger = new Ledger();
                msg.Data = await ledger.GetAdvancereceipts(filter);
                msg.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                msg.Code = ApiMessageCodes.ERROR;
                msg.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, msg);

        }

        [HttpPost]
        public HttpResponseMessage PartOpeningBalance([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                var user = new LoggedInUser();
                dto.CompanyId = user.DefaultCompanyId;
                dto.To = Utils.FormatDate(dto.To).ToShortDateString();
                var balance = objLedger.ClientWiseItems(dto.LedgerId, dto.LedgerSiteId, dto.CompanyId,
                  dto.To, dto.To);
                var balanceAsof1DayBeforeFromDate = (from d in balance.Tables[0].AsEnumerable()
                                                     select new BillingItemDTO
                                                     {
                                                         ProductId = d.Field<int>("ProductId"),
                                                         Item = d.Field<string>("Product"),
                                                         ItemCategory = d.Field<int>("ItemCategory"),
                                                         Rate = 0,
                                                         Quantity = Convert.ToInt32(d.Field<decimal>("ClosingBalance")),
                                                         ClosingBalance = Convert.ToInt32(d.Field<decimal>("ClosingBalance")),
                                                         ExcessQty = Convert.ToInt32(d.Field<decimal>("ExcessQty")),
                                                         Product = d.Field<string>("Product"),
                                                         Freight = 0,
                                                         FreightTax = 0,
                                                     }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, balanceAsof1DayBeforeFromDate);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage PrintPartyStock([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                dto.From = Utils.FormatDate(dto.From).ToShortDateString();
                dto.To = Utils.FormatDate(dto.To).ToShortDateString();
                DataSet mainDS = objLedger.StockRegister_rpt(dto.LedgerId, new LoggedInUser().DefaultCompanyId, dto.From, dto.To);

                #region oldCode
                /*string fileName = "1.xlsx";
                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/") + fileName;

                //  string pdfFile = filePath + "1.pdf";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                FileInfo file = new FileInfo(filePath);

                ExcelPackage ep = new ExcelPackage(file);
                ep.Workbook.Worksheets.Add("Register");
                ExcelWorksheet sheet = ep.Workbook.Worksheets["Register"];

                var dates = (from d in lstStock
                             select new { Date = d.TransDate.ToShortDateString() }).Distinct();

                #region stockRegister

                #region initSection
                var firstrow = 11;
                var row = firstrow; // 11th row to start
                var col = 4; // Column E
                var totalRows = row;
                var firtCol = 2;
                var obCol = 3;
                int lastCol = firtCol + (dates.Count() * 2) + 1;
                #endregion

                //--- Print dates
                sheet.Cells[row, col - 1].Value = "OB";
                sheet.Cells[row + 1, col - 2].Value = "Challan";
                sheet.Cells[row, lastCol + 1].Value = "Total";
                foreach (var d in dates)
                {
                    sheet.Cells[row, col, row, col + 1].Value = d.Date;
                    sheet.Cells[row, col, row, col + 1].Merge = true;
                    #region Challan
                    //-- get all challans of the date
                    var challans = (from ch in lstStock
                                    where ch.TransDate.ToShortDateString() == d.Date
                                    select new { Challan = ch.JobNumber, TranType = ch.TranType }).Distinct();

                    int j = col;
                    foreach (var ch in challans)
                    {
                        if (ch.TranType == 1)
                        { //issue challan
                            sheet.Cells[row + 1, j].Value = ch.Challan;
                        }
                        else
                        { //recieving challan
                            sheet.Cells[row + 1, j + 1].Value = ch.Challan;
                            sheet.Cells[row + 1, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        //get all products from the stock data
                        var products = (from pd in lstStock
                                        select new { Product = pd.Product }).Distinct();
                        //prepare row to enter product after challan row
                        int itRow = row + 2;
                        foreach (var pd in products)
                        {   //put product on the sheet
                            sheet.Cells[itRow, firtCol].Value = pd.Product;
                            //select product qty for the date, type of challan
                            var productQty = (from q in lstStock
                                              where q.TransDate.ToShortDateString() == d.Date && q.TranType == ch.TranType &&
                                              q.Product == pd.Product
                                              select new { Quantity = q.Quantity });
                            //get opening balance of the product
                            var openingBal = (from o in lstOpeningStock
                                              where o.Product == pd.Product
                                              select new { OpeningBal = o.Quantity }
                                                  ).FirstOrDefault();
                            //Fill opening stock
                            sheet.Cells[itRow, obCol].Value = openingBal != null ? openingBal.OpeningBal : 0;
                            sheet.Cells[itRow, obCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            //fill product qty
                            foreach (var q in productQty)
                            {
                                if (ch.TranType == 1)
                                { // issue qty
                                    sheet.Cells[itRow, j].Value = q.Quantity;
                                }
                                else
                                { //receving qty
                                    sheet.Cells[itRow, j + 1].Value = -q.Quantity;
                                    sheet.Cells[itRow, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                }
                            }
                            //get address from openign balance to last column of the current row
                            string addressToSumForLastColumn = sheet.SelectedRange[itRow, obCol, itRow, lastCol].Address;
                            //prepare the sum formula and put the formula on the last total column to calculate the sum
                            sheet.Cells[itRow, lastCol + 1].Formula = "Sum(" + addressToSumForLastColumn + ")";
                            sheet.Cells[itRow, lastCol + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                            itRow++;
                        }
                        totalRows = itRow;
                        sheet.Cells[row, col, totalRows, j].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        // j ++;
                        // sheet.Cells[row, col, itRow, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Hair);
                    }

                    #endregion

                    //-- footer totals
                    string issSumAddressFooter = sheet.SelectedRange[firstrow + 2, col, totalRows - 1, col].Address;
                    string recSumAddressFooter = sheet.SelectedRange[firstrow + 2, col + 1, totalRows - 1, col + 1].Address;
                    sheet.Cells[totalRows, col].Formula = "Sum(" + issSumAddressFooter + ")";
                    sheet.Cells[totalRows, col + 1].Formula = "Sum(" + recSumAddressFooter + ")";
                    sheet.Cells[totalRows, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet.Cells[totalRows, col + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //-------------------
                    col += 2;
                }
                //-------- end 
                sheet.Cells[totalRows, firtCol].Value = "Total";
                sheet.SelectedRange[row, firtCol, totalRows, lastCol + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.SelectedRange[row, firtCol, totalRows, lastCol + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.SelectedRange[row, firtCol, totalRows, lastCol + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                sheet.SelectedRange[row, firtCol, totalRows, lastCol + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                //--- overall total (last row last column)
                string lastColFooterSumAddress = sheet.SelectedRange[firstrow + 1, lastCol + 1, totalRows - 1, lastCol + 1].Address;
                sheet.Cells[totalRows, lastCol + 1].Formula = "Sum(" + lastColFooterSumAddress + ")";
                sheet.Cells[totalRows, lastCol + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //--- opening balance footer total
                string openingBalTotal = sheet.SelectedRange[firstrow + 1, obCol, totalRows - 1, obCol].Address;
                sheet.Cells[totalRows, obCol].Formula = "Sum(" + openingBalTotal + ")";
                sheet.Cells[totalRows, obCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                #endregion
                #region PrintHeaders

                //get logged-in user's company details
                CompanyDTO company = new Company(new LoggedInUser().DefaultCompanyId).GetDetails();
                int headerRow = 2, headerCol = 2;
                String companyNameRange = sheet.Cells[headerRow, headerCol, headerRow + 1, lastCol + 1].Address;
                String companyAddressRange = sheet.Cells[headerRow + 2, headerCol, headerRow + 2, lastCol + 1].Address;
                string companyAddress = company.Address1 + " " + company.Address2 + "\n" + company.City + "\n";

                setTextAndDecorate(sheet, companyNameRange, company.Name, 20f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, false);
                setTextAndDecorate(sheet, companyAddressRange, companyAddress, 12f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, false);
                string companyNameAndAddressRange = sheet.Cells[headerRow, headerCol, headerRow + 2, lastCol + 1].Address;
                setBorder(sheet, companyNameAndAddressRange);
                //get account information for the to section
                headerRow = 5;
                LedgerDTO ledgerInfo = new Ledger(dto.LedgerId).GetDetails();
                String clientNameRange = sheet.Cells[headerRow, headerCol, headerRow, lastCol - 1].Address;
                String clientAddressRange = sheet.Cells[headerRow + 1, headerCol, headerRow + 1, lastCol - 1].Address;
                string clientAddress = ledgerInfo.Address1 + " " + ledgerInfo.Address2 + "\n" + ledgerInfo.City + "\n";

                setTextAndDecorate(sheet, clientNameRange, "To:- " + ledgerInfo.Name, 12f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, false);
                setTextAndDecorate(sheet, clientAddressRange, clientAddress, 12f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, false);
                string clientNameAndAddressRange = sheet.Cells[headerRow, headerCol, 11, lastCol - 1].Address;
                setBorder(sheet, clientNameAndAddressRange);

                clientNameAndAddressRange = sheet.Cells[headerRow, headerCol, 11, lastCol + 1].Address;
                setBorder(sheet, clientNameAndAddressRange);
                //print dates on the right box
                string periodCellRange = sheet.Cells[7, lastCol, 7, lastCol + 1].Address;
                setTextAndDecorate(sheet, periodCellRange, "Period", 12f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, false);
                string fromDateCellRange = sheet.Cells[8, lastCol, 8, lastCol + 1].Address;
                setTextAndDecorate(sheet, fromDateCellRange, "From: " + dto.From, 12f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, false);
                string endDateCellRange = sheet.Cells[9, lastCol, 9, lastCol + 1].Address;
                setTextAndDecorate(sheet, endDateCellRange, "To: " + dto.To, 12f, true, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, false);
                sheet.View.ShowGridLines = false;
                #endregion
                ep.Save();
                ep.Dispose(); */
                #endregion
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();
                DataSet headerDataSet = objReport.GetReportHeader(dto.LedgerId, companyId);
                string fileName = CreateReportFile("PartyStock" + dto.LedgerId, "stock_balance.rdlc", headerDataSet, mainDS, BAL.Enums.ExportFormat.PDF);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        void setTextAndDecorate(ExcelWorksheet sheet, String range, String text, float font, bool merge, OfficeOpenXml.Style.ExcelHorizontalAlignment align, bool borderAround)
        {
            sheet.Cells[range].Merge = merge;
            sheet.Cells[range].Value = text;
            sheet.Cells[range].Style.Font.Size = font;
            sheet.Cells[range].Style.HorizontalAlignment = align;
            if (borderAround)
            {
                setBorder(sheet, range);
            }
        }
        void setBorder(ExcelWorksheet sheet, String range)
        {
            sheet.SelectedRange[range].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);// = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //sheet.SelectedRange[range].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //sheet.SelectedRange[range].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //sheet.SelectedRange[range].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> DeleteLedgerTransactions([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                var user = new LoggedInUserInfo();
                return Request.CreateResponse(HttpStatusCode.OK, await objLedger.DeleteLedgerTransaction(dto.LedgerTransactionId, user));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage BankEntryRegister([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();

                return Request.CreateResponse(HttpStatusCode.OK, objLedger.BankEntryRegister(dto.DrLedgerId, dto.CrLedgerId, dto.LedgerSiteId, from, to, dto.EntryType, dto.TranRefNumber, new LoggedInUser().DefaultCompanyId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage BankEntryRegister_rpt([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                DataSet mainDS = objLedger.BankEntryRegister_rpt(dto.CrLedgerId, dto.DrLedgerId, from, to, dto.EntryType, dto.TranRefNumber, new LoggedInUser().DefaultCompanyId);
                string fileName = CreateReportFile("BankRegister" + dto.LedgerId, "bank_register.rdlc", null, mainDS, BAL.Enums.ExportFormat.PDF);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage PartyStockBalance([FromBody] FilterCriteria dto)
        {
            try
            {
                ApiMessage response = new ApiMessage();
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                //if (!String.IsNullOrEmpty(dto.From))
                //    from = Utils.FormatDate(dto.From).ToShortDateString();
                //if (!String.IsNullOrEmpty(dto.To))
                //    to = Utils.FormatDate(dto.To).ToShortDateString();
                //response.Code = ApiMessageCodes.SUCCESS;
                var _data = objLedger.PartyStockBalance(dto.LedgerId, new LoggedInUser().DefaultCompanyId, dto.From, dto.To, dto.LedgerSiteId);
                return Request.CreateResponse(HttpStatusCode.OK, _data);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage PartyStockBalance_BySize([FromBody] FilterCriteria dto)
        {
            try
            {
                ApiMessage response = new ApiMessage();
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                //response.Code = ApiMessageCodes.SUCCESS;

                return Request.CreateResponse(HttpStatusCode.OK, objLedger.PartyStockBalanceBySize(dto.LedgerId, new LoggedInUser().DefaultCompanyId, from, to, dto.LedgerSiteId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage PartyStockBalance_Dashboard([FromBody] FilterCriteria dto)
        {
            try
            {
                ApiMessage response = new ApiMessage();
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                //response.Code = ApiMessageCodes.SUCCESS;

                return Request.CreateResponse(HttpStatusCode.OK, objLedger.PartyStockBalance_DashBoard(dto.LedgerId, new LoggedInUser().DefaultCompanyId, from, to));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AccountLedger([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                var user = new LoggedInUser();
                var data = objLedger.GetAccountLedger(dto.LedgerId, from, to, dto.LedgerSiteId, user.DefaultCompanyId, user.FinYearId);
                if (data != null)
                {
                    foreach (var d in data)
                    {
                        var bal = d.OpeningBalance + d.Credit - d.Debit;
                        if (bal < 0)
                        {
                            d.ClosingBalance = Math.Abs(bal).ToString() + " DR";
                        }
                        else
                        {
                            d.ClosingBalance = Math.Abs(bal).ToString() + " CR";
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage LedgerTransactions([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                //  String from = "", to = "";
                //if (!String.IsNullOrEmpty(dto.From))
                //    from = Utils.FormatDate(dto.From).ToShortDateString();
                //if (!String.IsNullOrEmpty(dto.To))
                //    to = Utils.FormatDate(dto.To).ToShortDateString();
                var user = new LoggedInUser();
                var data = objLedger.GetLedgerTransactions(dto.LedgerId, dto.From, dto.To, dto.LedgerSiteId, user.DefaultCompanyId, user.FinYearId);

                if (dto.LedgerId > 0)
                {
                    //re-index opening balance at top
                    var op = data.Where(o => o.EntryType == 17).FirstOrDefault();
                    if (op != null)
                    {
                        var index = data.IndexOf(op);
                        data.RemoveAt(index);
                        data.Insert(0, op);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage LedgerAccGroupTransactions([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                //  String from = "", to = "";
                //if (!String.IsNullOrEmpty(dto.From))
                //    from = Utils.FormatDate(dto.From).ToShortDateString();
                //if (!String.IsNullOrEmpty(dto.To))
                //    to = Utils.FormatDate(dto.To).ToShortDateString();
                var user = new LoggedInUser();
                var data = objLedger.GetAccountGroupLedgerTransactions(dto.AccountGroupId, dto.From, dto.To, dto.LedgerSiteId, user.DefaultCompanyId, user.FinYearId);

                if (dto.LedgerId > 0)
                {
                    //re-index opening balance at top
                    var op = data.Where(o => o.EntryType == 17).FirstOrDefault();
                    if (op != null)
                    {
                        var index = data.IndexOf(op);
                        data.RemoveAt(index);
                        data.Insert(0, op);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }


        [HttpGet]
        public HttpResponseMessage GetAccountBalanceForBill(int ledgerId)
        {
            try
            {
                Ledger objLedger = new Ledger();

                if (ledgerId > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetAccountBalanceForBill(ledgerId));
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { LedgerId = 0, ClosingBalance = 0 });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage GetPartyBalance([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                LoggedInUser user = new LoggedInUser();
                DataSet ds = objLedger.GetPartyBalance(dto.LedgerId, user.FinYearId, user.DefaultCompanyId, dto.LedgerSiteId, from, to);
                List<LedgerTransactionDTO> lst = new Utils<LedgerTransactionDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage ClientWiseItems([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                LoggedInUser user = new LoggedInUser();
                DataSet ds = objLedger.ClientWiseItems(dto.LedgerId, dto.LedgerSiteId, user.DefaultCompanyId, from, to, dto.PONumber, dto.BalanceType);
                List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage ItemWiseClients([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                LoggedInUser user = new LoggedInUser();
                DataSet ds = objLedger.ItemWiseClients(user.DefaultCompanyId, dto.ProductId, from, to, dto.BalanceType);
                List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage FixedSiteLedger([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();

                return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetFixedSiteLedger(dto.LedgerId, from, to, dto.WorkOrderId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }
        [HttpPost]
        public HttpResponseMessage AccountLedger_rpt([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                DataSet mainDS = objLedger.GetAccountLedger_rpt(dto.LedgerId, from, to);
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();
                DataSet headerDataSet = objReport.GetReportHeader(dto.LedgerId, companyId);
                string fileName = CreateReportFile("accountledger" + dto.LedgerId, "accountledger.rdlc", headerDataSet, mainDS, BAL.Enums.ExportFormat.PDF);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);

                //  return Request.CreateResponse(HttpStatusCode.OK, objLedger.GetAccountLedger(dto.LedgerId, from, to));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }


        [HttpPost]
        public HttpResponseMessage PrintReceipt([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger ledger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();
                DataSet headerDataSet = objReport.GetReportHeader(dto.LedgerId, companyId);
                DataSet mainDS = ledger.GetReceiptRegisterPRT(dto.LedgerId, from, to, dto.EntryType, dto.TranRefNumber);

                String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/receiptRegister.rdlc");
                ReportViewer rpt = new ReportViewer();
                rpt.LocalReport.ReportPath = reportPath;
                ReportDataSource rDsource = new ReportDataSource("DataSet1", mainDS.Tables[0]);
                rpt.LocalReport.DataSources.Add(rDsource);



                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                string fileName = dto.LedgerId + ".pdf";
                filePath = filePath + fileName;

                rpt.LocalReport.SubreportProcessing += delegate (object o, SubreportProcessingEventArgs e)
               {
                   ReportDataSource rsHeader = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                   e.DataSources.Add(rsHeader);
               };
                rpt.LocalReport.Refresh();
                byte[] reportData = rpt.LocalReport.Render("PDF");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllBytes(filePath, reportData);

                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
         * created another method with same name
        [HttpPost]
        public HttpResponseMessage PrintTransaction([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger ledger = new Ledger();
                int companyId = new LoggedInUser().DefaultCompanyId;
                int ledgerId = 0;
                BAL.Objects.Report objReport = new BAL.Objects.Report();
                DataSet mainDS = ledger.GetTransactionById(dto.LedgerTransactionId);
                if (mainDS.Tables.Count > 0)
                {
                    if (mainDS.Tables[0].Rows.Count > 0)
                    {
                        ledgerId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                    }
                }
                DataSet headerDataSet = objReport.GetReportHeader(ledgerId, companyId);
                // DataSet mainDS = ledger.GetReceiptRegisterPRT(dto.LedgerId, from, to, dto.EntryType, dto.TranRefNumber);

                String reportPath = System.Web.HttpContext.Current.Server.MapPath("~/rpts/receiptRegister.rdlc");
                ReportViewer rpt = new ReportViewer();
                rpt.LocalReport.ReportPath = reportPath;
                ReportDataSource rDsource = new ReportDataSource("DataSet1", mainDS.Tables[0]);
                rpt.LocalReport.DataSources.Add(rDsource);



                string filePath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
                string fileName = dto.LedgerId + ".pdf";
                filePath = filePath + fileName;

                rpt.LocalReport.SubreportProcessing += delegate(object o, SubreportProcessingEventArgs e)
                {
                    ReportDataSource rsHeader = new ReportDataSource("DataSet1", headerDataSet.Tables[0]);
                    e.DataSources.Add(rsHeader);
                };
                rpt.LocalReport.Refresh();
                byte[] reportData = rpt.LocalReport.Render("PDF");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllBytes(filePath, reportData);

                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        */
        [HttpPost]
        public HttpResponseMessage PrintTransaction([FromBody] FilterCriteria dto)
        {
            ReportUtility rep = new ReportUtility();
            ApiMessage message = new ApiMessage();
            try
            {
                message.Data = rep.DebitCreditNote(dto.LedgerTransactionId, false, ReminderType.NONE);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage EmailDrCrNote([FromBody] FilterCriteria dto)
        {
            ReportUtility rep = new ReportUtility();
            ApiMessage message = new ApiMessage();
            try
            {
                ReminderType remType = (ReminderType)dto.ReminderType;
                message.Data = rep.DebitCreditNote(dto.LedgerTransactionId, true, remType);

                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage GetDrCrNotes([FromBody] FilterCriteria dto)
        {
            Ledger ledger = new Ledger();
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                message.Data = ledger.GetDrCrNotes(dto.LedgerId, user.DefaultCompanyId, user.FinYearId, dto.EntryType);

                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage PartyStockBalance_Report([FromBody] FilterCriteria dto)
        {
            try
            {


                int partyId = 0;
                int companyId = new LoggedInUser().DefaultCompanyId;
                BAL.Objects.Report objReport = new BAL.Objects.Report();


                ApiMessage response = new ApiMessage();
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();

                DataSet mainDS = objLedger.PartyStockBalance_REPORT(dto.LedgerId, new LoggedInUser().DefaultCompanyId, from, to);
                //  DataSet headerDataSet = objReport.GetReportHeader(partyId, companyId);
                string fileName = CreateReportFile("AllpendReceipt" + dto.LedgerId, "AllpendReceipt.rdlc", null, mainDS, BAL.Enums.ExportFormat.PDF);
                return Request.CreateResponse(HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace);
            }
        }
        #endregion

        #region ProductRates


        [HttpPost]
        public HttpResponseMessage GetProductRates([FromBody] FilterCriteria dto)
        {
            Ledger leder = new Ledger();
            List<ProductRateDTO> allLedgers = leder.GetProductRates(dto.LedgerId, dto.LedgerSiteId, new LoggedInUser().DefaultCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, allLedgers);
        }
        [HttpPost]
        public HttpResponseMessage GetPartyWiseRates([FromBody] FilterCriteria dto)
        {
            //Ledger leder = new Ledger();
            //List<ProductRateDTO> allLedgers = leder.ProductRates(dto.LedgerId);
            //return Request.CreateResponse(HttpStatusCode.OK, allLedgers);
            Ledger leder = new Ledger();
            List<ProductRateDTO> allLedgers = leder.GetProductRates(dto.LedgerId, dto.LedgerSiteId, new LoggedInUser().DefaultCompanyId);
            return Request.CreateResponse(HttpStatusCode.OK, allLedgers);
        }
        [HttpPost]
        public async Task<ApiResponseMessage> CopyRates([FromBody] CopyRatesDTO copy)
        {
            var res = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUserInfo();
                Ledger leder = new Ledger();

                res.Data = await leder.CopyRates(copy, user);
                res.Code = ApiResponseMessageCodes.SUCCESS;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = ApiResponseMessageCodes.ERROR;
                res.Message = ex.Message;
                return res;
            }

        }
        [HttpPost]
        public HttpResponseMessage UpdateProductRates([FromBody] FilterCriteria filter)
        {
            bool result = true;//  objLedger.AddUpdateProductRates(ledgerId, lstItems);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage SaveProductRates()
        {
            string dto = System.Web.HttpContext.Current.Request["dto"];
            System.Web.HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            JObject jsonObject = new JObject();


            jsonObject = JObject.Parse(dto);
            //  var wObj = jsonObject["Props"];
            Ledger objLedger = new Ledger();
            var items = jsonObject["Products"];
            int ledgerId = Convert.ToInt32(jsonObject["LedgerId"]);
            int ledgerSiteId = Convert.ToInt32(jsonObject["LedgerSiteId"]);
            List<ProductRateDTO> lstItems = new List<ProductRateDTO>();
            foreach (var item in items)
            {
                ProductRateDTO pdto = new ProductRateDTO();
                pdto.DamageRate = Convert.ToDouble(item["DamageRate"]);
                pdto.LedgerId = Convert.ToInt32(item["LedgerId"]);
                pdto.LossRate = Convert.ToDouble(item["LossRate"]);
                pdto.RentRate = Convert.ToDouble(item["RentRate"]);
                pdto.ProductId = Convert.ToInt32(item["ProductId"]);
                pdto.LedgerSiteId = ledgerSiteId;
                pdto.OpeningBalance = Convert.ToDouble(item["OpeningBalance"]);

                pdto.Unit = Convert.ToInt16(item["Unit"]);
                pdto.ProductSizeId = (item["ProductSizeId"] != null) ? Convert.ToInt16(item["ProductSizeId"]) : 0;
                pdto.UnitSizeRate = (item["UnitSizeRate"] != null) ? Convert.ToDouble(item["UnitSizeRate"]) : 0;

                pdto.ModifiedBy = new LoggedInUser().UserId;
                lstItems.Add(pdto);

            }
            bool result = objLedger.AddUpdateProductRates(ledgerId, lstItems);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        #endregion

        #region Sites

        private static readonly string[] AllowedSiteDocumentExtensions =
        {
            ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".webp",
            ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"
        };
        private const int MaxSiteDocumentBytes = 10 * 1024 * 1024;

        [HttpPost]
        public async Task<HttpResponseMessage> AddSite()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                var user = new LoggedInUser();
                ClientSiteDTO dto = await ReadClientSiteDtoFromRequestAsync();
                if (dto == null)
                {
                    message.Code = ApiMessageCodes.ERROR;
                    message.Message = "Invalid site payload.";
                    return Request.CreateResponse(HttpStatusCode.OK, message);
                }

                dto.FinYearId = user.FinYearId;

                ClientSiteDTO priorDocState = null;
                if (dto.LedgerSiteId > 0)
                {
                    priorDocState = new Ledger().GetSiteById(dto.LedgerSiteId);
                }

                var posted = new HttpPostedFile[5];
                for (int i = 0; i < 5; i++)
                {
                    posted[i] = HttpContext.Current.Request.Files["doc" + (i + 1)];
                }

                for (int i = 0; i < 5; i++)
                {
                    if (posted[i] == null || posted[i].ContentLength <= 0)
                    {
                        continue;
                    }
                    ValidateSitePostedFile(posted[i]);
                    ClearSiteDocumentSlot(dto, i + 1);
                }

                var ledger = new Ledger();
                ledger.AddSite(dto);

                string siteDir = Path.Combine(mapPath, "sites", dto.LedgerId.ToString(), dto.LedgerSiteId.ToString());
                Directory.CreateDirectory(siteDir);

                for (int i = 0; i < 5; i++)
                {
                    if (posted[i] == null || posted[i].ContentLength <= 0)
                    {
                        continue;
                    }
                    string ext = Path.GetExtension(posted[i].FileName);
                    if (string.IsNullOrEmpty(ext))
                    {
                        ext = InferExtensionFromContentType(posted[i].ContentType);
                    }
                    string fileName = posted[i].FileName;
                    string stored = fileName;// string.Format("d{0}_{1:N}", i + 1, fileName);
                    string oldName = GetSiteDocumentStoredName(priorDocState, i + 1);
                    if (!string.IsNullOrWhiteSpace(oldName) && !string.Equals(oldName, stored, StringComparison.OrdinalIgnoreCase))
                    {
                        TryDeleteSiteDocumentFile(dto.LedgerId, dto.LedgerSiteId, oldName);
                    }
                    posted[i].SaveAs(Path.Combine(siteDir, stored));
                    SetSiteDocumentStoredName(dto, i + 1, stored);
                }

                ledger.UpdateClientSiteDocuments(dto);

                if (priorDocState != null)
                {
                    for (int slot = 1; slot <= 5; slot++)
                    {
                        string oldFn = GetSiteDocumentStoredName(priorDocState, slot);
                        string newFn = GetSiteDocumentStoredName(dto, slot);
                        if (!string.IsNullOrWhiteSpace(oldFn) &&
                            string.IsNullOrWhiteSpace(newFn) &&
                            (posted[slot - 1] == null || posted[slot - 1].ContentLength <= 0))
                        {
                            TryDeleteSiteDocumentFile(dto.LedgerId, dto.LedgerSiteId, oldFn);
                        }
                    }
                }

                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpGet]
        public HttpResponseMessage DownloadSiteDocument(int ledgerId, int ledgerSiteId, int slot)
        {
            if (ledgerId <= 0 || ledgerSiteId <= 0 || slot < 1 || slot > 5)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid request.");
            }

            var user = new LoggedInUser();
            var party = new Ledger(ledgerId).GetDetails();
            if (party == null || party.CompanyId != user.DefaultCompanyId)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            var site = new Ledger().GetSiteById(ledgerSiteId);
            if (site == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            int siteLedgerId = site.LedgerId;
            if (siteLedgerId != ledgerId)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            string fileName = GetSiteDocumentStoredName(site, slot);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            string physical = Path.Combine(mapPath, "sites", ledgerId.ToString(), ledgerSiteId.ToString(), fileName);
            if (!File.Exists(physical))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var mime = MimeMapping.GetMimeMapping(physical);
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(new FileStream(physical, FileMode.Open, FileAccess.Read, FileShare.Read));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return result;
        }

        private async Task<ClientSiteDTO> ReadClientSiteDtoFromRequestAsync()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };


            if (HttpContext.Current.Request.Files.Count > 0 ||
                !string.IsNullOrEmpty(HttpContext.Current.Request.Form["dto"]))
            {
                string json = HttpContext.Current.Request.Form["dto"];
                return string.IsNullOrWhiteSpace(json)
                    ? null
                    : JsonConvert.DeserializeObject<ClientSiteDTO>(json, settings);
            }

            string body = await Request.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ClientSiteDTO>(body, settings);
        }

        static void ValidateSitePostedFile(HttpPostedFile file)
        {
            if (file.ContentLength > MaxSiteDocumentBytes)
            {
                throw new Exception("Each site document must be 10 MB or smaller.");
            }
            string ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext))
            {
                ext = InferExtensionFromContentType(file.ContentType);
            }
            if (string.IsNullOrEmpty(ext) || !AllowedSiteDocumentExtensions.Contains(ext.ToLowerInvariant()))
            {
                throw new Exception("Unsupported file type. Allowed: PDF, images, or Office documents.");
            }
        }

        static string InferExtensionFromContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return null;
            }
            switch (contentType.ToLowerInvariant())
            {
                case "application/pdf": return ".pdf";
                case "image/png": return ".png";
                case "image/jpeg": return ".jpg";
                case "image/gif": return ".gif";
                case "image/webp": return ".webp";
                case "application/msword": return ".doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return ".docx";
                case "application/vnd.ms-excel": return ".xls";
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": return ".xlsx";
                case "application/vnd.ms-powerpoint": return ".ppt";
                case "application/vnd.openxmlformats-officedocument.presentationml.presentation": return ".pptx";
                default: return null;
            }
        }

        static void ClearSiteDocumentSlot(ClientSiteDTO dto, int slot)
        {
            SetSiteDocumentStoredName(dto, slot, null);
        }

        static string GetSiteDocumentStoredName(ClientSiteDTO dto, int slot)
        {
            if (dto == null)
            {
                return null;
            }
            switch (slot)
            {
                case 1: return dto.Document1FileName;
                case 2: return dto.Document2FileName;
                case 3: return dto.Document3FileName;
                case 4: return dto.Document4FileName;
                case 5: return dto.Document5FileName;
                default: return null;
            }
        }

        static void SetSiteDocumentStoredName(ClientSiteDTO dto, int slot, string value)
        {
            switch (slot)
            {
                case 1: dto.Document1FileName = value; break;
                case 2: dto.Document2FileName = value; break;
                case 3: dto.Document3FileName = value; break;
                case 4: dto.Document4FileName = value; break;
                case 5: dto.Document5FileName = value; break;
            }
        }

        static void TryDeleteSiteDocumentFile(int ledgerId, int ledgerSiteId, string storedName)
        {
            if (string.IsNullOrWhiteSpace(storedName))
            {
                return;
            }
            string path = Path.Combine(mapPath, "sites", ledgerId.ToString(), ledgerSiteId.ToString(), storedName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetSites([FromBody] FilterCriteria dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {

                Ledger ledger = new Ledger();
                ledger.LedgerId = dto.LedgerId;

                List<ClientSiteDTO> sites = ledger.GetSites(dto);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = sites;
                var client = new Ledger(dto.LedgerId).GetDetails();
                message.Extra = client;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);

        }
        [HttpGet]
        public async Task<ApiMessage> AllClientsWithSites()
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger ledger = new Ledger();
                var user = new LoggedInUser();
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = await ledger.AllClientsWithSites(user.DefaultCompanyId);
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }
        [HttpPost]
        public HttpResponseMessage GetSiteById([FromBody] ClientSiteDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger ledger = new Ledger();
                ledger.LedgerId = dto.LedgerId;
                ClientSiteDTO site = ledger.GetSiteById(dto.LedgerSiteId);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = site;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetSiteTaxes([FromBody] LedgerTaxDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger ledger = new Ledger();

                List<LedgerTaxDTO> sites = ledger.GetSiteTaxes(dto.LedgerSiteId);

                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = sites;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetLastBill([FromBody] InvoiceDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger ledger = new Ledger();

                InvoiceDTO lastBill = ledger.GetLastBill(dto.LedgerId, dto.LedgerSiteId, new LoggedInUser().FinYearId);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = lastBill;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        #endregion

        [HttpPost]
        public HttpResponseMessage DueMaterialReminder([FromBody] FilterCriteria filter)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                EmailParameters p = new EmailParameters();
                CommHelpler com = new CommHelpler();
                LedgerDTO ledger = new Ledger(filter.LedgerId).GetDetails();
                com.sendSms(ledger.ContactPersonMobile, p, MessageEvent.MAT_REMINDER);
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage GetAllClientsSites([FromBody] SiteDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger ledger = new Ledger();
                ledger.CompanyId = new LoggedInUser().DefaultCompanyId;
                List<ClientSiteDTO> sites = ledger.GetAllClientSites(dto.JobNumber, dto.Site, dto.Client, dto.SiteEng);
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = sites;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }
        [HttpPost]
        public HttpResponseMessage DueBillReminder([FromBody] BillingItemDTO filter)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                EmailParameters p = new EmailParameters() { From = filter.From.ToShortDateString(), To = filter.To.ToShortDateString(), InvoiceAmount = filter.Total };
                CommHelpler com = new CommHelpler();
                LedgerDTO ledger = new Ledger(filter.LedgerId).GetDetails();

                if (filter.ReminderType > 0)
                {
                    ReminderType rType = (ReminderType)filter.ReminderType;
                    ReportUtility rep = new ReportUtility();
                    rep.SendDueBillEmailReminder(filter.InvoiceId, rType);
                }
                message.Code = ApiMessageCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public HttpResponseMessage EstimatedRentPerDay([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger leder = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                int companyId = new LoggedInUser().DefaultCompanyId;

                List<LedgerTransactionDTO> allLedgers = leder.EstimatedRentPerDay(dto.LedgerId, from, to, companyId);
                return Request.CreateResponse(HttpStatusCode.OK, allLedgers);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);

            }
        }
        [HttpPost]
        public HttpResponseMessage GetClientJobs([FromBody] WorkOrderDTO obj)
        {
            Ledger ledger = new Ledger(0);
            //obj.From = Utils.FormatDate(obj.From).ToShortDateString();
            //obj.To = Utils.FormatDate(obj.To).ToShortDateString();
            return Request.CreateResponse(HttpStatusCode.OK, ledger.GetClientJobs(obj.LedgerId));
        }
        //[HttpPost]
        //public HttpResponseMessage CustomMessage([FromBody] FilterCriteria filter)
        //{
        //    ApiMessage message = new ApiMessage();
        //    try
        //    {
        //        EmailParameters p = new EmailParameters();
        //        CommHelpler com = new CommHelpler();
        //        LedgerDTO ledger = new Ledger(filter.LedgerId).GetDetails();
        //        com.sendSms(ledger.Phone1, p, MessageEvent.MAT_REMINDER);
        //        message.Code = ApiMessageCodes.SUCCESS;
        //    }
        //    catch (Exception ex)
        //    {
        //        message.Code = ApiMessageCodes.ERROR;
        //        message.Message = ex.Message;
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, message);
        //}

        [HttpPost]
        public HttpResponseMessage GetUnbilledSites([FromBody] FilterCriteria dto)
        {
            try
            {
                Ledger objLedger = new Ledger();

                LoggedInUser user = new LoggedInUser();
                DataSet ds = objLedger.UnbilledSites(user.FinYearId, user.DefaultCompanyId);
                List<ClientSiteDTO> lst = new Utils<ClientSiteDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }

        [HttpPost]
        public HttpResponseMessage Cashbook([FromBody] FilterCriteria dto)
        {
            try
            {
                int companyId = new LoggedInUser().DefaultCompanyId;
                Ledger objLedger = new Ledger();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = Utils.FormatDate(dto.From).ToShortDateString();
                if (!String.IsNullOrEmpty(dto.To))
                    to = Utils.FormatDate(dto.To).ToShortDateString();
                LoggedInUser user = new LoggedInUser();
                DataSet ds = objLedger.Cashbook(dto.AccountGroupId, companyId, from, to);
                List<LedgerTransactionDTO> lst = new Utils<LedgerTransactionDTO>().ConstructList(ds);
                return Request.CreateResponse(HttpStatusCode.OK, lst);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ApiMessageHandler.GetMessage(ApiMessageCodes.LEDGER_TRANSACTION_ERROR, ex.Message));
            }
        }



        private string GetClientwiseItemBalanceReport(FilterCriteria dto)
        {
            Ledger objLedger = new Ledger();
            String from = "", to = "";
            if (!String.IsNullOrEmpty(dto.From))
                from = Utils.FormatDate(dto.From).ToShortDateString();
            if (!String.IsNullOrEmpty(dto.To))
                to = Utils.FormatDate(dto.To).ToShortDateString();
            LoggedInUser user = new LoggedInUser();
            DataSet ds = objLedger.ClientWiseItems(dto.LedgerId, 0, user.DefaultCompanyId, from, to, dto.PONumber, dto.BalanceType);
            List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);

            StringBuilder xml = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            foreach (LedgerbalanceDTO o in lst)
            {
                sb.Append("<Ledger>");
                sb.Append("<ClientName>" + o.ClientName + "</ClientName>");
                sb.Append("<Address>" + o.Address + "</Address>");
                sb.Append("<Email>" + o.Email + "</Email>");
                sb.Append("<Phone>" + o.Phone + "</Phone>");
                sb.Append("<ClosingBalance>" + o.ClosingBalance.ToString() + "</ClosingBalance>");
                sb.Append("<IssuedQty>" + o.IssuedQty.ToString() + "</IssuedQty>");
                sb.Append("<LedgerId>" + o.LedgerId.ToString() + "</LedgerId>");
                sb.Append("<OpeningBalance>" + o.OpeningBalance.ToString() + "</OpeningBalance>");
                sb.Append("<Product>" + o.Product.ToString() + "</Product>");
                sb.Append("<ReceivedQty>" + o.ReceivedQty.ToString() + "</ReceivedQty>");
                sb.Append("</Ledger>");

            }
            xml.Append("<?xml version='1.0' ?><Data>");
            xml.Append(sb.ToString());
            xml.Append("</Data>");

            return xml.ToString();
        }
        private string GetCashbookReport(FilterCriteria dto)
        {
            int companyId = new LoggedInUser().DefaultCompanyId;
            Ledger objLedger = new Ledger();
            String from = "", to = "";
            if (!String.IsNullOrEmpty(dto.From))
                from = Utils.FormatDate(dto.From).ToShortDateString();
            if (!String.IsNullOrEmpty(dto.To))
                to = Utils.FormatDate(dto.To).ToShortDateString();
            LoggedInUser user = new LoggedInUser();
            DataSet ds = objLedger.Cashbook(dto.AccountGroupId, companyId, from, to);
            List<LedgerTransactionDTO> lst = new Utils<LedgerTransactionDTO>().ConstructList(ds);

            StringBuilder xml = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            foreach (LedgerTransactionDTO o in lst)
            {
                sb.Append("<Transaction>");
                sb.Append("<Code>" + o.Code + "</Code>");
                sb.Append("<Name>" + o.Name + "</Name>");
                sb.Append("<EntryTypeName>" + o.EntryTypeName + "</EntryTypeName>");
                sb.Append("<TransactionDate>" + o.TransactionDate + "</TransactionDate>");
                sb.Append("<TransactionAmount>" + o.TransactionAmount.ToString() + "</TransactionAmount>");
                sb.Append("<Balance>" + o.Balance.ToString() + "</Balance>");
                sb.Append("<TransactionTypeName>" + o.TransactionTypeName + "</TransactionTypeName>");
                sb.Append("<Description>" + o.Description + "</Description>");
                sb.Append("</Transaction>");

            }
            xml.Append("<?xml version='1.0' ?><Data>");
            xml.Append(sb.ToString());
            xml.Append("</Data>");

            return xml.ToString();
        }
        private string GetStockSummaryReport(FilterCriteria dto)
        {
            int companyId = new LoggedInUser().DefaultCompanyId;
            //   Ledger objLedger = new Ledger();
            String onDate = "";
            if (!String.IsNullOrEmpty(dto.OnDate))
                onDate = Utils.FormatDate(dto.OnDate).ToShortDateString();
            LoggedInUser user = new LoggedInUser();
            Inventory objLedger = new Inventory();
            List<StockInventoryDto> lst = objLedger.StockSummary(companyId, onDate);
            //   List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);

            StringBuilder xml = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            foreach (StockInventoryDto o in lst)
            {
                sb.Append("<Stock>");
                sb.Append("<Code>" + o.Code + "</Code>");
                sb.Append("<ClientName>" + o.ClientName + "</ClientName>");
                sb.Append("<OpeningBalance>" + o.OpeningBalance.ToString() + "</OpeningBalance>");
                sb.Append("<IssuedQty>" + o.IssuedQty.ToString() + "</IssuedQty>");
                sb.Append("<ReceivedQty>" + o.ReceivedQty.ToString() + "</ReceivedQty>");
                sb.Append("<ClosingBalance>" + o.ClosingBalance.ToString() + "</ClosingBalance>");
                sb.Append("<LedgerId>" + o.LedgerId.ToString() + "</LedgerId>");
                sb.Append("</Stock>");

            }
            xml.Append("<?xml version='1.0' ?><Data>");
            xml.Append(sb.ToString());
            xml.Append("</Data>");

            return xml.ToString();
        }
        private string GetStockInhandReport(FilterCriteria dto)
        {
            int companyId = new LoggedInUser().DefaultCompanyId;
            Inventory objLedger = new Inventory();
            String onDate = "";
            if (!String.IsNullOrEmpty(dto.OnDate))
                onDate = Utils.FormatDate(dto.OnDate).ToShortDateString();
            LoggedInUser user = new LoggedInUser();
            List<LedgerbalanceDTO> lst = objLedger.StockInhand(companyId, onDate);
            //  List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);

            StringBuilder xml = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            foreach (LedgerbalanceDTO o in lst)
            {
                sb.Append("<Stock>");
                sb.Append("<Code>" + o.Code + "</Code>");
                sb.Append("<Name>" + o.Product + "</Name>");
                sb.Append("<ClosingBalance>" + o.ClosingBalance.ToString() + "</ClosingBalance>");
                sb.Append("</Stock>");

            }
            xml.Append("<?xml version='1.0' ?><Data>");
            xml.Append(sb.ToString());
            xml.Append("</Data>");

            return xml.ToString();
        }
        private string GetReportXML(string reportName, FilterCriteria dto)
        {

            string xml = string.Empty;
            switch (reportName)
            {
                case "ClientwiseItemBalance": xml = GetClientwiseItemBalanceReport(dto); break;
                case "Cashbook": xml = GetCashbookReport(dto); break;
                case "StockSummary": xml = GetStockSummaryReport(dto); break;
                case "StockInhand": xml = GetStockInhandReport(dto); break;

            }

            return xml;
        }
        private string GetReportTemplate(string reportName)
        {

            string template = string.Empty;
            switch (reportName)
            {
                case "ClientwiseItemBalance": template = ReportTemplate.CLIENT_WISE_ITEM_BALANCE; break;
                case "Cashbook": template = ReportTemplate.CASHBOOK; break;
                case "StockSummary": template = ReportTemplate.STOCK_SUMMARY; break;
                case "StockInhand": template = ReportTemplate.STOCK_IN_HAND; break;

            }

            return template;
        }
        [HttpPost]
        public HttpResponseMessage PrintReport([FromBody] FilterCriteria dto)
        {


            string reportName = dto.ReportName;
            string xml = GetReportXML(reportName, dto);
            ApiMessage message = new ApiMessage();

            ReportUtility rep = new ReportUtility();
            string html = rep.getReportHTML(GetReportTemplate(reportName), xml.ToString());
            try
            {
                Byte[] res = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    var config = new PdfGenerateConfig()
                    {
                        MarginBottom = 70,
                        MarginLeft = 20,
                        MarginRight = 20,
                        MarginTop = 70,
                        PageSize = PageSize.A4
                    };
                    var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, config);
                    pdf.Save(ms);
                    // var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/temp");
                    // pdf.Save(mappedPath + "/document.pdf");
                    res = ms.ToArray();
                }
                message.Code = ApiMessageCodes.SUCCESS;
                message.Data = res;
            }
            catch
            {
                message.Code = ApiMessageCodes.ERROR;
            }

            return Request.CreateResponse(HttpStatusCode.OK, message);
        }

        [HttpPost]
        public async Task<ApiMessage> CloseSite([FromBody] ClientSiteDTO dto)
        {
            ApiMessage message = new ApiMessage();
            try
            {
                Ledger ledger = new Ledger();
                var user = new LoggedInUserInfo();
                if (dto == null)
                {
                    throw new Exception("Invalid Input");
                }
                if (dto.LedgerId <= 0 || dto.LedgerSiteId <= 0)
                {
                    throw new Exception("Invalid Input");
                }
                dto.ClosedBy = user.UserId;
                dto.ClosedOn = DateTime.Now;

                message.Data = await ledger.CloseSite(dto, user);

                message.Code = ApiMessageCodes.SUCCESS;

            }
            catch (Exception ex)
            {
                message.Code = ApiMessageCodes.ERROR;
                message.Message = ex.Message;
            }
            return message;
        }
    }
}

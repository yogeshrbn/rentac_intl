using BAL.Common;
using BAL.DTO;
using BAL.Objects;
using Newtonsoft.Json;
using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using ReportViewer;
using System.Web.WebSockets;

namespace ReportViewer.Controllers
{
    //[System.Web.Http.RoutePrefix("api/[controller]")]
    [System.Web.Mvc.Authorize]
    public class ReportsController : BaseWebApiController
    {

        public ReportsController() { }


        [System.Web.Http.HttpGet]
        //   [System.Web.Http.Route("api/Reports/GetString")]
        public string GetString()
        {

            return "hello";
        }

        #region financialReports

        [System.Web.Http.HttpPost]
        // [System.Web.Http.Route("api/Reports/billPaymentSummary")]
        public async Task<IHttpActionResult> BillPaymentSummary([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var reports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _data = await reports.BillPaymentSummary(filter);
                var _payments = await reports.PartySiteWisePayments(filter);
                var totalBilledAmount = (from _d in _data
                                         select new BillPaymentReportDto
                                         {
                                             Total = _d.Total,
                                             InvoiceId = _d.InvoiceId,
                                         }).Distinct().Sum(o => o.Total);

                var totalPaidAmount = _payments.Sum(o => o.TransactionAmount);

                var comp = new Company(user.DefaultCompanyId).GetDetails();
                var ledgerObj = new Ledger(filter.LedgerId);
                var client = ledgerObj.GetDetails();
                filter.OnDate = filter.From;
                var openingBalance = await ledgerObj.getOpeningBalacneAsOnDate(filter);
                var data = new
                {
                    reportData = _data,
                    totalBilled = totalBilledAmount,
                    totalPaidAmount = totalPaidAmount,
                    payments = _payments,
                    balance = (totalBilledAmount - Convert.ToDouble(openingBalance)) - totalPaidAmount,
                    company = comp,
                    client = client,
                    from = Convert.ToDateTime(filter.From),
                    to = Convert.ToDateTime(filter.To),
                    openingBalance = openingBalance

                };
                apiRes.Data = data;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                if (filter.Print)
                {
                    var d = new { d = data };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;


                    string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\billpayment.report.xslt";

                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                                   // Or try these specific arguments:
                    con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                    con.Margins.Top = 5;
                    con.Margins.Bottom = 5;

                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "billpayment report.pdf";
                    //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                    //  var stream = new MemoryStream();
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = dwnFileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = ResponseMessage(result);
                    return response;
                }
                else
                {
                    return Ok(apiRes);
                }
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> PartyPayments([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var reports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var data = reports.PartyPayments(filter);

                if (filter.Print)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var raw = data ?? new List<BAL.DTO.LedgerTransactionDTO>();
                    foreach (var item in raw)
                    {
                        item.CrLedger = item.CrLedger ?? item.Name ?? "-";
                    }
                    var partyDisplay = raw.Select(x => x.CrLedger ?? "-").Distinct().ToList();
                    var parties = new List<object>();
                    foreach (var partyName in partyDisplay)
                    {
                        var partyItems = raw.Where(x => (x.CrLedger ?? "-") == partyName).ToList();
                        var siteGroups = partyItems.GroupBy(x => string.IsNullOrEmpty(x.Site) || x.Site == "null" || x.Site == "undefined" ? "-" : x.Site);
                        var sites = new List<object>();
                        foreach (var siteGrp in siteGroups)
                        {
                            var siteName = siteGrp.Key;
                            var items = siteGrp.Select(x => new
                            {
                                TransactionDate = x.TransactionDate,
                                TransactionAmount = x.TransactionAmount,
                                TransactionMode = (int)x.TransactionMode,
                                ChequeDate = x.ChequeDate ?? "-",
                                ChequeNumber = x.ChequeNumber ?? "-",
                                Narration = x.Narration ?? "-"
                            }).ToList();
                            var subtotal = siteGrp.Sum(x => x.TransactionAmount);
                            sites.Add(new { siteName = siteName, items = items, subtotal = subtotal });
                        }
                        var partySubtotal = partyItems.Sum(x => x.TransactionAmount);
                        parties.Add(new { partyName = partyName, sites = sites, partySubtotal = partySubtotal });
                    }
                    var grandTotal = raw.Sum(x => x.TransactionAmount);
                    var reportPayload = new { d = new { company = comp, from = Convert.ToDateTime(filter.From), to = Convert.ToDateTime(filter.To), parties = parties, grandTotal = grandTotal } };
                    string jsonText = JsonConvert.SerializeObject(reportPayload);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\partyPayments.report.xslt";
                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    var con = new HtmlToPdfConverter();
                    con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                    con.Margins.Top = 5;
                    con.Margins.Bottom = 5;
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "partyPayments.pdf";
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = dwnFileName };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    return ResponseMessage(result);
                }

                apiRes.Data = data;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                return Ok(apiRes);
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }
            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        // [System.Web.Http.Route("api/Reports/billPaymentSummary")]
        public async Task<IHttpActionResult> amountReceiveable([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var reports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _data = await reports.AmountReceiveable(filter);
                //var _payments = await reports.PartySiteWisePayments(filter);
                //var totalBilledAmount = (from _d in _data
                //                         select new BillPaymentReportDto
                //                         {
                //                             Total = _d.Total,
                //                             InvoiceId = _d.InvoiceId,
                //                         }).Distinct().Sum(o => o.Total);

                //var totalPaidAmount = _payments.Sum(o => o.TransactionAmount);

                var comp = new Company(user.DefaultCompanyId).GetDetails();
                var client = new Ledger(filter.LedgerId).GetDetails();

                var data = new
                {
                    reportData = _data,
                    //  totalBilled = totalBilledAmount,
                    // totalPaidAmount = totalPaidAmount,
                    //  payments = _payments,
                    //  balance = totalBilledAmount - totalPaidAmount,
                    company = comp,
                    client = client,
                    from = filter.From,
                    to = filter.To,

                };
                apiRes.Data = data;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                if (filter.Print)
                {

                    var d = new { d = data };
                    string jsonText = JsonConvert.SerializeObject(d);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;


                    string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\billOutStandingReport.xslt";

                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "billOutStandingReport.pdf";
                    //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                    //  var stream = new MemoryStream();
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = dwnFileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = ResponseMessage(result);
                    return response;
                }
                else
                {
                    return Ok(apiRes);
                }
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;

            }

            return Ok(apiRes);
        }


        [System.Web.Http.HttpPost]
        // [System.Web.Http.Route("api/Reports/billPaymentSummary")]
        public async Task<IHttpActionResult> PartyRegister([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var resports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _deliveryChallansDetails = await resports.DeliveryChallans(filter);
                var _returnChallansDetails = await resports.ReturnChallans(filter);
                double prevSent = 0, prevReceived = 0;

                if (_deliveryChallansDetails.Count() > 0)
                    prevSent = _deliveryChallansDetails.First().PrevSent;
                if (_returnChallansDetails.Count() > 0)
                    prevReceived = _returnChallansDetails.First().PrevReceived;

                var _deliveryChallans = (from d in _deliveryChallansDetails
                                         group d by d.WorkOrderId into g
                                         select new WorkOrderDTO
                                         {
                                             Vehicle = g.First().Vehicle,
                                             Driver = g.First().Driver,
                                             Number = g.First().ChallanNumber,
                                             Client = g.First().Client,
                                             Site = g.First().Site,

                                             WorkOrderDate = g.First().SentDate,
                                             Freight = g.First().Freight,
                                             Remarks = g.First().Remarks,
                                             Items = _deliveryChallansDetails.Where(o => o.WorkOrderId == g.Key).ToList()
                                         }
                                          );
                var _returnChallans = (from d in _returnChallansDetails
                                       group d by d.GRNId into g
                                       select new GRNDTO
                                       {
                                           VehicleNo = g.First().VehicleNo,
                                           GRN = g.First().GRN,
                                           Client = g.First().Client,
                                           Freight = g.First().Freight,
                                           ReceivingDate = g.First().ReceivingDate,
                                           Driver = g.First().Driver
                                              ,
                                           SiteName = g.First().SiteName,
                                           Items = _returnChallansDetails.Where(o => o.GRNId == g.Key).Select(o => new WorkOrderItemDTO
                                           {
                                               Product = o.Item,
                                               ChallanNumber = o.GRN,
                                               Breakage = o.Breakage,
                                               Quantity = o.Quantity,
                                               ShortQty = o.ShortQty,
                                               ExcessQty = o.ExcessQty,
                                               Scrap = o.Scrap,
                                               ChallanDate = o.ReceivingDate,
                                               Remarks = o.Remarks

                                           }).ToList()
                                       }
                                       );


                if (filter.Print)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var client = new Ledger(filter.LedgerId).GetDetails();
                    double TotalSentQty = 0, TotalReceivedQty = 0, TotalShortQty = 0, TotalScrapQty = 0;
                    TotalSentQty = _deliveryChallansDetails.Sum(o => o.SentQty);
                    TotalReceivedQty = _returnChallansDetails.Sum(o => o.Quantity);
                    TotalShortQty = _returnChallansDetails.Sum(o => o.ShortQty);
                    TotalScrapQty = _returnChallansDetails.Sum(o => o.Scrap);

                    var data = new
                    {
                        d = new
                        {
                            from = filter.From,
                            to = filter.To,
                            prevSent = prevSent,
                            prevReceived = prevReceived,
                            delivery = _deliveryChallans,
                            returns = _returnChallans,
                            company = comp,
                            client = client
                            ,
                            totalSentQty = TotalSentQty,
                            totalReceivedQty = TotalReceivedQty,
                            totalShortQty = TotalShortQty,
                            totalScrapQty = TotalScrapQty,

                            totalReturned = TotalReceivedQty + TotalShortQty + TotalScrapQty
                        }
                    };
                    string jsonText = JsonConvert.SerializeObject(data);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;


                    string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\party-register.xslt";

                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "party-register.pdf";
                    //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                    //  var stream = new MemoryStream();
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "test.pdf"
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = ResponseMessage(result);
                    return response;
                }
                else
                {
                    var data = new
                    {
                        prevSent = prevSent,
                        prevReceived = prevReceived,
                        delivery = _deliveryChallans,
                        returns = _returnChallans

                    };
                    apiRes.Data = data;
                    apiRes.Code = ApiResponseMessageCodes.SUCCESS;

                    return Ok(apiRes);
                }
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }


        [System.Web.Http.HttpPost]
        // [System.Web.Http.Route("api/Reports/billPaymentSummary")]
        public async Task<IHttpActionResult> DeliveryChallans([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var resports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _deliveryChallansDetails = await resports.DeliveryChallans(filter);

                var _deliveryChallans = (from d in _deliveryChallansDetails
                                         group d by d.WorkOrderId into g
                                         select new WorkOrderDTO
                                         {
                                             Vehicle = g.First().Vehicle,
                                             Driver = g.First().Driver,
                                             Number = g.First().ChallanNumber,
                                             Client = g.First().Client,
                                             Site = g.First().Site,

                                             WorkOrderDate = g.First().SentDate,
                                             Freight = g.First().Freight,
                                             Remarks = g.First().Remarks,
                                             Items = _deliveryChallansDetails.Where(o => o.WorkOrderId == g.Key).ToList()
                                         }
                                       );


                apiRes.Data = _deliveryChallans;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                if (filter.Print)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var client = new Ledger(filter.LedgerId).GetDetails();
                    double TotalSentQty = 0, TotalReceivedQty = 0, TotalShortQty = 0;
                    TotalSentQty = _deliveryChallansDetails.Sum(o => o.SentQty);

                    var data = new
                    {
                        d = new
                        {
                            from = filter.From,
                            to = filter.To,
                            delivery = _deliveryChallans,
                            company = comp,
                            client = client,
                            printDate = Utils.FormatDate(DateTime.Today),
                            totalSentQty = TotalSentQty,
                            totalReceivedQty = TotalReceivedQty,
                            totalShortQty = TotalShortQty,
                            totalReturned = TotalReceivedQty + TotalShortQty
                        }
                    };
                    string jsonText = JsonConvert.SerializeObject(data);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;


                    string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\party-delivery-challans.xslt";

                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "party-register.pdf";
                    //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                    //  var stream = new MemoryStream();
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "test.pdf"
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = ResponseMessage(result);
                    return response;
                }
                else
                {

                    apiRes.Data = _deliveryChallans;
                    apiRes.Code = ApiResponseMessageCodes.SUCCESS;

                    return Ok(apiRes);
                }

            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        // [System.Web.Http.Route("api/Reports/billPaymentSummary")]
        public async Task<IHttpActionResult> ReturnChallans([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var resports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _returnChallansDetails = await resports.ReturnChallans(filter);
                var _returnChallans = (from d in _returnChallansDetails
                                       group d by d.GRNId into g
                                       select new GRNDTO
                                       {
                                           VehicleNo = g.First().VehicleNo,
                                           GRN = g.First().GRN,
                                           Client = g.First().Client,
                                           Freight = g.First().Freight,
                                           ReceivingDate = g.First().ReceivingDate,
                                           Driver = g.First().Driver
                                              ,
                                           SiteName = g.First().SiteName,
                                           Items = _returnChallansDetails.Where(o => o.GRNId == g.Key).Select(o => new WorkOrderItemDTO
                                           {
                                               Product = o.Item,
                                               Breakage = o.Breakage,
                                               ChallanNumber = o.GRN,
                                               Quantity = o.Quantity,
                                               ShortQty = o.ShortQty,
                                               ExcessQty = o.ExcessQty,
                                               Scrap = o.Scrap,
                                               ChallanDate = o.ReceivingDate,
                                               Remarks = o.Remarks

                                           }).ToList()
                                       }
                                   );
                if (filter.Print)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var client = new Ledger(filter.LedgerId).GetDetails();
                    double TotalSentQty = 0, TotalReceivedQty = 0, TotalShortQty = 0;


                    var data = new
                    {
                        d = new
                        {
                            from = filter.From,
                            to = filter.To,
                            returns = _returnChallans,
                            company = comp,
                            client = client,
                            printDate = Utils.FormatDate(DateTime.Today),
                            totalReceivedQty = TotalReceivedQty,
                            totalShortQty = TotalShortQty,
                            totalReturned = TotalReceivedQty + TotalShortQty
                        }
                    };
                    string jsonText = JsonConvert.SerializeObject(data);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;


                    string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\party-return-challans.xslt";

                    var html = RenderXml(xml, fileName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "party-return.pdf";
                    //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                    //  var stream = new MemoryStream();
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "test.pdf"
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = ResponseMessage(result);
                    return response;
                }
                else
                {

                    apiRes.Data = _returnChallans;
                    apiRes.Code = ApiResponseMessageCodes.SUCCESS;

                    return Ok(apiRes);
                }


            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }

        private HttpResponseMessage PartyVarianceReportPdf(FilterCriteria filter, List<GRNDTO> rows, string title, string quantityLabel, string downloadFileName, Func<GRNDTO, double> qtySelector)
        {
            var user = new LoggedInUser();
            var comp = new Company(user.DefaultCompanyId).GetDetails();

            var rowEls = rows.Select(r => new XElement("row",
                new XElement("Party", r.Client ?? ""),
                new XElement("Site", r.SiteName ?? ""),
                new XElement("Challan", r.GRN ?? ""),
                new XElement("ChallanDate", Utils.FormatDate(r.ReceivingDate)),
                new XElement("Quantity", qtySelector(r))
            ));

            var totalQty = rows.Sum(qtySelector);

            var root = new XElement("d",
                new XElement("title", title),
                new XElement("quantityLabel", quantityLabel),
                new XElement("from", filter.From ?? ""),
                new XElement("to", filter.To ?? ""),
                new XElement("printDate", Utils.FormatDate(DateTime.Today)),
                new XElement("companyName", comp.Name ?? ""),
                new XElement("totalQuantity", totalQty),
                new XElement("rows", rowEls)
            );

            string xml = root.ToString();
            string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\party-variance-report.xslt";
            var html = RenderXml(xml, fileName);
            var strHtml = html.ToHtmlString();
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            var fileBytes = con.GeneratePdf(strHtml);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = downloadFileName
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> BreakageReport([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var resports = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                var rows = (await resports.BreakageReport(filter)).ToList();
                if (filter.Print)
                {
                    return ResponseMessage(PartyVarianceReportPdf(filter, rows, "Breakage Report", "Breakage", "breakage-report.pdf", r => r.Breakage));
                }
                apiRes.Data = rows;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                return Ok(apiRes);
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }
            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> LostReport([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var resports = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                var rows = (await resports.LostReport(filter)).ToList();
                if (filter.Print)
                {
                    return ResponseMessage(PartyVarianceReportPdf(filter, rows, "Lost Report", "Lost", "lost-report.pdf", r => r.ShortQty));
                }
                apiRes.Data = rows;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                return Ok(apiRes);
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }
            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> ExcessReport([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var resports = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                var rows = (await resports.ExcessReport(filter)).ToList();
                if (filter.Print)
                {
                    return ResponseMessage(PartyVarianceReportPdf(filter, rows, "Excess Report", "Excess", "excess-report.pdf", r => r.ExcessQty));
                }
                apiRes.Data = rows;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;
                return Ok(apiRes);
            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }
            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> VehicleTravelReport([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var resports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _data = await resports.VehicleTravelReport(filter);
                apiRes.Data = _data;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;

                return Ok(apiRes);


            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> MaterialPickupReminder([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var resports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _data = await resports.MaterialPickupReminder(filter);
                apiRes.Data = _data;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;

                return Ok(apiRes);


            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> TransporterReport([FromBody] FilterCriteria filter)
        {
            var apiRes = new ApiResponseMessage();
            try
            {
                var user = new LoggedInUser();
                var resports = new Report();
                filter.CompanyId = user.DefaultCompanyId;
                var _data = await resports.TransporterReport(filter);
                apiRes.Data = _data;
                apiRes.Code = ApiResponseMessageCodes.SUCCESS;

                return Ok(apiRes);


            }
            catch (Exception ex)
            {
                apiRes.Code = ApiResponseMessageCodes.ERROR;
                apiRes.Message = ex.Message;
            }

            return Ok(apiRes);
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> IssuedChallansRegisterReport([FromBody] BillingDTO filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest("Invalid request");

                var user = new LoggedInUser();
                var workOrder = new WorkOrder(0);
                var allData = (await workOrder.ItemIssued(
                    filter.From.ToShortDateString(),
                    filter.To.ToShortDateString(),
                    filter.ChallanType,
                    filter.LedgerId,
                    filter.LedgerSiteId,
                    user.DefaultCompanyId,
                    filter.ChallanNo ?? string.Empty,
                    filter.IssuedListStatusFilter)).ToList();

                var rows = allData
                    .GroupBy(x => x.WorkOrderId)
                    .Select(g =>
                    {
                        var first = g.First();
                        return new
                        {
                            Client = first.Client ?? string.Empty,
                            Site = first.Site ?? string.Empty,
                            ChallanNumber = first.ChallanNumber ?? string.Empty,
                            SentDate = first.SentDate,
                            SentQty = g.Sum(x => x.SentQty),
                            Deleted = first.Deleted
                        };
                    })
                    .OrderBy(r => r.SentDate)
                    .ThenBy(r => r.ChallanNumber)
                    .ToList();

                var comp = new Company(user.DefaultCompanyId).GetDetails();
                var payload = new
                {
                    data = new
                    {
                        comp = comp,
                        fromDate = filter.From,
                        toDate = filter.To,
                        reportTitle = "Items Issued Register",
                        challanTypeLabel = GetIssuedChallanTypeLabel(filter.ChallanType),
                        challan = rows
                    }
                };

                string jsonText = JsonConvert.SerializeObject(payload);
                XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                string xml = doc.InnerXml;

                string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\issued-challans-register.xslt";
                var html = RenderXml(xml, fileName);
                var strHtml = html.ToHtmlString();
                HtmlToPdfConverter con = new HtmlToPdfConverter();
                con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                con.Margins.Top = 5;
                con.Margins.Bottom = 5;

                var fileBytes = con.GeneratePdf(strHtml);
                string dwnFileName = "issued-challans-register.pdf";

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes)
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = dwnFileName
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        static string GetIssuedChallanTypeLabel(short challanType)
        {
            switch (challanType)
            {
                case 1: return "Contract Delivery Challan";
                case 2: return "Rent Delivery Challan";
                case 10: return "Un-Hire Delivery Challan";
                case 12: return "Transfer Challan";
                default: return string.Empty;
            }
        }

        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> ReceivedChallansRegisterReport([FromBody] BillingDTO filter)
        {
            try
            {
                if (filter == null)
                    return BadRequest("Invalid request");

                var user = new LoggedInUser();
                var workOrder = new WorkOrder(0);
                var list = workOrder.ItemReceived(
                    filter.From.ToShortDateString(),
                    filter.To.ToShortDateString(),
                    filter.ChallanType,
                    filter.LedgerId,
                    filter.LedgerSiteId,
                    user.DefaultCompanyId,
                    filter.ChallanNo ?? string.Empty,
                    filter.ReceivedListStatusFilter);

                var rows = list
                    .GroupBy(x => x.GRNId)
                    .Select(g =>
                    {
                        var first = g.First();
                        return new
                        {
                            Client = first.Client ?? string.Empty,
                            Site = first.SiteName ?? string.Empty,
                            ChallanNumber = first.GRN ?? string.Empty,
                            SentDate = first.ReceivingDate,
                            SentQty = g.Sum(x => x.Quantity),
                            Deleted = first.Deleted
                        };
                    })
                    .OrderBy(r => r.SentDate)
                    .ThenBy(r => r.ChallanNumber)
                    .ToList();

                var comp = new Company(user.DefaultCompanyId).GetDetails();
                var payload = new
                {
                    data = new
                    {
                        comp = comp,
                        fromDate = filter.From,
                        toDate = filter.To,
                        reportTitle = "Items Received Register",
                        challanTypeLabel = GetReceivedChallanTypeLabel(filter.ChallanType),
                        challan = rows
                    }
                };

                string jsonText = JsonConvert.SerializeObject(payload);
                XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                string xml = doc.InnerXml;

                string fileName = HttpContext.Current.Server.MapPath("~/Reports") + @"\received-challans-register.xslt";
                var html = RenderXml(xml, fileName);
                var strHtml = html.ToHtmlString();
                HtmlToPdfConverter con = new HtmlToPdfConverter();
                con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                con.Margins.Top = 5;
                con.Margins.Bottom = 5;

                var fileBytes = con.GeneratePdf(strHtml);
                string dwnFileName = "received-challans-register.pdf";

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes)
                };
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = dwnFileName
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        static string GetReceivedChallanTypeLabel(short challanType)
        {
            switch (challanType)
            {
                case 2: return "Rent Challan";
                case 10: return "Hire Challan";
                case 11: return "Contract Challan";
                case 12: return "Transfer Challan";
                default: return string.Empty;
            }
        }

        #endregion


        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Gstr1([FromBody] GSTRFilterDto filter)
        {
            ApiResponseMessage msg = new ApiResponseMessage();
            try
            {
                Report objReport = new Report();
                filter.CompanyId = new LoggedInUser().DefaultCompanyId;
                msg.Data = await objReport.Gstr11(filter);
                msg.Code = ApiResponseMessageCodes.SUCCESS;
                return Ok(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return Request.CreateResponse(HttpStatusCode.OK, new { stats = data, fundSummary = fundSummary, stock = stock, topItemsOnRent = itemsOnrent });
        }
    }
}

using BAL.DTO;
using BAL.Objects;
using Newtonsoft.Json;
using NReco.PdfGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ReportViewer.Controllers
{
    [System.Web.Http.RoutePrefix("api/InventoryReports")]
    [Authorize]
    public class InventoryReportsApiController : BaseWebApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ItemWiseClientBalance")]
        public IHttpActionResult ItemWiseClientBalance([FromBody] FilterCriteria dto)
        {
            try
            {
                var user = new LoggedInUser();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = dto.From;
                if (!String.IsNullOrEmpty(dto.To))
                    to = dto.To;

                Ledger objLedger = new Ledger();
                DataSet ds = objLedger.ItemWiseClients(user.DefaultCompanyId, dto.ProductId, from, to, dto.BalanceType ?? "rent");
                List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);

                if (!dto.Print)
                {
                    return Ok(lst);
                }
                if (dto.Pdf)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var toDate = !String.IsNullOrEmpty(dto.To) ? Convert.ToDateTime(dto.To) : DateTime.Now;
                    var _data = new { data = new { comp = comp, details = ds, toDate = toDate } };
                    string jsonText = JsonConvert.SerializeObject(_data);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    string repName = HttpContext.Current.Server.MapPath("~/Reports") + @"\itemWiseClientBalance.xslt";
                    var html = RenderXml(xml, repName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "itemWiseClientBalance.pdf";
                    return ResponseMessage(CreateFileResponse(fileBytes, dwnFileName));
                }
                #region excel
                var memoryStream = new MemoryStream();
                string name = "sheet1";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(name);
                worksheet.Name = name;
                worksheet.Column(1).Width = 50;

                var dateRate = from + " TO " + to;
                if (lst.Count > 0)
                {
                    int row = 1;
                    var groupedByParty = lst.GroupBy(x => new { x.LedgerId, x.ClientName }).OrderBy(g => g.Key.ClientName);
                    foreach (var partyGroup in groupedByParty)
                    {
                        worksheet.Cells[row, 1, row, 11].Merge = true;
                        worksheet.Cells[row, 1].Value = "Party: " + partyGroup.Key.ClientName;
                        worksheet.Cells[row, 1].Style.Font.Bold = true;
                        worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        row++;
                        worksheet.Cells[row, 1, row, 11].Merge = true;
                        worksheet.Cells[row, 1].Value = dateRate;
                        row++;
                        worksheet.Cells[row, 1].Value = "Item";
                        worksheet.Cells[row, 2].Value = "Unit";
                        worksheet.Cells[row, 3].Value = "OP";
                        worksheet.Cells[row, 4].Value = "Issue";
                        worksheet.Cells[row, 5].Value = "Receive";
                        worksheet.Cells[row, 6].Value = "Lost Qty";
                        worksheet.Cells[row, 7].Value = "Scrap Qty";
                        worksheet.Cells[row, 8].Value = "Excess Qty";
                        worksheet.Cells[row, 9].Value = "Balance Qty";
                        worksheet.Cells[row, 10].Value = "Size Qty";
                        worksheet.Cells[row, 11].Value = "Weight Qty";
                        worksheet.Cells[row, 1, row, 11].Style.Font.Bold = true;
                        row++;
                        var groupedBySite = partyGroup.GroupBy(x => new { x.LedgerSiteId, x.SiteAddress }).OrderBy(g => g.Key.SiteAddress ?? "");
                        foreach (var siteGroup in groupedBySite)
                        {
                            worksheet.Cells[row, 1, row, 11].Merge = true;
                            worksheet.Cells[row, 1].Value = "Site: " + (string.IsNullOrEmpty(siteGroup.Key.SiteAddress) ? "Default" : siteGroup.Key.SiteAddress);
                            worksheet.Cells[row, 1].Style.Font.Bold = true;
                            worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(243, 243, 243));
                            row++;
                            foreach (var item in siteGroup)
                            {
                                worksheet.Cells[row, 1].Value = item.Product;
                                worksheet.Cells[row, 2].Value = item.Unit;
                                worksheet.Cells[row, 3].Value = item.OpeningBalance;
                                worksheet.Cells[row, 4].Value = item.IssuedQty;
                                worksheet.Cells[row, 5].Value = item.ReceivedQty;
                                worksheet.Cells[row, 6].Value = item.ShortQty;
                                worksheet.Cells[row, 7].Value = item.ScrapQty;
                                worksheet.Cells[row, 8].Value = item.ExcessQty;
                                worksheet.Cells[row, 9].Value = item.ClosingBalance;
                                worksheet.Cells[row, 10].Value = item.SizeBalance;
                                worksheet.Cells[row, 11].Value = item.WeightBalance;
                                row++;
                            }
                            row++;
                        }
                        row += 2;
                    }
                    worksheet.OutLineApplyStyle = true;
                    if (worksheet.Dimension != null)
                    {
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;
                var excelBytes = memoryStream.ToArray();
                return ResponseMessage(CreateFileResponse(excelBytes, "itemWiseClientBalance.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
                #endregion
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ClientWiseItemBalance")]
        public IHttpActionResult ClientWiseItemBalance([FromBody] FilterCriteria dto)
        {
            try
            {
                var user = new LoggedInUser();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = dto.From;
                if (!String.IsNullOrEmpty(dto.To))
                    to = dto.To;

                Ledger objLedger = new Ledger();
                DataSet ds = objLedger.ClientWiseItems(dto.LedgerId, dto.LedgerSiteId, user.DefaultCompanyId, from, to,dto.PONumber);
                List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);

                if (!dto.Print)
                {
                    return Ok(lst);
                }
                if (dto.Pdf)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var _data = new { data = new { comp = comp, details = ds, toDate = Convert.ToDateTime(dto.To) } };
                    string jsonText = JsonConvert.SerializeObject(_data);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    string repName = HttpContext.Current.Server.MapPath("~/Reports") + @"\cleintWiseItemBalance.xslt";
                    var html = RenderXml(xml, repName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "cleintWiseItemBalance.pdf";
                    return ResponseMessage(CreateFileResponse(fileBytes, dwnFileName));
                }
                #region excel
                var memoryStream = new MemoryStream();
                string name = "sheet1";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();
                var ledger = new Ledger(dto.LedgerId);
                var ldto = ledger.GetDetails();
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(name);
                worksheet.Name = name;
                worksheet.Column(1).Width = 50;

                var dateRate = from + " TO " + to;
                if (lst.Count > 0)
                {
                    int row = 1;
                    var groupedByClientSite = lst.GroupBy(x => new { x.LedgerId, x.LedgerSiteId, x.ClientName, x.SiteAddress })
                        .OrderBy(g => g.Key.ClientName).ThenBy(g => g.Key.SiteAddress);
                    foreach (var group in groupedByClientSite)
                    {
                        worksheet.Cells[row, 1, row, 11].Merge = true;
                        worksheet.Cells[row, 1].Value = "Party: " + group.Key.ClientName + (string.IsNullOrEmpty(group.Key.SiteAddress) ? "" : " | Site: " + group.Key.SiteAddress);
                        worksheet.Cells[row, 1].Style.Font.Bold = true;
                        worksheet.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        row++;
                        worksheet.Cells[row, 1, row, 11].Merge = true;
                        worksheet.Cells[row, 1].Value = dateRate;
                        worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        row++;
                        worksheet.Cells[row, 1].Value = "Item";
                        worksheet.Cells[row, 2].Value = "Unit";
                        worksheet.Cells[row, 3].Value = "OP";
                        worksheet.Cells[row, 4].Value = "Issue";
                        worksheet.Cells[row, 5].Value = "Receive";
                        worksheet.Cells[row, 6].Value = "Lost Qty";
                        worksheet.Cells[row, 7].Value = "Scrap Qty";
                        worksheet.Cells[row, 8].Value = "Excess Qty";
                        worksheet.Cells[row, 9].Value = "Balance Qty";
                        worksheet.Cells[row, 10].Value = "Size Qty";
                        worksheet.Cells[row, 11].Value = "Weight Qty";
                        worksheet.Cells[row, 1, row, 11].Style.Font.Bold = true;
                        row++;
                        foreach (var item in group)
                        {
                            worksheet.Cells[row, 1].Value = item.Product;
                            worksheet.Cells[row, 2].Value = item.Unit;
                            worksheet.Cells[row, 3].Value = item.OpeningBalance;
                            worksheet.Cells[row, 4].Value = item.IssuedQty;
                            worksheet.Cells[row, 5].Value = item.ReceivedQty;
                            worksheet.Cells[row, 6].Value = item.ShortQty;
                            worksheet.Cells[row, 7].Value = item.ScrapQty;
                            worksheet.Cells[row, 8].Value = item.ExcessQty;
                            worksheet.Cells[row, 9].Value = item.ClosingBalance;
                            worksheet.Cells[row, 10].Value = item.SizeBalance;
                            worksheet.Cells[row, 11].Value = item.WeightBalance;
                            row++;
                        }
                        row += 2;
                    }
                    worksheet.OutLineApplyStyle = true;
                    if (worksheet.Dimension != null)
                    {
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;
                var excelBytes = memoryStream.ToArray();
                return ResponseMessage(CreateFileResponse(excelBytes, "clientWiseItemBalance.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
                #endregion
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// PDF for party stock register in challan-wise mode (rows are supplied from the screen after Find).
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("PartyStockChallanWisePdf")]
        public IHttpActionResult PartyStockChallanWisePdf([FromBody] PartyStockChallanWisePdfRequest dto)
        {
            const int maxRows = 8000;
            try
            {
                if (dto == null)
                    return BadRequest("Request body is required.");
                if (!dto.Print || !dto.Pdf)
                    return BadRequest("For PDF export, set both Print and Pdf to true.");
                if (dto.Rows == null || dto.Rows.Count == 0)
                    return BadRequest("There is nothing to print. Choose a party, run Find, then export again.");
                if (dto.Rows.Count > maxRows)
                    return BadRequest(string.Format("This export allows at most {0} rows. Narrow the date range or contact support.", maxRows));

                var user = new LoggedInUser();
                var comp = new Company(user.DefaultCompanyId).GetDetails();

                DateTime fromDate;
                DateTime toDate;
                bool haveFrom = DateTime.TryParse(dto.From, out fromDate);
                bool haveTo = DateTime.TryParse(dto.To, out toDate);
                if (!haveTo)
                    toDate = DateTime.Now;

                string fromLabel = haveFrom && fromDate.Year > 1900
                    ? fromDate.ToString("dd/MM/yyyy")
                    : (string.IsNullOrWhiteSpace(dto.From) ? "—" : dto.From.Trim());
                string toLabel = toDate.ToString("dd/MM/yyyy");
                string dateRangeText = fromLabel + " to " + toLabel;

                var ledger = new Ledger(dto.LedgerId);
                var partyLedger = ledger.GetDetails();
                string partyName = partyLedger != null && !string.IsNullOrWhiteSpace(partyLedger.Name)
                    ? partyLedger.Name.Trim()
                    : ("Party #" + dto.LedgerId);

                string siteLabel = "All sites";
                if (dto.LedgerSiteId > 0)
                {
                    var sites = new WorkOrder(0).GetClientSites(dto.LedgerId);
                    var site = sites != null ? sites.FirstOrDefault(s => s.LedgerSiteId == dto.LedgerSiteId) : null;
                    if (site != null && !string.IsNullOrWhiteSpace(site.Site))
                        siteLabel = site.Site.Trim();
                    else
                        siteLabel = "Site #" + dto.LedgerSiteId;
                }

                var tableRows = new List<object>();
                foreach (var r in dto.Rows)
                {
                    tableRows.Add(new
                    {
                        Item = r.Item ?? "",
                        OpeningBalance = r.OpeningBalance,
                        DisplayDate = r.DisplayDate ?? "",
                        ChallanNo = r.ChallanNo ?? "",
                        Issue = r.Issue,
                        Receive = r.Receive,
                        Balance = r.Balance,
                        ClosingBalance = r.ClosingBalance
                    });
                }

                var payload = new
                {
                    data = new
                    {
                        comp,
                        partyName,
                        siteAddress = siteLabel,
                        dateRangeText,
                        details = new { Table = tableRows }
                    }
                };

                string jsonText = JsonConvert.SerializeObject(payload);
                XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                string xml = doc.InnerXml;

                string repPath = HttpContext.Current.Server.MapPath("~/Reports/clientwiseitembalancebychallan.xslt");
                var html = RenderXml(xml, repPath);
                string strHtml = html.ToHtmlString();
                var pdfBytes = new HtmlToPdfConverter().GeneratePdf(strHtml);
                return ResponseMessage(CreateFileResponse(pdfBytes, "partyStockChallanWise.pdf", "application/pdf"));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        private HttpResponseMessage CreateFileResponse(byte[] fileBytes, string fileName, string contentType = "application/octet-stream")
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return result;
        }
    }
}

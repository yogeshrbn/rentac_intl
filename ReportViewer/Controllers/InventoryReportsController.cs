using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
using Newtonsoft.Json;
using NReco.PdfGenerator;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using WebGrease.Activities;

namespace ReportViewer.Controllers
{
    [CustomActionFilter]
    [System.Web.Mvc.Authorize]

    public class InventoryReportsController : BaseMVCController
    {


        //[System.Web.Mvc.Route("api/InventoryReports/Print")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/[controller]/Print")]
        public ActionResult Print([FromBody] FilterCriteria dto)
        {

            //string fileName = Server.MapPath("~/Reports") + @"\ewayBill.xslt";

            //string medals = System.IO.File.ReadAllText(Server.MapPath("~/Data/data.xml"));
            var user = new LoggedInUser();


            Ledger objLedger = new Ledger();
            var data = objLedger.StockRegister(dto.LedgerId, dto.LedgerSiteId, user.DefaultCompanyId, dto.From, dto.To);
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

            var memoryStream = new MemoryStream();
            string name = "sheet1";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage();
            int sentStartCol, receveStartCol;
            var ledger = new Ledger(dto.LedgerId);
            var ldto = ledger.GetDetails();
            ExcelWorksheet worksheet;
            worksheet = package.Workbook.Worksheets.Add(name);

            worksheet.Name = name;
            worksheet.Cells["A1:B1"].Merge = true;
            string clientAndSite = ldto.Name;
            if (data.Count() > 0)
            {
                clientAndSite = String.Join(Environment.NewLine, ldto.Name, data.First().Site);
            }

            worksheet.Cells["A1:B1"].Value = clientAndSite;

            worksheet.Cells["A2:A3"].Merge = true;
            worksheet.Cells["A2:A3"].Value = "Product";

            worksheet.Cells["B2:B3"].Merge = true;
            worksheet.Cells["B2:B3"].Value = "Unit";



            var sentDates = data.Where(o => o.TranType == 1).Select(o => o.TransDate).Distinct().ToList();
            var receiveDates = data.Where(o => o.TranType == 2).Select(o => o.TransDate).Distinct().ToList();
            int sentDatesCount = sentDates.Count;
            sentDatesCount = sentDatesCount == 0 ? 1 : sentDatesCount;
            worksheet.Cells[1, 3, 1, sentDatesCount + 2].Merge = true;

            worksheet.Cells[1, 3, 1, sentDatesCount + 2].Value = "Delivery";
            worksheet.Cells[1, 3, 1, sentDatesCount + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            int colFrom = 3;
            sentStartCol = 3;
            foreach (var c in sentDates)
            {

                var challans = String.Join(Environment.NewLine, data.Where(o => o.TransDate == c && o.TranType == 1).Select(o => o.JobNumber).Distinct());

                worksheet.Cells[2, colFrom, 2, colFrom].Value = c;
                worksheet.Cells[2, colFrom, 2, colFrom].Style.Numberformat.Format = "dd/MM/yyyy";
                worksheet.Cells[3, colFrom, 3, colFrom].Value = challans;
                worksheet.Cells[3, colFrom, 3, colFrom].Style.WrapText = true;
                worksheet.Cells[2, colFrom, 2, colFrom].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, colFrom, 3, colFrom].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                colFrom++;
            }

            if (sentDates.Count() == 0)
                colFrom++;




            worksheet.Cells[1, colFrom, 3, colFrom].Merge = true;
            worksheet.Cells[1, colFrom, 3, colFrom].Value = "Total Delivery";
            colFrom++;
            var recCols = receiveDates.Count == 0 ? 1 : receiveDates.Count;
            worksheet.Cells[1, colFrom, 1, recCols + colFrom - 1].Merge = true;

            worksheet.Cells[1, colFrom, 1, recCols + colFrom - 1].Value = "Return";
            worksheet.Cells[1, colFrom, 1, recCols + colFrom - 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            receveStartCol = colFrom;
            foreach (var c in receiveDates)
            {
                worksheet.Cells[2, colFrom, 2, colFrom].Value = c;
                worksheet.Cells[2, colFrom, 2, colFrom].Style.Numberformat.Format = "dd/MM/yyyy";

                var challans = String.Join(Environment.NewLine, data.Where(o => o.TransDate == c && o.TranType == 2).Select(o => o.JobNumber).Distinct());
                worksheet.Cells[3, colFrom, 3, colFrom].Value = challans;
                worksheet.Cells[3, colFrom, 3, colFrom].Style.WrapText = true;
                worksheet.Cells[2, colFrom, 2, colFrom].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, colFrom, 3, colFrom].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                colFrom++;
            }
            if (receiveDates.Count() == 0)
                colFrom++;



            worksheet.Cells[1, colFrom, 3, colFrom].Merge = true;
            worksheet.Cells[1, colFrom, 3, colFrom].Value = "Total Pickup";
            colFrom++;

            worksheet.Cells[1, colFrom, 2, colFrom + 2].Merge = true;
            worksheet.Cells[1, colFrom, 2, colFrom + 2].Value = "Balance";
            worksheet.Cells[1, colFrom, 2, colFrom + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[3, colFrom, 3, colFrom].Value = "Qty";
            worksheet.Cells[3, colFrom + 1, 3, colFrom + 1].Value = "Size";
            worksheet.Cells[3, colFrom + 2, 3, colFrom + 2].Value = "Weight";
            worksheet.Cells[3, colFrom, 3, colFrom].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[3, colFrom + 1, 3, colFrom + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[3, colFrom + 2, 3, colFrom + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            //var unqiueProducts = data.Select(o => new BillingItemDTO { Product = o.Product, Unit = o.Unit }).Distinct().ToList();
            var row = 4;
            var unqiueProducts = data.GroupBy(o => o.Product).Select(g => g.First()).ToList();
            var sortedProducts = new List<BillingItemDTO>();
            sortedProducts.InsertRange(0, unqiueProducts.Where(o => o.Unit == "Set"));
            sortedProducts.InsertRange(sortedProducts.Count(), unqiueProducts.Where(o => o.Unit != "Set"));

            foreach (var p in sortedProducts)
            {
                worksheet.Cells[row, 1, row, 1].Value = p.Product;
                worksheet.Cells[row, 2, row, 2].Value = p.Unit;

                int col = sentStartCol;
                foreach (var c in sentDates)
                {
                    var sentQty = data.Where(o => o.TransDate == c && o.Product == p.Product && o.TranType == 1).Sum(o => o.Quantity);

                    worksheet.Cells[row, col, row, col].Value = sentQty;
                    worksheet.Cells[row, col, row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, col, row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#b6edd7"));
                    col++;
                }
                if (sentDates.Count() == 0)
                    col++;

                var totalQty = data.Where(o => o.Product == p.Product && o.TranType == 1).Sum(o => o.Quantity);
                worksheet.Cells[row, col, row, col].Value = totalQty;
                worksheet.Cells[row, col, row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col, row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#fff2df"));
                //return items
                col = receveStartCol;
                foreach (var c in receiveDates)
                {
                    var sentQty = data.Where(o => o.TransDate == c && o.Product == p.Product && o.TranType == 2).Sum(o => o.Quantity);

                    worksheet.Cells[row, col, row, col].Value = sentQty;
                    worksheet.Cells[row, col, row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, col, row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d2d9f5"));
                    col++;
                }
                if (receiveDates.Count() == 0)
                    col++;

                var totalReturnQty = data.Where(o => o.Product == p.Product && o.TranType == 2).Sum(o => o.Quantity);
                worksheet.Cells[row, col, row, col].Value = totalReturnQty;
                worksheet.Cells[row, col, row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col, row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#fff2df"));
                col++;
                worksheet.Cells[row, col, row, col].Value = totalQty - totalReturnQty;

                var balanceItem = items.FirstOrDefault(o => o.ProductId == p.ProductId);

                worksheet.Cells[row, col + 1, row, col + 1].Value = balanceItem.SizeBalance;
                worksheet.Cells[row, col + 2, row, col + 2].Value = balanceItem.WeightBalance;

                row++;
            }

            worksheet.Columns.Width = 15;
            worksheet.Column(1).Width = 40;
            worksheet.OutLineApplyStyle = true;

            worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells[1, 1, 3, worksheet.Dimension.Columns].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 3, worksheet.Dimension.Columns].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


            package.SaveAs(memoryStream);



            memoryStream.Position = 0;
            var contentType = "application/octet-stream";
            var fileName = "fileName.xlsx";
            return File(memoryStream, contentType, fileName);


        }



        //[System.Web.Mvc.Route("api/InventoryReports/Print")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/[controller]/ClientWiseItemBalance")]
        public ActionResult ClientWiseItemBalance([FromBody] FilterCriteria dto)
        {
            try
            {
                //string fileName = Server.MapPath("~/Reports") + @"\ewayBill.xslt";

                //string medals = System.IO.File.ReadAllText(Server.MapPath("~/Data/data.xml"));
                var user = new LoggedInUser();
                String from = "", to = "";
                if (!String.IsNullOrEmpty(dto.From))
                    from = dto.From;
                if (!String.IsNullOrEmpty(dto.To))
                    to = dto.To;

                Ledger objLedger = new Ledger();
                //  var data = objLedger.StockRegister(dto.LedgerId, dto.LedgerSiteId, user.DefaultCompanyId, dto.From, dto.To);
                //   var data = objLedger.ClientWiseItems(dto.LedgerId, dto.LedgerSiteId, user.DefaultCompanyId, from, to);

                DataSet ds = objLedger.ClientWiseItems(dto.LedgerId, dto.LedgerSiteId, user.DefaultCompanyId, from, to, dto.PONumber);
                List<LedgerbalanceDTO> lst = new Utils<LedgerbalanceDTO>().ConstructList(ds);

                if (!dto.Print)
                {
                    return Content(JsonConvert.SerializeObject(lst), "application/json");
                }
                if (dto.Pdf)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var _data = new { data = new { comp = comp, details = ds, toDate = Convert.ToDateTime( dto.To) } };
                    string jsonText = JsonConvert.SerializeObject(_data);
                    // To convert JSON text contained in string json into an XML node
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    //Server.MapPath("~/Reports") + @"\ewayBill.xslt";
                    string repName = Server.MapPath("~/Reports") + @"\cleintWiseItemBalance.xslt";

                    var html = RenderXml(xml, repName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "cleintWiseItemBalance.pdf";
                    //  return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);

                    //  var stream = new MemoryStream();                  
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
                }
                #region excel
                var memoryStream = new MemoryStream();
                string name = "sheet1";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage package = new ExcelPackage();

                var ledger = new Ledger(dto.LedgerId);
                var ldto = ledger.GetDetails();
                ExcelWorksheet worksheet;
                worksheet = package.Workbook.Worksheets.Add(name);

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
                    //var modelTable = worksheet.Cells["A1:F:" + row];
                    //modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    //modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.OutLineApplyStyle = true;

                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                //    modelTable.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                worksheet.OutLineApplyStyle = true;

                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                package.SaveAs(memoryStream);



                memoryStream.Position = 0;
                var contentType = "application/octet-stream";
                var fileName = "fileName.xlsx";
                return File(memoryStream, contentType, fileName);
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }




        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/[controller]/ItemWiseClientBalance")]
        public ActionResult ItemWiseClientBalance([FromBody] FilterCriteria dto)
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
                    return Content(JsonConvert.SerializeObject(lst), "application/json");
                }
                if (dto.Pdf)
                {
                    var comp = new Company(user.DefaultCompanyId).GetDetails();
                    var toDate = !String.IsNullOrEmpty(dto.To) ? Convert.ToDateTime(dto.To) : DateTime.Now;
                    var _data = new { data = new { comp = comp, details = ds, toDate = toDate } };
                    string jsonText = JsonConvert.SerializeObject(_data);
                    XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
                    string xml = doc.InnerXml;

                    string repName = Server.MapPath("~/Reports") + @"\itemWiseClientBalance.xslt";
                    var html = RenderXml(xml, repName);
                    var strHtml = html.ToHtmlString();
                    HtmlToPdfConverter con = new HtmlToPdfConverter();
                    var fileBytes = con.GeneratePdf(strHtml);
                    string dwnFileName = "itemWiseClientBalance.pdf";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dwnFileName);
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
                return File(memoryStream, "application/octet-stream", "itemWiseClientBalance.xlsx");
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public HtmlString RenderXml(string xml, string xsltPath)
        {
            XsltArgumentList args = new XsltArgumentList();
            var utils = new Utils();
            args.AddExtensionObject("urn:util-format", utils);
            // Create XslCompiledTransform object to loads and compile XSLT file.  
            XslCompiledTransform tranformObj = new XslCompiledTransform();

            tranformObj.Load(xsltPath);

            // Create XMLReaderSetting object to assign DtdProcessing, Validation type  
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.DtdProcessing = DtdProcessing.Parse;
            xmlSettings.ValidationType = ValidationType.DTD;

            // Create XMLReader object to Transform xml value with XSLT setting   
            using (XmlReader reader = XmlReader.Create(new StringReader(xml), xmlSettings))
            {
                StringWriter writer = new StringWriter();
                tranformObj.Transform(reader, args, writer);
                string html = writer.ToString();
                html = LoadCss(html);
                // Generate HTML string from StringWriter  
                HtmlString htmlString = new HtmlString(html);

                return htmlString;
            }
        }
        string LoadCss(string html)
        {


            var cssPath = Server.MapPath("~/Content") + @"\print.css";
            var printCss = System.IO.File.ReadAllText(cssPath);
            printCss = "<style>" + printCss + "</style>";
            html = html.Replace(" #pdf", printCss);
            html = html.Replace(" #preview", "");

            return html;
        }
    }
}

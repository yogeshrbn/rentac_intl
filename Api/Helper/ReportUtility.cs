using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
using BAL.Objects;
using BAL.Services;
using FarmaAPI.Helper;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.ctx;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using NReco.PdfGenerator;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Utils;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Spire.Pdf.Fields;
using Spire.Pdf.HtmlConverter;
using Spire.Pdf.Widget;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Xsl;
using static iTextSharp.tool.xml.html.HTML;
//using static Org.BouncyCastle.Math.EC.ECCurve;
namespace FarmaAPI.Helper
{
    public class ReportUtility
    {
        private static readonly string mapPath = Path.Combine(HttpRuntime.AppDomainAppPath, "");// System.Web.HttpContext.Current.Server.MapPath("~");
        private static readonly string tempPath = Path.Combine(HttpRuntime.AppDomainAppPath, "temp") + @"\";//System.Web.HttpContext.Current.Server.MapPath("~/temp/");
        private static readonly string reportFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "rpts") + @"\";// System.Web.HttpContext.Current.Server.MapPath("~/rpts/");
        private static readonly string docsPath = Path.Combine(HttpRuntime.AppDomainAppPath, "docs");

        /// <summary>
        /// Applies general/print config printWeightOnChallan (none | masters | challan) to challan item dataset and header for templates.
        /// </summary>
        //public  void ApplyPrintWeightOnChallanSettings(DataSet mainDs, DataSet headerDataSet, int companyId)
        //{
        //    var cfg = new Config();
        //    var printConfigs = cfg.GetConfig(companyId, "general", "print");
        //    var mode = ResolvePrintWeightOnChallanMode(printConfigs);
        //    EnsureHeaderPrintWeightOnChallan(headerDataSet, mode);
        //    ApplyPrintWeightOnChallanToMainTable(mainDs, mode);
        //}

        //private   string ResolvePrintWeightOnChallanMode(IEnumerable<ConfigDTO> printConfigs)
        //{
        //    if (printConfigs == null)
        //        return "challan";
        //    var c = printConfigs.FirstOrDefault(o => string.Equals(o.Key, "printWeightOnChallan", StringComparison.OrdinalIgnoreCase));
        //    if (c == null || string.IsNullOrWhiteSpace(c.Value))
        //        return "challan";
        //    var v = c.Value.Trim().ToLowerInvariant();
        //    if (v == "none" || v == "masters" || v == "challan")
        //        return v;
        //    return "challan";
        //}

        private static void EnsureHeaderPrintWeightOnChallan(DataSet headerDataSet, string mode)
        {
            if (headerDataSet == null || headerDataSet.Tables.Count == 0 || headerDataSet.Tables[0].Rows.Count == 0)
                return;
            var t = headerDataSet.Tables[0];
            if (!t.Columns.Contains("PrintWeightOnChallan"))
                t.Columns.Add("PrintWeightOnChallan", typeof(string));
            t.Rows[0]["PrintWeightOnChallan"] = mode;
        }

        private static string FindMasterWeightPerUnitColumn(DataTable t)
        {
            foreach (var name in new[] { "ProductWeight", "MasterWeight", "ItemWeight", "WeightPerUnit", "UnitWeight", "ProductWt" })
            {
                if (t.Columns.Contains(name))
                    return name;
            }
            return null;
        }

        private static void ApplyPrintWeightOnChallanToMainTable(DataSet mainDs, string mode)
        {
            if (mainDs == null || mainDs.Tables.Count == 0)
                return;
            var t = mainDs.Tables[0];
            if (!t.Columns.Contains("Weight"))
                return;

            if (mode == "none")
            {
                foreach (DataRow r in t.Rows)
                    r["Weight"] = DBNull.Value;
                return;
            }

            if (mode == "masters")
            {
                var wcol = FindMasterWeightPerUnitColumn(t);
                if (wcol == null || !t.Columns.Contains("SentQty"))
                    return;

                double total = 0;
                foreach (DataRow r in t.Rows)
                {
                    var w = r[wcol] != DBNull.Value ? Convert.ToDouble(r[wcol]) : 0d;
                    var q = r["SentQty"] != DBNull.Value ? Convert.ToDouble(r["SentQty"]) : 0d;
                    total += w * q;
                }

                foreach (DataRow r in t.Rows)
                    r["Weight"] = total;
            }
        }

        public async Task<string> EmailRentIssueReceipt(int workOrderId, int companyId, bool _sendEmail)
        {
            WorkOrder wOrder = new WorkOrder(0);
            var s = new AzureStorageService();
            int partyId = 0;
            var user = new LoggedInUser();
            //int companyId = new LoggedInUser().DefaultCompanyId;
            BAL.Objects.Report objReport = new BAL.Objects.Report();
            String fileToSave = "", fileName = "";
            DataSet mainDS = wOrder.ItemIssuedForPrint(workOrderId, companyId);
            bool fileExists = false;

            if (mainDS.Tables.Count > 0)
            {
                if (mainDS.Tables[0].Rows.Count > 0)
                {
                    partyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                    companyId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["CompanyId"]);
                    var challanNumber = Convert.ToString(mainDS.Tables[0].Rows[0]["ChallanNumber"]);
                    fileName = workOrderId + ".pdf";
                    fileToSave = tempPath + @"\" + fileName;
                }
            }

            // Select template group by challan type (rent/contract/sales).
            // NOTE: challanType is available from the main dataset.
            var challanType = Convert.ToInt16(mainDS.Tables[0].Rows[0]["ChallanType"]);
            var templateGroup = TemplateGroups.ISSUECHALLAN;
            var configCategory = ConfigCategoryNames.ISSUECHALLAN;
            if (challanType == (short)ChallanType.LIFT_DELIVERY)
            {
                templateGroup = TemplateGroups.CONTRACTDELIVERYCHALLAN;
                configCategory = ConfigCategoryNames.CONTRACTDELIVERYCHALLAN;
            }
            else if (challanType == (short)ChallanType.SALE)
            {
                templateGroup = TemplateGroups.SALESDELIVERYCHALLAN;
                configCategory = ConfigCategoryNames.SALESDELIVERYCHALLAN;
            }

            var templateService = new TemplateService();
            var template = await templateService.GetDefaultPdfTemplate(companyId, templateGroup,
                configCategory, ConfigSubCategoryNames.TEMPLATES);
            if (template == null)
            {
                throw new Exception("Report template could not found.");
            }
            DataSet headerDataSet = wOrder.GetChallanReportHeader(workOrderId, 1);

            //ApplyPrintWeightOnChallanSettings(mainDS, headerDataSet, companyId);

            var config = new Config();
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var challanConfigs = config.GetConfig(companyId, "issuechallan", "issuechallan");

            var printSignature = true;
            if (printConfigs != null && printConfigs.Count() > 0)
            {
                var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (c != null)
                {
                    printSignature = c.Value.Contains("challans");
                }
            }


            if (headerDataSet.Tables[0].Rows.Count > 0)
            {
                headerDataSet.Tables[0].Columns.Add("PrintRateType");
                headerDataSet.Tables[0].Rows[0]["PrintRateType"] = "sale";
                if (headerDataSet.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                {
                    var logo = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyLogo"]);

                    if (HttpContext.Current != null)
                    {
                        logo = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/docs/comp/" + logo;
                    }
                    headerDataSet.Tables[0].Rows[0]["CompanyLogo"] = logo;
                }
                var signaure = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Signature"]);
                if (!String.IsNullOrEmpty(signaure) && printSignature)
                {
                    headerDataSet.Tables[0].Rows[0]["Signature"] = docsPath + "/comp/" + signaure;
                }
            }

            if (challanConfigs != null)
            {
                var printRateType = challanConfigs.Where(o => o.Key.ToLower() == "printratetype").FirstOrDefault();
                if (printRateType != null)
                {
                    if (!String.IsNullOrEmpty(printRateType.Value))
                    {
                        headerDataSet.Tables[0].Rows[0]["PrintRateType"] = printRateType.Value;
                    }
                }
            }


            string xml = mainDS.GetXml();
            string headerXML = headerDataSet.GetXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlDocument headerDOC = new XmlDocument();
            headerDOC.LoadXml(headerXML);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Header", "");
            //var confg = new Config();
            //var chlConfig = confg.GetConfig(user.DefaultCompanyId, "ISSUECHALLAN", null);
            XmlNode configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");
            if (printConfigs != null)
            {
                challanConfigs.AddRange(printConfigs);
            }
            string jsonText = JsonConvert.SerializeObject(challanConfigs);

            string wrappedJson = "{\"root\":" + jsonText + "}";
            XmlDocument configDoc = JsonConvert.DeserializeXmlNode(wrappedJson, "config");
            // To convert JSON text contained in string json into an XML node
            if (configDoc.FirstChild != null)
            {
                configNode.InnerXml = configDoc.FirstChild.InnerXml;
            }

            double total = Convert.ToDouble(mainDS.Tables[0].Rows[0]["ChallanTotal"]);
            XmlElement elem = doc.CreateElement("Rupees");
            elem.InnerText = Utils.ConvertNumbertoWords(total.ToString());

            node.InnerXml = headerDOC.FirstChild.FirstChild.InnerXml;

            node.AppendChild(elem);
            doc.FirstChild.PrependChild(node);
            doc.FirstChild.AppendChild(configNode);
            xml = doc.OuterXml;

            if (challanType == 10)
            {
                string _fileName = template.FileName.Split('.')[0];
                var hireFileName = _fileName + "_hire.xsl";

                var repFilePath = reportFilePath + hireFileName;
                if (File.Exists(repFilePath))
                {
                    template.FileName = hireFileName;
                }
            }
            //  string retpData = GetReportBody(GetReportFileName(ReortFileName.ISSUE_ITEM), xml);
            string retpData = GetReportBody(GetReportFileName(template.FileName), xml);
            string clientEmail = Convert.ToString(headerDataSet.Tables[0].Rows[0]["BillEmail"]);
            string clientName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);


            EmailParameters param = new EmailParameters();
            if (headerDataSet.Tables.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows.Count > 0)
                {
                    param.PartyName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
                    param.Company = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Company"]);
                    param.ChallanNumber = Convert.ToString(headerDataSet.Tables[0].Rows[0]["ChallanNumber"]);


                    param.CompanyEmail = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyEmail"]);
                    param.CompanyCity = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyCity"]);
                }
            }
            if (!fileExists)
            {
                fileName = "DC" + workOrderId.ToString() + "-" + partyId + ".pdf"; ;// workOrderId.ToString() + DateTime.Now.ToString().Replace(":", " ") + ".pdf";

                string mapPath = tempPath;
                fileToSave = mapPath + fileName;
                if (File.Exists(fileToSave))
                {
                    File.Delete(fileToSave);
                }

                if (template.TemplateId == 13 || template.TemplateId == 1 || template.TemplateId == 14 || template.ApplyPrintConfig)
                {
                    createPdfnReco(retpData, fileToSave, template.Orientation == 2, template.ApplyPrintConfig);
                }
                else
                {
                    //  HtmlToPdfConverter con = new HtmlToPdfConverter();
                    createPdf(retpData, fileToSave, template.Orientation == 2);

                    //byte[] file = con.GeneratePdf(retpData);
                    //File.WriteAllBytes(fileToSave, file);
                    //string txtFile = workOrderId.ToString() + DateTime.Now.ToString().Replace(":", " ") + ".css";
                    //String txtFilePath = mapPath += txtFile;
                    //using (StreamWriter sw = new System.IO.StreamWriter(txtFilePath))
                    //{
                    //    sw.Write(retpData);
                    //}
                }

                if (File.Exists(fileToSave))
                {
                    await s.UploadFileAsync(user.FinYearId, companyId + "/challan", fileName, fileToSave);
                }
            }


            //if (_sendEmail)
            //{
            //    sendEmail(clientEmail, param.Company, "Item Issued", param, EmailTemplate.ISSUE_ITEM, fileName);

            //}
            return fileName;
        }
        public void createPdfnReco(String HTML, String file, bool landScape = false, bool applyPrintConfig = false)
        {
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            con.Size = NReco.PdfGenerator.PageSize.A4;
            con.Margins.Top = 2;
            con.Margins.Bottom = 2;
            if (applyPrintConfig)
            {
                con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                               // Or try these specific arguments:
                con.CustomWkHtmlArgs = "--print-media-type --enable-smart-shrinking";
                con.Margins.Top = 2;
                con.Margins.Bottom = 2;
            }
            if (landScape)
            {
                con.Orientation = PageOrientation.Landscape;
            }
            var fileBytes = con.GeneratePdf(HTML);

            using (var stream = System.IO.File.Create(file))
            {

                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        public void createPdf(String HTML, String file, bool landScape = false)
        {
            Document document = new Document();
            if (landScape)
            {
                document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                document.SetMargins(15f, 15f, 15f, 15f);
            }
            var output = new FileStream(file, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, output);
            String cssFilePath = reportFilePath + "/issue.txt";


            document.Open();

            byte[] cssArray = File.ReadAllBytes(cssFilePath); ;// Encoding.ASCII.GetBytes(cssText);



            MemoryStream ms = new System.IO.MemoryStream(cssArray);

            iTextSharp.tool.xml.css.StyleAttrCSSResolver cssResolver = new StyleAttrCSSResolver();
            iTextSharp.tool.xml.css.ICssFile cssFile = iTextSharp.tool.xml.XMLWorkerHelper.GetCSS(ms);
            //cssResolver.AddCssFile("", true);
            cssResolver.AddCss(cssFile);

            // HTML
            HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());

            // Pipelines
            iTextSharp.tool.xml.pipeline.end.PdfWriterPipeline pdf = new iTextSharp.tool.xml.pipeline.end.PdfWriterPipeline(document, writer);
            HtmlPipeline html = new HtmlPipeline(htmlContext, pdf);
            CssResolverPipeline css = new CssResolverPipeline(cssResolver, html);
            // html pagina parsen in een arraylist van IElements
            //var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\Shiv01.ttf";
            //var fontName = "Shivaji01";
            //FontFactory.Register(fontPath, fontName);


            // XML Worker
            XMLWorker worker = new XMLWorker(css, true);

            iTextSharp.tool.xml.parser.XMLParser p = new iTextSharp.tool.xml.parser.XMLParser(worker);



            byte[] htmlsArray = Encoding.ASCII.GetBytes(HTML);
            MemoryStream msHTML = new System.IO.MemoryStream(htmlsArray);
            //  XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, msHTML, ms);
            p.Parse(msHTML);
            document.Close();
            output.Close();
        }

        /// <summary>
        /// Inserts a white image at top of the generated pdf. this is for if we genreate pdf using spire.pdf free version.
        /// Currently this function is not in use
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="newFileName"></param>
        private void InsertImageToPdf(string sourceFileName, string newFileName)
        {
            //String imageFileName = System.Web.HttpContext.Current.Server.MapPath("~/images/white.jpg");
            string imageFileName = mapPath + "/images/white.jpg";
            using (Stream pdfStream = new FileStream(sourceFileName, FileMode.Open))
            using (Stream imageStream = new FileStream(imageFileName, FileMode.Open))
            using (Stream newpdfStream = new FileStream(newFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(pdfStream);
                iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(pdfReader, newpdfStream);
                iTextSharp.text.pdf.PdfContentByte pdfContentByte = pdfStamper.GetOverContent(1);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageStream);

                image.SetAbsolutePosition(0, iTextSharp.text.PageSize.A4.Height - image.ScaledHeight);

                pdfContentByte.AddImage(image, true);
                pdfStamper.Close();
            }
        }


        public async Task<string> PrintAndEmailReceiveReceipt(int grnId, bool _sendEmail)
        {
            WorkOrder wOrder = new WorkOrder(0);


            BAL.Objects.Report objReport = new BAL.Objects.Report();

            DataSet mainDS = wOrder.ItemReceived_Report(grnId);

            GRN grn = new GRN();

            DataSet headerDataSet = grn.GRNHeader(grnId);

            Config config = new Config();
            int companyId = Convert.ToInt16(mainDS.Tables[0].Rows[0]["CompanyId"]);
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var printSignature = true;
            if (printConfigs != null && printConfigs.Count() > 0)
            {
                var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (c != null)
                {
                    printSignature = c.Value.Contains("challans");
                }
            }
            if (headerDataSet.Tables[0].Rows.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                {
                    var logo = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyLogo"]);
                    logo = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/docs/comp/" + logo;
                    headerDataSet.Tables[0].Rows[0]["CompanyLogo"] = logo;
                }
                var signaure = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Signature"]);
                if (!String.IsNullOrEmpty(signaure) && printSignature)
                {
                    headerDataSet.Tables[0].Rows[0]["Signature"] = docsPath + "/comp/" + signaure;
                }
            }

            string xml = mainDS.GetXml();
            string headerXML = headerDataSet.GetXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlDocument headerDOC = new XmlDocument();
            headerDOC.LoadXml(headerXML);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Header", "");

            //double total = Convert.ToDouble(mainDS.Tables[0].Compute("Sum(SubTotal)", ""));
            //XmlElement elem = doc.CreateElement("Rupees");
            //elem.InnerText = Utils.ConvertNumbertoWords(total.ToString());

            node.InnerXml = headerDOC.FirstChild.FirstChild.InnerXml;
            //node.AppendChild(elem);

            var challanConfigs = config.GetConfig(companyId, "RECEIVINGCHALLAN", "RECEIVINGCHALLAN");
            string jsonText = JsonConvert.SerializeObject(challanConfigs);

            string wrappedJson = "{\"root\":" + jsonText + "}";
            XmlDocument configDoc = JsonConvert.DeserializeXmlNode(wrappedJson, "Config");
            XmlNode configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");
            if (configDoc.FirstChild != null)
            {
                configNode.InnerXml = configDoc.FirstChild.InnerXml;
            }

            //if (headerDataSet.Tables.Count > 1)
            //{
            //    XmlNode configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");

            //    var writer = new StringWriter();
            //    headerDataSet.Tables[1].WriteXml(writer);
            //    configNode.InnerXml = writer.ToString();
            //    doc.FirstChild.PrependChild(configNode);


            //}
            doc.FirstChild.PrependChild(node);
            doc.FirstChild.AppendChild(configNode);
            xml = doc.OuterXml;
            // var repFilename = GetReportFileName(ReortFileName.RECEIVE_ITEM);
            var chType = Convert.ToInt16(mainDS.Tables[0].Rows[0]["TypeId"]);

            var templateService = new TemplateService();
            var templateGroup = TemplateGroups.RETURNS;
            var configCategory = ConfigCategoryNames.RETURNS;
            if (chType == (short)ChallanType.CONTRACT_RETURN)
            {
                templateGroup = TemplateGroups.CONTRACTRETURNS;
                configCategory = ConfigCategoryNames.CONTRACTRETURNS;
            }
            else if (chType == (short)ChallanType.SALE)
            {
                templateGroup = TemplateGroups.SALESRETURNS;
                configCategory = ConfigCategoryNames.SALESRETURNS;
            }

            var template = await templateService.GetDefaultPdfTemplate(companyId, templateGroup, configCategory, ConfigSubCategoryNames.TEMPLATES);
            string repFilename = "recItem-rent.xsl";
            bool applyPrintConfig = false;
            if (template != null)
            {
                repFilename = template.FileName;
                // throw new Exception("Report template could not found.");
                applyPrintConfig = template.ApplyPrintConfig;
            }

            if (chType == 13)
            {
                repFilename = "recItem-rent_unhire.xsl";//repFilename.Replace("recItem-rent.xsl", "recItem-rent_unhire.xsl");
            }
            repFilename = GetReportFileName(repFilename);

            string retpData = GetReportBody(repFilename, xml);

            string fileName = Guid.NewGuid().ToString() + "grn.pdf";

            //string fileToSave =  System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string fileToSave = mapPath + "/temp/";
            fileToSave += fileName;
            if (File.Exists(fileToSave))
            {
                File.Delete(fileToSave);
            }

            if (!applyPrintConfig)
            {
                createPdf(retpData, fileToSave);
            }
            else
            {
                createPdfnReco(retpData, fileToSave, false, applyPrintConfig);

            }
            // HtmlToPdfConverter con = new HtmlToPdfConverter();

            //  byte[] file = con.GeneratePdf(retpData);
            //File.WriteAllBytes(fileToSave, file);
            string clientEmail = Convert.ToString(headerDataSet.Tables[0].Rows[0]["BillEmail"]);
            string clientName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
            EmailParameters param = new EmailParameters();
            if (headerDataSet.Tables.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows.Count > 0)
                {
                    param.PartyName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
                    param.Company = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Company"]);
                    param.GRN = Convert.ToString(headerDataSet.Tables[0].Rows[0]["GRN"]);
                    param.Sender = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Sender"]);
                    param.Receiver = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Receiver"]);
                    param.CompanyEmail = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyEmail"]);
                    param.CompanyCity = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyCity"]);
                }
            }

            if (_sendEmail)
            {
                sendEmail(clientEmail, param.Company, "Item Received", param, EmailTemplate.RECEIVE_ITEM, fileName);
            }

            return fileName;
        }

        public string DueBills(int ledgerSiteId, int companyId, int ledgerId, DateTime asofDate, int finYearId)
        {
            Billing bill = new Billing();
            var from = new DateTime(asofDate.Year, asofDate.Month, 1);
            DataSet ds = bill.DueBillsSummary(ledgerSiteId, companyId, ledgerId, from, asofDate, finYearId);

            string headerXML = CompanyDetails();

            string xml = ds.GetXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Header", "");
            node.InnerXml = headerXML;
            doc.FirstChild.PrependChild(node);
            string retpData = GetReportBody(GetReportFileName(ReortFileName.DUE_BILLS), doc.InnerXml);

            string fileName = Guid.NewGuid().ToString() + ".pdf";

            //  string fileToSave = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string fileToSave = mapPath + "/temp/";
            fileToSave += fileName;
            if (File.Exists(fileToSave))
            {
                File.Delete(fileToSave);
            }
            HtmlToPdfConverter con = new HtmlToPdfConverter();

            byte[] file = con.GeneratePdf(retpData);
            File.WriteAllBytes(fileToSave, file);
            return fileName;
        }

        public byte[] DueBillsPdf(int ledgerSiteId, int companyId, int ledgerId, DateTime asofDate, int finYearId)
        {
            Billing bill = new Billing();
            var from = new DateTime(asofDate.Year, asofDate.Month, 1);
            DataSet ds = bill.DueBillsSummary(ledgerSiteId, companyId, ledgerId, from, asofDate, finYearId);
            string headerXML = CompanyDetails();
            string xml = ds.GetXml();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "Header", "");
            node.InnerXml = headerXML;
            doc.FirstChild.PrependChild(node);
            string retpData = GetReportBody(GetReportFileName(ReortFileName.DUE_BILLS), doc.InnerXml);
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            return con.GeneratePdf(retpData);
        }

        public string ConvertToPdf(string htmlData, string fileName)
        {
            HtmlToPdfConverter con = new HtmlToPdfConverter();

            byte[] file = con.GeneratePdf(htmlData);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllBytes(fileName, file);
            return fileName;
        }
        public static byte[] ConvertToPdf(string htmlData)
        {
            HtmlToPdfConverter con = new HtmlToPdfConverter();

            byte[] file = con.GeneratePdf(htmlData);
            return file;
            //using (var stream = new MemoryStream(file))
            //{
            //    return stream;
            //}
            //if (File.Exists(fileName))
            //{
            //    File.Delete(fileName);
            //}
            //File.WriteAllBytes(fileName, file);
            //return fileName;
        }
        public void SendDueBillEmailReminder(int invoiceId, ReminderType remType)
        {
            int ledgerId = 0;
            Billing billing = new Billing();
            DataSet mainDS = billing.PrintBill(invoiceId);
            BAL.Objects.Report objReport = new BAL.Objects.Report();
            if (mainDS.Tables.Count > 0)
            {
                if (mainDS.Tables[0].Rows.Count > 0)
                {
                    ledgerId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                }
            }

            DataSet headerDataSet = objReport.GetReportHeader(ledgerId, new LoggedInUser().DefaultCompanyId);
            string clientEmail = Convert.ToString(headerDataSet.Tables[0].Rows[0]["BillEmail"]);
            string clientName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
            string clientPhone = Convert.ToString(headerDataSet.Tables[0].Rows[0]["ClientPhone"]);
            EmailParameters param = new EmailParameters();
            if (headerDataSet.Tables.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows.Count > 0)
                {
                    param.PartyName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
                    param.Company = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Company"]);

                }
            }
            if (mainDS.Tables.Count > 0)
            {
                if (mainDS.Tables[0].Rows.Count > 0)
                {
                    param.InvoiceNumber = Convert.ToString(mainDS.Tables[0].Rows[0]["InvoiceNumber"]);
                    param.InvoiceAmount = Convert.ToDouble(mainDS.Tables[0].Rows[0]["Total"]);
                    param.InvoiceDate = Convert.ToString(mainDS.Tables[0].Rows[0]["InvoiceDate"]);

                }
            }
            if (remType == ReminderType.EMAIL)
            {
                sendEmail(clientEmail, param.Company, "Due Bill Reminder", param, EmailTemplate.BILL_REMINDER);
            }
            else if (remType == ReminderType.SMS)
            {
                CommHelpler comHelper = new CommHelpler();
                var billInfo = billing.SelInvoiceHeader(invoiceId, 0);
                if (billInfo != null && billInfo.Tables.Count > 0 && billInfo.Tables[0].Rows.Count > 0)
                {
                    var dr = billInfo.Tables[0].Rows[0];
                    var sparams = new Dictionary<string, string>();
                    sparams.Add("party", Convert.ToString(dr["PartyName"]));
                    sparams.Add("billAmount", Convert.ToString(dr["Total"]));
                    sparams.Add("company", Convert.ToString(dr["CompanyName"]));

                    comHelper.sendSms(Convert.ToString(dr["ContactPersonMobile"]), SMSTemplates.BILL_GENERATED, MessageEvent.BILL_GENRATED, sparams);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="note"></param>
        /// <param name="_sendEmail"></param>
        /// <returns></returns>
        public string DebitCreditNote(int transactionId, bool reminder, ReminderType remType)
        {
            int ledgerId = 0;
            string reportFile = "", emailFile = "";
            int companyId = new LoggedInUser().DefaultCompanyId;
            Ledger ledger = new Ledger();
            DataSet mainDS = ledger.GetTransactionById(transactionId);
            BAL.Objects.Report objReport = new BAL.Objects.Report();
            if (mainDS.Tables.Count > 0)
            {
                if (mainDS.Tables[0].Rows.Count > 0)
                {
                    // JAI : ledgerId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["CrLederId"]);
                    ledgerId = Convert.ToInt32(mainDS.Tables[0].Rows[0]["LedgerId"]);
                }
            }



            DataSet headerDataSet = objReport.GetReportHeader(ledgerId, companyId);
            if (headerDataSet.Tables[0].Rows.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows[0]["CompanyLogo"] != DBNull.Value)
                {
                    var logo = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyLogo"]);
                    logo = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + "/docs/comp/" + logo;
                    headerDataSet.Tables[0].Rows[0]["CompanyLogo"] = logo;
                }

            }

            string clientEmail = Convert.ToString(headerDataSet.Tables[0].Rows[0]["BillEmail"]);
            string clientName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
            string clientPhone = Convert.ToString(headerDataSet.Tables[0].Rows[0]["ClientPhone"]);
            string subject = "";
            MessageEvent msgEvent = MessageEvent.DEBIT_NOTE;
            EmailParameters param = new EmailParameters();
            if (headerDataSet.Tables.Count > 0)
            {
                if (headerDataSet.Tables[0].Rows.Count > 0)
                {
                    param.PartyName = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Client"]);
                    param.Company = Convert.ToString(headerDataSet.Tables[0].Rows[0]["Company"]);
                    param.ClientBIllAddress = Convert.ToString(headerDataSet.Tables[0].Rows[0]["BillingAddress"]);
                    param.CompanyAddress = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyAddress"]);
                    param.CompanyLogo = Convert.ToString(headerDataSet.Tables[0].Rows[0]["CompanyLogo"]);


                }
            }

            if (mainDS.Tables.Count > 0)
            {
                if (mainDS.Tables[0].Rows.Count > 0)
                {
                    param.ReceiptNumber = Convert.ToString(mainDS.Tables[0].Rows[0]["ReceiptNumber"]);
                    param.CreationDate = Convert.ToString(mainDS.Tables[0].Rows[0]["TransactionDate"]);
                    param.InvoiceNumber = Convert.ToString(mainDS.Tables[0].Rows[0]["TranRefNumber"]);
                    param.Amount = Convert.ToDouble(mainDS.Tables[0].Rows[0]["TransactionAmount"]);
                    param.Narration = Convert.ToString(mainDS.Tables[0].Rows[0]["Narration"]);

                    int transactionType = Convert.ToInt16(mainDS.Tables[0].Rows[0]["TransactionType"]);
                    int entryType = Convert.ToInt16(mainDS.Tables[0].Rows[0]["EntryType"]);
                    if (entryType == 10 | entryType == 13)  //debit/credit note
                    {
                        switch (transactionType)
                        {
                            case 1:
                            case 3:
                                reportFile = ReortFileName.DEBIT_NOTE;
                                emailFile = EmailTemplate.DEBIT_NOTE;
                                subject = "Debit Note Issued";
                                msgEvent = MessageEvent.DEBIT_NOTE;
                                break;
                            case 2:
                            case 4:
                                reportFile = ReortFileName.CREDIT_NOTE;
                                emailFile = EmailTemplate.CREDIT_NOTE;
                                subject = "Credit Note Issued";
                                msgEvent = MessageEvent.CREDIT_NOTE;
                                break;
                        }
                    }
                    else
                    {
                        switch (entryType)
                        {
                            case 8:
                                reportFile = ReortFileName.CASH_RECEIPT;
                                emailFile = EmailTemplate.CASH_RECEIPT;
                                subject = "Cash Received";
                                msgEvent = MessageEvent.AMT_RECEIVED;
                                break;
                            default:
                                reportFile = ReortFileName.BANK_ENTRY;
                                emailFile = EmailTemplate.BANK_ENTRY;
                                subject = "Amount Received";
                                msgEvent = MessageEvent.AMT_RECEIVED;
                                break;
                        }
                    }
                }
            }



            string fileName = transactionId + ".pdf";
            string retpData = GetReportBody(GetReportFileName(reportFile), param.GetXML());
            //string fileToSave = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string fileToSave = mapPath + "/temp/";
            fileToSave += fileName;
            if (File.Exists(fileToSave))
            {
                File.Delete(fileToSave);
            }
            HtmlToPdfConverter con = new HtmlToPdfConverter();

            byte[] file = con.GeneratePdf(retpData);
            File.WriteAllBytes(fileToSave, file);

            if (reminder)
            {
                if (remType == ReminderType.EMAIL)
                {

                    sendEmail(clientEmail, param.Company, subject, param, emailFile, fileName);

                }
                else if (remType == ReminderType.SMS)
                {
                    CommHelpler comhelper = new CommHelpler();
                    comhelper.sendSms(clientPhone, param, msgEvent);
                }
            }
            return fileName;


        }

        public void sendEmail(string to, string displayName, string subJect, EmailParameters param, string emailTemplate, params string[] attachment)
        {

            string mailURL = mapPath + "/mailtemplates/" + emailTemplate; // MailerUtility.GetEmailURL(emailTemplate);
            string body = MailerUtility.GetMailBody(mailURL, param.GetXML());
            for (int i = 0; i < attachment.Length; i++)
            {
                attachment[i] = mapPath + @"\temp\" + attachment[i];
            }


            SendEmails.SendEmail(to, displayName, subJect, body, attachment);
        }

        public string getReportHTML(string reportTemplate, string xml)
        {
            string reportTemplatePath = Path.Combine(HttpRuntime.AppDomainAppPath, "ReportTemplates", reportTemplate);
            return GetReportBody(reportTemplatePath, xml);
        }
        public void sendEmails(string to, string subject, int companyId, int ledgerid, EmailParameters ep, string fileName)
        {
            LedgerDTO ldto = new Ledger(ledgerid).GetDetails();
            CompanyDTO cDto = new Company(companyId);
            ep.PartyName = ldto.Name;
            ep.Company = cDto.Name;

            sendEmail(to, "Rentac", subject, ep, fileName);
            //  SendEmail(to,"Rentac",subject,ep,
        }

        public static string GetReportFileName(string fileName)
        {
            return reportFilePath + fileName;
        }
        public string GetReportBody(string XsltUrl, string xmldata)
        {
            XsltArgumentList args = new XsltArgumentList();
            var utils = new Utils();
            args.AddExtensionObject("urn:util-format", utils);

            XslCompiledTransform xDoc = new XslCompiledTransform();
            xDoc.Load(XsltUrl, XsltSettings.TrustedXslt, new XmlUrlResolver());
            StringBuilder resultString = new StringBuilder();
            XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(resultString));
            XmlTextReader xmlReader = new XmlTextReader(new StringReader(xmldata));
            xDoc.Transform(xmlReader, args, xmlWriter);
            string result = resultString.ToString();
            result = result.Replace("xmlns:asp=\"remove\"", "");

            return result;
        }

        String CompanyDetails()
        {
            CompanyDTO comp = new Company(new LoggedInUser().DefaultCompanyId).GetDetails();

            EmailParameters ep = new EmailParameters();
            ep.Company = comp.Name;
            ep.CompanyCity = comp.City;
            ep.CompanyEmail = comp.Email;
            ep.CompanyGSTNo = comp.GSTNo;
            ep.CompanyPhone1 = comp.Phone1;
            ep.CompanyPhone2 = comp.Phone2;
            ep.CompanyAddress = comp.Address1 + " " + comp.Address2;
            ep.CompanyReportHeader = comp.ReportHeader;
            return ep.GetXML();
        }

        public string MergeFiles(List<String> fileNamesInTemp, string outPutFile)
        {
            // String tempPath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string tempPath = mapPath + "/temp/";
            // Open the output document
            PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (String fileName in fileNamesInTemp)
            {
                string filePath = tempPath + fileName;


                PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            // Save the document...
            string filename = tempPath + outPutFile;
            outputDocument.Save(filename);
            return outPutFile;
        }
        public string MergeFiles(List<Stream> fileNamesInTemp, string outPutFile)
        {
            // String tempPath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            string tempPath = mapPath + "/temp/";
            // Open the output document
            PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (var str in fileNamesInTemp)
            {
                //string filePath = tempPath + fileName;


                PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(str, PdfDocumentOpenMode.Import);
                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfSharp.Pdf.PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            // Save the document...
            string filename = tempPath + outPutFile;
            outputDocument.Save(filename);
            return outPutFile;
        }
    }
    public struct ReortFileName
    {
        public const string ISSUE_ITEM = "issueItem-rent.xsl";
        public const string RECEIVE_ITEM = "recItem-rent.xsl";
        public const string DUE_BILLS = "dueBills.xsl";
        public const string DEBIT_NOTE = "debitNote.xsl";
        public const string CREDIT_NOTE = "creditNote.xsl";
        public const string CASH_RECEIPT = "cash-receipt.xsl";
        public const string BANK_ENTRY = "bank-entry.xsl";
        public const string FORGOT_PASSWORD_LINK = "forgotpwdlink.xsl";
        public const string NEW_CLIENT_WELCOME_EMAIL = "clientwelcome.xsl";

    }
}
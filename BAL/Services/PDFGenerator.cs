using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Xsl;
using System.Xml;
using BAL.Objects;
using Newtonsoft.Json;
using NReco.PdfGenerator;
using System.Data;
using BAL.DTO;
using PdfSharp.Pdf.IO;
using System.Windows.Shell;
using BAL.Enums;
using System.Windows.Forms;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;

using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline.ctx;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.html;
using Omu.ValueInjecter;

namespace BAL.Services
{
    public class PDFGenerator
    {
        public static string cssPath = "";
        public static string docPath = "";
        public static string reportsPath = "";

        public void GenerateQuotationPdf(DataSet ds, List<object> products, out string dwnFileName, out byte[] fileBytes)
        {
            //var quotationId = strParams[1];
            var billing = new Billing();
            int companyId = Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]);
            // var ds = billing.GetQuotationItems(Convert.ToInt32(quotationId));
            var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);

            var config = new Config();
            var printConfigs = config.GetConfig(companyId, "general", "print");
            var printSignature = true;
            var printQrCode = true;
            var company = new Company(companyId).GetDetails();
            ds.Tables[0].Columns.Add("QrCode");

            if (printConfigs != null && printConfigs.Count() > 0)
            {
                var c = printConfigs.Where(o => o.Key.ToLower() == "signature").FirstOrDefault();
                if (c != null)
                {
                    printSignature = c.Value.Contains("quotations");
                }
                c = printConfigs.Where(o => o.Key.ToLower() == "qrcode").FirstOrDefault();
                if (c != null)
                {
                    printQrCode = c.Value.Contains("quotations");
                }
            }

            dwnFileName = "";
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dwnFileName = Convert.ToString(ds.Tables[0].Rows[0]["QuotationNumber"]) + ".pdf";
                }
            }
            if (!String.IsNullOrEmpty(compLogo))
            {
                ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
            }
            var signaure = Convert.ToString(ds.Tables[0].Rows[0]["Signature"]);

            if (!String.IsNullOrEmpty(signaure) && printSignature)
            {
                ds.Tables[0].Rows[0]["Signature"] = docPath + "/comp/" + signaure;
            }
            if (!String.IsNullOrEmpty(company.QrCode) && printQrCode)
            {
                ds.Tables[0].Rows[0]["QrCode"] = docPath + "/comp/" + company.QrCode;
            }

            var configs = config.GetConfig(companyId, "quotation", "quotation");

            List<object> productsToAdd = new List<object>();
            if (configs != null)
            {
                var attRateSheet = configs.Where(o => o.Key.ToLower() == "attachratesheet").FirstOrDefault();
                if (attRateSheet != null && (attRateSheet.Value == "1" || Convert.ToBoolean(attRateSheet.Value) == true))
                {
                    productsToAdd = products;
                }

            }
            var rowsToSpan = 6;
            rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge1"]) > 0 ? 1 : 0;
            rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge2"]) > 0 ? 1 : 0;
            rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge3"]) > 0 ? 1 : 0;
            rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge4"]) > 0 ? 1 : 0;
            rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["charge5"]) > 0 ? 1 : 0;
            rowsToSpan += Convert.ToDecimal(ds.Tables[0].Rows[0]["DiscountAmount"]) > 0 ? 1 : 0;


            var d = new { d = new { data = ds, items = productsToAdd, rowsToSpan = rowsToSpan } };
            string jsonText = JsonConvert.SerializeObject(d);
            // To convert JSON text contained in string json into an XML node
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);


            //var bgNode = doc.CreateNode(XmlNodeType.Element, "bgImage", "");
            //bgNode.InnerText = docPath + "orange-curve.png";
            //doc.FirstChild.AppendChild(bgNode);

            var configNode = doc.CreateNode(XmlNodeType.Element, "Config", "");

            var configData = config.GetBillingConfig(companyId);
            string configJson = JsonConvert.SerializeObject(new { c = new { config = configData } });
            configNode.InnerXml = JsonConvert.DeserializeXmlNode(configJson).InnerXml;
            doc.FirstChild.AppendChild(configNode);

            string xml = doc.InnerXml;
            var templateService = new TemplateService();
            var template = Task.Run(async () => await templateService.GetDefaultPdfTemplate(Convert.ToInt32(ds.Tables[0].Rows[0]["CompanyId"]), TemplateGroups.QUOTATIONS, ConfigCategoryNames.QUOTATIONS, ConfigSubCategoryNames.TEMPLATES)).Result;

            string fileName = "quotation.xslt";
            string customCss = "";
            bool applyPrintConfig = false;
            if (template != null)
            {
                fileName = template.FileName;
                customCss = template.Style;
                applyPrintConfig = template.ApplyPrintConfig;
                // throw new Exception("Report template could not found.");
            }

            fileName = reportsPath + @"\" + fileName;

            var html = RenderXml(xml, fileName, customCss);
            var strHtml = html.ToHtmlString();
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            if (applyPrintConfig)
            {
                con.CustomWkHtmlArgs += " --header-spacing 2"; // Adjust spacing
                                                               // Or try these specific arguments:
                con.CustomWkHtmlArgs = "--print-media-type  --javascript-delay 500 --enable-smart-shrinking --dpi 300";
                con.Margins.Top = 5;
                con.Margins.Bottom = 5;

            }
            //con.Margins.Top = 0;
            //con.Margins.Left = 0;
            //con.Margins.Bottom = 0;
            //con.Margins.Right = 0;

            //var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            //htmlDoc.LoadHtml(strHtml);
            //HtmlNode divByXPath = htmlDoc.DocumentNode.SelectSingleNode("//header");
            //if (divByXPath != null)
            //{
            //    con.PageHeaderHtml = divByXPath.InnerHtml;

            //}
            fileBytes = con.GeneratePdf(strHtml);
            //fileBytes = createPdf(strHtml);
        }
        public void GenerateQuotationPdf(int quotationId, out string dwnFileName, out byte[] fileBytes)
        {
            //var quotationId = strParams[1];
            var billing = new Billing();

            var ds = billing.GetQuotationItems(Convert.ToInt32(quotationId));
            var compLogo = Convert.ToString(ds.Tables[0].Rows[0]["CompanyLogo"]);
            dwnFileName = "";
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dwnFileName = Convert.ToString(ds.Tables[0].Rows[0]["QuotationNumber"]) + ".pdf";
                }
            }
            if (!String.IsNullOrEmpty(compLogo))
            {
                ds.Tables[0].Rows[0]["CompanyLogo"] = docPath + "/comp/" + compLogo;
            }
            var d = new { data = ds };
            string jsonText = JsonConvert.SerializeObject(d);
            // To convert JSON text contained in string json into an XML node
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonText);
            string xml = doc.InnerXml;
            string fileName = reportsPath + @"\quotation.xslt";

            var html = RenderXml(xml, fileName);
            var strHtml = html.ToHtmlString();
            HtmlToPdfConverter con = new HtmlToPdfConverter();
            fileBytes = con.GeneratePdf(strHtml);
        }

        public HtmlString RenderXml(string xml, string xsltPath, string customCSS = "")
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
                html = LoadCss(html, customCSS);
                // Generate HTML string from StringWriter  
                HtmlString htmlString = new HtmlString(html);

                return htmlString;
            }
        }
        string LoadCss(string html, string customCSS = "")
        {

            //var cssPath = cssPath + @"\print.css";
            var printCss = System.IO.File.ReadAllText(cssPath + @"\print.css");
            printCss = "<style>" + printCss + "</style>";
            html = html.Replace(" #pdf", printCss);
            html = html.Replace(" #preview", "");
            if (customCSS != null)
            {
                customCSS = "<style>" + customCSS + "</style>";
            }
            html = html.Replace(" #customCSS", customCSS);
            return html;
        }
        public void MergePDF(List<byte[]> fileNamesInTemp, MemoryStream output)
        {
            // String tempPath = System.Web.HttpContext.Current.Server.MapPath("~/temp/");
            // Open the output document
            PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (var fileName in fileNamesInTemp)
            {
                var ms = new MemoryStream(fileName);

                // PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                PdfSharp.Pdf.PdfDocument inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfDocumentOpenMode.Import);

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
            //  string filename = tempPath + outPutFile;
            // outputDocument.Save(filename);
            outputDocument.Save(output);
            //   return outPutFile;
        }


        public byte[] createPdf(String HTML, bool landScape = false)
        {
            StringReader sr = new StringReader(HTML);
            Document pdfDoc = new Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 0f);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();
                String cssFilePath = cssPath + "/print.css";

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
                iTextSharp.tool.xml.pipeline.end.PdfWriterPipeline pdf = new iTextSharp.tool.xml.pipeline.end.PdfWriterPipeline(pdfDoc, writer);
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
                p.Parse(msHTML);
                pdfDoc.Close();


                memoryStream.Close();
                byte[] bytes = memoryStream.ToArray();
                return bytes;
            }
        }
    }

}

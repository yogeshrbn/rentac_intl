using BAL.Objects;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Xml.Xsl;
using System.Xml;

namespace ReportViewer.Controllers
{
    public class BaseWebApiController : ApiController
    {
        string LoadCss(string html)
        {


            var cssPath = HttpContext.Current.Server.MapPath("~/Content") + @"\print.css";
            var printCss = System.IO.File.ReadAllText(cssPath);
            printCss = "<style>" + printCss + "</style>";
            html = html.Replace(" #pdf", printCss);
            html = html.Replace(" #preview", "");

            return html;
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
    }
}
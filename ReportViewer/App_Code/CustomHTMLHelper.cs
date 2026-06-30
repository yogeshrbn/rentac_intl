using BAL.Objects;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;

namespace ReportViewer
{
    public static class CustomHTMLHelper
    {
        /// <summary>  
        /// Applies an XSL transformation to an XML document  
        /// </summary>  
        /// <param name="helper"></param>  
        /// <param name="xml"></param>  
        /// <param name="xsltPath"></param>  
        /// <returns></returns>  
        public static HtmlString RenderXml(this HtmlHelper helper, string xml, string xsltPath, string cssPath = null)
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
                var strHTML = writer.ToString();
                if (!String.IsNullOrEmpty(cssPath) && File.Exists(cssPath))
                {
                    var printCss = System.IO.File.ReadAllText(cssPath);
                    printCss = "<style>" + printCss + "</style>";
                    strHTML = strHTML.Replace(" #preview", printCss);
                    strHTML = strHTML.Replace(" #pdf", "");
                }
                // Generate HTML string from StringWriter  
                HtmlString htmlString = new HtmlString(strHTML);
                return htmlString;
            }
        }


        public static string RemoveSecondOccurrence(string html, string elementName)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find all elements with the specified tag name
            var elements = doc.DocumentNode.SelectNodes($"//{elementName}");

            if (elements == null || elements.Count < 2)
            {
                Console.WriteLine($"Found {elements?.Count ?? 0} occurrences of '{elementName}'. Need at least 2.");
                return html;
            }

            // Get the second occurrence (index 1)
            var secondElement = elements[1];

            // Remove the element
            secondElement.Remove();

            return doc.DocumentNode.OuterHtml;
        }

        // Alternative method using XPath with specific criteria
        public static string RemoveSecondOccurrenceWithClass(string html, string elementName, string className)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find elements with specific class
            var elements = doc.DocumentNode.SelectNodes($"//{elementName}[contains(@class, '{className}')]");

            if (elements == null || elements.Count < 2)
            {
                Console.WriteLine($"Found {elements?.Count ?? 0} occurrences. Need at least 2.");
                return html;
            }

            // Remove the second occurrence
            elements[1].Remove();

            return doc.DocumentNode.OuterHtml;
        }
    }
}
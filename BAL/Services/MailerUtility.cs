using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Xsl;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
namespace BAL.Services
{
    public class MailerUtility
    {
        public MailerUtility()
        {

        }

        public static string GetEmailURL(string fileName)
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/mailtemplates/" + fileName);
        }

        public static string GetReportTemplatURL(string fileName)
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/ReportTemplates/" + fileName);
        }

        public static string GetMailBody(string XsltUrl, string xmldata)
        {
            XslCompiledTransform xDoc = new XslCompiledTransform();
            xDoc.Load(XsltUrl, XsltSettings.TrustedXslt, new XmlUrlResolver());
            StringBuilder resultString = new StringBuilder();
            XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(resultString));
            XmlTextReader xmlReader = new XmlTextReader(new StringReader(xmldata));
            xDoc.Transform(xmlReader, xmlWriter);
            string result = resultString.ToString();
            result = result.Replace("xmlns:asp=\"remove\"", "");

            return result;
        }


    }
    public  struct EmailTemplate
    {
        public const string ISSUE_ITEM = "issueItem.xsl";
        public const string RECEIVE_ITEM = "recItem.xsl";
        public const string DEBIT_NOTE = "debitNote.xsl";
        public const string CREDIT_NOTE = "creditNote.xsl";
        public const string BILL_REMINDER = "bill-reminder.xsl";
        public const string CASH_RECEIPT = "cash-receipt.xsl";
        public const string BANK_ENTRY = "bank-entry.xsl";
        public const string USER_CREATION = "user-creation.xsl";

    }

    public struct ReportTemplate
    {
        public const string CLIENT_WISE_ITEM_BALANCE = "clientwiseItemBalance.xsl";
        public const string ITEM_WISE_CLIENT_BALANCE = "itemwiseClientBalance.xsl";
        public const string CASHBOOK = "cashbook.xsl";
        public const string STOCK_IN_HAND = "stockInhand.xsl";
        public const string STOCK_SUMMARY = "stockSummary.xsl";


    }
}
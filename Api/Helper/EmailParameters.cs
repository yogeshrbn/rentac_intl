using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
namespace FarmaAPI.Helper
{
    /// <summary>
    /// Summary description for EmailParameters
    /// </summary>
    public class EmailParameters
    {
        public EmailParameters()
        {

        }

        public string PartyName
        {
            get;
            set;
        }

        public string ChallanNumber
        {
            get;
            set;
        }
        public string Company
        {
            get;
            set;
        }
        public string CompanyGSTNo { get; set; }
        public string ClientGSTNo { get; set; }
        public string CompanyPhone1 { get; set; }
        public string CompanyPhone2 { get; set; }
        public string BillStateGSTCode { get; set; }
        public string SiteStateGSTCode { get; set; }
        public string ReportHeader { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyLogo { get; set; }

        public string CompanyReportHeader { get; set; }

        public string GRN { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Driver { get; set; }
        public string Vehicle { get; set; }
        public string InvoiceNumber { get; set; }
        public double InvoiceAmount { get; set; }
        public string InvoiceDate { get; set; }
        public double Amount { get; set; }
        public string CreationDate { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ReceiptNumber { get; set; }
        public string Narration { get; set; }
        public string ClientBIllAddress { get; set; }
        public string ClientShippingAddress { get; set; }
        

        public string GetXML()
        {
            string xmlString = "<Data>";
            EmailParameters EParam = this;

            if (EParam.PartyName != null && EParam.PartyName != string.Empty && EParam.PartyName != "")
            {
                xmlString += "<PartyName>" + EParam.PartyName + "</PartyName>";
            }
            if (EParam.ChallanNumber != null && EParam.ChallanNumber != string.Empty && EParam.ChallanNumber != "")
            {
                xmlString += "<ChallanNumber>" + EParam.ChallanNumber + "</ChallanNumber>";
            }
            if (EParam.GRN != null && EParam.GRN != string.Empty && EParam.GRN != "")
            {
                xmlString += "<GRN>" + EParam.GRN + "</GRN>";
            }
            if (EParam.Sender != null && EParam.Sender != string.Empty && EParam.Sender != "")
            {
                xmlString += "<Sender>" + EParam.Sender + "</Sender>";
            }
            if (EParam.Receiver != null && EParam.Receiver != string.Empty && EParam.Receiver != "")
            {
                xmlString += "<Receiver>" + EParam.Receiver + "</Receiver>";
            }
            xmlString += "<Company>" + EParam.Company + "</Company>";
            xmlString += "<CompanyPhone1>" + EParam.CompanyPhone1 + "</CompanyPhone1>";


            xmlString += "<CompanyPhone2>" + EParam.CompanyPhone2 + "</CompanyPhone2>";

            xmlString += "<CompanyCity>" + EParam.CompanyCity + "</CompanyCity>";
            xmlString += "<CompanyAddress>" + EParam.CompanyAddress + "</CompanyAddress>";
            xmlString += "<ReportHeader>" + EParam.ReportHeader + "</ReportHeader>";
            xmlString += "<CompanyGSTNo>" + EParam.CompanyGSTNo + "</CompanyGSTNo>";
            xmlString += "<Driver>" + EParam.Driver + "</Driver>";
            xmlString += "<Vehicle>" + EParam.Vehicle + "</Vehicle>";
            xmlString += "<InvoiceNumber>" + EParam.InvoiceNumber + "</InvoiceNumber>";
            xmlString += "<InvoiceAmount>" + EParam.InvoiceAmount + "</InvoiceAmount>";
            xmlString += "<From>" + EParam.From + "</From>";
            xmlString += "<To>" + EParam.To + "</To>";
            xmlString += "<Amount>" + EParam.Amount + "</Amount>";
            xmlString += "<ReceiptNumber>" + EParam.ReceiptNumber + "</ReceiptNumber>";
            xmlString += "<CreationDate>" + EParam.CreationDate + "</CreationDate>";
            xmlString += "<Narration>" + EParam.Narration + "</Narration>";
            xmlString += "<ClientBIllAddress>" + EParam.ClientBIllAddress + "</ClientBIllAddress>";
            xmlString += "<ClientShippingAddress>" + EParam.ClientShippingAddress + "</ClientShippingAddress>";
            xmlString += "<InvoiceDate>" + EParam.InvoiceDate + "</InvoiceDate>";
            xmlString += "<ForgotPasswordVerifyLink>" + EParam.ForgotPasswordVerifyLink + "</ForgotPasswordVerifyLink>";
            xmlString += "<CompanyLogo>" + EParam.CompanyLogo + "</CompanyLogo>";

            xmlString += "</Data>";


            return xmlString;

        }

        public string Password { get; set; }

        public int MemberId { get; set; }

        public string MailId { get; set; }

        public int ClubId { get; set; }
        public string Sport { get; set; }
        public string ForgotPasswordVerifyLink { get; set; }
    }
}
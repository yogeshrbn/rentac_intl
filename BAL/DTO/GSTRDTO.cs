using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class GSTR1DTO
    {
        public int Vouchers { get; set; }
        public decimal TaxAbleAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public IEnumerable<GSTR1DetailDTO> Details { get; set; }
    }

    public class GSTR1DetailDTO
    {
        /// <summary>       
        /// </summary>
        public string Item { get; set; }

        public string Party { get; set; }
        public string VoucherNumber { get; set; }
        public decimal TaxAbleAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public int PartyStateId { get; set; }
        public short VoucherType { get; set; }
        public int CompanyStateId { get; set; }
        public string BillToGST { get; set; }
        public string PartyGST { get; set; }
    }
    public class HSNSummaryDTO
    {
        /// <summary>       
        /// </summary>
     
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public string HSNCode { get; set; }
        public decimal TaxAbleAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxRate { get; set; }
        public bool B2B { get; set; }
        public bool B2C { get; set; }


    }
    public class GSTR1SummaryDTO
    {
        public GSTR1DTO b2b { get; set; }
        public GSTR1DTO b2c_largeInvoice { get; set; }
        public GSTR1DTO b2c_smallInvoice { get; set; }
        public GSTR1DTO nillRated { get; set; }
        public GSTR1DTO notesRegistered { get; set; }
        public GSTR1DTO notesUnRegistered { get; set; }
        public GSTR1DTO exportInvoices { get; set; }
        public GSTR1DTO advanceReceived { get; set; }
        public GSTR1DTO advanceAdjustment { get; set; }
        public GSTR1DTO advanceRefund { get; set; }
        public GSTR1DTO total { get; set; }

        public IEnumerable<HSNSummaryDTO> hsnSummary { get; set; }

    }
    public class GSTR3BDTO
    {
    }

    public class GSTTaxReport


    {
        public int InvoiceId { get; set; }
        public string GSTIN { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public string Client { get; set; }
        public string BillingAddress { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public decimal TaxableValue { get; set; }
        public decimal CessAmount { get; set; }

    }
}

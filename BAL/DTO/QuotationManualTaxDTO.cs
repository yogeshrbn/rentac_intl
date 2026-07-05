using System;

namespace BAL.DTO
{
    /// <summary>
    /// Manual per-tax rate selection on quotation header (stored in QuotationManualTax).
    /// </summary>
    public class QuotationManualTaxDTO
    {
        public Guid Id { get; set; }
        public int QuotationId { get; set; }
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public string TaxCode { get; set; }
        public decimal Rate { get; set; }
        public string RateType { get; set; }
    }
}

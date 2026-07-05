using System;

namespace BAL.DTO
{
    /// <summary>
    /// Tax applied on a single quotation line item (stored in QuotationTax).
    /// </summary>
    public class QuotationTaxDTO
    {
        public Guid Id { get; set; }
        public int QuotationItemId { get; set; }
        public int QuotationId { get; set; }
        public int ProductId { get; set; }
        public int TaxCategoryId { get; set; }
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public string TaxCode { get; set; }
        public decimal Rate { get; set; }
        public string RateType { get; set; }
        public decimal Amount { get; set; }
    }
}

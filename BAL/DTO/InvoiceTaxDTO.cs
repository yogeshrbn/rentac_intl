namespace BAL.DTO
{
    /// <summary>
    /// Per-line tax on a sale invoice (stored in InvoiceTax). Extends <see cref="TaxDTO"/>.
    /// </summary>
    public class InvoiceTaxDTO : TaxDTO
    {
        public long InvoiceTaxId { get; set; }
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int TaxCategoryId { get; set; }
        public string TaxCode { get; set; }
        public string RateType { get; set; }
    }
}

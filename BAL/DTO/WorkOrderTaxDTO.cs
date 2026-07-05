using System;

namespace BAL.DTO
{
    /// <summary>
    /// Tax applied on a single issue-challan line item (stored in WorkOrderTax).
    /// </summary>
    public class WorkOrderTaxDTO
    {
        public Guid Id { get; set; }
        public int WorkOrderItemId { get; set; }
        public int SiteId { get; set; }
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

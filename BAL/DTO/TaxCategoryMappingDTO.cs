using System;

namespace BAL.DTO
{
    public class TaxCategoryMappingDTO
    {
        public long Id { get; set; }
        public int TaxCategoryId { get; set; }
        public int TaxId { get; set; }
        public bool IsDefault { get; set; }
        public string TaxName { get; set; }
        public string TaxCode { get; set; }
        public decimal Rate { get; set; }
        public string RateType { get; set; }
    }
}

using System;

namespace BAL.DTO
{
    public class TaxMasterDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public string RateType { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompound { get; set; }
        public bool IsDefault { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string ApplicableTo { get; set; }
        public string CustomerType { get; set; }
        public string Location { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] RowVersion { get; set; }
    }
}

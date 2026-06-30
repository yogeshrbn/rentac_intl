using BAL.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Models/Production.cs
namespace BAL.Models
{
    public class Production
    {
        public long ProductionId { get; set; }
        public long ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public long ClientId { get; set; }
        public string SaleOrderNo { get; set; }
        public string Description { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string GuId { get; set; } = string.Empty;


      
        public List<ProductionBOMDto> BOM { get; set; }

        public List<ProductionOperationDto> Operations { get; set; }
    }
}

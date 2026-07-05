using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class TaxCategoryDTO
    {
        public int TaxCategoryId { get; set; }
        public string TaxName { get; set; }
        public double IGST { get; set; }
        public double SGST { get; set; }
        public double CGST { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public List<int> TaxIds { get; set; }
        public List<TaxCategoryMappingDTO> Mappings { get; set; }
    }
}

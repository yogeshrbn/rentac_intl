using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class BaseDTO
    {
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int CompanyId { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string GuId { get; set; }
        public int FinYearId { get; set; }
        
    }
    
}

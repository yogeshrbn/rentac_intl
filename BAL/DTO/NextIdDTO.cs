using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
namespace BAL.DTO
{
    public class NextIdDTO
    {
        public NextIDTables Table { get; set; }
        public string NextId { get; set; }
        public string FinYear { get; set; }
        public int CompanyId { get; set; }
        public string Prefix { get; set; }
        public string InitalValue { get; set; }

    }
}

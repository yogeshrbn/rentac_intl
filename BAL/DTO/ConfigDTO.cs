using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;

namespace BAL.DTO
{
    public class ConfigDTO
    {
        public String Category { get; set; }
        public String SubCategory { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
        public int CompanyId { get; set; }
    }

}

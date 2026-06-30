using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class FinYearDTO
    {
        public int FinYearId { get; set; }
        public string FinYear { get; set; }
        public bool Selected { get; set; } 
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
    }
}

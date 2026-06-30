using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class RentRateDTO
    {
        public int ProductId { get; set; }
        public long Rate { get; set; }
        public double MRP { get; set; }
        public double LossRate { get; set; }
        public string EffectiveDate { get; set; }
        public bool Active { get; set; }
        public int UOM { get; set; }
        public String UOMName { get; set; }
        public int RentRateId { get; set; }

    }
}

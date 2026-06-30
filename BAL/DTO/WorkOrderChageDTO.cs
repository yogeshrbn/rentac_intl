using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class WorkOrderChageDTO : OtherChargeDTO
    {
        public int WorkOrderId { get; set; }
        public double Amount { get; set; }
        public int WorkOrderChargeId { get; set; }
    }
}

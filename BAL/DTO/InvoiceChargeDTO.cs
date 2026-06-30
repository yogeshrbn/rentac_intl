using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class InvoiceChargeDTO : OtherChargeDTO
    {
        public int InvoiceId { get; set; }
        public int ChallanId { get; set; }
        public byte ChallanType { get; set; }
        public double Amount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class InvoiceIRNDTO
    {
        public int InvoiceId { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string AckNo { get; set; }
        public DateTime AckDate { get; set; }

        public string IRN { get; set; }
        public string SingedInvoice { get; set; }
        public string SingedQrCode { get; set; }
        public string IRNStatus { get; set; }

    }
}

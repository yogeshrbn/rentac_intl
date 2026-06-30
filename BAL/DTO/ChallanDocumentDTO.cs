using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ChallanDocumentDTO : DocumentDTO
    {
        public int ChallanDocumentId { get; set; }
        public int WorkOrderId { get; set; }
    }
}

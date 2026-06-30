using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ChallanItemDTO : ProductDTO
    {


        public double PurchaseRate { get; set; }
        public double Quantity { get; set; }
        public string BatchNumber { get; set; }
        public double SubTotal { get; set; }
        public DateTime Expiry { get; set; }
        public int ChallanItemId { get; set; }
        public ChallanDTO Challan { get; set; }
        public ChallanItemDTO() { }
      
    }
}

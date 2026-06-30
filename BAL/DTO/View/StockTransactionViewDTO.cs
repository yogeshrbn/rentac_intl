using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO.View
{
    public class StockTransactionViewDTO
    {

        public int ProductId { get; set; }
        public string Product { get; set; }
        public int StockTransactionId { get; set; }
        public double Quantity { get; set; }
        public int FinYear { get; set; }
        public DateTime PostingDate { get; set; }
        public short PostingType { get; set; }
        public string Remarks { get; set; }
        public int VoucherId { get; set; }
        public byte Deleted { get; set; }

    }
    public class StockTransactionHeaderViewDTO
    {
        public DateTime PostingDate { get; set; }
        public short PostingType { get; set; }
        public string Remarks { get; set; }
        public string VoucherId { get; set; }

        public int StockTransactionHeaderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PostingTypeName { get; set; }
        public string CreatedByName { get; set; }
        public List<StockTransactionViewDTO> Items { get; set; }

    }

    public class StockAdjustmentListFilterDTO
    {
        public string From { get; set; }
        public string To { get; set; }
        public int StockTransactionHeaderId { get; set; }
    }
}

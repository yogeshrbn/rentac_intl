using BAL.DTO.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class StockTransactionDTO
    {
        public int ProductId { get; set; }
        public string Product { get; set; }
        public int ProductSizeId { get; set; }
        public int StockTransactionId { get; set; }
        public double Quantity { get; set; }
        public int FinYear { get; set; }
        public DateTime PostingDate { get; set; }
        public short PostingType { get; set; }
        public string Remarks { get; set; }
        public string VoucherId { get; set; }
        public int PostedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CompanyId { get; set; }
        public byte Deleted { get; set; }
    }

    public class StockTransactionHeaderDTO
    {
        public DateTime PostingDate { get; set; }
        public short PostingType { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        public int RbnClientId { get; set; }
        public string VoucherId { get; set; }
        public int CompanyId { get; set; }
        public string GuId { get; set; }
        public int FinYear { get; set; }
        public string PostingTypeName { get; set; }
        public string CreatedByName { get; set; }
        public int StockTransactionHeaderId { get; set; }
        public List<StockTransactionDTO> Items { get; set; }

    }
}

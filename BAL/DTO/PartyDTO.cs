using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class PartyDTO
    {

    }
    public class PartyStockBalanceDTO
    {
        public int CompanyId { get; set; }

        public string Company { get; set; }
        public string Product { get; set; }

        public decimal IssuedQty { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal ClosingBalance { get; set; }


    }
}

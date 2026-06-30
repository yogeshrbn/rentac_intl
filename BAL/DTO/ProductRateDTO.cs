using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ProductRateDTO
    {
        public bool ApplyUnit2Rate { get; set; }
        public double RentRate { get; set; }
        public double UnitSizeRate { get; set; }
        public double LossRate { get; set; }
        public double SaleRate { get; set; }
        public double PurchaseRate { get; set; }

        public double DamageRate { get; set; }
        public double OpeningBalance { get; set; }
        public double ProductId { get; set; }
        public double Size { get; set; }

        public String Product { get; set; }
        public String ProductCode { get; set; }
        public Int32 ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public short Unit { get; set; }
        public string UnitName { get; set; }

        public string Client { get; set; }
        public int ProductSizeId { get; set; }
        public int ProductRateId { get; set; }

    }

    public class CopyRatesDTO
    {
        public int CopyFromSiteId { get; set; }
        public List<ClientSiteDTO> CopyTo { get; set; }
    }

}

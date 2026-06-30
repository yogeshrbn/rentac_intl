using BAL.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class PackageDTO
    {
        public int PackageId { get; set; }
        public decimal Amount { get; set; }

        public string Name { get; set; }
        public int Challans { get; set; }
        public int Users { get; set; }
        public int Companies { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public decimal GST { get; set; }
        public decimal Total { get; set; }
    }

    public class ClientPackageDTO : PackageDTO
    {
        public int RBNClientID { get; set; }
        public int CompanyId { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime ValidTill { get; set; }
        public string MonthlyYearly { get; set; }
        public int PurchasedBy { get; set; }
        public int RemainingDays { get; set; }
        public bool IsDemo { get; set; }
        public string payment_id { get; set; }

    }
}

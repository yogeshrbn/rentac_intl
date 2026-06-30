using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class SubscriptionDTO
    {
    }
    public class MonthlyChallansDTO
    {
        public int Challans { get; set; }
    }

    public class SubscriptionRatesDTO
    {
        public decimal Rate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class RentacBillingFilterDTO
    {
        public DateTime FromDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
    }
    public class RentacBillingDTO
    {
        public int BillingId { get; set; }
        public int ClientId { get; set; }
        public int CompanyId { get; set; }
        public int PackageId { get; set; }
        public string LiteItemDescription { get; set; }
        public string payment_id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Paid by the client including GST
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Amount excluding GST
        /// </summary>
        public decimal BaseAmount { get; set; }

        public decimal IGSTRate { get; set; }
        public decimal IGST { get; set; }
        public decimal SGSTRate { get; set; }
        public decimal SGST { get; set; }
        public decimal CGSTRate { get; set; }
        public decimal CGST { get; set; }
        public decimal Tax { get; set; }

        public DateTime DueDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public int BillingPinCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentRefId { get; set; }
        public DateTime? PaymentDate { get; set; } 
        public int Paid { get; set; }
        public int TotalItems { get; set; }
        public string Remarks { get; set; }
        public decimal Balance { get; set; }
        public List<RentacBillingDetailsDTO> Details { get; set; }
    }

    public class RentacBillingDetailsDTO
    {
        public int BillingId { get; set; }
        public int Challans { get; set; }
        public int Bills { get; set; }
        public DateTime CreationDate { get; set; }
        public int ItemReceipts { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string ItemType { get; set; }
        public string Range { get; set; }

        public string BillingAddress { get; set; }

    }
}

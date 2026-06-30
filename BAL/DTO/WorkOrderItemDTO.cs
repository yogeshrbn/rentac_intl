using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class WorkOrderItemDTO : ProductDTO
    {

        public int WorkOrderId { get; set; }
       // public double PurchaseRate { get; set; }
        public double PurchaseQty { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime RentStartDate { get; set; }
        public DateTime RecoveryDate { get; set; }

        public double SentQty { get; set; }
     //   public double Rate { get; set; }
        public double Quantity { get; set; }
        public string BatchNumber { get; set; }
        public double SubTotal { get; set; }
        public DateTime Expiry { get; set; }
        public int WorkOrderItemId { get; set; }
       // public int WorkOrderId { get; set; }
        public WorkOrderDTO WorkOrder { get; set; }
        public string Number { get; set; }
        public string JobNumber { get; set; }
        public string ChallanNumber { get; set; }
        public short ChallanType { get; set; }
        public DateTime ChallanDate { get; set; }
        public string State { get; set; }
        public string WorkOrderDate { get; set; }
        public double Freight { get; set; }
        public double ApproxValue { get; set; }
        public double FreightTax { get; set; }
        public string Driver { get; set; }
        public string Vehicle { get; set; }

        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public int LedgerSiteId { get; set; }
        public int TransporterId { get; set; }
        public int TeamId { get; set; }

        public int SiteId { get; set; }
        public string StartDate { get; set; }
        public string Remarks { get; set; }
        public BAL.Enums.SiteItemType SiteItemType { get; set; }
        public WorkOrderItemDTO() { }
        public double PrevSent { get; set; }
        public double PrevReceived { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int InvoiceId { get; set; }
        public double ExcessQty { get; set; }
        public double ShortQty { get; set; }
        public double ReceivingQty { get; set; }
        public bool ConsiderFullReceive { get; set; }
        public double Scrap { get; set; }

        public double Breakage { get; set; }
        public double BreakageRate { get; set; }
        /// <summary>Description of damaged part / component for return challan lines.</summary>
        public string DamageComponent { get; set; }
        public double BreakageAmount { get; set; }
        public string WorkOrderNumber { get; set; }
        public string Client { get; set; }
        public string PartyCode { get; set; }

        public int ProductSizeId { get; set; }
      //  public string Size { get; set; }
        public int GRNItemId { get; set; }
        public int GRNId { get; set; }

        public int LedgerId { get; set; }
        public string Site { get; set; }
        public bool ChargeReturnedDate { get; set; }
        public int GroupItemId { get; set; }
        public string CompanyName { get; set; }

        public byte Deleted { get; set; }

        public double IGSTAmount { get; set; }
        public double SGSTAmount { get; set; }
        public double CGSTAmount { get; set; }

        public double IGSTRate { get; set; }
        public double SGSTRate { get; set; }
        public double CGSTRate { get; set; }

        public string EwayBillNo { get; set; }
        public bool Dispatched { get; set; }
        public DateTime DispatchedDate { get; set; }

        public string LRNumber { get; set; }
        public string CRNumber { get; set; }
        public string GRNumber { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class GRNDTO : MasterDTO
    {
        public int GRNId { get; set; }
        public string JobNumber { get; set; }
        public int ProductId { get; set; }
        public int ProductSizeId { get; set; }
        public string Item { get; set; }
        public string HSNCode { get; set; }

        public DateTime ReceivingDate { get; set; }
        public DateTime RentStopDate { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Scrap { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }

        public double ShortQty { get; set; }
        public double ExcessQty { get; set; }
        public double ReceivingQty { get; set; }
        public string GRN { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public List<WorkOrderItemDTO> Items { get; set; }
        /// <summary>
        /// only used to get the grn details by grnId
        /// </summary>
        public List<GRNDTO> GrnItems { get; set; }

        public double PrevReceived { get; set; }
        public string Receiver { get; set; }
        public string Sender { get; set; }
        public string ShipFrom { get; set; }
        public int LedgerId { get; set; }
        public string Client { get; set; }
        public double Breakage { get; set; }
        public double BreakageRate { get; set; }
        /// <summary>Damaged component text when loading GRN line items.</summary>
        public string DamageComponent { get; set; }
        public int GRNItemId { get; set; }
        public int GroupItemId { get; set; }
        public bool ConsiderFullReceive { get; set; }
        public string Remarks { get; set; }

        public bool ChargeReturnedDate { get; set; }
        public int WorkOrderId { get; set; }
        public int LedgerSiteId { get; set; }
        public byte Deleted { get; set; } = 0;
        public int TransactionId { get; set; }
        public string CompanyName { get; set; }
        public string Unit { get; set; }

        public short ChallanType { get; set; }
        /// <summary>1 = Rent, 2 = Hire (e.g. material adjustment).</summary>
        public byte AdjType { get; set; }
        public int JobCardId { get; set; }
        public string Driver { get; set; }
        public string VehicleNo { get; set; }
        public double TotalOtherCharges { get; set; }
        public double Freight { get; set; }
        public List<GRNChageDTO> OtherCharges { get; set; }
        public string Tnc { get; set; }
        public int WarehouseId { get; set; }
        public string EwayBillNo { get; set; }
        public decimal ApproximateValue { get; set; }
        public decimal Weight { get; set; }
        public string LRNumber { get; set; }
        public string CRNumber { get; set; }
        public string GRNumber { get; set; }
        public string PONumber { get; set; }
        public bool InwardConfirm { get; set; }
        public DateTime InwardConfirmDate { get; set; }

        public int ProjectOwnerId { get; set; }
        public string ProjectOwnerName { get; set; }
        public string ProjectOwnerPhone { get; set; }

    }
    public class GRNChageDTO : OtherChargeDTO
    {
        public int GRNId { get; set; }
        public int CompanyId { get; set; }

        public double Amount { get; set; }
        public int GRNChargeId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
namespace BAL.DTO
{
    public class WorkOrderDTO : MasterDTO
    {
        int _workorderId;
        public int WorkOrderId { get { return _workorderId; } set { _workorderId = value; } }
        public int GrnId { get; set; }
        public LedgerDTO ClientDetails { get; set; }
        public string IssueTime { get; set; }
        public string WorkOrderNumber { get; set; }
        public DateTime WorkOrderDate { get; set; }
        public DateTime RentStartDate { get; set; }
        public double SubTotal { get; set; }
        public double TotalTax { get; set; }
        //This will be printed on top of the challan (1- Original Copy, 2- Duplicate Copy, 3- Transport Copy)
        public int ChallanHeaderType { get; set; }
        public double DiscountRate { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        //  public ChallanType Type { get; set; }
        public List<WorkOrderItemDTO> Items { get; set; }
        public List<TaxDTO> ApplicableTaxes { get; set; }
        public List<WorkOrderChageDTO> OtherCharges { get; set; }
        public string Number { get; set; }
        public String Client { get; set; }
        // public String Company { get; set; }
        public Int16 StoreId { get; set; }
        public double ClientAmount { get; set; }
        public string SitePic { get; set; }
        //public int CreatedBy { get; set; }
        //public int CompanyId { get; set; }
        public int LedgerId { get; set; }
        public string Site { get; set; }
        public string Vehicle { get; set; }
        public string Driver { get; set; }
        public string JobNumber { get; set; }
        public bool Closed { get; set; }
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int SiteId { get; set; }
        public string Details { get; set; }
        public string InvoiceNumber { get; set; }
        public double TotalReceived { get; set; }
        public double BalanceAmount { get; set; }
        public double ReminderPercentage { get; set; }
        public ChallanType ChallanType { get; set; }
        public int OverDueDays { get; set; }
        public String DueDate { get; set; }
        public int ParentWorkOrderId { get; set; }
        public string State { get; set; }

        public string Code { get; set; } //WorkOrder/GRN/Invoice
        //  public Ledger Ledger { get; set; }
        // public ChallanType ChallanType { get; set; }

        public WorkOrderDTO(int workorderId)
        {
            _workorderId = workorderId;
        }
        public WorkOrderDTO() { }

        List<SiteDTO> _sites = new List<SiteDTO>();
        public List<SiteDTO> Sites { get { return _sites; } }

        public int TransactionId { get; set; }

        public double IGSTAmount { get; set; }
        public double SGSTAmount { get; set; }
        public double CGSTAmount { get; set; }

        public double IGSTRate { get; set; }
        public double SGSTRate { get; set; }
        public double CGSTRate { get; set; }
        public List<WorkOrderOperationDto> Operations { get; set; }

        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BOMId { get; set; }
        public short Quantity { get; set; }
        public int ProductId { get; set; }
        public int LedgerSiteId { get; set; }
        public int TransporterId { get; set; }
        public int TeamId { get; set; }
        public DateTime RecoveryDate { get; set; }

        public short JobsCount { get; set; }
        public int JobCardId { get; set; }

        public string Remarks { get; set; }
        public string Tnc { get; set; }
        /// <summary>SEZ-related description shown on issue challan.</summary>
        public string SezDescription { get; set; }
        /// <summary>Optional ship-from text on issue challan (stored only; not used on print templates).</summary>
        public string ShipFrom { get; set; }
        /// <summary>
        /// Reference number can be PO number or Quotation Number
        /// </summary>
        public string RefNo { get; set; }
        public double Freight { get; set; }
        public double TotalOtherCharges { get; set; }
        public double ApproximateValue { get; set; }
        public double PrevSent { get; set; }
        public int WarehouseId { get; set; }
        public decimal Weight { get; set; }
        public string LRNumber { get; set; }
        public string CRNumber { get; set; }
        public string GRNumber { get; set; }

        public DateTime PODate { get; set; }
        public string PONumber { get; set; }

        /// <summary>1 = Rent, 2 = Hire (e.g. material adjustment).</summary>
        public byte AdjType { get; set; }

        public int ProjectOwnerId { get; set; }
        public string ProjectOwnerName { get; set; }
        public string ProjectOwnerPhone { get; set; }

    }

    public class WorkOrderOperationDto : OperationDto
    {
        public int WorkOrderId { get; set; }
        public short Quantity { get; set; }
    }

    public class JobCardDto : MasterDTO
    {
        public int JobCardId { get; set; }
        public int LedgerSiteId { get; set; }
        public int EmployeeId { get; set; }
        public string Employees { get; set; }
 

        public string Remarks { get; set; }
        public string JobNumber { get; set; }

        public int Quantity { get; set; }
        public int Days { get; set; }
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string ContactName { get; set; }
        public string ContactNo { get; set; }
        public int JobCardTypeKey { get; set; }
        public string JobCardType { get; set; }
        public short TypeId { get; set; }

        public string AreaCovered { get; set; }
        public string UnitAreaRate { get; set; }

        public string ContractArea { get; set; }
        public string Height { get; set; }
        public string PONumber { get; set; }
        public string SiteAddress { get; set; }

        /// <summary>Contract activity label from ops/sales list (e.g. INSTALL). Used only for API payloads.</summary>
        public string Activity { get; set; }
        /// <summary>Optional data URL image when updating install activity status (ops dashboard).</summary>
        public string ActivityImageDocument { get; set; }

    }

    public class ChallanChangePartyDto
    {
        public int WorkOrderId { get; set; }
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public string ChallanNumber { get; set; }


    }
}

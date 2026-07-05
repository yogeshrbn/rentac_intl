using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.Enums;
namespace BAL.DTO
{
    public class SiteDTO : WorkOrderDTO
    {
        //  public int SiteId { get; set; }
        //   public int WorkOrderId { get; set; }
        //  public String JobNumber { get; set; }
        public String ChallanNumber { get; set; }
        //  public String Site { get; set; }
        public String ShaftSize { get; set; }
        public String ShaftHeight { get; set; }
        public String SiteEng { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public String Doc1 { get; set; }
        public String Doc2 { get; set; }
        public String Doc3 { get; set; }
        //   public double SubTotal { get; set; }
        public double TaxAmount { get; set; }
        //  public double Total { get; set; }
        //   public double Freight { get; set; }
        //   public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ParentSiteId { get; set; }
        //   public List<WorkOrderItemDTO> Items { get; set; }
        public List<TaxDTO> Taxes { get; set; }
        public List<WorkOrderTaxDTO> AppliedTaxes { get; set; }
        //   public bool Closed { get; set; }
        public bool PaymentClosed { get; set; }
        //     public string Vehicle { get; set; }
        //    public string Driver { get; set; }
        public double FreightTax { get; set; }
        //    public string State { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        //  public int LedgerSiteId { get; set; }

        public decimal Weight { get; set; }


    }
}

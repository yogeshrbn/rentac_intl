using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class VehicleDTO
    {
        public int VehicleId { get; set; }
        public string RegNumber { get; set; }
        public string ChachisNumber { get; set; }
        public string EngineNumber { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }

    }

    public class VehicleTravelReport
    {
        public string Vehicle { get; set; }
        public string ChallanNumber { get; set; }
        public DateTime WorkOrderDate { get; set; }
        public decimal Freight { get; set; }
        public string PicLocation { get; set; }
        public string DropLocation { get; set; }
        public string Driver { get; set; }

        public string CompanyName { get; set; }
        public string SiteName { get; set; }
 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class WarehouseDTO
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int CreatedBy { get; set; }
        public int RbnClientId { get; set; }
        public byte StatusId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }


        // Navigation properties
        public string CreatedByName { get; set; }
        public string ClientName { get; set; }
        public string StatusName { get; set; }
    }

    
}

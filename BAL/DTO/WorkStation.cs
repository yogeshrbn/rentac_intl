using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class WorkStationDto
    {
        public int WorkStationId { get; set; }
        public int WorkStationTypeId { get; set; }
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string GuId { get; set; }

        public short StatusId { get; set; }
        public string StatusName { get; set; }
        public string WorkStationTypeName { get; set; }

    }
    public class WorkStationTypeDto
    {
        public int WorkStationTypeId { get; set; }
        public int CompanyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string GuId { get; set; }

        public short StatusId { get; set; }
        public string StatusName { get; set; }


    }


    public class OperationDto : MasterDTO
    {
        public int OperationId { get; set;}
        public int ParentOperationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentOperation { get; set; }
        public string Code { get; set; }

    }
}

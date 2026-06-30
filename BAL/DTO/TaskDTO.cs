using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class TaskDTO : MasterDTO
    {
        public int TaskId { get; set; }
        public string Task { get; set; }
        public DateTime DeliveryDate { get; set; }
        /// <summary>
        /// Currently assigned to will be owner
        /// </summary>
        public string Owner { get; set; }
        public int AssignedTo { get; set; }
        public int AssignedBy { get; set; }
        public string Assignee { get; set; }
        public DateTime AssignedOn { get; set; }

    }

    public class TaskFilterDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public short StatusId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}

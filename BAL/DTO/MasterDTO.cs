using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BAL.DTO
{
   public class MasterDTO
    {
       public int UserId { get; set; }
       public int RbnClientId { get; set; }
       public int CreatedBy { get; set; }
       public DateTime CreationDate { get; set; }
       public int FinYearId { get; set; }
       public int FileFormat { get; set; }
       public short ReminderType { get; set; }
       public string Party { get; set; }
       public string Company { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string GuId { get; set; }
        public short StatusId { get; set; }
        public string StatusName { get; set; }
        public string Source { get; set; }


    }
}

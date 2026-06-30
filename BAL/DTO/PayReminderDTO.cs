using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class PayReminderDTO : MasterDTO
    {
        public int SiteId { get; set; }
        public int Days { get; set; }
        public Double Amount { get; set; }
        public int CreatedBy { get; set; }
        public int PayReminderId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
namespace BAL.Objects
{
    public class PayReminder :PayReminderDTO
    {

        public PayReminder(int _siteId)
        {
            this.SiteId = _siteId;
        }

      


    }
}

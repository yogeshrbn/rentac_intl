using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Enums
{
    public enum InvoiceStatus
    {
        Approved = 1,
        Cancelled = 2,
        Draft = 3,
        SendForApprval = 4,
        Settled = 5
    }
}

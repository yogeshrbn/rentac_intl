using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Enums
{
    public enum StockPostingType
    {
        OB = 1,
        STOCK_IN_RENT = 2,
        STOCK_IN_PURCHASE = 3,
        STOCK_OUT_RENT = 4,
        STOCK_OUT_SALE = 5,
        STOCK_OUT_REPAIR_JOB = 6
    }
}

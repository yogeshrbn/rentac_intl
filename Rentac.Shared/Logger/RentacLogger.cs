using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentac.Shared.Logger
{
    public class RentacLogger
    {
        public static readonly ILogger logger = LogManager.GetCurrentClassLogger();
    }
}

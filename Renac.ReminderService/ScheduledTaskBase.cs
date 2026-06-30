using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renac.ReminderService
{
    public abstract class ScheduledTaskBase : IScheduledTask
    {
        public abstract string Name { get; }
        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
        public abstract bool IsOneAmTask { get; }
        public abstract bool IsMinuteTask { get; }
    }
}

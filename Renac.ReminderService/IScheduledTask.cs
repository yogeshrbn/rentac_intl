using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renac.ReminderService
{
    public interface IScheduledTask
    {
        string Name { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
        bool IsOneAmTask { get; }
        bool IsMinuteTask { get; }
    }
}

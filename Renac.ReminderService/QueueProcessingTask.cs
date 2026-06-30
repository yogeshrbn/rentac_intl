using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renac.ReminderService
{
    public class QueueProcessingTask : ScheduledTaskBase
    {
        private readonly ILogger<QueueProcessingTask> _logger;

        public override string Name => "Queue Processing";
        public override bool IsOneAmTask => false;
        public override bool IsMinuteTask => true;

        public QueueProcessingTask(ILogger<QueueProcessingTask> logger)
        {
            _logger = logger;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Processing queue items...");

                // Your queue processing logic here
                // Example: Process messages from a queue
                await Task.Delay(50, cancellationToken);

                _logger.LogDebug("Queue processing completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing queue");
                throw;
            }
        }
    }
}

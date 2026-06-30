using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renac.ReminderService
{
    public class ServiceBusTask : ScheduledTaskBase
    {
        private readonly ILogger<ServiceBusTask> _logger;
        // private readonly ServiceBusProcessor _serviceBusProcessor;

        public override string Name => "Service Bus";
        public override bool IsOneAmTask => false;
        public override bool IsMinuteTask => true;

        public ServiceBusTask(ILogger<ServiceBusTask> logger/*, ServiceBusProcessor serviceBusProcessor*/)
        {
            _logger = logger;
            // _serviceBusProcessor = serviceBusProcessor;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Processing Service Bus messages...");

                // Your Service Bus logic here
                // Example: Process messages from Azure Service Bus
                // await _serviceBusProcessor.ProcessMessagesAsync(cancellationToken);
                await Task.Delay(50, cancellationToken);

                _logger.LogDebug("Service Bus processing completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Service Bus messages");
                throw;
            }
        }
    }
}

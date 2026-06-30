using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rentac.Reminders
{
    public class RemindersFunction
    {
        IConfiguration _configuration;
        public RemindersFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("delayedContractActivity")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                var _notifyUrl = _configuration["NOTIFY_URL"] + "notify/CollectContractReminders";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_notifyUrl);
                    var response = await client.PostAsync(_notifyUrl, null);
                }
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                Console.Write($"C# Timer trigger function executed at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

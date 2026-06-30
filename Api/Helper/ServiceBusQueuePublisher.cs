using Azure.Messaging.ServiceBus;
using BAL.DTO;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace FarmaAPI.Helper
{
    public class ServiceBusQueuePublisher
    {


        public async Task PublishContractReminderAsync(NotificationDto message,string queName)
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBusConnection"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("ServiceBusConnection app setting is missing.");
            }

            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queName);
            
            var body = JsonConvert.SerializeObject(message);
            
            await sender.SendMessageAsync(new ServiceBusMessage(body));


        }
    }

    public class ContractReminderQueueMessage
    {
        public int JobCardId { get; set; }
        public int CompanyId { get; set; }
        public short StatusId { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Token { get; set; }
        public string XCompanyId { get; set; }
    }
}

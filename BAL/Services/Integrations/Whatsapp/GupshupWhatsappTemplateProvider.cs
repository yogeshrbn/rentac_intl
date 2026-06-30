using BAL.DTO;
using BAL.Services.Integrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Integrations.Whatsapp
{
    public class GupshupWhatsappTemplateProvider : IWhatsappTemplateProvider
    {
        private readonly WhatsappService _gupshup;

        public GupshupWhatsappTemplateProvider(WhatsappService gupshup)
        {
            _gupshup = gupshup;
        }

        public string Name => WhatsappProviderNames.Gupshup;

        public string ApiKey { get; set; }

        public Task<List<string>> SendTemplateMessageAsync(WhatsappTemplateMessage message)
        {
            return _gupshup.SendTemplateMessage(message);
        }
    }
}

using BAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Integrations.Whatsapp
{
    /// <summary>
    /// Sends WhatsApp template messages via a vendor (Gupshup, DoubleTick, etc.).
    /// </summary>
    public interface IWhatsappTemplateProvider
    {
        string ApiKey { get; set; }
        string Name { get; }

        /// <summary>
        /// Returns provider message id(s). Implementations should set <see cref="WhatsappTemplateMessage.AppId"/> when known (e.g. Gupshup app id, DoubleTick &quot;from&quot; number).
        /// </summary>
        Task<List<string>> SendTemplateMessageAsync(WhatsappTemplateMessage message);
    }
}

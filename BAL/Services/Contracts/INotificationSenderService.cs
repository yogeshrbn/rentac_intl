using BAL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Contracts
{
    /// <summary>
    /// Sends outbound notifications over email, SMS (Msg91 flow), and WhatsApp (Gupshup templates).
    /// </summary>
    public interface INotificationSenderService
    {
        Task<bool> SendEmailAsync(NotificationDto dto);

        /// <summary>
        /// Msg91 flow API: set <see cref="NotificationDto.Subject"/> to template id,
        /// <see cref="NotificationDto.Receipients"/> to comma-separated numbers,
        /// and optionally <see cref="NotificationDto.MetaData"/> to a JSON object of template variables.
        /// </summary>
        Task<bool> SendSmsAsync(NotificationDto dto);

        Task<bool> SendSmsAsync(string recipients, string templateId, IDictionary<string, string> variables);

        /// <param name="dto">Set <see cref="NotificationDto.ws_source"/> to <c>DoubleTick</c> or <c>Gupshup</c>, or rely on <c>defaultWhatsappProvider</c> in app settings.</param>
        Task<bool> SendWhatsappAsync(NotificationDto dto, List<WhatsappTemplateMessageParamter> parameters);

        Task<bool> SendWhatsappAsync(NotificationDto dto, List<WhatsappTemplateMessageComponent> components);
    }
}

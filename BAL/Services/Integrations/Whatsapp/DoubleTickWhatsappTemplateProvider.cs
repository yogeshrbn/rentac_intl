using BAL.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.Integrations.Whatsapp
{
    /// <summary>
    /// WhatsApp template messages via DoubleTick public API (<c>POST https://public.doubletick.io/whatsapp/message/template</c>).
    /// </summary>
    public class DoubleTickWhatsappTemplateProvider : IWhatsappTemplateProvider
    {
        private const string DefaultApiUrl = "https://public.doubletick.io/whatsapp/message/template";

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string Name => WhatsappProviderNames.DoubleTick;

        public string ApiKey { get; set; }

        public async Task<List<string>> SendTemplateMessageAsync(WhatsappTemplateMessage message)
        {
            if (message?.template == null)
                throw new ArgumentException("Template payload is required.", nameof(message));

            var apiKey = this.ApiKey;
            var fromNumber = ConfigurationManager.AppSettings["doubleTickFromNumber"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("DoubleTick API key is not configured (doubleTickApiKey or gnsdoubletikckey).");
            if (string.IsNullOrWhiteSpace(fromNumber))
                throw new InvalidOperationException("DoubleTick sender number is not configured (doubleTickFromNumber).");

            var apiUrl = ConfigurationManager.AppSettings["doubleTickApiUrl"];

            if (string.IsNullOrWhiteSpace(apiUrl))
                throw new InvalidOperationException("DoubleTick API apiUrl is not configured.");

            var to = NormalizePhone(message.to);
            if (string.IsNullOrEmpty(to))
                throw new ArgumentException("Recipient phone (to) is invalid.", nameof(message));

            var templateData = BuildTemplateData(message.template);
            var lang = message.template.language?.code ?? "en";
            if (lang.Contains("_"))
                lang = lang.Split('_')[0];

            var content = new JObject
            {
                ["templateName"] = message.template.name,
                ["language"] = lang
            };
            if (templateData != null && templateData.Properties().Any())
                content["templateData"] = templateData;

            var msgObj = new JObject
            {
                ["content"] = content,
                ["from"] = fromNumber,
                ["to"] = to,
                ["messageId"] = Guid.NewGuid().ToString()
            };

            var payload = new JObject { ["messages"] = new JArray(msgObj) };
            var json = payload.ToString(Formatting.None);

            using (var client = new HttpClient())
            using (var contentHttp = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", apiKey.Trim());
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");

                var response = await client.PostAsync(apiUrl, contentHttp).ConfigureAwait(false);
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error("DoubleTick template send failed: {Status} {Body}", response.StatusCode, body);
                    var errMsg = TryParseErrorMessage(body) ?? response.ReasonPhrase;
                    throw new InvalidOperationException($"DoubleTick error: {errMsg}");
                }

                var messageId = ExtractMessageId(body);
                if (string.IsNullOrEmpty(messageId))
                {
                    _logger.Warn("DoubleTick success but no messageId in body: {Body}", body);
                    messageId = Guid.NewGuid().ToString();
                }

                message.AppId = fromNumber;
                return new List<string> { messageId };
            }
        }

        private static JObject BuildTemplateData(WhatsappTemplateMessageTemplate template)
        {
            var data = new JObject();
            if (template.components == null || template.components.Count == 0)
                return data;

            var bodyPlaceholders = new List<string>();
            JObject headerToken = null;
            JArray buttonsArray = null;

            foreach (var comp in template.components)
            {
                var ctype = comp.type?.ToLowerInvariant();
                if (ctype == "body")
                    bodyPlaceholders.AddRange(ExtractBodyPlaceholders(comp.parameters));
                else if (ctype == "header")
                {
                    var headerJ = BuildHeaderJson(comp.parameters);
                    if (headerJ != null)
                        headerToken = (JObject)headerJ;
                }
                else if (ctype == "button" && comp.parameters != null && comp.parameters.Count > 0 && buttonsArray == null)
                {
                    var btnList = GetButtonParameters(comp.parameters);
                    if (btnList != null && btnList.Count > 0)
                        buttonsArray = new JArray(btnList);
                }
            }

            if (bodyPlaceholders.Count > 0)
                data["body"] = JObject.FromObject(new { placeholders = bodyPlaceholders });
            if (headerToken != null)
                data["header"] = headerToken;
            if (buttonsArray != null)
                data["buttons"] = buttonsArray;

            return data;
        }

        private static JToken BuildHeaderJson(List<WhatsappTemplateMessageParamter> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return null;

            var doc = parameters.OfType<WhatsappTemplateMessageDocumentParamter>().FirstOrDefault();
            if (doc?.document?.link != null)
            {
                return JObject.FromObject(new
                {
                    type = doc.type.ToUpper(),
                    filename = doc.fileName,
                    mediaUrl = doc.document.link
                });
            }

            var texts = ExtractBodyPlaceholders(parameters);
            if (texts.Count == 0)
                return null;

            return JObject.FromObject(new
            {
                type = "TEXT",
                placeholder = string.Join(" ", texts)
            });
        }

        private static List<JObject> GetButtonParameters(List<WhatsappTemplateMessageParamter> parameters)
        {
            var list = new List<JObject>();
            foreach (var p in parameters)
            {
                if (p is WhatsappTemplateMessageTextParamter tp && !string.IsNullOrEmpty(tp.text))
                    list.Add(JObject.FromObject(new { type = "URL", parameter = tp.text }));
            }
            return list.Count > 0 ? list : null;
        }

        private static List<string> ExtractBodyPlaceholders(List<WhatsappTemplateMessageParamter> parameters)
        {
            var list = new List<string>();
            if (parameters == null)
                return list;

            foreach (var p in parameters)
            {
                if (p is WhatsappTemplateMessageTextParamter tp)
                    list.Add(tp.text ?? string.Empty);
                else if (p is WhatsappTemplateMessageDocumentParamter dp)
                    list.Add(dp.document?.link ?? string.Empty);
                else
                    list.Add(string.Empty);
            }
            return list;
        }

        private static string NormalizePhone(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length >= 10)
            {
                if (digits.Length == 10)
                    return "91" + digits;
                return digits;
            }
            return null;
        }

        private static string ExtractMessageId(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            try
            {
                var jo = JObject.Parse(body);
                var id = jo["messageId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                    return id;

                if (jo["messages"] is JArray arr && arr.Count > 0)
                {
                    id = arr[0]["messageId"]?.ToString();
                    if (!string.IsNullOrEmpty(id))
                        return id;
                }

                id = jo["data"]?["messageId"]?.ToString();
                return id;
            }
            catch
            {
                return null;
            }
        }

        private static string TryParseErrorMessage(string body)
        {
            try
            {
                var jo = JObject.Parse(body);
                return jo["message"]?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}

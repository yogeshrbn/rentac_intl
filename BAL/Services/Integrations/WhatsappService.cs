using Azure;
using BAL.DAL;
using BAL.DAL.Integrations;
using BAL.DTO;
using BAL.Objects;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;

namespace BAL.Services.Integrations
{
    public class WhatsappService
    {
        string _partnerApiBaseUrl = ConfigurationManager.AppSettings["gupShupPartnerBaseUrl"];
        string partnerEmail = ConfigurationManager.AppSettings["gupShupEmail"];
        string _partnerSecret = ConfigurationManager.AppSettings["gupShupSecret"];
        string _appCallBackUrl = ConfigurationManager.AppSettings["appCallbackUrl"];

        LoggingService logging = new LoggingService();
        public async Task<bool> CreateApp(WhatsappDTO dto)
        {

            var _partnerToken = await CreateOnGetValidPartnerToken();
            if (_partnerToken == null)
            {
                throw new Exception("Could not get or generate partner token");
            }
            var _data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("name",dto.Name),
                    new KeyValuePair<string, string>("templateMessaging","true"),
                    new KeyValuePair<string, string>("disableOptinPrefUrl","true")

                };

            var content = new FormUrlEncodedContent(_data);
            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app";
                content.Headers.Add("token", _partnerToken.Token);
                response = await client.PostAsync(_url, content);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Generate whatsapp partener token response is empty");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    dto.App_Id = Convert.ToString(data["appId"]);
                    var _dal = new WhatsappDAL();
                    var result = await _dal.CreateApp(dto);
                    if (result)
                    {
                        result = await SetCallback(dto);
                        if (result)
                        {
                            result = await UpdateEmbedLink(dto);
                        }
                    }
                    return result;
                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return false;

        }
        public async Task<bool> SetCallback(WhatsappDTO dto)
        {


            var _partnerToken = await CreateOnGetValidPartnerToken();
            if (_partnerToken == null)
            {
                throw new Exception("Could not get or generate partner token");
            }
            var _data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("url",_appCallBackUrl),
                    new KeyValuePair<string, string>("directForwarding","true"),
                    new KeyValuePair<string, string>("notifyWithPhone","true"),
                    new KeyValuePair<string, string>("modes","READ,DELIVERED,SENT,DELETED,OTHERS,TEMPLATE,ACCOUNT")
                };

            var content = new FormUrlEncodedContent(_data);
            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app/" + dto.App_Id + "/callback";
                content.Headers.Add("token", _partnerToken.Token);
                response = await client.PutAsync(_url, content);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Could not setup callback");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status = Convert.ToString(data["status"]);
                    if (status == "success")
                    {
                        var _dal = new WhatsappDAL();
                        return await _dal.SetCallback(dto);
                    }
                    else
                    {
                        throw new Exception("Could not set callback: " + status);
                    }
                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return false;
        }
        public async Task<bool> RefreshAppDetails(WhatsappDTO dto)
        {


            var _partnerToken = await CreateOnGetValidPartnerToken();
            if (_partnerToken == null)
            {
                throw new Exception("Could not get or generate partner token");
            }



            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app/" + dto.App_Id + "/details";
                client.DefaultRequestHeaders.Add("token", _partnerToken.Token);
                response = await client.GetAsync(_url);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Could not get app details");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status = Convert.ToString(data["status"]);
                    if (status == "success")
                    {
                        var _dal = new WhatsappDAL();
                        var details = data["appDetails"];
                        dto.LanguageCode = Convert.ToString(details["languageCode"]);
                        dto.Live = Convert.ToBoolean(details["live"]);
                        dto.LiveTs = Convert.ToString(details["liveTs"]);
                        dto.Stopped = Convert.ToBoolean(details["stopped"]);
                        dto.Phone = Convert.ToString(details["phone"]);
                        dto.TemplateMessaging = Convert.ToBoolean(details["templateMessaging"]);
                        dto.Type = Convert.ToString(details["type"]);
                        dto.Version = Convert.ToInt16(details["version"]);
                        var result = await _dal.UpdateAppDetails(dto);
                        if (result && dto.Live == true)
                        {
                            result = await GetAppToken(dto);
                            result = await UpdateSubscription(dto);
                        }
                    }
                    else
                    {
                        throw new Exception("Could not get or generate partner token: " + status);
                    }
                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return false;
        }
        public async Task<bool> GetAppToken(WhatsappDTO dto)
        {


            var _partnerToken = await CreateOnGetValidPartnerToken();
            if (_partnerToken == null)
            {
                throw new Exception("Could not get or generate partner token");
            }



            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app/" + dto.App_Id + "/token";
                client.DefaultRequestHeaders.Add("token", _partnerToken.Token);
                response = await client.GetAsync(_url);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Could not get app token");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status = Convert.ToString(data["status"]);
                    if (status == "success")
                    {
                        var _dal = new WhatsappDAL();
                        var details = data["token"];
                        dto.AppToken = Convert.ToString(details["token"]);

                        return await _dal.UpdateAppToken(dto);
                    }
                    else
                    {
                        throw new Exception("Could not get app token: " + status);
                    }
                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return false;
        }

        public async Task<bool> UpdateSubscription(WhatsappDTO dto)
        {


            var _partnerToken = await CreateOnGetValidPartnerToken();
            if (_partnerToken == null)
            {
                throw new Exception("Could not get or generate partner token");
            }

            var _data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("modes","ALL"),
                    new KeyValuePair<string, string>("tag","V3"),
                    new KeyValuePair<string, string>("showOnUI","true"),
                    new KeyValuePair<string, string>("version","3"),
                    new KeyValuePair<string, string>("url", _appCallBackUrl)



                };

            var content = new FormUrlEncodedContent(_data);

            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app/" + dto.App_Id + "/subscription";
                client.DefaultRequestHeaders.Add("token", _partnerToken.Token);
                response = await client.PostAsync(_url, content);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Could not setup subscription for app: " + dto.App_Id);
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status = Convert.ToString(data["status"]);
                    if (status == "success")
                    {
                        var _dal = new WhatsappDAL();
                        var details = data["subscription"];
                        dto.SubscriptionEnabled = Convert.ToBoolean(details["active"]);


                        return await _dal.UpdateAppSubscription(dto);
                    }
                    else
                    {
                        throw new Exception("Could not setup subscription: " + status);
                    }
                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return false;
        }

        public async Task<bool> UpdateEmbedLink(WhatsappDTO dto)
        {
            var dal = new WhatsappDAL();

            var _partnerToken = await CreateOnGetValidPartnerToken();
            if (_partnerToken == null)
            {
                throw new Exception("Could not get or generate partner token");
            }
            var _data = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("name",dto.Name),
                    new KeyValuePair<string, string>("templateMessaging","true"),
                    new KeyValuePair<string, string>("disableOptinPrefUrl","true")

                };

            var content = new FormUrlEncodedContent(_data);
            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app/" + dto.App_Id + "/onboarding/embed/link?regenerate=true&user=" + partnerEmail +
                    "&lang=en_US";
                client.DefaultRequestHeaders.Add("token", _partnerToken.Token);
                response = await client.GetAsync(_url);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Emabed link generation response is empty");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var status = Convert.ToString(data["status"]);
                    if (status == "success")
                    {
                        dto.Embed_Link = Convert.ToString(data["link"]);
                        var _dal = new WhatsappDAL();
                        return await _dal.UpdateEmbedLink(dto);
                    }

                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return false;


        }


        public async Task<bool> SavePartnerToken(GupShupToken token)
        {
            var dal = new WhatsappDAL();
            return await dal.SavePartnerToken(token);
        }

        public async Task<GupShupToken> GetParnerToken()
        {
            var dal = new WhatsappDAL();
            return await dal.GetPartnerToken(DateTime.Now);
        }

        public async Task<GupShupToken> CreateOnGetValidPartnerToken()
        {
            var dal = new WhatsappDAL();
            try
            {
                var token = await dal.GetPartnerToken(DateTime.Now);
                if (token != null)
                {
                    return token;
                }
                var newToken = await GenerateToken();
                if (newToken != null)
                {
                    var result = SavePartnerToken(newToken);
                    if (result == null)
                    {
                        logging.LogError("Could not save partnerToken");
                    }
                    return newToken;
                }
                return null;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message, "CreateOnGetValidPartnerToken");
                return null;
            }

        }

        public async Task<GupShupToken> GenerateToken()
        {
            try
            {

                var _data = new List<KeyValuePair<string, string>>
                {

                    new KeyValuePair<string, string>("email",partnerEmail),
                    new KeyValuePair<string, string>("password",_partnerSecret)
                };
                var content = new FormUrlEncodedContent(_data);
                var response = new HttpResponseMessage();
                using (var client = new HttpClient())
                {
                    try
                    {
                        var _url = _partnerApiBaseUrl + "account/login";
                        response = await client.PostAsync(_url, content);
                    }
                    catch (HttpRequestException ex)
                    {
                        logging.LogError(ex, ex.Message, "GenerateToken");
                        throw ex;
                    }
                }
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var dataStr = await response.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(dataStr))
                    {
                        throw new Exception("Generate whatsapp partener token response is empty");
                    }
                    var data = JObject.Parse(dataStr);
                    if (data != null)
                    {
                        var gupshupToken = new GupShupToken();
                        gupshupToken.Token = Convert.ToString(data["token"]);
                        gupshupToken.CreatedOn = DateTime.Now;
                        gupshupToken.ValidTill = DateTime.Now.AddMinutes(1381);
                        gupshupToken.GuId = Guid.NewGuid().ToString();

                        return gupshupToken;

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                logging.LogError(ex, ex.Message, "GenerateToken");
                throw ex;
            }
        }

        public async Task<IEnumerable<WhatsappDTO>> ListApps(int companyId, int clientId)
        {
            var dal = new WhatsappDAL();
            return await dal.ListApps(companyId, clientId);
        }


        public async Task<List<String>> SendTemplateMessage(WhatsappTemplateMessage message)
        {


            var dal = new WhatsappDAL();
            var apps = await ListApps(message.CompanyId, message.ClientId);
            var wsAppDto = new WhatsappDTO();
            if (apps == null)
            {
                throw new Exception("No app is live");
            }
            wsAppDto = apps.Where(o => o.Live == true).FirstOrDefault();
            if (wsAppDto == null)
            {
                throw new Exception("No app is live");
            }
            if (String.IsNullOrEmpty(wsAppDto.AppToken))
            {
                throw new Exception("No app is live");
            }
            message.AppId = wsAppDto.App_Id;
            message.messaging_product = "whatsapp";
            var _data = JsonConvert.SerializeObject(message);
            var content = new StringContent(_data);
            var response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var _url = _partnerApiBaseUrl + "app/" + wsAppDto.App_Id + "/v3/message";
                client.DefaultRequestHeaders.Add("Authorization", wsAppDto.AppToken);
                response = await client.PostAsync(_url, content);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                if (String.IsNullOrEmpty(dataStr))
                {
                    throw new Exception("Emabed link generation response is empty");
                }
                var data = JObject.Parse(dataStr);
                if (data != null)
                {
                    var messages = (JArray)data["messages"];
                    if (messages != null)
                    {
                        var _listMessages = new List<String>();
                        foreach (var token in messages)
                        {
                            _listMessages.Add(Convert.ToString(token["id"]));
                        }
                        return _listMessages;
                    }

                }
            }
            else
            {
                var dataStr = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(dataStr);
                var _message = Convert.ToString(data["message"]);
                logging.LogError(dataStr);
                throw new Exception(_message);
            }
            return null;
        }
    }
}

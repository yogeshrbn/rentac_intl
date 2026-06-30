using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BAL.Objects;
using BAL.DTO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;
namespace FarmaAPI.Helper
{
    public class CommHelpler
    {
        const string SMS_URL = "http://sms.nlet.in/api/mt/SendSMS";
        const string URL_MSG91 = "https://control.msg91.com/api/v5/flow";

        class MSG91Body
        {

        }
        public async Task<HttpResponseMessage> PostRequest(string templateId, IDictionary<string, string> variables, string receipitents)
        {
            try
            {
                var key = ConfigurationManager.AppSettings["msg91AuthKey"];

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var jobject = new Dictionary<string, string>();
                var phones = receipitents.Split(',');
                var mobiles = new List<string>();
                foreach (string phone in phones)
                {
                    var ph = phone;
                    if (ph.Length >= 10)
                    {
                        ph = ph.Substring(ph.Length - 10, 10);
                        ph = "91" + ph;
                        mobiles.Add(ph);

                    }
                }
                receipitents = String.Join(",", mobiles);
                jobject.Add("mobiles", receipitents);
                foreach (var j in variables)
                {
                    jobject.Add(j.Key, j.Value);
                }
                var dicArray = new List<Dictionary<string, string>>();
                dicArray.Add(jobject);
                var data = new
                {

                    template_id = templateId,
                    short_url = "0",
                    short_url_expiry = "seconds",
                    recipients = dicArray,
                    realTimeResponse = 1

                };
                var strContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(strContent);
                using (var client = new HttpClient())
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("authkey", key);
                    var x = await client.PostAsync(URL_MSG91, content);
                    return x;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool sendSms(string to, string templateId, MessageEvent msgFor)
        {
            if (to == null)
            {
                return false;
            }
            try
            {
                /*
                WebClient webClient = new WebClient();
                webClient.QueryString.Add("user", "rbntech");
                webClient.QueryString.Add("password", "rbn@123");
                webClient.QueryString.Add("senderid", "RENTAC");
                webClient.QueryString.Add("channel", "Trans");
                webClient.QueryString.Add("DCS", "0");
                webClient.QueryString.Add("flashsms", "0");
                webClient.QueryString.Add("number", to);
                webClient.QueryString.Add("text", templateId);
                webClient.QueryString.Add("route", "47");

                string result = webClient.DownloadString(SMS_URL);
                //JObject jsonObject = JObject.Parse(result);

                int code = 0;// Convert.ToInt32(jsonObject.GetValue("ErrorCode"));
                if (code == 0)
                {
                    Communication com = new Communication();
                    LoggedInUser user = new LoggedInUser();
                    com.RbnClientId = user.RbnClientId;
                    com.CompanyId = user.DefaultCompanyId;
                    com.Message = templateId;
                    com.Recipient = to;
                    com.UserId = user.UserId;
                    com.For = msgFor.ToString();
                    com.Add();
                }*/


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> sendSms(string to, string templateId, MessageEvent msgFor, IDictionary<string, string> variables)
        {
            var x = await PostRequest(templateId, variables, to).ConfigureAwait(false);
            return true;
        }

        public bool sendSms(string to, EmailParameters p, MessageEvent msgFor)
        {
            return sendSms(to, GetMessageBody(p.GetXML(), msgFor), msgFor);
        }

        public string GetMessageBody(string xmlData, MessageEvent msgFor)
        {
            Communication com = new Communication();
            int code = (int)Enum.Parse(typeof(MessageEvent), msgFor.ToString());
            LoggedInUser user = new LoggedInUser();
            CommDTO dto = com.GetMessageTemplate(code, user.RbnClientId);
            if (dto == null) return "";
            XslCompiledTransform xDoc = new XslCompiledTransform();
            String xsl = "<?xml version=\"1.0\" encoding=\"utf-8\"?><xsl:transform version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">";
            xsl += "<xsl:template match=\"/\">";
            xsl += dto.Template;
            xsl += "</xsl:template></xsl:transform>";
            StringReader reader = new StringReader(xsl);
            xDoc.Load(new XmlTextReader(reader), XsltSettings.TrustedXslt, new XmlUrlResolver());
            StringBuilder resultString = new StringBuilder();
            XmlTextWriter xmlWriter = new XmlTextWriter(new StringWriter(resultString));
            XmlTextReader xmlReader = new XmlTextReader(new StringReader(xmlData));
            xDoc.Transform(xmlReader, xmlWriter);
            string result = resultString.ToString();
            result = result.Replace("xmlns:asp=\"remove\"", "");

            return result;

        }
    }

    public enum MessageEvent
    {
        ISSUE_CHALLAN = 1001,
        RECEIVE_ITEM = 1002,
        MAT_REMINDER = 1003,
        BILL_REMINDER = 1004,
        BILL_GENRATED = 1005,
        AMT_RECEIVED = 1006,
        DEBIT_NOTE = 1007,
        CREDIT_NOTE = 1008,
        PARTY_LOGIN = 1009
    }

    public enum ReminderType
    {
        SMS = 1,
        EMAIL = 2,
        NONE = 3
    }

    public struct SMSTemplates
    {
        public static string ISSUE_CHALLAN = "6753ede6d6fc0530dc72f114";
        public static string BILL_GENERATED = "6761466cd6fc055d2d7b0082";
        public static string PARTY_LOGIN = "67659f9ad6fc0568514bd7f2";


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
namespace FarmaAPI.Helper
{
    public class ApiMessageHandler
    {

        public static ApiMessage GetMessage(ApiMessageCodes code)
        {
            string messageFile = System.Web.HttpContext.Current.Server.MapPath("~/messages.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(messageFile);
            XmlNode messageNode = doc.SelectSingleNode("//Message[code='" + Convert.ToInt16(code).ToString() + "']");
            if (messageNode != null)
            {
                ApiMessage message = new ApiMessage();
               // message.Code = messageNode.ChildNodes[0].InnerText;
                message.Description = messageNode.ChildNodes[1].InnerText;
                return message;
            }
            else
                return null;
        }
        public static ApiMessage GetMessage(ApiMessageCodes code, string excptionMessage)
        {
            ApiMessage msg = GetMessage(code);
            if (msg != null)
            {
                msg.Description += "</br>" + excptionMessage;
            }
            return msg;
        }
        public static ApiMessage GetMessage(ApiMessageCodes code, Exception ex)
        {
            ApiMessage msg = GetMessage(code);
            if (msg != null)
            {
                msg.Description += "</br>" + ex.Message + "</br>" + ex.StackTrace;
            }
            return msg;
        }
    }



    public class ApiMessage
    {

        #region Messages
        public const string SUCCESS = "SUCCESS";
        public const string ERROR = "Error has occurred.";

        #endregion
        public ApiMessageCodes Code { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }

        public int ErrorCode { get; set; }
        public object Data { get; set; }
        public object Extra { get; set; }

    }

    public enum ApiMessageCodes
    {
        ERROR = 500,
        SUCCESS = 200,
        BILLING_GENERATED = 100,
        NOT_FOUND= 404,
        LEDGER_TRANSACTION_ERROR = 101,
        COULD_NOT_SUCCEED=600
    }
}
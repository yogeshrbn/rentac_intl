
using BAL.DTO;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Http.Filters;
using Microsoft.Ajax.Utilities;
using FarmaAPI.Helper;
using BAL.Exceptions;
namespace FarmaAPI
{
    public class HttpMiddlewareModule : IHttpModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(end_request);
            context.BeginRequest += new EventHandler(begin_request);
            context.LogRequest += new EventHandler(log_request);


        }
        public void log_request(object sender, EventArgs e)
        {
            //Log operation goes here    
            HttpContext context = ((HttpApplication)sender).Context;
            if (context.Request.HttpMethod != "OPTIONS")
            {
                var request = context.Request;
                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();

                        var logDto = new LogDTO();
                        logDto.Url = request.Url.ToString();
                        if (request.UrlReferrer != null)
                        {
                            logDto.IPAddress = request.UrlReferrer.LocalPath;
                            logDto.Message = documentContents;
                            logger.Debug(JsonConvert.SerializeObject(logDto));
                        }

                    }
                }
            }
        }
        public void begin_request(object sender, EventArgs e)
        {

        }
        public void end_request(object sender, EventArgs e)
        {

        }
        public void on_error(ExceptionHandlerContext context)
        {

        }
    }


    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }
            var apiMessage = new ApiMessage();
            apiMessage.Message = exceptionMessage;
            apiMessage.Code = ApiMessageCodes.ERROR;
            if (actionExecutedContext.Exception.GetType() == typeof(UDFException))
            {
                var excep = (UDFException)actionExecutedContext.Exception;
                apiMessage.ErrorCode = excep.ErrorCode;
            }
            //We can log this exception message to the file or database.
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {

                Content = new StringContent(JsonConvert.SerializeObject(apiMessage)),
                ReasonPhrase = apiMessage.Message,// "Internal Server Error.Please Contact your Administrator.",
                StatusCode = HttpStatusCode.InternalServerError
            };

            var logDto = new LogDTO();


            logger.Error<Exception>(actionExecutedContext.Exception);

            actionExecutedContext.Response = response;
        }
    }
}
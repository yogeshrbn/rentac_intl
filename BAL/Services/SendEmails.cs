using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BAL.DTO;
using NLog;
using BAL.Objects;
using BAL.Common;
namespace BAL.Services
{
    /// <summary>
    /// Summary description for SendEmails
    /// </summary>
    public class SendEmails
    {
        // public static readonly ILog logger = LogManager.GetLogger(typeof(SendEmails));
        public static LoggingService logger = new LoggingService();
        public SendEmails()
        {
            //
            // TODO: Add constructor logic here
            //
        }



        public static bool SendEmail(string toEmailaddress, string displayName, string subject,
            string emailBody, params string[] attachments)
        {


            var logService = new LoggingService();
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            var user = new LoggedInUserInfo();

            string userName = ConfigurationManager.AppSettings["userId"];
            string password = ConfigurationManager.AppSettings["password"];


            bool flag = false;
            bool UseSMTPSSL = false;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["authenticate"]))
            {
                flag = Convert.ToBoolean(ConfigurationManager.AppSettings["authenticate"]);
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UsesmtpSSL"]))
            {
                UseSMTPSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["UsesmtpSSL"]);
            }

            string host = ConfigurationManager.AppSettings["mailServer"];
            string address = ConfigurationManager.AppSettings["fromEmailID"];
            int smtpPort = 587;
            if (user.DefaultCompanyId != 0)
            {
                var config = new Config();
                var emailConfif = config.GetConfig(user.DefaultCompanyId, "email", "setup");
                if (emailConfif != null && emailConfif.Count > 0)
                {
                    var server = emailConfif.Where(o => o.Key == "server").FirstOrDefault();
                    var port = emailConfif.Where(o => o.Key == "port").FirstOrDefault();
                    var username = emailConfif.Where(o => o.Key == "username").FirstOrDefault();
                    var smtpPassword = emailConfif.Where(o => o.Key == "password").FirstOrDefault();
                    var fromEmail = emailConfif.Where(o => o.Key == "from_email").FirstOrDefault();

                    if (server != null)
                    {
                        host = server.Value;
                    }
                    if (username != null)
                    {
                        userName = username.Value;
                    }
                    if (port != null)
                    {
                        smtpPort = Convert.ToInt16(port.Value);
                    }
                    if (fromEmail != null)
                    {
                        address = fromEmail.Value;
                    }
                }
            }


            MailAddress from = new MailAddress(address, displayName);
            try
            {
                smtpClient.EnableSsl = UseSMTPSSL;
                smtpClient.Host = host;
                mailMessage.From = from;
                mailMessage.To.Add(toEmailaddress);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = emailBody;
                // mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(fileAttach), "Ticket.pdf"));
                if (flag)
                {
                    NetworkCredential credentials = new NetworkCredential(userName, password);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = credentials;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = true;
                }

                string sendMail = Convert.ToString(ConfigurationManager.AppSettings["AllowSendMails"]);

                if (attachments != null)
                {
                    if (attachments.Length > 0)
                    {
                        foreach (string attachmentFile in attachments)
                        {

                            // string attachmentFile = System.Web.HttpContext.Current.Server.MapPath("~/temp/" + fileName);
                            if (File.Exists(attachmentFile))
                            {

                                Attachment item = new Attachment(attachmentFile);
                                mailMessage.Attachments.Add(item);
                                //  item.Dispose();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(sendMail) || (sendMail.ToUpper() != "NO"))
                {
                    smtpClient.Port = smtpPort;
                    if (!string.IsNullOrEmpty(emailBody))

                        smtpClient.Send(mailMessage);
                    mailMessage.Attachments.Dispose();


                }
                // update Mail log status of IsDelivered
                //MailBoxManager.UpdateDeliveryStatus(MailerLogID, true, "Successfully Send.");
                return true;
            }
            catch (Exception ex)
            {
                logService.LogError(ex, ex.Message);
                // update Mail log status of IsDelivered =False and Logtext = ex.mesage
                return false;
            }
        }

        public async Task<bool> SendEmail(NotificationDto dto)
        {


            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();

            string userName = ConfigurationManager.AppSettings["userId"];
            string password = ConfigurationManager.AppSettings["password"];
            bool flag = false;
            bool UseSMTPSSL = false;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["authenticate"]))
            {
                flag = Convert.ToBoolean(ConfigurationManager.AppSettings["authenticate"]);
            }
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UsesmtpSSL"]))
            {
                UseSMTPSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["UsesmtpSSL"]);
            }

            string host = ConfigurationManager.AppSettings["mailServer"];
            string address = ConfigurationManager.AppSettings["fromEmailID"];
            MailAddress from = new MailAddress(address, "Rentac");
            var user = new LoggedInUserInfo();
            int smtpPort = 587;
            if (user.DefaultCompanyId != 0)
            {
                var config = new Config();
                var emailConfif = config.GetConfig(user.DefaultCompanyId, "email", "setup");
                if (emailConfif != null && emailConfif.Count > 0)
                {
                    var server = emailConfif.Where(o => o.Key == "server").FirstOrDefault();
                    var port = emailConfif.Where(o => o.Key == "port").FirstOrDefault();
                    var username = emailConfif.Where(o => o.Key == "username").FirstOrDefault();
                    var smtpPassword = emailConfif.Where(o => o.Key == "password").FirstOrDefault();
                    var fromEmail = emailConfif.Where(o => o.Key == "from_email").FirstOrDefault();

                    if (server != null)
                    {
                        host = server.Value;
                    }
                    if (username != null)
                    {
                        userName = username.Value;
                    }
                    if (port != null)
                    {
                        smtpPort = Convert.ToInt16(port.Value);
                    }
                    if (fromEmail != null)
                    {
                        address = fromEmail.Value;
                    }
                }
            }


            try
            {
                smtpClient.EnableSsl = UseSMTPSSL;
                smtpClient.Host = host;
                mailMessage.From = from;
                mailMessage.To.Add(dto.Receipients);
                mailMessage.Subject = dto.Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = dto.Body;
                // mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(fileAttach), "Ticket.pdf"));
                if (flag)
                {
                    NetworkCredential credentials = new NetworkCredential(userName, password);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = credentials;
                }
                else
                {
                    smtpClient.UseDefaultCredentials = true;
                }

                string sendMail = Convert.ToString(ConfigurationManager.AppSettings["AllowSendMails"]);


                if (dto.AttachmentDocs != null)
                {

                    foreach (var att in dto.AttachmentDocs)
                    {

                        //string attachmentFile = System.Web.HttpContext.Current.Server.MapPath("~/temp/" + fileName);
                        //if (File.Exists(attachmentFile))
                        //{
                        //    Attachment item = new Attachment(attachmentFile);
                        //    mailMessage.Attachments.Add(item);
                        //}
                        var ms = new MemoryStream(att.Buffer);

                     

                        Attachment item = new Attachment(ms, att.Name, att.ContentType);
                        mailMessage.Attachments.Add(item);


                    }
                }


                if (string.IsNullOrEmpty(sendMail) || (sendMail.ToUpper() != "NO"))
                {
                    smtpClient.Port = smtpPort;

                    //  smtpClient.SendCompleted += new SendCompletedEventHandler(SmtpClient_SendCompleted);
                    //  smtpClient.SendAsync(mailMessage, dto.GuId);
                    await smtpClient.SendMailAsync(mailMessage);
                    dto.Status = "delivered";
                    await UpdateStatus(dto);
                    return true;

                }
                else
                {
                    dto.Status = "cancelled";
                    await UpdateStatus(dto);
                }
                return false;

                // update Mail log status of IsDelivered
                //MailBoxManager.UpdateDeliveryStatus(MailerLogID, true, "Successfully Send.");
                //  return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                dto.Status = "failed";
                dto.DeliveryMessage = ex.Message;
                await UpdateStatus(dto);
                // update Mail log status of IsDelivered =False and Logtext = ex.mesage
                return false;
            }
        }

        private async Task<bool> UpdateStatus(NotificationDto dto)
        {

            var service = new NotificationService();

            dto.ModifiedOn = DateTime.Now;

            return await service.StatusUpdate(dto);
        }

    }
}
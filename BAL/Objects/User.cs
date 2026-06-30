using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using BAL.DAL;
using BAL.DTO;
using BAL.Services;

namespace BAL.Objects
{
    public class User
    {
        public bool SetDefaultCompany(int userId, int companyId)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.SetDefaultCompany(userId, companyId);
        }
        public List<UserDTO> GetAllUsers(int rbnClientId)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetAllUsers(rbnClientId);
        }
        public bool CreateUser(UserDTO dto)
        {
            UserDAL objDAL = new UserDAL();
            try
            {
                var r = objDAL.CreateUser(dto);
                if (r)
                {
                    //send email
                    string mailURL = MailerUtility.GetEmailURL(EmailTemplate.USER_CREATION);
                    string xml = "<Data>";
                    var sb = new StringBuilder(xml);
                    sb.Append("<Name>" + dto.FullName + "</Name>");
                    sb.Append("<LoginName>" + dto.Phone + "</LoginName>");
                    sb.Append("<Password>" + dto.Password + "</Password>");
                    sb.Append("</Data>");
                    string body = MailerUtility.GetMailBody(mailURL, sb.ToString());

                    SendEmails.SendEmail(dto.Email, dto.FullName, "Account Created", body);
                }

                return r;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateUser(UserDTO dto)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.UpdateUser(dto);
        }
        public bool ResetPassword(UserDTO dto)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.ResetPassword(dto);
        }
        public UserDTO GetByEmail(string email)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetByEmail(email);
        }

        public UserDTO GetByPhone(string phone)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetByPhone(phone);
        }

        public bool UpdateStatus(int userId, bool status)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.UpdateStatus(userId, status);
        }
        public FunctionDTO GetRouteAccess(int userId, string route)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetRouteAccess(userId, route);
        }
        public List<FunctionDTO> GetAllowedRoutes(int userId)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetAllowedRoutes(userId);
        }
        public UserDTO GetById(int userId, int finYear)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetById(userId, finYear);
        }

        public UserDTO GetById(int v)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProfilePic(int userId, string picPath)
        {

            UserDAL objDAL = new UserDAL();
            return objDAL.UpdateProfilePic(userId, picPath);
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {

            UserDAL objDAL = new UserDAL();
            return objDAL.ChangePassword(userId, currentPassword, newPassword);
        }
        public bool CreateVarificationLink(VerificationLinkDTO dto)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.CreateVarificationLink(dto);
        }
        public VerificationLinkDTO GetVarificationLink(string guId)
        {
            UserDAL objDAL = new UserDAL();
            return objDAL.GetVarificationLink(guId);
        }
    }
}

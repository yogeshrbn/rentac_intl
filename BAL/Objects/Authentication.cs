using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class Authentication
    {
        public UserDTO Authenticate(string userName, string password)
        {
            AuthenticationDAL dal = new AuthenticationDAL();
            return dal.Authenticate(userName, password);
        }
        public void SaveToken(int userId, string token,int finYearId)
        {
            AuthenticationDAL dal = new AuthenticationDAL();
            dal.SaveToken(userId, token,finYearId);
        }
        public UserDTO GetToken(string token)
        {
            AuthenticationDAL dal = new AuthenticationDAL();
            return dal.GetToken(token);
        }
    }
}

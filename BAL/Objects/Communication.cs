using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTO;
using BAL.DAL;
namespace BAL.Objects
{
    public class Communication : CommDTO
    {
        public bool Add()
        {
            CommDAL dal = new CommDAL();
            return dal.Add(this);
        }
        public CommDTO GetMessageTemplate(int code, int rbnClientId)
        {
            CommDAL dal = new CommDAL();
            return dal.GetMessageTemplate(code, rbnClientId);
        }
    }
}

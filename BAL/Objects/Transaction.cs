using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DAL;
using BAL.DTO;
namespace BAL.Objects
{
    public class Transaction
    {
        public bool Add(TransactionDTO dto)
        {
            TransactionDAL dal = new TransactionDAL();
            return dal.Add(dto);
        }
    }
}

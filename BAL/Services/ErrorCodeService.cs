using BAL.DAL;
using BAL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class ErrorCodeService
    {
        static ErrorCodeDAL DAL = new ErrorCodeDAL();
        public static ErrorCodes GetMessage(int errorCode)
        {
            return DAL.GetMessage(errorCode);
        }
    }
}

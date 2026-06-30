using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Common
{

    public class ApiResponseMessage
    {

        #region Messages
        public const string SUCCESS = "SUCCESS";
        public const string ERROR = "Error has occurred.";

        #endregion
        public ApiResponseMessageCodes Code { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }

        public int ErrorCode { get; set; }
        public object Data { get; set; }
    }

    public enum ApiResponseMessageCodes
    {
        ERROR = 500,
        BAD_REQUEST=405,

        SUCCESS = 200,
        BILLING_GENERATED = 100,
        LEDGER_TRANSACTION_ERROR = 101,
        COULD_NOT_SUCCEED = 600
    }
}

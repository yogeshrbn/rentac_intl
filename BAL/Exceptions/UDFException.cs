using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Exceptions
{
    public class UDFException : Exception
    {
        public UDFException(string message, int errorCode) : base(message)
        {
            Message = message;
            ErrorCode = errorCode;
        }
        public UDFException(string message, int errorCode, Exception innerException) : base(message,innerException)
        {
            Message = message;
            ErrorCode = errorCode;
            
        }
        public string Message { get; set; }
        public int ErrorCode { get; set; }

    }
    public class ErrorCodes
    {
        //Billing Error Codes
        public static int NOTHING_TO_BILL = 700;
        public static int BILL_NOT_FOUND_FOR_BLLID = 701;
        public static int BILL_NOT_CANCELLED = 702;

        public static int PURCHASE_NOT_FOUND_BYID = 730;


        //company error coes
        public static int INVALID_GST = 800;
        public static int INVALID_LEGALNAME = 801;
        public static int INVALID_TRADE_NAME = 802;
        public static int INVALID_GST_REG_DATE = 803;
        public static int INVALID_EINVOICE_STARTDATE = 804;

        public static int INVALID_IRP_USERNAME = 805;
        public static int INVALID_IRP_USERPASSWORD = 806;


        //EwayBillCodes
        public static int ERROR_WHILE_PREPARING_DATA = 900;
        public static int FAILED_UPDATE_INFO_ON_BILL = 901;
        public static int FAILED_UPDATE_INFO_ON_EWAYBILL = 902;
        public static int STATE_GST_CODE_NOT_FOUND = 903;
        public static int INVALID_INFO_FOR_VEHICLE_UPDATE = 904;
        public static int INVALID_INFO_FOR_TRANSPORTER_UPDATE = 905;
        public static int INVALID_INFO_FOR_BILL_CANCEL = 906;
        public static int ERROR_WHILE_CANCEL_BILL_IN_DB = 907;
        public static int ERROR_WHILE_UPDATE_EWAYBILL_IN_CHALLAN = 907;


        public static int CONTRACT_NOT_FOUND = 950;

        //MatLOSS
        public static int MATLOSS_INVALID_FROM_TO_DATE = 980;
        public static int MATLOSS_INVALID_KEY_BY_ID = 981;

        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }

}

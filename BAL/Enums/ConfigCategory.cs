using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Enums
{
    public enum ConfigCategory
    {
        Invoice = 1,
        InvoiceGST = 2,
        BILLNO = 3,
        ISSUECHALLAN = 4,
        RECCHALLAN = 5,
        TRANSACTION = 6
    }
    public enum ConfigKey
    {
        Prefix = 1,
        Start = 2

    }

    public static class ConfigCategoryNames
    {

        public static string ISSUECHALLAN = "ISSUECHALLAN";
        public static string QUOTATIONS = "QUOTATIONS";
        public static string RETURNS = "RETURNS";
        public static string SALES = "SALES";
        public static string RENTBILLS = "RENTBILLS";
        public static string CONTRACTDELIVERYCHALLAN = "CONTRACTDELIVERYCHALLAN";
        public static string CONTRACTRETURNS = "CONTRACTRETURNS";
        public static string SALESDELIVERYCHALLAN = "SALESDELIVERYCHALLAN";
        public static string SALESRETURNS = "SALESRETURNS";
        public static string EWAYBILL = "EWAYBILL";

        public static string TRANSFERCHALLAN = "TRANSFERCHALLAN";

      
    }
    public static class ConfigSubCategoryNames
    {

        public static string TEMPLATES = "templates";

    }

}

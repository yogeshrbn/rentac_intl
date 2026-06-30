using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Enums;
using BAL.Objects;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class NextIdController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage WorkOrderNumber([FromBody] WorkOrderDTO dto)
        {
            string lastId = "";
            try
            {
                BAL.Objects.Config cnfig = new BAL.Objects.Config();
                var user = new LoggedInUser();
                if (dto == null)
                {
                    throw new Exception("Chllan type not supplied");
                }
                if (dto.ChallanType == 0)
                {
                    throw new Exception("Chllan type not supplied");
                }
                //string prefix = cnfig.GetKeyValue(ConfigCategory.ISSUECHALLAN, ConfigCategory.ISSUECHALLAN, ConfigKey.Prefix, new LoggedInUser().DefaultCompanyId, null);
                var configs = cnfig.GetConfig(user.DefaultCompanyId, "ISSUECHALLAN", "ISSUECHALLAN");
                if (configs == null)
                {
                    throw new Exception("Please configure challan prefix");
                }
                var samePrefix = configs.FirstOrDefault(c => c.Key.ToLower() == "sameprefix");
                bool blnSamePrefix = true;
                if (samePrefix != null)
                {
                    bool.TryParse(samePrefix.Value, out blnSamePrefix);
                }
                string prefixKey = "billPrefix";
                string startKey = "start";
                if (!blnSamePrefix)
                {
                    switch ((byte)dto.ChallanType)
                    {
                        case 1:
                            prefixKey = "contractPrefix";
                            break;
                        case 2:
                        case 12:
                            prefixKey = "rentPrefix";
                            break;
                        case 9:
                            prefixKey = "adjPrefix";
                            break;
                        case 10:
                            prefixKey = "hirePrefix";
                            break;
                        case 5:
                            prefixKey = "salePrefix";
                            break;
                    }
                    switch ((byte)dto.ChallanType)
                    {
                        case 1:
                            prefixKey = "contractStart";
                            break;
                        case 2:
                        case 12:
                            prefixKey = "rentStart";
                            break;
                        case 9:
                            prefixKey = "adjStart";
                            break;
                        case 10:
                            prefixKey = "hireStart";
                            break;
                        case 5:
                            prefixKey = "saleStart";
                            break;
                    }
                }
                NextId n = new NextId();

                var prefixConfig = configs.Where(o => o.Key.ToLower() == prefixKey.ToLower()).FirstOrDefault();
                var startConfig = configs.Where(o => o.Key.ToLower() == startKey.ToLower()).FirstOrDefault();

                string prefix = "", start = "1";

                if (prefixConfig != null)
                {
                    prefix = prefixConfig.Value;
                }
                if (startConfig != null)
                {
                    start = startConfig.Value;
                }

                lastId = n.GenNextNumber(NextIDTables.WorkOrder, ConfigCategory.ISSUECHALLAN, ConfigCategory.ISSUECHALLAN,
                    user.DefaultCompanyId, user.FinYearId, prefix, start);

                //lastId = lastId.Replace(prefix, "");
                lastId = prefix + lastId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Request.CreateResponse(HttpStatusCode.OK, lastId);
        }

        /// <summary>Next rent / GST bill number preview (Invoice sequence, BILLNO config).</summary>
        [HttpPost]
        public HttpResponseMessage RentInvoiceNumber()
        {
            try
            {
                var user = new LoggedInUser();
                NextId n = new NextId();
                string numberPart = n.GenNextNumber(NextIDTables.Invoice, ConfigCategory.InvoiceGST, ConfigCategory.BILLNO,
                    user.DefaultCompanyId, user.FinYearId);
                Config cnfig = new Config();
                string prefix = cnfig.GetKeyValue(ConfigCategory.InvoiceGST, ConfigCategory.BILLNO, ConfigKey.Prefix, user.DefaultCompanyId, null);
                if (prefix == null)
                {
                    prefix = "";
                }
                string lastId = prefix + numberPart;
                return Request.CreateResponse(HttpStatusCode.OK, lastId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //string getNextNumber(BAL.Enums.NextIDTables table, string prefix, string start)
        //{
        //    NextId obj = new NextId();
        //    return obj.GetNextId(table, new LoggedInUser().FinYearId.ToString(), new LoggedInUser().DefaultCompanyId, start, prefix);

        //}
    }
}

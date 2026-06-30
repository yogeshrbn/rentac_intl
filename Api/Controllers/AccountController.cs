using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
using BAL.Common;
using FarmaAPI.Helper;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Generators;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class AccountController : ApiController
    {
        #region AccountGroup
        /// <summary>
        /// Adds a new accountgroup
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage SaveGroup([FromBody] AccountGroupDTO dto)
        {
            dto.AccountGroupId = SaveAccGroup(dto);
            return Request.CreateResponse(HttpStatusCode.OK, dto);

        }

        /// <summary>
        /// Saves the group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>AccountGrupId</returns>
        int SaveAccGroup(AccountGroupDTO dto)
        {
            AccountGroup objGroup = new AccountGroup
            {
                Name = dto.Name,
                ParentGroup = dto.ParentGroup,
                GroupCode = dto.GroupCode,
                StoreId = dto.StoreId,
                AccountGroupId = dto.AccountGroupId
            };
            objGroup.Save();
            return objGroup.AccountGroupId;
        }
        /// <summary>
        /// Adds a new accountgroup
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage GetAllGroups([FromBody] AccountGroupDTO dto)
        {
            AccountGroup objGroup = new AccountGroup();
            List<AccountGroupDTO> gruops = objGroup.GetAll(1);
            return Request.CreateResponse(HttpStatusCode.OK, gruops);
        }
        /// <summary>
        /// Adds a new accountgroup
        /// </summary>
        /// <param name="dto">AccountGroup DTO</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public HttpResponseMessage GetGroupInfo([FromBody] AccountGroupDTO dto)
        {
            AccountGroup objGroup = new AccountGroup(dto.AccountGroupId);
            return Request.CreateResponse(HttpStatusCode.OK, objGroup);
        }

        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpPost]
        public HttpResponseMessage UpdateGroupStatus([FromBody] AccountGroupDTO dto)
        {
            AccountGroup objGroup = new AccountGroup() { AccountGroupId = dto.AccountGroupId, IsActive = dto.IsActive };
            return Request.CreateResponse(HttpStatusCode.OK, objGroup.UpdateStatus());
        }

        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpPost]
        public HttpResponseMessage SaveLedger([FromBody] LedgerDTO dto)
        {
            Ledger leder = new Ledger(dto.LedgerId);
            DataCopier<LedgerDTO, Ledger> dcopier = new DataCopier<LedgerDTO, Ledger>();
            dcopier.CopyData(dto, leder);
            return Request.CreateResponse(HttpStatusCode.OK, leder.Save());
        }

        ///// <summary>
        /////Actiates and de-activates an account group
        ///// </summary>
        ///// <param name="dto">AccountGroupDTO</param>
        ///// <returns>true/false</returns>
        //[HttpPost]
        //public HttpResponseMessage SaveAddress([FromBody] LedgerDTO dto)
        //{
        //    Ledger leder = new Ledger(dto.LedgerId);
        //    DataCopier<LedgerDTO, Ledger> dcopier = new DataCopier<LedgerDTO, Ledger>();
        //    dcopier.CopyData(dto, leder);
        //    return Request.CreateResponse(HttpStatusCode.OK, leder.SaveAddress());
        //}
        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        //[HttpPost]
        //public HttpResponseMessage GetAllLedger([FromBody] LedgerDTO dto)
        //{
        //    Ledger leder = new Ledger(dto.LedgerId);
        //    DataCopier<LedgerDTO, Ledger> dcopier = new DataCopier<LedgerDTO, Ledger>();
        //    dcopier.CopyData(dto, leder);
        //    return Request.CreateResponse(HttpStatusCode.OK, leder.GetAll());
        //}
        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpGet]
        public HttpResponseMessage GetAllLedger(string name)
        {
            Ledger leder = new Ledger(0);
            leder.StoreId = 1;
            List<LedgerDTO> allLedgers = leder.GetAll(new LoggedInUser().DefaultCompanyId, true, name);
            SearchResult<LedgerDTO> objResult = new SearchResult<LedgerDTO> { total_count = allLedgers.Count, incomplete_results = false, items = allLedgers };

            return Request.CreateResponse(HttpStatusCode.OK, allLedgers);
        }

       

        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        [HttpPost]
        public HttpResponseMessage GetLedgerInfo([FromBody] LedgerDTO dto)
        {
            Ledger leder = new Ledger(dto.LedgerId);
            return Request.CreateResponse(HttpStatusCode.OK, leder);
        }
        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        //[HttpPost]
        //public HttpResponseMessage ChangeLedgerStatus([FromBody] LedgerDTO dto)
        //{
        //    Ledger leder = new Ledger(dto.LedgerId);
        //    leder.IsActive = dto.IsActive;
        //    return Request.CreateResponse(HttpStatusCode.OK, leder.ChangeStatus());
        //}
        /// <summary>
        ///Actiates and de-activates an account group
        /// </summary>
        /// <param name="dto">AccountGroupDTO</param>
        /// <returns>true/false</returns>
        //[HttpPost]
        //public HttpResponseMessage FindLedger([FromBody] LedgerDTO dto)
        //{
        //    Ledger leder = new Ledger(0);
        //    leder.StoreId = dto.StoreId;
        //    return Request.CreateResponse(HttpStatusCode.OK, leder.FindLedger(dto.Code, dto.Name));
        //}
        /// <summary>
        /// Selets all entry types for a particular Account group.
        /// This is being used in bank entry screen
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetGroupEntryTypes([FromBody] AccountGroupDTO dto)
        {
            AccountGroup group = new AccountGroup();
            return Request.CreateResponse(HttpStatusCode.OK, group.GetEntryTypes(dto.AccountGroupId));
        }
        [HttpPost]
        public HttpResponseMessage BankEntryTypes([FromBody] AccountGroupDTO dto)
        {
            AccountGroup group = new AccountGroup();
            return Request.CreateResponse(HttpStatusCode.OK, group.GetEntryTypes(18));
        }
        /// <summary>
        /// Selects all ledgers/accounts of a particular group. This is used at bank entry screen to select all banks.
        /// it can be used other places as well.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetAccountsByGroup([FromBody] AccountGroupDTO dto)
        {
            AccountGroup group = new AccountGroup();
            var user = new LoggedInUser();
            return Request.CreateResponse(HttpStatusCode.OK, group.GetAccountsByGroup(dto.AccountGroupId, user.DefaultCompanyId));
        }
        [HttpPost]
        public HttpResponseMessage GetBanks([FromBody] AccountGroupDTO dto)
        {
            AccountGroup group = new AccountGroup();
            var user = new LoggedInUser();
            return Request.CreateResponse(HttpStatusCode.OK, group.GetAccountsByGroup(Convert.ToInt16(BAL.Enums.AccountGroupType.Banks), user.DefaultCompanyId));
        }
        [HttpPost]
        public HttpResponseMessage PurchaseAccounts([FromBody] AccountGroupDTO dto)
        {
            AccountGroup group = new AccountGroup();
            var user = new LoggedInUser();
            return Request.CreateResponse(HttpStatusCode.OK, group.GetAccountsByGroup(Convert.ToInt16(BAL.Enums.AccountGroupType.Purchase), user.DefaultCompanyId));
        }


        [HttpPost]
        public HttpResponseMessage GetBankEntries([FromBody] FilterCriteria dto)
        {
            AccountGroup objGroup = new AccountGroup();
            String from = "", to = "";
            if (!String.IsNullOrEmpty(dto.From))
                from = Utils.FormatDate(dto.From).ToShortDateString();
            if (!String.IsNullOrEmpty(dto.To))
                to = Utils.FormatDate(dto.To).ToShortDateString();
            int companyId = new LoggedInUser().DefaultCompanyId;
            List<LedgerTransactionDTO> bEntries = objGroup.GetBankEntries(dto.BankId, dto.LedgerId, dto.LedgerSiteId, from, to,
                dto.EntryType, dto.TranRefNumber, companyId);
            return Request.CreateResponse(HttpStatusCode.OK, bEntries);
        }
        #endregion
    }
}

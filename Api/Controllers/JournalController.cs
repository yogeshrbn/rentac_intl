using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BAL.DTO;
using BAL.Objects;
namespace FarmaAPI.Controllers
{
    [Authorize]
    public class JournalController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage CreateEntry([FromBody] JournalDTO dto)
        {
            try
            {
                Journal objJournal = new Journal();
                objJournal.SiteId = dto.SiteId;
                objJournal.InvoiceNumber = dto.InvoiceNumber;
                objJournal.Remarks = dto.Remarks;
                objJournal.TransactionId = dto.TransactionId;
                objJournal.EntryDate = dto.EntryDate;
                objJournal.ChallanNumber = dto.ChequeNumber;
                objJournal.InvoiceNumber = dto.InvoiceNumber;
                objJournal.Amount = dto.Amount;
                objJournal.Bank = dto.Bank;
                objJournal.ChequeNumber = dto.ChequeNumber;
                objJournal.ClientId = dto.ClientId;
                objJournal.FinYearId = new LoggedInUser().FinYearId;
                bool success = objJournal.CreateEntry();
                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, objJournal.GetSiteJournal());
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Un-known error occurred");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetSiteJournals([FromBody] JournalDTO dto)
        {
            try
            {
                Journal objJournal = new Journal() { SiteId = dto.SiteId };
                return Request.CreateResponse(HttpStatusCode.OK, objJournal.GetSiteJournal());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetJournal([FromBody] JournalDTO dto)
        {
            try
            {
                Journal objJournal = new Journal() ;
                return Request.CreateResponse(HttpStatusCode.OK, objJournal.GetJournal(dto.FromDate,dto.ToDate));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

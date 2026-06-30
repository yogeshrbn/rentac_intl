using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class ClientSiteDTO : LedgerDTO
    {
        public string SiteAddress { get; set; }
        public string Address2 { get; set; }
        public int ProjectOwnerId { get; set; }
        public string ProjectOwnerName { get; set; }

        public int LedgerSiteId { get; set; }
        // public int StateId { get; set; }
        public string StateName { get; set; }
        public string Project { get; set; }

        public string SiteGST { get; set; }
        //public string State { get; set; }
        //public string City { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public double MaterialBalance { get; set; }
        public double BillBalance { get; set; }
        public List<LedgerTaxDTO> Taxes { get; set; }
        public double Total { get; set; }
        public double TotalReceived { get; set; }
        public double BalanceAmount { get; set; }
        public int Jobs { get; set; }
        public int TaxCategoryId { get; set; }

        public string Remarks { get; set; }

        public byte Closed { get; set; }
        public int ClosedBy { get; set; }
        public DateTime ClosedOn { get; set; }

        public string ClosedRemarks { get; set; }
        public string PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public byte PrintLastBillDetails { get;set; } = 0;
        public byte PrintBalanceMaterial { get; set; } = 0;
        public byte UseForBilling { get; set; } = 0;
        public int BillingToSiteId { get; set; } = 0;

        /// <summary>Stored file name under docs/sites/{LedgerId}/{LedgerSiteId}/ (slot 1).</summary>
        public string Document1FileName { get; set; }
        public string Document2FileName { get; set; }
        public string Document3FileName { get; set; }
        public string Document4FileName { get; set; }
        public string Document5FileName { get; set; }

    }


}

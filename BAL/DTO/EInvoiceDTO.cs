using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class EInvoiceDTO
    {
        public string TaxSch { get; set; }
        public string SupType { get; set; }

        /// <summary>
        /// Must be Y OR N
        /// </summary>
        public string IgstOnIntra { get; set; }

        public InvoiceDTO DocDtls { get; set; }

        public CompanyDTO SellerDtls { get; set; }
        public LedgerDTO BuyerDto { get; set; }

        public List<BillingItemDTO> ItemList { get; set; }

    }
}

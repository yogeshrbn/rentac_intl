using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class CompanyDTO : MasterDTO
    {
        int _companyId = 0;
        public int CompanyId { get { return _companyId; } set { _companyId = value; } }
        public int ParentCompanyId { get; set; }
        public bool IsBranch { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Contact { get; set; }
        public string ZipCode { get; set; }
        public short StateCode { get; set; }
        public string Web { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int StateId { get; set; }

        public string QrCode { get; set; }
        public bool IsActive { get; set; }
        public string GSTNo { get; set; }
        public string TIN { get; set; }
        public string TAN { get; set; }
        public string VAT { get; set; }

        public string PAN { get; set; }
        public string SignAuthority { get; set; }
        public string ReportHeader { get; set; }
        public string Logo { get; set; }
        public string Signature { get; set; }
        public string GSTRegistrationDate { get; set; }
        public string TradeName { get; set; }
        public string LegalName { get; set; }
        public string GSTStatus { get; set; }
        public DateTime EInvoiceStartDate { get; set; }
        public int EInvoiceEnabledBy { get; set; }
        public bool EInvoiceEnabled { get; set; }
        public DateTime EInvoiceEnabledOn { get; set; }
        public string IRPUserName { get; set; }
        public bool IRPConnected { get; set; }
        public string IRPToken { get; set; }
        public DateTime IRPTokenExpiry{ get; set; }
        public bool IRPTokenExpired { get; set; }

        public string IRPPassword { get; set; }
        public DateTime IRPUpdatedOn { get; set; }
        public int IRPUpdatedBy { get; set; }

        public string EwayUserName { get; set; }
        public bool EwayConnected { get; set; }
        public DateTime EwayLastAuthenticatedOn { get; set; }
        public string EwayPassword { get; set; }
        public bool EwayBillEnabled { get; set; }
        public string MSMENumber { get; set; }
        public string Warehouses { get; set; }
        public int DefaultWarehouseId { get; set; }

        public CompanyDTO(int companyId)
        {
            _companyId = companyId;
        }

        public CompanyDTO() { }

        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string IFSCCode { get; set; }
        public string BankAccNumber { get; set; }

        public string WhatsAppProvider { get; set; }
        public string GupShupKey { get; set; }
        public string DoubleTickKey { get; set; }


    }
}

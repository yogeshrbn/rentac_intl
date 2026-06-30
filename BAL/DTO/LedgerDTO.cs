using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class LedgerDTO : MasterDTO
    {
        public int LedgerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TradeName { get; set; }
        public byte UseTradeNameForBilling { get; set; }
        public byte ForQuotation { get; set; }
        public string GroupName { get; set; }

        public double Balance { get; set; }
        // public string Group { get; set; }
        // public double OpeningBalance { get; set; }
        public string DLNo { get; set; }
        public string DLExpDate { get; set; }
        //public string PAN { get; set; }
        public string STNo { get; set; }
        public string STExpDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string Web { get; set; }
        public string OffPhone { get; set; }
        public string Fax { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonOffPhone { get; set; }
        public string ContactPersonMobile { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Contact { get; set; }
        public string TIN { get; set; }
        public string TAN { get; set; }
        public Int16 AccountGroup { get; set; }
        public String GSTNo { get; set; }
        public double OpeningBal { get; set; }
        public Int16 TransType { get; set; }
        public short BalanceType { get; set; }
        public String AadharCard { get; set; }
        public String ServiceTaxNumber { get; set; }
        public String PAN { get; set; }
        public int CreditDays { get; set; }
        public double DefaultRate { get; set; }
        public int CompanyId { get; set; }
        public int StateId { get; set; }
        public short StateCode { get; set; }
        public byte SysDefined { get; set; }
        public AddressDTO BillingAddress { get; set; }
        public AddressDTO ShippingAddress { get; set; }
        public List<AddressDTO> Addresses { get; set; }

        public string ShipAddress1 { get; set; }
        public string ShipAddress2 { get; set; }
        public string ShipCity { get; set; }
        public int ShipStateId { get; set; }
        public string ShipZipCode { get; set; }
        public string ShipStateCode { get; set; }


        public LedgerDTO(int companyId)
        {
            LedgerId = companyId;
        }
        public LedgerDTO()
        {

        }

        public LedgerDTO ShallowCopy()
        {
            return (LedgerDTO)this.MemberwiseClone();
        }


         
    }
}

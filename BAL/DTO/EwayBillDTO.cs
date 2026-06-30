using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class EwayBillDTO
    {
        public string DocType { get; set; }
        public string DocSubType { get; set; }
        public string OtherTypeDesc { get; set; }

        public string DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public String GenGstin { get; set; }
        public string DelPinCode { get; set; }
        public string DelStateCode { get; set; }

        public string DelPlace { get; set; }
        public string CustomerName { get; set; }
        public int EwayBillId { get; set; }
        public int TransporterId { get; set; }
        public string TransporterName { get; set; }
        public string TransporterGST { get; set; }
        public string Status { get; set; }
        public short SubTypeId { get; set; }
        public short Distance { get; set; }
        /// <summary>Optional declared goods / invoice total for e-way (totInvValue). When set, used for portal generation instead of invoice total.</summary>
        public decimal? ApproximateValue { get; set; }
        public short TransportationMode { get; set; }
        public short TransactionType { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string TransporterDocNo { get; set; }
        public DateTime TransporterDocDate { get; set; }
        public int InvoiceId { get; set; }
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string GuId { get; set; }

        public string EwayBillNo { get; set; }
        public DateTime EwayBillDate { get; set; }
        public DateTime EwayBillValidUpTo { get; set; }
        public string EwayBillAlert { get; set; }

        public int EwayBillCreatedBy { get; set; }

        public List<BillingItemDTO> Items { get; set; }


        public string ShipToGST { get; set; }
        public string DispatchFromGST { get; set; }

        public EwayBillPartyDto Buyer { get; set; }
        public EwayBillPartyDto Seller { get; set; }


        public string ShipFromAddress { get; set; }
        public string ShipFromZipCode { get; set; }
        public string ShipFromCity { get; set; }
        public string ShipFromStateCode { get; set; }
        public int ShipFromStateId { get; set; }

        public string ShipToZipCode { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToAddress { get; set; }
        public int ShipToStateId { get; set; }

        public string ShipToStateCode { get; set; }



    }

    public class EwayBillPartyDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string TradeName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string GST { get; set; }
        public string StateName { get; set; }
        public int StateId { get; set; }
        public string StateCode { get; set; }

    }

    public class EwayBillFilterDto
    {
        public string DocType { get; set; }
        public int LedgerId { get; set; }
        public string Status { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int CompanyId { get; set; }

    }
    public class UdpateVehicleDto : EwayBillDTO
    {
        public string FromPlace { get; set; }
        public int FromStateId { get; set; }

        public string ReasonCode { get; set; }
        public string Remarks { get; set; }
    }

    public class CancelEwayBillDto : EwayBillDTO

    {
        public int CancelReasonCode { get; set; }
        public string CancelRemarks { get; set; }
        public int CancelledBy { get; set; }
        public DateTime CancelledOn { get; set; }
    }
    public class EwayBillToken
    {
        public string Username { get; set; }
        public string AuthToken { get; set; }
        public string ClientId { get; set; }

        public DateTime TokenExpiry { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int CompanyId { get; set; }
        public string Sek { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTO
{
    public class FilterCriteria
    {
        public int CompanyId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int ChallanType { get; set; }
        public string OnDate { get; set; }
        public int LedgerId { get; set; }
        public int ProductId { get; set; }
        public int CrLedgerId { get; set; }
        public int DrLedgerId { get; set; }
        public int AccountGroupId { get; set; }
        public Int16 EntryType { get; set; }
        public Int16 TransactionType { get; set; }
        public string TranRefNumber { get; set; }
        public int RbnClientId { get; set; }
        public String TransactionDate { get; set; }
        public int LedgerTransactionId { get; set; }
        public Boolean Print { get; set; }
        public Boolean Excel { get; set; }
        public Boolean Pdf { get; set; }

        public int WorkOrderId { get; set; }
        public int ContractId { get; set; }
        public int LedgerSiteId { get; set; }
        public double Amount { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public short ReminderType { get; set; }
        public string ReportName { get; set; }
        public int BankId { get; set; }
        public string Mobile { get; set; }

        public byte Closed { get; set; }
        public short InvoiceType { get; set; }

        public string VehicleNo { get; set; }

        public int TransporterId { get; set; }
        public int EmployeeId { get; set; }
        public int WarehouseId { get; set; }
        /// <summary>
        /// rent - for rental challans
        /// hire - for hire challans
        /// </summary>
        public string BalanceType { get; set; }

        public string PONumber { get; set; }
    }

    public class PartyFilter
    {
        public string From { get; set; }
        public string To { get; set; }
        public int CompanyId { get; set; }
    }

    public class FilterDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int LedgerId { get; set; }
        public int LedgerSiteId { get; set; }
        public short EntryType { get; set; }


    }
}

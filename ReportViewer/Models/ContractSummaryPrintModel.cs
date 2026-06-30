using System;
using System.Collections.Generic;
using BAL.DTO;

namespace ReportViewer.Models
{
    public class ContractSummaryChallanDelivery
    {
        public string ChallanNumber { get; set; }
        public DateTime SentDate { get; set; }
        public IReadOnlyList<WorkOrderItemDTO> Items { get; set; }
    }

    public class ContractSummaryChallanReturn
    {
        public string ChallanNumber { get; set; }
        public DateTime ChallanDate { get; set; }
        public IReadOnlyList<WorkOrderItemDTO> Items { get; set; }
    }

    public class ContractSummaryJobSection
    {
        public string ActivityLabel { get; set; }
        public int JobCardId { get; set; }
        public IReadOnlyList<ContractSummaryChallanDelivery> DeliveryChallans { get; set; }
        public IReadOnlyList<ContractSummaryChallanReturn> ReturnChallans { get; set; }
    }

    public class ContractSummaryInventoryRow
    {
        public string Product { get; set; }
        public double Sent { get; set; }
        public double Returned { get; set; }
        public double Balance { get; set; }
    }

    /// <summary>Data for PDF/HTML contract info summary (challans, inventory, bills vs payments).</summary>
    public class ContractSummaryPrintModel
    {
        public ContractViewDto Contract { get; set; }
        public IReadOnlyList<ContractSummaryJobSection> JobSections { get; set; }
        public IReadOnlyList<ContractSummaryInventoryRow> Inventory { get; set; }
        public IReadOnlyList<BillingDTO> Bills { get; set; }
        public IReadOnlyList<LedgerTransactionDTO> Payments { get; set; }
        public double BilledTotal { get; set; }
        public double PaidTotal { get; set; }
        public double BalanceBilledVsPaid { get; set; }
        public string ErrorMessage { get; set; }
    }
}

namespace BAL.DTO
{
    public class AccountsSiteOutstandingDto
    {
        public int ContractId { get; set; }
        public string PartyName { get; set; }
        public string Site { get; set; }
        public decimal ProjectValue { get; set; }
        public decimal Billed { get; set; }
        public decimal Received { get; set; }
        public decimal Pending { get; set; }
        public decimal RetentionPercent { get; set; }
    }
}

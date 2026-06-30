namespace BAL.DTO
{
    public class AccountsAgeingSummaryDto
    {
        public decimal Bucket0To30 { get; set; }
        public decimal Bucket31To60 { get; set; }
        public decimal Bucket61To90 { get; set; }
        public decimal Bucket90Plus { get; set; }
    }
}

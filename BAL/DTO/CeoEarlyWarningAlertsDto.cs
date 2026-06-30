namespace BAL.DTO
{
    public class CeoEarlyWarningAlertsDto
    {
        public int ProjectMarginBelow20 { get; set; }
        public int CustomerOverdue60Days { get; set; }
        public decimal IdleStockPercent { get; set; }
        public decimal LabourCostPercent { get; set; }
        public int DamageAboveThreshold { get; set; }
    }
}


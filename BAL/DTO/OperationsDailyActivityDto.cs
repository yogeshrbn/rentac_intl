using System;

namespace BAL.DTO
{
    public class OperationsDailyActivityDto
    {
        public DateTime ActivityDate { get; set; }
        public int InstallationCount { get; set; }
        public int DismantlingCount { get; set; }
    }
}

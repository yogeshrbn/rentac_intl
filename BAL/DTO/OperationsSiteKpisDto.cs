namespace BAL.DTO
{
    public class OperationsSiteKpisDto
    {
        public int ActiveSites { get; set; }
        public int CompletedSites { get; set; }
        public int DelayedSites { get; set; }
        public int IdleSites { get; set; }
        public double InstallationPerTeamPerDay { get; set; }
        public double DismantlingPerDay { get; set; }
    }
}

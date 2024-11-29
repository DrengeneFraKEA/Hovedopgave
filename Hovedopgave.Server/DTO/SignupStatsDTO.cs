namespace Hovedopgave.Server.DTO
{
    public class SignupStatsDTO
    {
        public int TotalSignups { get; set; }
        public int UserSignups { get; set; }
        public int TeamSignups { get; set; }
        public int OrganizationSignups { get; set; }
        public int DailySignups { get; set; }
        public int WeeklySignups { get; set; }
        public int MonthlySignups { get; set; }
    }
}

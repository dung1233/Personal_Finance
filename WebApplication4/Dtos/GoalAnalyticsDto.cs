namespace WebApplication4.Dtos
{
    public class GoalAnalyticsDto
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public double ProgressPercentage { get; set; }
        public int ContributionCount { get; set; }
        public DateTime? LastContributionDate { get; set; }
        public decimal RemainingAmount { get; set; }
    }

}

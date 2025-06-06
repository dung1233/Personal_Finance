namespace WebApplication4.Dtos
{
    public class GoalStatisticsDto
    {
        public int TotalGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int ActiveGoals { get; set; }
        public decimal TotalTargetAmount { get; set; }
        public decimal TotalCurrentAmount { get; set; }
        public double AverageCompletionRate { get; set; }
    }

}

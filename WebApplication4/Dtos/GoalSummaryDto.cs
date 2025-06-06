namespace WebApplication4.Dtos
{
    public class GoalSummaryDto
    {
        public int TotalGoals { get; set; }

        public int CompletedGoals { get; set; }

        public int ActiveGoals { get; set; }

        public int OverdueGoals { get; set; }

        public decimal TotalTargetAmount { get; set; }

        public decimal TotalSavedAmount { get; set; }
    }
}

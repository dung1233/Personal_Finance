namespace WebApplication4.Dtos
{
    public class BudgetPerformanceDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public double PercentageUsed { get; set; }
        public string Status { get; set; } = string.Empty; // "under", "near", "over"
    }

}

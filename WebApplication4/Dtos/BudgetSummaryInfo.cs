namespace WebApplication4.Dtos
{
    public class BudgetSummaryInfo
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = null!;
        public decimal RemainingAmount { get; set; }
    }
}

namespace WebApplication4.Dtos
{
    public class BudgetSummaryDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal AlertThreshold { get; set; } // Đổi từ double thành decimal
        public bool IsAlert { get; set; }
    }
}
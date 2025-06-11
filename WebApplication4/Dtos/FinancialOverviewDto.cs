namespace WebApplication4.Dtos
{
    public class FinancialOverviewDto
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalRemaining { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }
        public decimal MonthlyNetIncome { get; set; }
        public double BudgetUtilizationRate { get; set; }
        public double FinancialHealthScore { get; set; }
    }
}

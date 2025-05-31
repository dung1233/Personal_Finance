namespace WebApplication4.Dtos
{
    public class BudgetAlertDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal AlertThreshold { get; set; }
        public double PercentageSpent { get; set; }
    }

}

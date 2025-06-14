namespace WebApplication4.Dtos
{
    public class BudgetDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = null!;
        public decimal BudgetAmount { get; set; }
        public string BudgetPeriod { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal AlertThreshold { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }

    }

 


}

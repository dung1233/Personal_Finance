namespace WebApplication4.Dtos
{
    public class BudgetUpdateDto
    {
        public int BudgetId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal AlertThreshold { get; set; }
        public bool IsActive { get; set; }
    }

}

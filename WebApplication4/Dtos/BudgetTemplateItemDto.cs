namespace WebApplication4.Dtos
{
    public class BudgetTemplateItemDto
    {
        public int CategoryId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal AlertThreshold { get; set; }
    }

    public class BudgetTemplateRequestDto
    {
        public string BudgetPeriod { get; set; } = "Monthly";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<BudgetTemplateItemDto> Templates { get; set; } = new();
    }

}

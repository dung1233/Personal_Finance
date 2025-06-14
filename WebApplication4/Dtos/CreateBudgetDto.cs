using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    public class CreateBudgetDto
    {
        [Required]
        public string BudgetName { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal BudgetAmount { get; set; }

        [Required]
        [RegularExpression("Weekly|Monthly|Quarterly|Yearly")]
        public string BudgetPeriod { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal AlertThreshold { get; set; } = 80m;

        public int CategoryId { get; set; }
        public int AccountId { get; set; }
    }

}

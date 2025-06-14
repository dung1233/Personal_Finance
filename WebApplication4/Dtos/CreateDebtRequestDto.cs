using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    public class CreateDebtRequestDto
    {
        [Required]
        public string DebtName { get; set; } = null!;

        [Required]
        [RegularExpression("^(Credit Card|Student Loan|Mortgage|Personal Loan|Auto Loan|Other)$", ErrorMessage = "Invalid DebtType")]
        public string DebtType { get; set; } = null!;

        public string? Creditor { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "OriginalAmount must be greater than 0")]
        public decimal OriginalAmount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "CurrentBalance must be greater than 0")]
        public decimal CurrentBalance { get; set; }

        public decimal? InterestRate { get; set; }

        public decimal? MinimumPayment { get; set; }

        public int? PaymentDueDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        public DateTime? PayoffDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid AccountId")]
        public int? AccountId { get; set; }
    }
}
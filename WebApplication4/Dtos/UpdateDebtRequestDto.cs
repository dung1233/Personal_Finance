using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    public class UpdateDebtRequestDto
    {
        public string? DebtName { get; set; }

        [RegularExpression("^(Credit Card|Student Loan|Mortgage|Personal Loan|Auto Loan|Other)$", ErrorMessage = "Invalid DebtType")]
        public string? DebtType { get; set; }

        public string? Creditor { get; set; }

        public decimal? InterestRate { get; set; }

        public decimal? MinimumPayment { get; set; }

        public int? PaymentDueDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        public DateTime? PayoffDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid AccountId")]
        public int? AccountId { get; set; }
    }
}
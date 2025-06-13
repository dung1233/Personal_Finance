using WebApplication4.Enums;

namespace WebApplication4.Dtos
{
    public class CreateLoanRequestDto
    {
        public string BorrowerName { get; set; }
        public string? BorrowerPhone { get; set; }
        public string? BorrowerEmail { get; set; }
        public string LoanName { get; set; }
        public string LoanType { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? PaymentFrequency { get; set; }
        public decimal? MinimumPayment { get; set; }
        public string? Notes { get; set; }

        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    }
}

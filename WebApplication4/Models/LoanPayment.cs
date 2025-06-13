using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class LoanPayment
    {
        [Key]
        public int PaymentId { get; set; }
        public int LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public decimal LateFee { get; set; }
        public decimal? RemainingBalance { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string? Notes { get; set; }
        public string? Receipt { get; set; }
        public DateTime CreatedAt { get; set; }

        public Loan Loan { get; set; }
    }

}

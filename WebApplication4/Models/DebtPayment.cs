using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class DebtPayment
    {
        [Key]
        public int PaymentId { get; set; }
        public int DebtId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public Debt Debt { get; set; } = null!;
    }

}

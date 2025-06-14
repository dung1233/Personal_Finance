using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class DebtPayment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int DebtId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentAmount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrincipalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestAmount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // === Navigation Property ===
        public Debt Debt { get; set; }
    }
}

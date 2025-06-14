using System;

namespace WebApplication4.Dtos
{
    public class DebtResponseDto
    {
        public int DebtId { get; set; }

        public string DebtName { get; set; }

        public string DebtType { get; set; } // e.g., "Credit Card", "Personal Loan", etc.

        public string? Creditor { get; set; }

        public decimal OriginalAmount { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal? InterestRate { get; set; }

        public decimal? MinimumPayment { get; set; }

        /// <summary>
        /// Ngày đến hạn trong tháng (1-31)
        /// </summary>
        public int? PaymentDueDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        public DateTime? PayoffDate { get; set; }

        public bool IsActive { get; set; }
    }
}

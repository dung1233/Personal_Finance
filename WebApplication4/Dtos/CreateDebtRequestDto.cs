namespace WebApplication4.Dtos
{
    public class CreateDebtRequestDto
    {
        public string DebtName { get; set; } = null!;
        public string DebtType { get; set; } = null!;
        public string? Creditor { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MinimumPayment { get; set; }
        public int? PaymentDueDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public DateTime? PayoffDate { get; set; }
    }

}

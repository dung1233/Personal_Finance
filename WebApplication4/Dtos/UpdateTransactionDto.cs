namespace WebApplication4.Dtos.Transactions
{
    public class UpdateTransactionDto
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public string? Merchant { get; set; }
        public string? Tags { get; set; }
    }
}

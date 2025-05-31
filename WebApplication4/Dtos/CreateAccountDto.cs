namespace WebApplication4.Dtos
{
    public class CreateAccountDto
    {
        public string AccountName { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public string? BankName { get; set; }
        public string AccountNumber { get; set; } = null!;
        public decimal? InitialBalance { get; set; } = 0;
        public decimal? CreditLimit { get; set; }
        public string Currency { get; set; } = "USD";
    }
}

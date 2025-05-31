namespace WebApplication4.Dtos
{
    public class UpdateAccountDto
    {
        public string AccountName { get; set; } = null!;
        public string? BankName { get; set; }
        public decimal? CreditLimit { get; set; }
        public string Currency { get; set; } = "USD";
    }
}

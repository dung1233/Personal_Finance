namespace WebApplication4.Dtos
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public string? BankName { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

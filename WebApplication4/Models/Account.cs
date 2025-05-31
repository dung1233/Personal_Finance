namespace WebApplication4.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string AccountName { get; set; } = null!;
        public string? AccountType { get; set; } = null!;
        public string? BankName { get; set; }
        public string AccountNumber { get; set; } = null!; // sẽ mã hóa khi lưu
        public decimal Balance { get; set; }
        public decimal? CreditLimit { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

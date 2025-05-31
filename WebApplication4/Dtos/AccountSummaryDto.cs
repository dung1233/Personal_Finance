namespace WebApplication4.Dtos
{
    public class AccountSummaryDto
    {
        public decimal TotalBalance { get; set; }
        public Dictionary<string, decimal> BalanceByCurrency { get; set; } = new();
    }
}

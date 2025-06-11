namespace WebApplication4.Dtos
{
    public class AccountBalanceAlertDto
    {
        public string AlertType { get; set; } = null!; // INSUFFICIENT_TOTAL_BALANCE, INSUFFICIENT_ACCOUNT_BALANCE, BUDGET_OVERSPENDING_RISK
        public string Message { get; set; } = null!;

        // Thông tin tài khoản (nếu có)
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public decimal? AccountBalance { get; set; }

        // Thông tin budget (nếu có)
        public int? BudgetId { get; set; }
        public string? BudgetName { get; set; }
        public decimal? CurrentSpending { get; set; }
        public decimal? RemainingAmount { get; set; }
        public int? DaysLeft { get; set; }
        public decimal? SuggestedDailyLimit { get; set; }

        // Thông tin tài chính tổng quan
        public decimal? TotalBalance { get; set; }
        public decimal? RequiredAmount { get; set; }
        public decimal? Shortage { get; set; }

        // Chi tiết budget
        public List<BudgetSummaryInfo>? BudgetDetails { get; set; }
    }
}

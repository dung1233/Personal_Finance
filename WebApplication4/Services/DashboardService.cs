using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverviewResponseDto> GetOverviewAsync(int userId)
        {
            var totalIncome = await _context.Transactions
     .Where(t => t.UserId == userId && t.TransactionType == "Income")
     .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var totalExpense = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionType == "Expense")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var totalAssets = await _context.Accounts
                .Where(a => a.UserId == userId)
                .SumAsync(a => (decimal?)a.Balance) ?? 0;

            var netWorth = totalAssets;


            return new DashboardOverviewResponseDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                NetWorth = netWorth
            };
        }
        public async Task<AccountsSummaryResponseDto> GetAccountsSummaryAsync(int userId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .Select(a => new AccountBalanceDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Balance = a.Balance
                }).ToListAsync();

            return new AccountsSummaryResponseDto
            {
                Accounts = accounts
            };
        }


    }

}

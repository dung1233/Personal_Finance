using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;
using WebApplication4.Services;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication4.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        private string Encrypt(string input)
        {
            // Đơn giản hóa, bạn nên dùng thứ mạnh hơn trong thực tế
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<List<AccountDto>> GetAccountsAsync(int userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .Select(a => new AccountDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType,
                    BankName = a.BankName,
                    Balance = a.Balance,
                    Currency = a.Currency,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<AccountDto?> GetAccountByIdAsync(int userId, int accountId)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.AccountId == accountId && a.IsActive);
            if (acc == null) return null;

            return new AccountDto
            {
                AccountId = acc.AccountId,
                AccountName = acc.AccountName,
                AccountType = acc.AccountType,
                BankName = acc.BankName,
                Balance = acc.Balance,
                Currency = acc.Currency,
                IsActive = acc.IsActive,
                CreatedAt = acc.CreatedAt
            };
        }

        public async Task<AccountDto> CreateAccountAsync(int userId, CreateAccountDto dto)
        {
            // Check trùng tên
            var existing = await _context.Accounts.AnyAsync(a => a.UserId == userId && a.AccountName == dto.AccountName && a.IsActive);
            if (existing)
                throw new Exception("Account name already exists.");

            var account = new Account
            {
                UserId = userId,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                BankName = dto.BankName,
                AccountNumber = Encrypt(dto.AccountNumber),
                Balance = dto.InitialBalance ?? 0,
                CreditLimit = dto.CreditLimit ?? 0,
                Currency = dto.Currency,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new AccountDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountType = account.AccountType,
                BankName = account.BankName,
                Balance = account.Balance,
                Currency = account.Currency,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt
            };
        }

        public async Task<bool> UpdateAccountAsync(int userId, int accountId, UpdateAccountDto dto)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.AccountId == accountId && a.IsActive);
            if (acc == null) return false;

            acc.AccountName = dto.AccountName;
            acc.BankName = dto.BankName;
            acc.CreditLimit = dto.CreditLimit ?? acc.CreditLimit;
            acc.Currency = dto.Currency;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int userId, int accountId)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.AccountId == accountId);
            if (acc == null) return false;

            acc.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBalanceAsync(int userId, int accountId, AccountBalanceDto dto)
        {
            var acc = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.AccountId == accountId && a.IsActive);
            if (acc == null) return false;

            acc.Balance = dto.NewBalance;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AccountSummaryDto> GetSummaryAsync(int userId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .ToListAsync();

            var result = new AccountSummaryDto
            {
                TotalBalance = accounts.Sum(a => a.Balance),
                BalanceByCurrency = accounts
                    .GroupBy(a => a.Currency)
                    .ToDictionary(g => g.Key, g => g.Sum(a => a.Balance))
            };

            return result;
        }

        public Task<List<string>> GetAccountTypesAsync()
        {
            return Task.FromResult(new List<string>
            {
                "Checking", "Savings", "Credit Card", "Investment", "Cash", "Other"
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Dtos.Transactions;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class TransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> GetTransactionsAsync(int userId)

        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .Include(t => t.Account)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.TransactionId,  // sửa ở đây
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Merchant = t.Merchant,
                Tags = t.Tags,
                CategoryName = t.Category?.Name,
                AccountName = t.Account?.AccountName,
                IsRecurring = t.IsRecurring
            }).ToList();

        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int userId, int id)
        {
            var t = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.UserId == userId);

            if (t == null) return null;

            return new TransactionDto
            {
                Id = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Merchant = t.Merchant,
                Tags = t.Tags,
                CategoryName = t.Category?.Name,
                AccountName = t.Account?.AccountName,
                IsRecurring = t.IsRecurring
            };
        }

        public async Task<bool> CreateTransactionAsync(int userId, CreateTransactionDto dto)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == dto.AccountId && a.UserId == userId);
            if (account == null) return false;

            var transaction = new Transaction
            {
                UserId = userId,
                AccountId = dto.AccountId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                TransactionType = dto.TransactionType,
                TransactionDate = dto.TransactionDate,
                Description = dto.Description,
                Merchant = dto.Merchant,
                Tags = dto.Tags,
                IsRecurring = dto.IsRecurring,
                RecurringFrequency = dto.RecurringFrequency,
                CreatedAt = DateTime.UtcNow
            };

            // Cập nhật số dư tài khoản
            if (dto.TransactionType == "Expense")
                account.Balance -= dto.Amount;
            else if (dto.TransactionType == "Income")
                account.Balance += dto.Amount;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int userId, int id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id && t.UserId == userId);
            if (transaction == null) return false;

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == transaction.AccountId && a.UserId == userId);
            if (account == null) return false;

            // Khôi phục số dư
            if (transaction.TransactionType == "Expense")
                account.Balance += transaction.Amount;
            else if (transaction.TransactionType == "Income")
                account.Balance -= transaction.Amount;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<TransactionDto>> SearchTransactionsAsync(int userId, string? keyword)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t =>
                    t.Description!.Contains(keyword) ||
                    t.Merchant!.Contains(keyword) ||
                    t.Tags!.Contains(keyword)
                );
            }

            var transactions = await query.OrderByDescending(t => t.TransactionDate).ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Merchant = t.Merchant,
                Tags = t.Tags,
                CategoryName = t.Category?.Name,
                AccountName = t.Account?.AccountName,
                IsRecurring = t.IsRecurring
            }).ToList();
        }
        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(5)
                .Include(t => t.Category)
                .Include(t => t.Account)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Merchant = t.Merchant,
                Tags = t.Tags,
                CategoryName = t.Category?.Name,
                AccountName = t.Account?.AccountName,
                IsRecurring = t.IsRecurring
            }).ToList();
            // giống như trên
        }
        public async Task<int> ImportBulkTransactionsAsync(int userId, List<CreateTransactionDto> transactions)
        {
            int count = 0;

            foreach (var dto in transactions)
            {
                var success = await CreateTransactionAsync(userId, dto);
                if (success) count++;
            }

            return count;
        }
        public async Task<List<TransactionDto>> GetRecurringTransactionsAsync(int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.IsRecurring)
                .Include(t => t.Category)
                .Include(t => t.Account)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.TransactionId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Merchant = t.Merchant,
                Tags = t.Tags,
                CategoryName = t.Category?.Name,
                AccountName = t.Account?.AccountName,
                IsRecurring = t.IsRecurring
            }).ToList();
        }
        public async Task<bool> UpdateTransactionCategoryAsync(int userId, int transactionId, int categoryId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId && t.UserId == userId);
            if (transaction == null) return false;

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.UserId == userId);
            if (category == null) return false;

            transaction.CategoryId = categoryId;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> TransferAsync(int userId, TransferDto dto)
        {
            if (dto.FromAccountId == dto.ToAccountId || dto.Amount <= 0) return false;

            var from = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == dto.FromAccountId && a.UserId == userId);
            var to = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == dto.ToAccountId && a.UserId == userId);
            if (from == null || to == null || from.Balance < dto.Amount) return false;

            // Trừ tiền
            from.Balance -= dto.Amount;
            to.Balance += dto.Amount;

            var outTransaction = new Transaction
            {
                UserId = userId,
                AccountId = dto.FromAccountId,
                CategoryId = null,
                Amount = dto.Amount,
                TransactionType = "Transfer", // ✅ sửa lại
                TransactionDate = dto.Date,
                Description = dto.Description ?? $"Transfer to {to.AccountName}",
                CreatedAt = DateTime.UtcNow
            };

            var inTransaction = new Transaction
            {
                UserId = userId,
                AccountId = dto.ToAccountId,
                CategoryId = null,
                Amount = dto.Amount,
                TransactionType = "Transfer", // ✅ sửa lại
                TransactionDate = dto.Date,
                Description = dto.Description ?? $"Transfer from {from.AccountName}",
                CreatedAt = DateTime.UtcNow
            };


            _context.Transactions.AddRange(outTransaction, inTransaction);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<TransactionSummaryDto> GetTransactionSummaryAsync(int userId, DateTime from, DateTime to)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionDate >= from && t.TransactionDate <= to)
                .ToListAsync();

            return new TransactionSummaryDto
            {
                TotalExpense = transactions.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount),
                TotalIncome = transactions.Where(t => t.TransactionType == "Income").Sum(t => t.Amount)
            };
        }




    }
}

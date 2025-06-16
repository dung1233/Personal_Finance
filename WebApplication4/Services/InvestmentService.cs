using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly AppDbContext _context;

        public InvestmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InvestmentDto>> GetAllAsync()
        {
            return await _context.Investments
                .Include(i => i.Account)
                .Select(i => new InvestmentDto
                {
                    InvestmentId = i.InvestmentId,
                    UserId = i.UserId,
                    InvestmentName = i.InvestmentName,
                    InvestmentType = i.InvestmentType.ToString(),
                    Symbol = i.Symbol,
                    Quantity = i.Quantity,
                    PurchasePrice = i.PurchasePrice,
                    CurrentPrice = i.CurrentPrice,
                    PurchaseDate = i.PurchaseDate,
                    TotalInvested = i.TotalInvested,
                    CurrentValue = i.CurrentValue,
                    Broker = i.Broker,
                    IsActive = i.IsActive,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    AccountId = i.AccountId,
                    AccountName = i.Account != null ? i.Account.AccountName : null,
                    TotalGainLoss = i.TotalGainLoss,
                    GainLossPercentage = i.GainLossPercentage,
                    PriceChange = i.PriceChange,
                    PriceChangePercentage = i.PriceChangePercentage
                })
                .ToListAsync();
        }

        public async Task<InvestmentDto?> GetByIdAsync(int id)
        {
            var i = await _context.Investments.Include(x => x.Account).FirstOrDefaultAsync(x => x.InvestmentId == id);
            if (i == null) return null;
            return new InvestmentDto
            {
                InvestmentId = i.InvestmentId,
                UserId = i.UserId,
                InvestmentName = i.InvestmentName,
                InvestmentType = i.InvestmentType.ToString(),
                Symbol = i.Symbol,
                Quantity = i.Quantity,
                PurchasePrice = i.PurchasePrice,
                CurrentPrice = i.CurrentPrice,
                PurchaseDate = i.PurchaseDate,
                TotalInvested = i.TotalInvested,
                CurrentValue = i.CurrentValue,
                Broker = i.Broker,
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                AccountId = i.AccountId,
                AccountName = i.Account != null ? i.Account.AccountName : null,
                TotalGainLoss = i.TotalGainLoss,
                GainLossPercentage = i.GainLossPercentage,
                PriceChange = i.PriceChange,
                PriceChangePercentage = i.PriceChangePercentage
            };
        }

        public async Task<InvestmentDto> CreateAsync(CreateInvestmentDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validation dữ liệu đầu vào
                if (dto.TotalInvested != dto.Quantity * dto.PurchasePrice)
                    throw new Exception("TotalInvested must equal Quantity * PurchasePrice");
                if (dto.CurrentValue.HasValue && dto.CurrentValue != dto.Quantity * dto.CurrentPrice)
                    throw new Exception("CurrentValue must equal Quantity * CurrentPrice");

                // Tạo bản ghi Investment
                var investment = new Investment
                {
                    UserId = userId,
                    InvestmentName = dto.InvestmentName,
                    InvestmentType = dto.InvestmentType,
                    Symbol = dto.Symbol,
                    Quantity = dto.Quantity,
                    PurchasePrice = dto.PurchasePrice,
                    CurrentPrice = dto.CurrentPrice,
                    PurchaseDate = dto.PurchaseDate,
                    TotalInvested = dto.TotalInvested,
                    CurrentValue = dto.CurrentValue,
                    Broker = dto.Broker,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AccountId = dto.AccountId
                };
                _context.Investments.Add(investment);

                if (dto.AccountId.HasValue && dto.CurrentValue.HasValue)
                {
                    // Kiểm tra tài khoản
                    var account = await _context.Accounts.FindAsync(dto.AccountId.Value);
                    if (account == null || account.UserId != userId)
                        throw new Exception("Tài khoản không tồn tại hoặc không thuộc người dùng");

                    // Xử lý lãi/lỗ nếu currentValue khác totalInvested
                    var gainLoss = dto.CurrentValue.Value - dto.TotalInvested;
                    if (Math.Abs(gainLoss) > 0.01m)
                    {
                        int categoryId;
                        string transactionType;
                        string description;

                        if (gainLoss > 0)
                        {
                            transactionType = "Income";
                            description = $"Tiền lãi từ đầu tư: {dto.InvestmentName}";
                            var profitCategory = await _context.Categories
                                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == "Lãi đầu tư");
                            if (profitCategory == null)
                            {
                                profitCategory = new Category
                                {
                                    UserId = userId,
                                    Name = "Lãi đầu tư",
                                    Description = "Thu nhập từ hoạt động đầu tư",
                                    CategoryType = "Income",
                                    Color = "#228B22",
                                    Icon = "dollar-sign",
                                    IsDefault = true,
                                    IsActive = true,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };
                                _context.Categories.Add(profitCategory);
                                await _context.SaveChangesAsync();
                            }
                            categoryId = profitCategory.CategoryId;
                        }
                        else
                        {
                            transactionType = "Expense";
                            description = $"Tiền lỗ từ đầu tư: {dto.InvestmentName}";
                            var lossCategory = await _context.Categories
                                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == "Lỗ đầu tư");
                            if (lossCategory == null)
                            {
                                lossCategory = new Category
                                {
                                    UserId = userId,
                                    Name = "Lỗ đầu tư",
                                    Description = "Khoản lỗ phát sinh từ đầu tư",
                                    CategoryType = "Expense",
                                    Color = "#B22222",
                                    Icon = "trending-down",
                                    IsDefault = true,
                                    IsActive = true,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };
                                _context.Categories.Add(lossCategory);
                                await _context.SaveChangesAsync();
                            }
                            categoryId = lossCategory.CategoryId;
                        }

                        var gainLossTransaction = new Transaction
                        {
                            UserId = userId,
                            AccountId = dto.AccountId.Value,
                            CategoryId = categoryId,
                            Amount = Math.Abs(gainLoss),
                            TransactionType = transactionType,
                            Description = description,
                            TransactionDate = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.Transactions.Add(gainLossTransaction);

                        // Cập nhật số dư tài khoản
                        if (gainLoss > 0)
                            account.Balance += Math.Abs(gainLoss);
                        else
                            account.Balance -= Math.Abs(gainLoss);

                        // Đảm bảo tài khoản được cập nhật
                        _context.Entry(account).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetByIdAsync(investment.InvestmentId) ?? throw new Exception("Tạo thất bại");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Lỗi khi tạo đầu tư: {ex.Message}");
            }
        }
        public async Task<bool> UpdateAsync(int id, UpdateInvestmentDto dto)
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null) return false;

            // Cập nhật thông tin đầu tư
            investment.InvestmentName = dto.InvestmentName;
            investment.InvestmentType = dto.InvestmentType;
            investment.Symbol = dto.Symbol;
            investment.Quantity = dto.Quantity;
            investment.PurchasePrice = dto.PurchasePrice;
            investment.CurrentPrice = dto.CurrentPrice;
            investment.PurchaseDate = dto.PurchaseDate;
            investment.TotalInvested = dto.TotalInvested;
            investment.CurrentValue = dto.CurrentValue;
            investment.Broker = dto.Broker;
            investment.IsActive = dto.IsActive;
            investment.UpdatedAt = DateTime.UtcNow;
            investment.AccountId = dto.AccountId;

            // Xử lý giao dịch lãi/lỗ
            if (dto.AccountId.HasValue && dto.CurrentValue.HasValue)
            {
                var gainLoss = dto.CurrentValue.Value - dto.TotalInvested;

                // Nếu lãi hoặc lỗ khác 0 thì tạo transaction
                if (Math.Abs(gainLoss) > 0.01m)
                {
                    int categoryId;
                    string transactionType;
                    string description;

                    if (gainLoss > 0)
                    {
                        // Lãi: cộng vào tài khoản
                        transactionType = "Income";
                        description = $"Tiền lãi từ đầu tư: {investment.InvestmentName}";

                        // Tìm hoặc tạo category "Lãi đầu tư"
                        var profitCategory = await _context.Categories
                            .FirstOrDefaultAsync(c => c.UserId == investment.UserId && c.Name == "Lãi đầu tư");
                        if (profitCategory == null)
                        {
                            profitCategory = new Category
                            {
                                UserId = investment.UserId,
                                Name = "Lãi đầu tư",
                                Description = "Thu nhập từ hoạt động đầu tư",
                                CategoryType = "Income",
                                Color = "#228B22",
                                Icon = "dollar-sign",
                                IsDefault = true,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            _context.Categories.Add(profitCategory);
                            await _context.SaveChangesAsync();
                        }
                        categoryId = profitCategory.CategoryId;
                    }
                    else
                    {
                        // Lỗ: trừ khỏi tài khoản
                        transactionType = "Expense";
                        description = $"Tiền lỗ từ đầu tư: {investment.InvestmentName}";

                        // Tìm hoặc tạo category "Lỗ đầu tư"
                        var lossCategory = await _context.Categories
                            .FirstOrDefaultAsync(c => c.UserId == investment.UserId && c.Name == "Lỗ đầu tư");
                        if (lossCategory == null)
                        {
                            lossCategory = new Category
                            {
                                UserId = investment.UserId,
                                Name = "Lỗ đầu tư",
                                Description = "Khoản lỗ phát sinh từ đầu tư",
                                CategoryType = "Expense",
                                Color = "#B22222",
                                Icon = "trending-down",
                                IsDefault = true,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            _context.Categories.Add(lossCategory);
                            await _context.SaveChangesAsync();
                        }
                        categoryId = lossCategory.CategoryId;
                    }

                    var transaction = new Transaction
                    {
                        UserId = investment.UserId,
                        AccountId = dto.AccountId.Value,
                        CategoryId = categoryId,
                        Amount = Math.Abs(gainLoss),
                        TransactionType = transactionType,
                        Description = description,
                        TransactionDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Transactions.Add(transaction);

                    // Cập nhật số dư tài khoản
                    var account = await _context.Accounts.FindAsync(dto.AccountId.Value);
                    if (account != null)
                    {
                        if (gainLoss > 0)
                            account.Balance += Math.Abs(gainLoss);
                        else
                            account.Balance -= Math.Abs(gainLoss);

                        account.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null) return false;

            // Khi xóa đầu tư, có thể cần hoàn trả số tiền đã đầu tư về tài khoản
            // Tùy thuộc vào logic business của bạn
            if (investment.AccountId.HasValue)
            {
                var account = await _context.Accounts.FindAsync(investment.AccountId.Value);
                if (account != null)
                {
                    // Hoàn trả giá trị hiện tại của đầu tư (nếu có)
                    var returnAmount = investment.CurrentValue ?? investment.TotalInvested;
                    account.Balance += returnAmount;
                    account.UpdatedAt = DateTime.UtcNow;

                    // Tạo transaction ghi nhận việc thu hồi đầu tư
                    var transaction = new Transaction
                    {
                        UserId = investment.UserId,
                        AccountId = investment.AccountId.Value,
                        CategoryId = 10, // Lãi đầu tư (hoặc tạo category riêng cho "Thu hồi đầu tư")
                        Amount = returnAmount,
                        TransactionType = "Income",
                        Description = $"Thu hồi đầu tư: {investment.InvestmentName}",
                        TransactionDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Transactions.Add(transaction);
                }
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InvestmentSummaryDto> GetSummaryAsync()
        {
            var investments = await _context.Investments
                .Where(i => i.IsActive) // Chỉ lấy đầu tư đang hoạt động
                .Include(i => i.Account)
                .ToListAsync();

            if (!investments.Any())
            {
                return new InvestmentSummaryDto
                {
                    TotalInvested = 0,
                    TotalCurrentValue = 0,
                    TotalGainLoss = 0,
                    TotalGainLossPercentage = 0,
                    TotalInvestments = 0,
                    InvestmentsByType = new List<InvestmentByTypeDto>(),
                    TopPerformers = new List<TopPerformingInvestmentDto>(),
                    WorstPerformers = new List<TopPerformingInvestmentDto>()
                };
            }

            var totalInvested = investments.Sum(i => i.TotalInvested);
            var totalCurrentValue = investments.Sum(i => i.CurrentValue ?? 0);
            var totalGainLoss = totalCurrentValue - totalInvested;

            var summary = new InvestmentSummaryDto
            {
                TotalInvested = totalInvested,
                TotalCurrentValue = totalCurrentValue,
                TotalGainLoss = totalGainLoss,
                TotalGainLossPercentage = totalInvested > 0 ? (totalGainLoss / totalInvested) * 100 : 0,
                TotalInvestments = investments.Count,
                InvestmentsByType = investments
                    .GroupBy(i => i.InvestmentType.ToString())
                    .Select(g => new InvestmentByTypeDto
                    {
                        InvestmentType = g.Key,
                        Count = g.Count(),
                        TotalInvested = g.Sum(i => i.TotalInvested),
                        TotalCurrentValue = g.Sum(i => i.CurrentValue ?? 0),
                        GainLoss = g.Sum(i => (i.CurrentValue ?? 0) - i.TotalInvested),
                        GainLossPercentage = g.Sum(i => i.TotalInvested) > 0
                            ? (g.Sum(i => (i.CurrentValue ?? 0) - i.TotalInvested) / g.Sum(i => i.TotalInvested)) * 100
                            : 0
                    }).ToList(),
                TopPerformers = investments
                    .Where(i => i.GainLossPercentage.HasValue)
                    .OrderByDescending(i => i.GainLossPercentage.Value)
                    .Take(3)
                    .Select(i => new TopPerformingInvestmentDto
                    {
                        InvestmentId = i.InvestmentId,
                        InvestmentName = i.InvestmentName,
                        Symbol = i.Symbol ?? "",
                        TotalInvested = i.TotalInvested,
                        CurrentValue = i.CurrentValue ?? 0,
                        GainLoss = i.TotalGainLoss ?? 0,
                        GainLossPercentage = i.GainLossPercentage ?? 0
                    }).ToList(),
                WorstPerformers = investments
                    .Where(i => i.GainLossPercentage.HasValue)
                    .OrderBy(i => i.GainLossPercentage.Value)
                    .Take(3)
                    .Select(i => new TopPerformingInvestmentDto
                    {
                        InvestmentId = i.InvestmentId,
                        InvestmentName = i.InvestmentName,
                        Symbol = i.Symbol ?? "",
                        TotalInvested = i.TotalInvested,
                        CurrentValue = i.CurrentValue ?? 0,
                        GainLoss = i.TotalGainLoss ?? 0,
                        GainLossPercentage = i.GainLossPercentage ?? 0
                    }).ToList()
            };

            return summary;
        }
    }
}
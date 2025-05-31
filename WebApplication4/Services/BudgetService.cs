using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;
using Microsoft.EntityFrameworkCore;
namespace WebApplication4.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly AppDbContext _context;

        public BudgetService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<BudgetDto?> CreateBudgetAsync(int userId, CreateBudgetDto dto)
        {
            // Validate: Ngày không hợp lệ
            if (dto.StartDate >= dto.EndDate)
                return null;

            // Check trùng kỳ ngân sách cùng category
            var isOverlap = await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == dto.CategoryId &&
                b.IsActive &&
                b.StartDate < dto.EndDate &&
                b.EndDate > dto.StartDate);

            if (isOverlap)
                return null;

            var budget = new Budget
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                BudgetName = dto.BudgetName,
                BudgetAmount = dto.BudgetAmount,
                BudgetPeriod = dto.BudgetPeriod,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AlertThreshold = dto.AlertThreshold,
                SpentAmount = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                BudgetId = budget.BudgetId,
                BudgetName = budget.BudgetName,
                BudgetAmount = budget.BudgetAmount,
                BudgetPeriod = budget.BudgetPeriod,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                SpentAmount = budget.SpentAmount,
                AlertThreshold = budget.AlertThreshold,
                IsActive = budget.IsActive,
                CategoryId = budget.CategoryId
            };
        }
        public async Task<IEnumerable<BudgetDto>> GetBudgetsAsync(int userId)
        {
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive)
                .ToListAsync();

            return budgets.Select(b => new BudgetDto
            {
                BudgetId = b.BudgetId,
                BudgetName = b.BudgetName,
                BudgetAmount = b.BudgetAmount,
                BudgetPeriod = b.BudgetPeriod,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                SpentAmount = b.SpentAmount,
                AlertThreshold = b.AlertThreshold,
                IsActive = b.IsActive,
                CategoryId = b.CategoryId
            });
        }
        public async Task<BudgetDto?> GetBudgetByIdAsync(int userId, int budgetId)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.UserId == userId);

            if (budget == null) return null;

            return new BudgetDto
            {
                BudgetId = budget.BudgetId,
                BudgetName = budget.BudgetName,
                BudgetAmount = budget.BudgetAmount,
                BudgetPeriod = budget.BudgetPeriod,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                SpentAmount = budget.SpentAmount,
                AlertThreshold = budget.AlertThreshold,
                IsActive = budget.IsActive,
                CategoryId = budget.CategoryId
            };
        }
        public async Task<BudgetDto?> UpdateBudgetAsync(int userId, int budgetId, UpdateBudgetDto dto)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.UserId == userId);
            if (budget == null) return null;

            // Validate ngày
            if (dto.StartDate >= dto.EndDate)
                return null;

            // Kiểm tra trùng kỳ
            var isOverlap = await _context.Budgets.AnyAsync(b =>
                b.BudgetId != budgetId &&
                b.UserId == userId &&
                b.CategoryId == dto.CategoryId &&
                b.IsActive &&
                b.StartDate < dto.EndDate &&
                b.EndDate > dto.StartDate);

            if (isOverlap)
                return null;

            // Update fields
            budget.BudgetName = dto.BudgetName;
            budget.BudgetAmount = dto.BudgetAmount;
            budget.BudgetPeriod = dto.BudgetPeriod;
            budget.StartDate = dto.StartDate;
            budget.EndDate = dto.EndDate;
            budget.AlertThreshold = dto.AlertThreshold;
            budget.CategoryId = dto.CategoryId;
            budget.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                BudgetId = budget.BudgetId,
                BudgetName = budget.BudgetName,
                BudgetAmount = budget.BudgetAmount,
                BudgetPeriod = budget.BudgetPeriod,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                SpentAmount = budget.SpentAmount,
                AlertThreshold = budget.AlertThreshold,
                IsActive = budget.IsActive,
                CategoryId = budget.CategoryId
            };
        }

        public async Task<bool> DeleteBudgetAsync(int userId, int budgetId)
        {
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == budgetId && b.UserId == userId);
            if (budget == null) return false;

            budget.IsActive = false;
            budget.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<BudgetDto>> GetCurrentBudgetsAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.IsActive &&
                            b.StartDate <= today &&
                            b.EndDate >= today)
                .Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    BudgetName = b.BudgetName,
                    BudgetAmount = b.BudgetAmount,
                    SpentAmount = b.SpentAmount,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    CategoryId = b.CategoryId,
                    BudgetPeriod = b.BudgetPeriod,
                    AlertThreshold = b.AlertThreshold,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            return budgets;
        }
        public async Task<List<BudgetSummaryDto>> GetBudgetSummaryAsync(int userId)
        {
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive)
                .ToListAsync();

            var summaries = budgets.Select(b => new BudgetSummaryDto
            {
                BudgetId = b.BudgetId,
                BudgetName = b.BudgetName,
                BudgetAmount = b.BudgetAmount,
                SpentAmount = b.SpentAmount,
                AlertThreshold = b.AlertThreshold,
                IsAlert = b.SpentAmount >= (b.BudgetAmount * b.AlertThreshold / 100m)
            }).ToList();

            return summaries;
        }
        public async Task<List<BudgetPerformanceDto>> GetBudgetPerformanceAsync(int userId)
        {
            var today = DateTime.Today;

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive &&
                            b.StartDate <= today && b.EndDate >= today)
                .ToListAsync();

            var result = new List<BudgetPerformanceDto>();

            foreach (var budget in budgets)
            {
                var spent = await _context.Transactions
                    .Where(t => t.UserId == userId
                             && t.CategoryId == budget.CategoryId
                             && t.TransactionDate >= budget.StartDate
                             && t.TransactionDate <= budget.EndDate)
                    .SumAsync(t => t.Amount);

                double percentageUsed = budget.BudgetAmount == 0 ? 0 : (double)(spent / budget.BudgetAmount) * 100;
                string status = percentageUsed switch
                {
                    <= 70 => "under",
                    <= 100 => "near",
                    _ => "over"
                };

                result.Add(new BudgetPerformanceDto
                {
                    BudgetId = budget.BudgetId,
                    BudgetName = budget.BudgetName,
                    BudgetAmount = budget.BudgetAmount,
                    SpentAmount = spent,
                    PercentageUsed = Math.Round(percentageUsed, 2),
                    Status = status
                });
            }

            return result;
        }
        public async Task<List<BudgetDto>> CreateBudgetsFromTemplateAsync(int userId, BudgetTemplateRequestDto dto)
        {
            var budgets = dto.Templates.Select(t => new Budget
            {
                UserId = userId,
                CategoryId = t.CategoryId,
                BudgetAmount = t.BudgetAmount,
                BudgetName = "", // optional tên theo category?
                BudgetPeriod = dto.BudgetPeriod,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AlertThreshold = t.AlertThreshold,
                SpentAmount = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            _context.Budgets.AddRange(budgets);
            await _context.SaveChangesAsync();

            return budgets.Select(b => new BudgetDto
            {
                BudgetId = b.BudgetId,
                BudgetName = b.BudgetName,
                BudgetAmount = b.BudgetAmount,
                BudgetPeriod = b.BudgetPeriod,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                AlertThreshold = b.AlertThreshold,
                SpentAmount = b.SpentAmount,
                IsActive = b.IsActive,
                CategoryId = b.CategoryId
            }).ToList();
        }

        public async Task<List<BudgetDto>> UpdateBudgetsBulkAsync(int userId, List<BudgetUpdateDto> budgets)
        {
            var budgetIds = budgets.Select(b => b.BudgetId).ToList();
            var userBudgets = await _context.Budgets
                .Where(b => budgetIds.Contains(b.BudgetId) && b.UserId == userId)
                .ToListAsync();

            var updatedBudgets = new List<BudgetDto>();

            foreach (var b in userBudgets)
            {
                var update = budgets.FirstOrDefault(x => x.BudgetId == b.BudgetId);
                if (update == null) continue;

                b.BudgetAmount = update.BudgetAmount;
                b.AlertThreshold = update.AlertThreshold;
                b.IsActive = update.IsActive;
                b.UpdatedAt = DateTime.UtcNow; // Nên dùng UtcNow

                // Add to result
                updatedBudgets.Add(new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    BudgetName = b.BudgetName,
                    BudgetAmount = b.BudgetAmount,
                    BudgetPeriod = b.BudgetPeriod,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    SpentAmount = b.SpentAmount,
                    AlertThreshold = b.AlertThreshold,
                    IsActive = b.IsActive,
                    CategoryId = b.CategoryId
                });
            }

            await _context.SaveChangesAsync();
            return updatedBudgets;
        }

        public async Task<List<BudgetAlertDto>> GetBudgetAlertsAsync(int userId)
        {
            var today = DateTime.Now.Date; // Chỉ lấy ngày, bỏ giờ phút giây

            Console.WriteLine($"Today (Date only): {today}");

            var alerts = await _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.IsActive &&
                            b.StartDate <= today &&
                            b.EndDate >= today && // Bây giờ sẽ so sánh 31/05/2025 00:00:00 >= 31/05/2025 00:00:00 = TRUE
                            (b.SpentAmount * 100m) / b.BudgetAmount >= b.AlertThreshold)
                .Select(b => new BudgetAlertDto
                {
                    BudgetId = b.BudgetId,
                    BudgetName = b.BudgetName,
                    BudgetAmount = b.BudgetAmount,
                    SpentAmount = b.SpentAmount,
                    AlertThreshold = b.AlertThreshold,
                    PercentageSpent = Math.Round((double)((b.SpentAmount * 100m) / b.BudgetAmount), 2)
                })
                .ToListAsync();

            return alerts;
        }
        public async Task<List<BudgetDto>> GetBudgetsByCategoryAsync(int userId, int categoryId)
        {
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.CategoryId == categoryId && b.IsActive)
                .Select(b => new BudgetDto
                {
                    BudgetId = b.BudgetId,
                    BudgetName = b.BudgetName,
                    BudgetAmount = b.BudgetAmount,
                    BudgetPeriod = b.BudgetPeriod,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    SpentAmount = b.SpentAmount,
                    AlertThreshold = b.AlertThreshold,
                    IsActive = b.IsActive,
                    CategoryId = b.CategoryId
                })
                .ToListAsync();

            return budgets;
        }








    }


}

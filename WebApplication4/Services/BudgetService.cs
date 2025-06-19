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
            // Kiểm tra StartDate và EndDate
            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

            // Kiểm tra tài khoản hợp lệ
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == dto.AccountId && a.UserId == userId && a.IsActive);
            if (account == null)
                throw new ArgumentException("Tài khoản không hợp lệ hoặc không tồn tại.");

            // Kiểm tra tổng ngân sách
            var totalBudget = await _context.Budgets
                .Where(b => b.UserId == userId && b.AccountId == dto.AccountId && b.IsActive)
                .SumAsync(b => b.BudgetAmount);
            if (totalBudget + dto.BudgetAmount > account.Balance)
                throw new ArgumentException("Tổng ngân sách vượt quá số dư tài khoản.");

            // Kiểm tra overlap
            var isOverlap = await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == dto.CategoryId &&
                b.IsActive &&
                b.StartDate < dto.EndDate &&
                b.EndDate > dto.StartDate);
            if (isOverlap)
                throw new ArgumentException("Khoảng thời gian ngân sách trùng lặp với ngân sách hiện có cho danh mục này.");

            // Tạo ngân sách mới
            var budget = new Budget
            {
                UserId = userId,
                AccountId = dto.AccountId,
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

            // Tạo DTO để trả về
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

            if (dto.StartDate >= dto.EndDate)
                return null;

            var isOverlap = await _context.Budgets.AnyAsync(b =>
                b.BudgetId != budgetId &&
                b.UserId == userId &&
                b.CategoryId == dto.CategoryId &&
                b.IsActive &&
                b.StartDate < dto.EndDate &&
                b.EndDate > dto.StartDate);

            if (isOverlap)
                return null;

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

            var summaries = new List<BudgetSummaryDto>();

            foreach (var b in budgets)
            {
                // Tính tổng chi tiêu thực tế cho ngân sách này
                var spent = await _context.Transactions
                    .Where(t => t.UserId == userId
                        && t.CategoryId == b.CategoryId
                        && t.TransactionType == "Expense"
                        && t.TransactionDate >= b.StartDate
                        && t.TransactionDate <= b.EndDate)
                    .SumAsync(t => t.Amount);

                summaries.Add(new BudgetSummaryDto
                {
                    BudgetId = b.BudgetId,
                    BudgetName = b.BudgetName,
                    BudgetAmount = b.BudgetAmount,
                    SpentAmount = spent,
                    AlertThreshold = b.AlertThreshold,
                    IsAlert = spent >= (b.BudgetAmount * b.AlertThreshold / 100m)
                });
            }

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
                BudgetName = "", // Optional: Tên theo category?
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
                b.UpdatedAt = DateTime.UtcNow;

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
            var today = DateTime.UtcNow.Date;

            var alerts = await _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.IsActive &&
                            b.StartDate <= today &&
                            b.EndDate >= today &&
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

        // ==================== NEW METHODS FOR ACCOUNT BALANCE ALERTS ====================

        public async Task<List<AccountBalanceAlertDto>> GetAccountBalanceAlertsAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            var alerts = new List<AccountBalanceAlertDto>();

            var totalBalance = await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .SumAsync(a => a.Balance);

            var activeBudgets = await _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.IsActive &&
                            b.StartDate <= today &&
                            b.EndDate >= today)
                .ToListAsync();

            var totalBudgetAmount = activeBudgets.Sum(b => b.BudgetAmount - b.SpentAmount);

            if (totalBalance < totalBudgetAmount)
            {
                alerts.Add(new AccountBalanceAlertDto
                {
                    AlertType = "INSUFFICIENT_TOTAL_BALANCE",
                    Message = $"Số dư tài khoản ({totalBalance:C}) không đủ để thực hiện tất cả budget đang hoạt động ({totalBudgetAmount:C})",
                    TotalBalance = totalBalance,
                    RequiredAmount = totalBudgetAmount,
                    Shortage = totalBudgetAmount - totalBalance,
                    BudgetDetails = activeBudgets.Select(b => new BudgetSummaryInfo
                    {
                        BudgetId = b.BudgetId,
                        BudgetName = b.BudgetName,
                        RemainingAmount = b.BudgetAmount - b.SpentAmount
                    }).ToList()
                });
            }

            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .ToListAsync();

            foreach (var account in accounts)
            {
                var budgetPerAccount = accounts.Count > 0 ? totalBudgetAmount / accounts.Count : 0;

                if (account.Balance < budgetPerAccount)
                {
                    alerts.Add(new AccountBalanceAlertDto
                    {
                        AlertType = "INSUFFICIENT_ACCOUNT_BALANCE",
                        Message = $"Tài khoản '{account.AccountName}' có số dư thấp ({account.Balance:C}) so với budget dự kiến ({budgetPerAccount:C})",
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        AccountBalance = account.Balance,
                        RequiredAmount = budgetPerAccount,
                        Shortage = budgetPerAccount - account.Balance
                    });
                }
            }

            foreach (var budget in activeBudgets)
            {
                var remainingAmount = budget.BudgetAmount - budget.SpentAmount;
                var daysLeft = (budget.EndDate - today).Days;

                if (daysLeft > 0)
                {
                    var dailyBudget = remainingAmount / daysLeft;
                    var averageDailySpent = budget.SpentAmount / Math.Max(1, (today - budget.StartDate).Days);

                    if (averageDailySpent > dailyBudget * 1.5m)
                    {
                        alerts.Add(new AccountBalanceAlertDto
                        {
                            AlertType = "BUDGET_OVERSPENDING_RISK",
                            Message = $"Budget '{budget.BudgetName}' có nguy cơ vượt quá với tốc độ chi tiêu hiện tại",
                            BudgetId = budget.BudgetId,
                            BudgetName = budget.BudgetName,
                            CurrentSpending = budget.SpentAmount,
                            RemainingAmount = remainingAmount,
                            DaysLeft = daysLeft,
                            SuggestedDailyLimit = dailyBudget
                        });
                    }
                }
            }

            return alerts;
        }

        public async Task<BudgetFeasibilityCheckDto> CheckBudgetFeasibilityAsync(int userId, CreateBudgetDto dto)
        {
            var result = new BudgetFeasibilityCheckDto
            {
                IsFeasible = true,
                Warnings = new List<string>(),
                Suggestions = new List<string>()
            };

            var totalBalance = await _context.Accounts
    .Where(a => a.UserId == userId && a.IsActive)
    .SumAsync(a => a.Balance); // 10000.00

            var existingBudgets = await _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.IsActive &&
                            b.StartDate <= dto.EndDate &&
                            b.EndDate >= dto.StartDate)
                .SumAsync(b => b.BudgetAmount - b.SpentAmount); // 1000000.00

            var totalRequiredAmount = existingBudgets + dto.BudgetAmount; // 1000000 + 1500000 = 2500000

            if (totalBalance < totalRequiredAmount)
            {
                result.IsFeasible = false;
                result.Warnings.Add($"Số dư tài khoản ({totalBalance:C}) không đủ để thực hiện budget này cùng với các budget khác ({totalRequiredAmount:C})");
                result.Suggestions.Add($"Cần bổ sung thêm {totalRequiredAmount - totalBalance:C} vào tài khoản");
            }
            else if (totalBalance < totalRequiredAmount * 1.2m)
            {
                result.Warnings.Add("Số dư tài khoản ở mức thấp, nên cân nhắc giảm budget hoặc tăng thu nhập");
                result.Suggestions.Add("Nên giữ ít nhất 20% số dư để ứng phó với chi phí bất ngờ");
            }

            var historicalSpending = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.CategoryId == dto.CategoryId &&
                            t.TransactionType == "Expense" &&
                            t.TransactionDate >= DateTime.UtcNow.AddMonths(-3))
                .AverageAsync(t => (decimal?)t.Amount) ?? 0;

            if (historicalSpending > 0 && dto.BudgetAmount < historicalSpending * 0.8m)
            {
                result.Warnings.Add($"Budget đề xuất ({dto.BudgetAmount:C}) thấp hơn 80% chi tiêu trung bình 3 tháng qua ({historicalSpending:C})");
                result.Suggestions.Add($"Cân nhắc tăng budget lên {historicalSpending:C} dựa trên lịch sử chi tiêu");
            }

            return result;
        }

        public async Task<FinancialOverviewDto> GetFinancialOverviewAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var totalBalance = await _context.Accounts
                .Where(a => a.UserId == userId && a.IsActive)
                .SumAsync(a => a.Balance);

            var activeBudgets = await _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.IsActive &&
                            b.StartDate <= today &&
                            b.EndDate >= today)
                .ToListAsync();

            var totalBudget = activeBudgets.Sum(b => b.BudgetAmount);
            var totalSpent = activeBudgets.Sum(b => b.SpentAmount);
            var totalRemaining = totalBudget - totalSpent;

            var monthlyIncome = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionType == "Income" &&
                            t.TransactionDate >= new DateTime(today.Year, today.Month, 1))
                .SumAsync(t => t.Amount);

            var monthlyExpense = await _context.Transactions
                .Where(t => t.UserId == userId &&
                            t.TransactionType == "Expense" &&
                            t.TransactionDate >= new DateTime(today.Year, today.Month, 1))
                .SumAsync(t => t.Amount);

            return new FinancialOverviewDto
            {
                TotalBalance = totalBalance,
                TotalBudget = totalBudget,
                TotalSpent = totalSpent,
                TotalRemaining = totalRemaining,
                MonthlyIncome = monthlyIncome,
                MonthlyExpense = monthlyExpense,
                MonthlyNetIncome = monthlyIncome - monthlyExpense,
                BudgetUtilizationRate = totalBudget > 0 ? (double)(totalSpent / totalBudget) * 100 : 0,
                FinancialHealthScore = CalculateFinancialHealthScore(totalBalance, totalRemaining, monthlyIncome, monthlyExpense)
            };
        }

        private double CalculateFinancialHealthScore(decimal totalBalance, decimal totalRemaining, decimal monthlyIncome, decimal monthlyExpense)
        {
            double score = 100;

            if (totalBalance < monthlyExpense * 2) score -= 30;
            else if (totalBalance < monthlyExpense * 3) score -= 15;

            if (totalRemaining < totalBalance * 0.1m) score -= 25;
            else if (totalRemaining < totalBalance * 0.2m) score -= 15;

            if (monthlyExpense > monthlyIncome) score -= 40;
            else if (monthlyExpense > monthlyIncome * 0.9m) score -= 20;

            return Math.Max(0, Math.Min(100, score));
        }
    }
}
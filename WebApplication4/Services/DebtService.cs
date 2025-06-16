using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class DebtService : IDebtService
    {
        private readonly AppDbContext _context;

        public DebtService(AppDbContext context)
        {
            _context = context;
        }

        // Giữ nguyên các phương thức hiện có
        public async Task<List<DebtResponseDto>> GetDebtsAsync(int userId)
        {
            return await _context.Debts
                .Where(d => d.UserId == userId && d.IsActive)
                .Select(d => new DebtResponseDto
                {
                    DebtId = d.DebtId,
                    DebtName = d.DebtName,
                    DebtType = d.DebtType,
                    Creditor = d.Creditor,
                    OriginalAmount = d.OriginalAmount,
                    CurrentBalance = d.CurrentBalance,
                    InterestRate = d.InterestRate,
                    MinimumPayment = d.MinimumPayment,
                    PaymentDueDate = d.PaymentDueDate,
                    NextPaymentDate = d.NextPaymentDate,
                    PayoffDate = d.PayoffDate,
                    IsActive = d.IsActive
                })
                .ToListAsync();
        }

        public async Task<DebtDetailResponseDto> GetDebtByIdAsync(int debtId, int userId)
        {
            var debt = await _context.Debts
                .Include(d => d.DebtPayments)
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId && d.IsActive);

            if (debt == null)
            {
                throw new KeyNotFoundException("Debt not found or inactive.");
            }

            return new DebtDetailResponseDto
            {
                DebtId = debt.DebtId,
                DebtName = debt.DebtName,
                DebtType = debt.DebtType,
                Creditor = debt.Creditor,
                OriginalAmount = debt.OriginalAmount,
                CurrentBalance = debt.CurrentBalance,
                InterestRate = debt.InterestRate,
                MinimumPayment = debt.MinimumPayment,
                PaymentDueDate = debt.PaymentDueDate,
                NextPaymentDate = debt.NextPaymentDate,
                PayoffDate = debt.PayoffDate,
                IsActive = debt.IsActive,
                PaymentHistory = debt.DebtPayments.Select(p => new DebtPaymentDto
                {
                    PaymentId = p.PaymentId,
                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate,
                    PrincipalAmount = p.PrincipalAmount,
                    InterestAmount = p.InterestAmount,
                    Notes = p.Notes
                }).ToList()
            };
        }

        private static readonly HashSet<string> ValidDebtTypes = new HashSet<string>
        {
            "Credit Card", "Student Loan", "Mortgage", "Personal Loan", "Auto Loan", "Other"
        };

        public async Task CreateDebtAsync(CreateDebtRequestDto request, int userId)
        {
            if (!ValidDebtTypes.Contains(request.DebtType))
                throw new ArgumentException("Invalid DebtType.");

            if (request.AccountId.HasValue)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == request.AccountId && a.UserId == userId && a.IsActive);
                if (account == null)
                    throw new ArgumentException("Invalid or inactive account.");
                if (request.MinimumPayment.HasValue && account.Balance < request.MinimumPayment.Value)
                    throw new ArgumentException("Insufficient account balance for minimum payment.");
            }

            var debt = new Debt
            {
                UserId = userId,
                DebtName = request.DebtName,
                DebtType = request.DebtType,
                Creditor = request.Creditor,
                OriginalAmount = request.OriginalAmount,
                CurrentBalance = request.CurrentBalance,
                InterestRate = request.InterestRate,
                MinimumPayment = request.MinimumPayment,
                PaymentDueDate = request.PaymentDueDate,
                NextPaymentDate = request.NextPaymentDate,
                PayoffDate = request.PayoffDate,
                AccountId = request.AccountId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();

            // Nếu có AccountId, cộng tiền vào tài khoản
            if (request.AccountId.HasValue)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == request.AccountId && a.UserId == userId && a.IsActive);
                if (account != null)
                {
                    account.Balance += request.OriginalAmount;
                    account.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateDebtAsync(int debtId, UpdateDebtRequestDto request, int userId)
        {
            if (request.DebtType != null && !ValidDebtTypes.Contains(request.DebtType))
                throw new ArgumentException("Invalid DebtType.");

            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId && d.IsActive);

            if (debt == null)
                throw new KeyNotFoundException("Debt not found or inactive.");

            if (request.AccountId.HasValue)
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == request.AccountId && a.UserId == userId && a.IsActive);
                if (account == null)
                    throw new ArgumentException("Invalid or inactive account.");
                if (request.MinimumPayment.HasValue && account.Balance < request.MinimumPayment.Value)
                    throw new ArgumentException("Insufficient account balance for minimum payment.");
            }

            debt.DebtName = request.DebtName ?? debt.DebtName;
            debt.DebtType = request.DebtType ?? debt.DebtType;
            debt.Creditor = request.Creditor ?? debt.Creditor;
            debt.InterestRate = request.InterestRate ?? debt.InterestRate;
            debt.MinimumPayment = request.MinimumPayment ?? debt.MinimumPayment;
            debt.PaymentDueDate = request.PaymentDueDate ?? debt.PaymentDueDate;
            debt.NextPaymentDate = request.NextPaymentDate ?? debt.NextPaymentDate;
            debt.PayoffDate = request.PayoffDate ?? debt.PayoffDate;
            debt.AccountId = request.AccountId ?? debt.AccountId;
            debt.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteDebtAsync(int debtId, int userId)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId && d.IsActive);

            if (debt == null)
                throw new KeyNotFoundException("Debt not found or already deleted.");

            debt.IsActive = false;
            debt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task ActivateDebtAsync(int debtId, bool activate, int userId)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId);

            if (debt == null)
                throw new KeyNotFoundException("Debt not found.");

            debt.IsActive = activate;
            debt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<DebtSummaryResponseDto> GetDebtSummaryAsync(int userId)
        {
            var summary = await _context.Debts
                .Where(d => d.UserId == userId && d.IsActive)
                .GroupBy(d => 1)
                .Select(g => new DebtSummaryResponseDto
                {
                    TotalDebts = g.Count(),
                    TotalOriginalAmount = g.Sum(d => d.OriginalAmount),
                    TotalCurrentBalance = g.Sum(d => d.CurrentBalance),
                    TotalMinimumPayment = g.Sum(d => d.MinimumPayment ?? 0),
                    TotalInterestPaid = _context.DebtPayments
                        .Where(p => p.Debt.UserId == userId && p.Debt.IsActive)
                        .Sum(p => p.InterestAmount ?? 0),
                    ActiveDebts = g.Select(d => new DebtResponseDto
                    {
                        DebtId = d.DebtId,
                        DebtName = d.DebtName,
                        DebtType = d.DebtType,
                        Creditor = d.Creditor,
                        OriginalAmount = d.OriginalAmount,
                        CurrentBalance = d.CurrentBalance,
                        InterestRate = d.InterestRate,
                        MinimumPayment = d.MinimumPayment,
                        PaymentDueDate = d.PaymentDueDate,
                        NextPaymentDate = d.NextPaymentDate,
                        PayoffDate = d.PayoffDate,
                        IsActive = d.IsActive
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (summary == null)
            {
                summary = new DebtSummaryResponseDto
                {
                    TotalDebts = 0,
                    TotalOriginalAmount = 0,
                    TotalCurrentBalance = 0,
                    TotalMinimumPayment = 0,
                    TotalInterestPaid = 0,
                    ActiveDebts = new List<DebtResponseDto>()
                };
            }

            summary.DebtToIncomeRatio = await CalculateDebtToIncomeRatioAsync(userId);
            return summary;
        }

        private async Task<decimal> CalculateDebtToIncomeRatioAsync(int userId)
        {
            var totalMonthlyPayment = await _context.Debts
                .Where(d => d.UserId == userId && d.IsActive)
                .SumAsync(d => d.MinimumPayment ?? 0);
            var monthlyIncome = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionType == "Income" && t.TransactionDate >= DateTime.UtcNow.AddMonths(-1))
                .SumAsync(t => t.Amount);
            return monthlyIncome > 0 ? totalMonthlyPayment / monthlyIncome * 100 : 0;
        }

        // Thêm phương thức mới để xử lý thanh toán nợ
        public async Task ProcessDebtPaymentAsync(int debtId, ProcessDebtPaymentRequestDto request, int userId)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId && d.IsActive);
            if (debt == null)
                throw new KeyNotFoundException("Debt not found or inactive.");

            var parameters = new[]
            {
                new MySqlParameter("@p_DebtId", debtId),
                new MySqlParameter("@p_PaymentDate", request.PaymentDate),
                new MySqlParameter("@p_PaymentAmount", request.PaymentAmount)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL ProcessDebtPayment(@p_DebtId, @p_PaymentDate, @p_PaymentAmount)",
                    parameters);
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Failed to process payment: {ex.Message}");
            }
        }

        public async Task<IEnumerable> GetDebtDueNotificationsAsync(int userId, int days)
        {
            var currentDate = DateTime.UtcNow.Date;
            var dueDateLimit = currentDate.AddDays(days);

            var debts = await _context.Debts
                .Where(d => d.UserId == userId && d.IsActive && d.NextPaymentDate.HasValue && d.NextPaymentDate.Value <= dueDateLimit)
                .Select(d => new DebtDueNotificationDto
                {
                    DebtId = d.DebtId,
                    DebtName = d.DebtName,
                    Creditor = d.Creditor,
                    NextPaymentDate = d.NextPaymentDate.Value, // Explicitly cast Nullable<DateTime> to DateTime
                    MinimumPayment = d.MinimumPayment ?? 0,
                    CurrentBalance = d.CurrentBalance,
                    NotificationMessage = d.NextPaymentDate.Value.Date < currentDate
                        ? $"Overdue: Your payment of {d.MinimumPayment ?? 0:N2} VND for {d.DebtName} was due on {d.NextPaymentDate.Value:yyyy-MM-dd}."
                        : $"Reminder: Your payment of {d.MinimumPayment ?? 0:N2} VND for {d.DebtName} is due on {d.NextPaymentDate.Value:yyyy-MM-dd}."
                })
                .ToListAsync();

            return debts;
        }
    }

   
}
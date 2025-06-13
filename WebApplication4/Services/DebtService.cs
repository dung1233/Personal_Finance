using Microsoft.EntityFrameworkCore;
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

        public async Task<List<DebtResponseDto>> GetAllDebtsAsync(int userId)
        {
            return await _context.Debts
                .Where(d => d.UserId == userId)
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
                }).ToListAsync();
        }

        public async Task<DebtDetailResponseDto?> GetDebtByIdAsync(int userId, int debtId)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId);
            if (debt == null) return null;

            var payments = await _context.DebtPayments
                .Where(p => p.DebtId == debtId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new DebtPaymentDto
                {
                    PaymentId = p.PaymentId,
                    PaymentAmount = p.PaymentAmount,
                    PrincipalAmount = p.PrincipalAmount,
                    InterestAmount = p.InterestAmount,
                    PaymentDate = p.PaymentDate,
                    Notes = p.Notes
                }).ToListAsync();

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
                Payments = payments
            };
        }

        public async Task<DebtResponseDto> CreateDebtAsync(int userId, CreateDebtRequestDto dto)
        {
            var debt = new Debt
            {
                UserId = userId,
                DebtName = dto.DebtName,
                DebtType = dto.DebtType,
                Creditor = dto.Creditor,
                OriginalAmount = dto.OriginalAmount,
                CurrentBalance = dto.CurrentBalance,
                InterestRate = dto.InterestRate,
                MinimumPayment = dto.MinimumPayment,
                PaymentDueDate = dto.PaymentDueDate,
                NextPaymentDate = dto.NextPaymentDate,
                PayoffDate = dto.PayoffDate,
                IsActive = true
            };

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();

            return new DebtResponseDto
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
                IsActive = debt.IsActive
            };
        }

        public async Task<DebtResponseDto?> UpdateDebtAsync(int userId, int debtId, UpdateDebtRequestDto dto)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId);
            if (debt == null) return null;

            debt.DebtName = dto.DebtName;
            debt.DebtType = dto.DebtType;
            debt.Creditor = dto.Creditor;
            debt.OriginalAmount = dto.OriginalAmount;
            debt.CurrentBalance = dto.CurrentBalance;
            debt.InterestRate = dto.InterestRate;
            debt.MinimumPayment = dto.MinimumPayment;
            debt.PaymentDueDate = dto.PaymentDueDate;
            debt.NextPaymentDate = dto.NextPaymentDate;
            debt.PayoffDate = dto.PayoffDate;
            debt.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return new DebtResponseDto
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
                IsActive = debt.IsActive
            };
        }

        public async Task<bool> DeleteDebtAsync(int userId, int debtId)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId);
            if (debt == null) return false;

            _context.Debts.Remove(debt);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleDebtActiveStatusAsync(int userId, int debtId)
        {
            var debt = await _context.Debts
                .FirstOrDefaultAsync(d => d.DebtId == debtId && d.UserId == userId);
            if (debt == null) return false;

            debt.IsActive = !debt.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DebtSummaryResponseDto> GetDebtSummaryAsync(int userId)
        {
            var debts = await _context.Debts
                .Where(d => d.UserId == userId)
                .ToListAsync();

            return new DebtSummaryResponseDto
            {
                TotalDebts = debts.Count,
                TotalOriginalAmount = debts.Sum(d => d.OriginalAmount),
                TotalCurrentBalance = debts.Sum(d => d.CurrentBalance),
                TotalMonthlyPayment = debts.Sum(d => d.MinimumPayment ?? 0)
            };
        }
    }

}

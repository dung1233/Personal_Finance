namespace WebApplication4.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;

public class LoansService : ILoanService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoansService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirst("id").Value);

    public async Task<List<LoanResponseDto>> GetLoansAsync(int userId)
    {
        return await _context.Loans
            .Where(l => l.LenderId == userId)
            .Select(l => new LoanResponseDto
            {
                LoanId = l.LoanId,
                BorrowerName = l.BorrowerName,
                LoanName = l.LoanName,
                PrincipalAmount = l.PrincipalAmount,
                OutstandingBalance = l.OutstandingBalance,
                TotalReceived = l.TotalReceived,
                InterestRate = l.InterestRate,
                Status = l.Status,
                StartDate = l.StartDate,
                DueDate = l.DueDate
            }).ToListAsync();
    }

    public async Task<LoanResponseDto> GetLoanByIdAsync(int loanId, int userId)
    {
        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == loanId && l.LenderId == userId);
        if (loan == null) throw new Exception("Loan not found");

        return new LoanResponseDto
        {
            LoanId = loan.LoanId,
            BorrowerName = loan.BorrowerName,
            LoanName = loan.LoanName,
            PrincipalAmount = loan.PrincipalAmount,
            OutstandingBalance = loan.OutstandingBalance,
            TotalReceived = loan.TotalReceived,
            InterestRate = loan.InterestRate,
            Status = loan.Status,
            StartDate = loan.StartDate,
            DueDate = loan.DueDate
        };
    }

    public async Task<int> CreateLoanAsync(int userId, CreateLoanRequestDto dto)
    {
        var loan = new Loan
        {
            LenderId = userId,
            BorrowerName = dto.BorrowerName,
            BorrowerPhone = dto.BorrowerPhone,
            BorrowerEmail = dto.BorrowerEmail,
            LoanName = dto.LoanName,
            LoanType = dto.LoanType,
            PrincipalAmount = dto.PrincipalAmount,
            OutstandingBalance = dto.PrincipalAmount,
            InterestRate = dto.InterestRate ?? 0,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            PaymentFrequency = dto.PaymentFrequency,
            MinimumPayment = dto.MinimumPayment,
            Priority = dto.Priority.ToString(),
            Status = "Active",
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan.LoanId;
    }

    public async Task AddPaymentAsync(int loanId, CreateLoanPaymentDto dto, int userId)
    {
        var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == loanId && l.LenderId == userId);
        if (loan == null) throw new Exception("Loan not found");

        var payment = new LoanPayment
        {
            LoanId = loanId,
            PaymentAmount = dto.PaymentAmount,
            PaymentDate = dto.PaymentDate,
            PrincipalAmount = dto.PrincipalAmount,
            InterestAmount = dto.InterestAmount,
            LateFee = dto.LateFee ?? 0,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = "Completed",
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        loan.TotalReceived += dto.PaymentAmount;
        loan.OutstandingBalance -= dto.PrincipalAmount ?? dto.PaymentAmount;
        loan.UpdatedAt = DateTime.UtcNow;
        loan.LastPaymentDate = dto.PaymentDate;

        if (loan.OutstandingBalance <= 0)
        {
            loan.Status = "Completed";
            loan.CompletedAt = DateTime.UtcNow;
        }

        _context.LoanPayments.Add(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<LoanSummaryDto> GetLoanSummaryAsync(int userId)
    {
        var loans = await _context.Loans
            .Where(l => l.LenderId == userId)
            .ToListAsync();

        if (loans.Count == 0) return new LoanSummaryDto();

        var totalInterestEarned = await _context.LoanPayments
            .Where(p => p.Loan.LenderId == userId && p.PaymentStatus == "Completed")
            .SumAsync(p => p.InterestAmount);

        return new LoanSummaryDto
        {
            TotalLoans = loans.Count,
            TotalLent = loans.Sum(l => l.PrincipalAmount),
            TotalReceived = loans.Sum(l => l.TotalReceived),
            TotalOutstanding = loans.Sum(l => l.OutstandingBalance),
            ActiveOutstanding = loans.Where(l => l.IsActive).Sum(l => l.OutstandingBalance),
            OverdueAmount = loans
                .Where(l => l.NextPaymentDate < DateTime.Today && l.Status == "Active")
                .Sum(l => l.OutstandingBalance),
            AvgInterestRate = loans.Average(l => l.InterestRate),
            TotalInterestEarned = totalInterestEarned ?? 0 // Explicitly handle nullable decimal
        };
    }
    public async Task<List<LoanAlertDto>> GetLoanAlertsAsync(int userId)
    {
        var today = DateTime.Today;
        var threshold = today.AddDays(7); // cảnh báo trong vòng 7 ngày tới

        var alerts = await _context.Loans
            .Where(l => l.LenderId == userId &&
                        l.Status == "Active" &&
                        l.DueDate != null &&
                        (l.DueDate <= threshold || l.DueDate < today))
            .Select(l => new LoanAlertDto
            {
                LoanId = l.LoanId,
                LoanName = l.LoanName,
                BorrowerName = l.BorrowerName,
                DueDate = l.DueDate.Value,
                OutstandingBalance = l.OutstandingBalance,
                Status = l.Status,
                DaysUntilDue = EF.Functions.DateDiffDay(today, l.DueDate.Value)
            })
            .ToListAsync();

        return alerts;
    }



}

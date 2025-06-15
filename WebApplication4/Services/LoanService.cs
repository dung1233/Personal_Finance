using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _context;

        public LoanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoanResponseDto> CreateLoanAsync(CreateLoanDto dto, int userId)
        {
            // 1. Lấy tài khoản cho vay
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == dto.AccountId && a.UserId == userId && a.IsActive);
            if (account == null)
                throw new ArgumentException("Tài khoản không tồn tại hoặc không thuộc quyền của bạn.");

            // 2. Kiểm tra số dư
            if (account.Balance < dto.OriginalAmount)
                throw new InvalidOperationException("Số dư tài khoản không đủ để cho vay.");

            // 3. Tìm category "Cho vay"
            var loanCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == "Cho vay" && c.CategoryType == "Expense" && c.IsActive);
            if (loanCategory == null)
                throw new InvalidOperationException("Chưa có category 'Cho vay' (Expense). Hãy tạo trước.");

            // 4. Tạo Transaction ghi nhận chi tiền cho vay
            var transaction = new Transaction
            {
                UserId = userId,
                AccountId = account.AccountId,
                CategoryId = loanCategory.CategoryId,
                Amount = dto.OriginalAmount,
                TransactionType = "Expense",
                Description = $"Cho vay: {dto.Borrower}",
                TransactionDate = dto.LoanDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            // ⭐ Trừ tiền khỏi tài khoản
            account.Balance -= dto.OriginalAmount;
            account.UpdatedAt = DateTime.UtcNow;

            // 5. Tạo khoản vay
            var loan = new Loan
            {
                UserId = userId,
                LoanName = dto.LoanName,
                LoanType = dto.LoanType,
                Borrower = dto.Borrower,
                BorrowerPhone = dto.BorrowerPhone,
                BorrowerEmail = dto.BorrowerEmail,
                OriginalAmount = dto.OriginalAmount,
                CurrentBalance = dto.OriginalAmount,
                InterestRate = dto.InterestRate,
                ExpectedPayment = dto.ExpectedPayment,
                PaymentDueDate = dto.PaymentDueDate,
                NextPaymentDate = dto.NextPaymentDate,
                LoanDate = dto.LoanDate,
                DueDate = dto.DueDate,
                ContractDocument = dto.ContractDocument,
                Notes = dto.Notes,
                AccountId = dto.AccountId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Loans.Add(loan);

            // 6. Lưu thay đổi (Transaction sẽ tự động trừ tiền khỏi account)
            await _context.SaveChangesAsync();

            // 7. Trả về DTO như cũ
            return new LoanResponseDto
            {
                LoanId = loan.LoanId,
                LoanName = loan.LoanName,
                LoanType = loan.LoanType,
                Borrower = loan.Borrower,
                BorrowerPhone = loan.BorrowerPhone,
                BorrowerEmail = loan.BorrowerEmail,
                OriginalAmount = loan.OriginalAmount,
                CurrentBalance = loan.CurrentBalance,
                InterestRate = loan.InterestRate,
                ExpectedPayment = loan.ExpectedPayment,
                PaymentDueDate = loan.PaymentDueDate,
                NextPaymentDate = loan.NextPaymentDate,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ContractDocument = loan.ContractDocument,
                Notes = loan.Notes,
                IsActive = loan.IsActive,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                AccountId = loan.AccountId,
                AccountName = account.AccountName,
                TotalPaid = 0,
                RemainingAmount = loan.CurrentBalance,
                IsOverdue = false,
                DaysOverdue = 0,
                IsFullyPaid = false,
                CompletionPercentage = 0,
                Status = "Active",
                RecentPayments = new List<LoanPaymentSummaryDto>()
            };
        }

        public async Task<LoanPaymentResponseDto> CreateLoanPaymentAsync(CreateLoanPaymentDto dto, int userId)
        {
            var loan = await _context.Loans
                .Include(l => l.Account)
                .FirstOrDefaultAsync(l => l.LoanId == dto.LoanId && l.UserId == userId);

            if (loan == null)
                throw new KeyNotFoundException("Không tìm thấy khoản vay.");

            if (dto.PaymentAmount <= 0)
                throw new ArgumentException("Số tiền thanh toán phải lớn hơn 0.");

            // Tạo payment
            var payment = new LoanPayment
            {
                LoanId = loan.LoanId,
                PaymentAmount = dto.PaymentAmount,
                PaymentDate = dto.PaymentDate,
                PrincipalAmount = dto.PrincipalAmount,
                InterestAmount = dto.InterestAmount,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.LoanPayments.Add(payment);

            // Cập nhật số dư khoản vay
            loan.CurrentBalance -= dto.PaymentAmount;
            if (loan.CurrentBalance < 0) loan.CurrentBalance = 0;
            loan.UpdatedAt = DateTime.UtcNow;

            // Cập nhật số dư tài khoản nhận tiền (nếu cần)
            if (loan.Account != null)
            {
                loan.Account.Balance += dto.PaymentAmount;
                loan.Account.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Trả về DTO kết quả
            return new LoanPaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                LoanId = loan.LoanId,
                LoanName = loan.LoanName,
                Borrower = loan.Borrower,
                PaymentAmount = payment.PaymentAmount,
                PaymentDate = payment.PaymentDate,
                PrincipalAmount = payment.PrincipalAmount,
                InterestAmount = payment.InterestAmount,
                PaymentMethod = payment.PaymentMethod,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt,
                NewCurrentBalance = loan.CurrentBalance,
                TotalPaid = loan.TotalPaid,
                CompletionPercentage = loan.CompletionPercentage,
                IsFullyPaid = loan.IsFullyPaid,
                NextPaymentDate = loan.NextPaymentDate,
                TransactionId = 0, // Nếu có tạo transaction thì gán ID ở đây
                AccountName = loan.Account?.AccountName ?? "",
                AccountBalanceAfter = loan.Account?.Balance ?? 0,
                Message = "Thanh toán thành công",
                IsSuccess = true
            };
        }

        public async Task<List<LoanSummaryDto>> GetLoansAsync(int userId)
        {
            var loans = await _context.Loans
                .Where(l => l.UserId == userId)
                .Include(l => l.Account)
                .ToListAsync();

            return loans.Select(l => new LoanSummaryDto
            {
                LoanId = l.LoanId,
                LoanName = l.LoanName,
                LoanType = l.LoanType,
                Borrower = l.Borrower,
                BorrowerPhone = l.BorrowerPhone,
                OriginalAmount = l.OriginalAmount,
                CurrentBalance = l.CurrentBalance,
                ExpectedPayment = l.ExpectedPayment,
                NextPaymentDate = l.NextPaymentDate,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                IsActive = l.IsActive,
                AccountName = l.Account?.AccountName ?? "",
                TotalPaid = l.TotalPaid,
                IsOverdue = l.IsOverdue,
                DaysOverdue = l.DaysOverdue,
                IsFullyPaid = l.IsFullyPaid,
                CompletionPercentage = l.CompletionPercentage,
                Status = l.IsFullyPaid ? "Completed" : (l.IsOverdue ? "Overdue" : "Active")
            }).ToList();
        }

        public async Task<LoanResponseDto?> GetLoanByIdAsync(int loanId, int userId)
        {
            var loan = await _context.Loans
                .Include(l => l.Account)
                .Include(l => l.LoanPayments)
                .FirstOrDefaultAsync(l => l.LoanId == loanId && l.UserId == userId);

            if (loan == null) return null;

            return new LoanResponseDto
            {
                LoanId = loan.LoanId,
                LoanName = loan.LoanName,
                LoanType = loan.LoanType,
                Borrower = loan.Borrower,
                BorrowerPhone = loan.BorrowerPhone,
                BorrowerEmail = loan.BorrowerEmail,
                OriginalAmount = loan.OriginalAmount,
                CurrentBalance = loan.CurrentBalance,
                InterestRate = loan.InterestRate,
                ExpectedPayment = loan.ExpectedPayment,
                PaymentDueDate = loan.PaymentDueDate,
                NextPaymentDate = loan.NextPaymentDate,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ContractDocument = loan.ContractDocument,
                Notes = loan.Notes,
                IsActive = loan.IsActive,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                AccountId = loan.AccountId,
                AccountName = loan.Account?.AccountName ?? "",
                TotalPaid = loan.TotalPaid,
                RemainingAmount = loan.RemainingAmount,
                IsOverdue = loan.IsOverdue,
                DaysOverdue = loan.DaysOverdue,
                IsFullyPaid = loan.IsFullyPaid,
                CompletionPercentage = loan.CompletionPercentage,
                Status = loan.IsFullyPaid ? "Completed" : (loan.IsOverdue ? "Overdue" : "Active"),
                RecentPayments = loan.LoanPayments
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .Select(p => new LoanPaymentSummaryDto
                    {
                        PaymentId = p.PaymentId,
                        PaymentAmount = p.PaymentAmount,
                        PaymentDate = p.PaymentDate,
                        PrincipalAmount = p.PrincipalAmount,
                        InterestAmount = p.InterestAmount,
                        PaymentMethod = p.PaymentMethod,
                        Notes = p.Notes,
                        PaymentStatus = p.PaymentStatus
                    }).ToList()
            };
        }

        public async Task<List<LoanPaymentDetailDto>> GetLoanPaymentsAsync(int loanId, int userId)
        {
            var loan = await _context.Loans
                .Include(l => l.LoanPayments)
                .FirstOrDefaultAsync(l => l.LoanId == loanId && l.UserId == userId);

            if (loan == null) throw new KeyNotFoundException("Không tìm thấy khoản vay.");

            return loan.LoanPayments
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new LoanPaymentDetailDto
                {
                    PaymentId = p.PaymentId,
                    LoanId = p.LoanId,
                    LoanName = loan.LoanName,
                    Borrower = loan.Borrower,
                    PaymentAmount = p.PaymentAmount,
                    PaymentDate = p.PaymentDate,
                    PrincipalAmount = p.PrincipalAmount,
                    InterestAmount = p.InterestAmount,
                    PaymentMethod = p.PaymentMethod,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt,
                    PaymentStatus = p.PaymentStatus,
                    IsPartialPayment = p.IsPartialPayment,
                    IsOverPayment = p.IsOverPayment,
                    LoanBalanceAfter = loan.CurrentBalance,
                    CompletionPercentageAfter = loan.CompletionPercentage
                }).ToList();
        }
    }
}
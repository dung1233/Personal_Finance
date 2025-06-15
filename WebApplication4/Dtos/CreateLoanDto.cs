using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    // DTO cho việc tạo mới khoản cho vay (POST)
    public class CreateLoanDto
    {
        [Required(ErrorMessage = "Tên khoản vay là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khoản vay không được vượt quá 100 ký tự")]
        public string LoanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại khoản vay là bắt buộc")]
        public string LoanType { get; set; } = string.Empty; // Personal Loan, Business Loan, Emergency Loan, Family Loan, Friend Loan, Other

        [Required(ErrorMessage = "Tên người vay là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên người vay không được vượt quá 100 ký tự")]
        public string Borrower { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string? BorrowerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255)]
        public string? BorrowerEmail { get; set; }

        [Required(ErrorMessage = "Số tiền cho vay là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền cho vay phải lớn hơn 0")]
        public decimal OriginalAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Lãi suất phải từ 0% đến 100%")]
        public decimal InterestRate { get; set; } = 0.00m;

        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền dự kiến thu phải lớn hơn 0")]
        public decimal? ExpectedPayment { get; set; }

        [Range(1, 31, ErrorMessage = "Ngày đáo hạn phải từ 1 đến 31")]
        public int? PaymentDueDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        [Required(ErrorMessage = "Ngày cho vay là bắt buộc")]
        public DateTime LoanDate { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(500)]
        public string? ContractDocument { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Tài khoản cho vay là bắt buộc")]
        public int AccountId { get; set; }
    }

    // DTO cho việc cập nhật khoản cho vay (PUT)
    public class UpdateLoanDto
    {
        [Required(ErrorMessage = "ID khoản vay là bắt buộc")]
        public int LoanId { get; set; }

        [Required(ErrorMessage = "Tên khoản vay là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khoản vay không được vượt quá 100 ký tự")]
        public string LoanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại khoản vay là bắt buộc")]
        public string LoanType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên người vay là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên người vay không được vượt quá 100 ký tự")]
        public string Borrower { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string? BorrowerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255)]
        public string? BorrowerEmail { get; set; }

        [Range(0, 100, ErrorMessage = "Lãi suất phải từ 0% đến 100%")]
        public decimal InterestRate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền dự kiến thu phải lớn hơn 0")]
        public decimal? ExpectedPayment { get; set; }

        [Range(1, 31, ErrorMessage = "Ngày đáo hạn phải từ 1 đến 31")]
        public int? PaymentDueDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(500)]
        public string? ContractDocument { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // DTO trả về thông tin khoản cho vay (GET)
    public class LoanResponseDto
    {
        public int LoanId { get; set; }
        public string LoanName { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public string Borrower { get; set; } = string.Empty;
        public string? BorrowerPhone { get; set; }
        public string? BorrowerEmail { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal? ExpectedPayment { get; set; }
        public int? PaymentDueDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ContractDocument { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;

        // Computed fields
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public bool IsFullyPaid { get; set; }
        public decimal CompletionPercentage { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Overdue, Completed, Inactive

        // Payment history
        public List<LoanPaymentSummaryDto> RecentPayments { get; set; } = new();
    }

    // DTO tóm tắt thông tin khoản cho vay (cho danh sách)
    public class LoanSummaryDto
    {
        public int LoanId { get; set; }
        public string LoanName { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public string Borrower { get; set; } = string.Empty;
        public string? BorrowerPhone { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal? ExpectedPayment { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; }
        public string AccountName { get; set; } = string.Empty;

        // Computed fields
        public decimal TotalPaid { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public bool IsFullyPaid { get; set; }
        public decimal CompletionPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // DTO cho danh sách khoản cho vay (GET all)
    public class LoanListResponseDto
    {
        public List<LoanSummaryDto> Loans { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int CompletedLoans { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalCollected { get; set; }

        // Pagination
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    // DTO cho thông báo sắp đến hạn (GET alerts)
    public class LoanAlertDto
    {
        public int LoanId { get; set; }
        public string LoanName { get; set; } = string.Empty;
        public string Borrower { get; set; } = string.Empty;
        public string? BorrowerPhone { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal? ExpectedPayment { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public string AlertType { get; set; } = string.Empty; // DueToday, DueTomorrow, Overdue, DueThisWeek
        public int DaysUntilDue { get; set; } // Âm nếu quá hạn
        public string AlertMessage { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty; // High, Medium, Low
        public string AccountName { get; set; } = string.Empty;
    }

    // DTO cho thống kê khoản cho vay
    public class LoanStatisticsDto
    {
        public int TotalLoans { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int CompletedLoans { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal CollectionRate { get; set; } // Tỷ lệ thu hồi (%)
        public decimal AverageInterestRate { get; set; }
        public decimal MonthlyExpectedIncome { get; set; }

        // Top borrowers
        public List<TopBorrowerDto> TopBorrowers { get; set; } = new();

        // Monthly collection data for charts
        public List<MonthlyCollectionDto> MonthlyCollections { get; set; } = new();
    }

    // DTO cho top người vay
    public class TopBorrowerDto
    {
        public string Borrower { get; set; } = string.Empty;
        public int LoanCount { get; set; }
        public decimal TotalBorrowed { get; set; }
        public decimal TotalOutstanding { get; set; }
        public bool HasOverdue { get; set; }
    }

    // DTO cho dữ liệu thu hồi hàng tháng
    public class MonthlyCollectionDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal CollectedAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal CollectionRate { get; set; }
    }

    // DTO cho lịch sử thanh toán (tóm tắt)
    public class LoanPaymentSummaryDto
    {
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }

    // DTO cho query parameters (lọc, tìm kiếm)
    public class LoanQueryDto
    {
        public string? SearchTerm { get; set; } // Tìm theo tên khoản vay hoặc người vay
        public string? LoanType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOverdue { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? AccountId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string? SortBy { get; set; } = "LoanDate"; // LoanDate, Borrower, Amount, DueDate
        public bool SortDescending { get; set; } = true;
    }

    // DTO cho xóa khoản vay (soft delete)
    public class DeleteLoanDto
    {
        [Required(ErrorMessage = "ID khoản vay là bắt buộc")]
        public int LoanId { get; set; }

        public string? Reason { get; set; } // Lý do xóa
        public bool ForceDelete { get; set; } = false; // Xóa cứng (nếu cần)
    }
}
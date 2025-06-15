using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    // DTO cho việc tạo thanh toán khoản vay (POST loan payment)
    public class CreateLoanPaymentDto
    {
        [Required(ErrorMessage = "ID khoản vay là bắt buộc")]
        public int LoanId { get; set; }

        [Required(ErrorMessage = "Số tiền thanh toán là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền thanh toán phải lớn hơn 0")]
        public decimal PaymentAmount { get; set; }

        [Required(ErrorMessage = "Ngày thanh toán là bắt buộc")]
        public DateTime PaymentDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền gốc không được âm")]
        public decimal? PrincipalAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền lãi không được âm")]
        public decimal? InterestAmount { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    // DTO cho response khi xử lý thanh toán thành công
    public class LoanPaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int LoanId { get; set; }
        public string LoanName { get; set; } = string.Empty;
        public string Borrower { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin cập nhật về khoản vay
        public decimal NewCurrentBalance { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal CompletionPercentage { get; set; }
        public bool IsFullyPaid { get; set; }
        public DateTime? NextPaymentDate { get; set; }

        // Thông tin về transaction được tạo
        public int TransactionId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public decimal AccountBalanceAfter { get; set; }

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }

    // DTO cho danh sách thanh toán khoản vay
    public class LoanPaymentListDto
    {
        public List<LoanPaymentDetailDto> Payments { get; set; } = new();
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPrincipal { get; set; }
        public decimal TotalInterest { get; set; }

        // Pagination
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    // DTO chi tiết thanh toán khoản vay
    public class LoanPaymentDetailDto
    {
        public int PaymentId { get; set; }
        public int LoanId { get; set; }
        public string LoanName { get; set; } = string.Empty;
        public string Borrower { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public bool IsPartialPayment { get; set; }
        public bool IsOverPayment { get; set; }

        // Thông tin khoản vay tại thời điểm thanh toán
        public decimal LoanBalanceAfter { get; set; }
        public decimal CompletionPercentageAfter { get; set; }
    }

    // DTO cho việc xử lý thanh toán (ProcessLoanPayment stored procedure)
    public class ProcessLoanPaymentRequestDto
    {
        [Required(ErrorMessage = "ID khoản vay là bắt buộc")]
        public int LoanId { get; set; }

        [Required(ErrorMessage = "Ngày thanh toán là bắt buộc")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Số tiền thanh toán là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền thanh toán phải lớn hơn 0")]
        public decimal PaymentAmount { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(500)]
        public string? Notes { get; set; }

        // Tùy chọn xử lý
        public bool AutoCalculateInterest { get; set; } = true; // Tự động tính lãi
        public bool UpdateNextPaymentDate { get; set; } = true; // Tự động cập nhật ngày thanh toán tiếp theo
        public bool CreateTransaction { get; set; } = true; // Tự động tạo transaction
        public bool SendNotification { get; set; } = true; // Gửi thông báo
    }

    // DTO cho bulk payment (thanh toán nhiều khoản cùng lúc)
    public class BulkLoanPaymentDto
    {
        public List<CreateLoanPaymentDto> Payments { get; set; } = new();
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? Notes { get; set; }
    }

    // DTO cho response của bulk payment
    public class BulkLoanPaymentResponseDto
    {
        public List<LoanPaymentResponseDto> SuccessfulPayments { get; set; } = new();
        public List<FailedLoanPaymentDto> FailedPayments { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPartialSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // DTO cho thanh toán thất bại
    public class FailedLoanPaymentDto
    {
        public int LoanId { get; set; }
        public string LoanName { get; set; } = string.Empty;
        public string Borrower { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
    }

    // DTO cho query thanh toán
    public class LoanPaymentQueryDto
    {
        public int? LoanId { get; set; }
        public string? SearchTerm { get; set; } // Tìm theo tên khoản vay hoặc người vay
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string? SortBy { get; set; } = "PaymentDate";
        public bool SortDescending { get; set; } = true;
    }

    // DTO cho báo cáo thanh toán
    public class LoanPaymentReportDto
    {
        public DateTime ReportDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // Tổng quan
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPrincipal { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal AveragePaymentAmount { get; set; }

        // Theo phương thức thanh toán
        public List<PaymentMethodSummaryDto> PaymentMethodSummary { get; set; } = new();

        // Theo tháng
        public List<MonthlyPaymentSummaryDto> MonthlyPayments { get; set; } = new();

        // Top người vay thanh toán nhiều nhất
        public List<TopPayerDto> TopPayers { get; set; } = new();
    }

    // DTO tóm tắt theo phương thức thanh toán
    public class PaymentMethodSummaryDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }

    // DTO tóm tắt thanh toán hàng tháng
    public class MonthlyPaymentSummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int PaymentCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
    }

    // DTO top người thanh toán
    public class TopPayerDto
    {
        public string Borrower { get; set; } = string.Empty;
        public int PaymentCount { get; set; }
        public decimal TotalPaid { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public bool IsConsistentPayer { get; set; } // Thanh toán đều đặn
    }
}
using System.ComponentModel.DataAnnotations;
using WebApplication4.Models;

namespace WebApplication4.Dtos;
public class InvestmentDto
{
    public int InvestmentId { get; set; }
    public int UserId { get; set; }
    public string InvestmentName { get; set; } = string.Empty;
    public string InvestmentType { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? CurrentPrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? Broker { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? AccountId { get; set; }
    public string? AccountName { get; set; }

    // Calculated fields
    public decimal? TotalGainLoss { get; set; }
    public decimal? GainLossPercentage { get; set; }
    public decimal? PriceChange { get; set; }
    public decimal? PriceChangePercentage { get; set; }
}

// Create/Update DTOs
public class CreateInvestmentDto
{
    [Required(ErrorMessage = "Tên đầu tư là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên đầu tư không được vượt quá 100 ký tự")]
    public string InvestmentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại đầu tư là bắt buộc")]
    public InvestmentType InvestmentType { get; set; }

    [StringLength(20, ErrorMessage = "Mã chứng khoán không được vượt quá 20 ký tự")]
    public string? Symbol { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public decimal? Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá mua phải lớn hơn 0")]
    public decimal? PurchasePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá hiện tại phải lớn hơn 0")]
    public decimal? CurrentPrice { get; set; }

    public DateTime? PurchaseDate { get; set; }

    [Required(ErrorMessage = "Tổng số tiền đầu tư là bắt buộc")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Tổng số tiền đầu tư phải lớn hơn 0")]
    public decimal TotalInvested { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá trị hiện tại phải lớn hơn 0")]
    public decimal? CurrentValue { get; set; }

    [StringLength(100, ErrorMessage = "Tên sàn giao dịch không được vượt quá 100 ký tự")]
    public string? Broker { get; set; }

    public int? AccountId { get; set; }
}

public class UpdateInvestmentDto : CreateInvestmentDto
{
    [Required]
    public int InvestmentId { get; set; }

    public bool IsActive { get; set; } = true;
}

// Summary/Dashboard DTOs
public class InvestmentSummaryDto
{
    public decimal TotalInvested { get; set; }
    public decimal TotalCurrentValue { get; set; }
    public decimal TotalGainLoss { get; set; }
    public decimal TotalGainLossPercentage { get; set; }
    public int TotalInvestments { get; set; }
    public List<InvestmentByTypeDto> InvestmentsByType { get; set; } = new();
    public List<TopPerformingInvestmentDto> TopPerformers { get; set; } = new();
    public List<TopPerformingInvestmentDto> WorstPerformers { get; set; } = new();
}

public class InvestmentByTypeDto
{
    public string InvestmentType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal TotalCurrentValue { get; set; }
    public decimal GainLoss { get; set; }
    public decimal GainLossPercentage { get; set; }
}

public class TopPerformingInvestmentDto
{
    public int InvestmentId { get; set; }
    public string InvestmentName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal TotalInvested { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal GainLoss { get; set; }
    public decimal GainLossPercentage { get; set; }
}

// Price update DTO
public class UpdateInvestmentPriceDto
{
    [Required]
    public int InvestmentId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Giá hiện tại phải lớn hơn 0")]
    public decimal CurrentPrice { get; set; }

    public decimal? CurrentValue { get; set; }
}

// Bulk price update DTO
public class BulkUpdatePricesDto
{
    public List<UpdateInvestmentPriceDto> Investments { get; set; } = new();
}


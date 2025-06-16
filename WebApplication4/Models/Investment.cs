using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace WebApplication4.Models
{
    public class Investment
    {
        [Key]
        public int InvestmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string InvestmentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public InvestmentType InvestmentType { get; set; }

        [StringLength(20)]
        public string? Symbol { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentPrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInvested { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentValue { get; set; }

        [StringLength(100)]
        public string? Broker { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int? AccountId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account? Account { get; set; }

        // Calculated Properties
        [NotMapped]
        public decimal? TotalGainLoss => CurrentValue - TotalInvested;

        [NotMapped]
        public decimal? GainLossPercentage => TotalInvested > 0 ?
            ((CurrentValue - TotalInvested) / TotalInvested) * 100 : 0;

        [NotMapped]
        public decimal? PriceChange => CurrentPrice - PurchasePrice;

        [NotMapped]
        public decimal? PriceChangePercentage => PurchasePrice > 0 ?
            ((CurrentPrice - PurchasePrice) / PurchasePrice) * 100 : 0;
    }

    public enum InvestmentType
    {
        [EnumMember(Value = "Stocks")]
        Stocks,
        [EnumMember(Value = "Bonds")]
        Bonds,
        [EnumMember(Value = "Mutual Funds")]
        MutualFunds,
        [EnumMember(Value = "ETF")]
        ETF,
        [EnumMember(Value = "Real Estate")]
        RealEstate,
        [EnumMember(Value = "Crypto")]
        Crypto,
        [EnumMember(Value = "Other")]
        Other
    }
}

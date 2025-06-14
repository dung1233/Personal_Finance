using System.Collections.Generic;

namespace WebApplication4.Dtos
{
    public class DebtSummaryResponseDto
    {
        public int TotalDebts { get; set; }

        public decimal TotalOriginalAmount { get; set; }

        public decimal TotalCurrentBalance { get; set; }

        public decimal TotalMinimumPayment { get; set; }

        public decimal TotalInterestPaid { get; set; }

        /// <summary>
        /// Tỷ lệ nợ / thu nhập (có thể tính ở backend)
        /// </summary>
        public decimal DebtToIncomeRatio { get; set; }

        /// <summary>
        /// Danh sách các khoản nợ đang hoạt động
        /// </summary>
        public List<DebtResponseDto> ActiveDebts { get; set; } = new List<DebtResponseDto>();
    }
}

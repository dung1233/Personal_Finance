using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    public class ProcessDebtPaymentRequestDto
    {
        [Required(ErrorMessage = "PaymentDate is required.")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "PaymentAmount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "PaymentAmount must be greater than 0.")]
        public decimal PaymentAmount { get; set; }
    }
}
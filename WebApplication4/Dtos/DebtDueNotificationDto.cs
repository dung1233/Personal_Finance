namespace WebApplication4.Dtos
{
    public class DebtDueNotificationDto
    {
        public int DebtId { get; set; }
        public string DebtName { get; set; }
        public string Creditor { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public decimal MinimumPayment { get; set; }
        public decimal CurrentBalance { get; set; }
        public string NotificationMessage { get; set; }
    }
}
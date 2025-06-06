using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class GoalContribution
    {
        [Key] // hoặc đặt tên trường theo chuẩn EF Core tự nhận
        public int ContributionId { get; set; }  // Khóa chính

        public int GoalId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ContributionDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // navigation property (nếu cần)
        public Goal Goal { get; set; }
    }


}

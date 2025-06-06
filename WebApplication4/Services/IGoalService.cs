using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface IGoalService
    {
        Task<List<GoalDto>> GetGoalsAsync(int userId);
        Task<GoalDto> GetGoalByIdAsync(int userId, int goalId);
        Task<GoalDto> CreateGoalAsync(int userId, CreateGoalDto dto);
        Task<GoalDto> UpdateGoalAsync(int userId, int goalId, UpdateGoalDto dto);
        Task<bool> DeleteGoalAsync(int userId, int goalId);
        Task<bool> MarkGoalCompleteAsync(int userId, int goalId);
        Task<bool> ToggleGoalActiveAsync(int userId, int goalId);
        Task<GoalSummaryDto> GetGoalSummaryAsync(int userId);
        Task<bool> UpdateContributionAsync(int id, ContributionUpdateRequest request);
        Task<bool> DeleteContributionAsync(int id);
        Task<GoalAnalyticsDto> GetGoalAnalyticsAsync(int goalId, int userId);

        // Add the missing method definition
        Task<GoalStatisticsDto> GetGoalStatisticsAsync(int userId);
    }
}

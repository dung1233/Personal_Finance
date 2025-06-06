using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class GoalService : IGoalService
    {
        private readonly AppDbContext _context;

        public GoalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GoalDto>> GetGoalsAsync(int userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .Select(g => GoalService.MapToDto(g))
                .ToListAsync();
        }

        public async Task<GoalDto> GetGoalByIdAsync(int userId, int goalId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);

            return goal == null ? null : MapToDto(goal);
        }

        public async Task<GoalDto> CreateGoalAsync(int userId, CreateGoalDto dto)
        {
            var goal = new Goal
            {
                UserId = userId,
                GoalName = dto.GoalName,
                Description = dto.Description,
                GoalType = dto.GoalType,
                TargetAmount = dto.TargetAmount,
                CurrentAmount = 0,
                TargetDate = dto.TargetDate,
                Priority = dto.Priority,
                IsCompleted = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return MapToDto(goal);
        }

        public async Task<GoalDto> UpdateGoalAsync(int userId, int goalId, UpdateGoalDto dto)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);
            if (goal == null) return null;

            goal.GoalName = dto.GoalName;
            goal.Description = dto.Description;
            goal.GoalType = dto.GoalType;
            goal.TargetAmount = dto.TargetAmount;
            goal.TargetDate = dto.TargetDate;
            goal.Priority = dto.Priority;
            goal.IsActive = dto.IsActive;
            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(goal);
        }

        public async Task<bool> DeleteGoalAsync(int userId, int goalId)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);
            if (goal == null) return false;

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkGoalCompleteAsync(int userId, int goalId)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);
            if (goal == null || goal.IsCompleted) return false;

            goal.IsCompleted = true;
            goal.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleGoalActiveAsync(int userId, int goalId)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);
            if (goal == null) return false;

            goal.IsActive = !goal.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GoalSummaryDto> GetGoalSummaryAsync(int userId)
        {
            var goals = await _context.Goals.Where(g => g.UserId == userId).ToListAsync();

            var now = DateTime.UtcNow;
            var summary = new GoalSummaryDto
            {
                TotalGoals = goals.Count,
                CompletedGoals = goals.Count(g => g.IsCompleted),
                ActiveGoals = goals.Count(g => g.IsActive && !g.IsCompleted),
                OverdueGoals = goals.Count(g => g.TargetDate.HasValue && g.TargetDate.Value < now && !g.IsCompleted),
                TotalTargetAmount = goals.Sum(g => g.TargetAmount),
                TotalSavedAmount = goals.Sum(g => g.CurrentAmount)
            };

            return summary;
        }

        private static GoalDto MapToDto(Goal g)
        {
            return new GoalDto
            {
                GoalId = g.GoalId,
                GoalName = g.GoalName,
                Description = g.Description,
                GoalType = g.GoalType,
                TargetAmount = g.TargetAmount,
                CurrentAmount = g.CurrentAmount,
                TargetDate = g.TargetDate,
                Priority = g.Priority,
                IsCompleted = g.IsCompleted,
                CompletedAt = g.CompletedAt,
                IsActive = g.IsActive,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            };
        }
        public async Task<bool> UpdateContributionAsync(int contributionId, ContributionUpdateRequest request)
        {
            var contribution = await _context.GoalContributions.FindAsync(contributionId);
            if (contribution == null) return false;

            contribution.Amount = request.Amount;
            contribution.Notes = request.Note; // hoặc request.Note tùy DTO

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteContributionAsync(int contributionId)
        {
            var contribution = await _context.GoalContributions
                .Include(c => c.Goal)
                .FirstOrDefaultAsync(c => c.ContributionId == contributionId);

            if (contribution == null) return false;

            var goal = contribution.Goal;

            goal.CurrentAmount -= contribution.Amount;

            if (goal.IsCompleted && goal.CurrentAmount < goal.TargetAmount)
            {
                goal.IsCompleted = false;
                goal.CompletedAt = null;
            }

            _context.GoalContributions.Remove(contribution);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<GoalAnalyticsDto?> GetGoalAnalyticsAsync(int goalId, int userId)
        {
            var goal = await _context.Goals
                .Include(g => g.GoalContributions)
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);

            if (goal == null) return null;

            var contributions = goal.GoalContributions;

            return new GoalAnalyticsDto
            {
                GoalId = goal.GoalId,
                GoalName = goal.GoalName,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                ProgressPercentage = goal.TargetAmount == 0 ? 0 : (double)(goal.CurrentAmount / goal.TargetAmount * 100),
                ContributionCount = contributions.Count,
                LastContributionDate = contributions.OrderByDescending(c => c.ContributionDate).FirstOrDefault()?.ContributionDate,
                RemainingAmount = goal.TargetAmount - goal.CurrentAmount
            };
        }
        public async Task<GoalStatisticsDto> GetGoalStatisticsAsync(int userId)
        {
            var goals = await _context.Goals
                .Where(g => g.UserId == userId && g.IsActive)
                .ToListAsync();

            var totalGoals = goals.Count;
            var completedGoals = goals.Count(g => g.IsCompleted);
            var totalTarget = goals.Sum(g => g.TargetAmount);
            var totalCurrent = goals.Sum(g => g.CurrentAmount);
            var avgRate = totalGoals == 0 ? 0 : (double)(totalCurrent / totalTarget * 100);

            return new GoalStatisticsDto
            {
                TotalGoals = totalGoals,
                CompletedGoals = completedGoals,
                ActiveGoals = totalGoals - completedGoals,
                TotalTargetAmount = totalTarget,
                TotalCurrentAmount = totalCurrent,
                AverageCompletionRate = avgRate
            };
        }






    }
}

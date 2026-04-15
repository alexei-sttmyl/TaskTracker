using TaskTracker.Models;
using TaskTracker.ViewModels;

namespace TaskTracker.Services
{
    public class AchievementInfo
    {
        public string HabitName { get; set; } = string.Empty;
        public bool DailyGoalMet { get; set; }
        public bool BigGoalMet { get; set; }
        public int CompletionDaysCount { get; set; }
        public int? TargetDays { get; set; }
    }

    public interface IHabitService
    {
        Task<List<HabitViewModel>> GetUserHabitsViewAsync(string userId);

        Task<List<DateTime>> GetCompletedDatesAsync(int habitId);

        Task<List<Habit>> GetUserHabitsAsync(string userId);

        Task CreateHabitAsync(Habit habit);

        Task CompleteHabitAsync(int habitId);

        Task<bool> CompleteHabitAsync(int habitId, string userId);

        Task<AchievementInfo?> CheckAchievementsAsync(int habitId, string userId);

        Task<List<DateTime>> GetCompletedDatesAsync(int habitId, string userId);

        Task<HabitViewModel?> GetHabitDetailsAsync(int habitId, string userId);

        Task<bool> UpdateHabitAsync(int habitId, Habit updatedHabit, string userId);

        Task<bool> DeleteHabitAsync(int habitId, string userId);

        List<NotificationItem> GetTodayReminders(string userId);

        Task<HabitStatsViewModel> GetHabitStatsAsync(string userId);

        Task<UserProfileViewModel> GetUserProfileAsync(string userId);

        Task<HabitViewModel?> GetHabitDetailsWithLogsAsync(int habitId, string userId, HabitLogFilter filter);

        int CalculateStreak(List<HabitLog> logs, int dailyGoal);
    }
}
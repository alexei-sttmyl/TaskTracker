using Microsoft.Extensions.Logging;
using TaskTracker.Models;
using TaskTracker.Repositories;
using TaskTracker.ViewModels;
using NotificationType = TaskTracker.Models.NotificationType;

namespace TaskTracker.Services
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _repository;
        private readonly ILogger<HabitService> _logger;

        public HabitService(IHabitRepository repository, ILogger<HabitService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<Habit>> GetUserHabitsAsync(string userId)
        {
            return await _repository.GetUserHabitsAsync(userId);
        }

        public async Task CreateHabitAsync(Habit habit)
        {
            await _repository.AddAsync(habit);
            await _repository.SaveChangesAsync();
        }

        public async Task CompleteHabitAsync(int habitId)
        {
            var habit = await _repository.GetByIdAsync(habitId);

            if (habit == null)
                return;

            var today = DateTime.Now.Date;

            var log = habit.Logs.FirstOrDefault(l => l.Date == today);

            if (log == null)
            {
                log = new HabitLog
                {
                    HabitId = habitId,
                    Date = today,
                    CompletionCount = 1,
                    CreatedAt = DateTime.Now
                };

                habit.Logs.Add(log);
            }
            else
            {
                log.CompletionCount++;
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<bool> CompleteHabitAsync(int habitId, string userId)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);

            if (habit == null)
            {
                _logger.LogWarning("Попытка доступа к чужой привычке ID:{HabitId} пользователем {UserId}", habitId, userId);
                return false;
            }

            var today = DateTime.Now.Date;

            var log = habit.Logs.FirstOrDefault(l => l.Date == today);

            if (log == null)
            {
                log = new HabitLog
                {
                    HabitId = habitId,
                    Date = today,
                    CompletionCount = 1,
                    CreatedAt = DateTime.Now
                };

                habit.Logs.Add(log);
                _logger.LogInformation("Первое выполнение привычки ID:{HabitId} за {Date}", habitId, today);
            }
            else
            {
                log.CompletionCount++;
                log.CreatedAt = DateTime.Now;
                _logger.LogInformation("Привычка ID:{HabitId} выполнена {Count} раз за {Date}", habitId, log.CompletionCount, today);
            }

            // Увеличиваем счётчик дней с достигнутой целью
            if (log.CompletionCount >= habit.DailyGoal)
            {
                habit.CompletionDaysCount++;
            }

            await _repository.SaveChangesAsync();
            return true;
        }

        // Проверка достижений после выполнения привычки
        public async Task<AchievementInfo?> CheckAchievementsAsync(int habitId, string userId)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);
            if (habit == null) return null;

            var today = DateTime.Now.Date;
            var todayLog = habit.Logs?.FirstOrDefault(l => l.Date == today);
            if (todayLog == null) return null;

            bool dailyGoalMet = todayLog.CompletionCount >= habit.DailyGoal;
            bool bigGoalMet = habit.TargetDays.HasValue && habit.CompletionDaysCount >= habit.TargetDays.Value;

            return new AchievementInfo
            {
                HabitName = habit.Name,
                DailyGoalMet = dailyGoalMet,
                BigGoalMet = bigGoalMet,
                CompletionDaysCount = habit.CompletionDaysCount,
                TargetDays = habit.TargetDays
            };
        }

        public int CalculateStreak(List<HabitLog> logs, int dailyGoal)
        {
            if (logs == null || logs.Count == 0)
                return 0;

            var completedDates = logs
                .Where(l => l.CompletionCount >= dailyGoal)
                .Select(l => l.Date)
                .OrderByDescending(d => d)
                .ToHashSet();

            if (completedDates.Count == 0)
                return 0;

            int streak = 0;
            var currentDate = DateTime.Now.Date;

            // Если сегодня не выполнено, начинаем со вчера
            if (!completedDates.Contains(currentDate))
            {
                currentDate = currentDate.AddDays(-1);
            }

            // Идём назад, пока дни подряд выполнены
            while (completedDates.Contains(currentDate))
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }

            return streak;
        }


        public async Task<List<HabitViewModel>> GetUserHabitsViewAsync(string userId)
        {
            var habits = await _repository.GetUserHabitsAsync(userId);

            var result = new List<HabitViewModel>();

            foreach (var habit in habits)
            {
                var today = DateTime.Now.Date;

                var todayLog = habit.Logs?
                    .FirstOrDefault(l => l.Date == today);

                int todayCount = todayLog?.CompletionCount ?? 0;

                int streak = CalculateStreak(habit.Logs?.ToList() ?? new List<HabitLog>(), habit.DailyGoal);

                result.Add(new HabitViewModel
                {
                    Id = habit.Id,
                    Name = habit.Name,
                    DailyGoal = habit.DailyGoal,
                    TodayCount = todayCount,
                    Streak = streak,
                    TotalCompletions = habit.Logs?.Sum(l => l.CompletionCount) ?? 0
                });
            }

            return result;
        }

        public async Task<List<DateTime>> GetCompletedDatesAsync(int habitId)
        {
            var habit = await _repository.GetByIdAsync(habitId);

            if (habit == null)
                return new List<DateTime>();

            return habit.Logs
                .Where(l => l.CompletionCount >= habit.DailyGoal)
                .Select(l => l.Date)
                .ToList();
        }

        public async Task<List<DateTime>> GetCompletedDatesAsync(int habitId, string userId)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);

            if (habit == null)
                return new List<DateTime>();

            return habit.Logs
                .Where(l => l.CompletionCount >= habit.DailyGoal)
                .Select(l => l.Date)
                .ToList();
        }

        public async Task<HabitViewModel?> GetHabitDetailsAsync(int habitId, string userId)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);

            if (habit == null)
                return null;

            var today = DateTime.Now.Date;

            var todayLog = habit.Logs?.FirstOrDefault(l => l.Date == today);
            int todayCount = todayLog?.CompletionCount ?? 0;

            int streak = CalculateStreak(habit.Logs?.ToList() ?? new List<HabitLog>(), habit.DailyGoal);

            var recentLogs = habit.Logs
                ?.OrderByDescending(l => l.Date)
                .Take(30)
                .Select(l => new HabitLogEntryViewModel
                {
                    Date = l.Date,
                    CreatedAt = l.CreatedAt,
                    CompletionCount = l.CompletionCount,
                    GoalMet = l.CompletionCount >= habit.DailyGoal
                })
                .ToList() ?? new List<HabitLogEntryViewModel>();

            return new HabitViewModel
            {
                Id = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                DailyGoal = habit.DailyGoal,
                TodayCount = todayCount,
                Streak = streak,
                TotalCompletions = habit.Logs?.Sum(l => l.CompletionCount) ?? 0,
                TargetDays = habit.TargetDays,
                StartDate = habit.StartDate,
                RecentLogs = recentLogs
            };
        }

        public async Task<bool> UpdateHabitAsync(int habitId, Habit updatedHabit, string userId)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);

            if (habit == null)
                return false;

            habit.Name = updatedHabit.Name;
            habit.Description = updatedHabit.Description;
            habit.DailyGoal = updatedHabit.DailyGoal;
            habit.TargetDays = updatedHabit.TargetDays;

            _logger.LogInformation("Привычка ID:{HabitId} обновлена пользователем {UserId}", habitId, userId);

            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHabitAsync(int habitId, string userId)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);

            if (habit == null)
                return false;

            await _repository.DeleteAsync(habit);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Привычка ID:{HabitId} удалена пользователем {UserId}", habitId, userId);

            return true;
        }

        public List<NotificationItem> GetTodayReminders(string userId)
        {
            var habits = _repository.GetUserHabitsAsync(userId).Result;
            var today = DateTime.Now.Date;
            var notifications = new List<NotificationItem>();

            foreach (var habit in habits)
            {
                var todayLog = habit.Logs?.FirstOrDefault(l => l.Date == today);
                var todayCount = todayLog?.CompletionCount ?? 0;
                var remaining = habit.DailyGoal - todayCount;

                if (remaining > 0)
                {
                    notifications.Add(new NotificationItem
                    {
                        Message = $"«{habit.Name}» — осталось {remaining} из {habit.DailyGoal} выполнений",
                        Timestamp = todayLog?.CreatedAt ?? habit.StartDate,
                        Type = NotificationType.Reminder.ToString()
                    });
                }
                else if (todayCount > 0)
                {
                    notifications.Add(new NotificationItem
                    {
                        Message = $"«{habit.Name}» — цель на сегодня достигнута! ✅",
                        Timestamp = todayLog?.CreatedAt ?? habit.StartDate,
                        Type = NotificationType.Success.ToString()
                    });
                }
            }

            if (notifications.Count == 0)
            {
                notifications.Add(new NotificationItem
                {
                    Message = "На сегодня нет напоминаний. Создайте привычку, чтобы начать!",
                    Timestamp = DateTime.Now,
                    Type = NotificationType.Info.ToString()
                });
            }

            return notifications;
        }

        public async Task<HabitStatsViewModel> GetHabitStatsAsync(string userId)
        {
            var habits = await _repository.GetUserHabitsAsync(userId);
            var today = DateTime.Now.Date;

            var stats = new HabitStatsViewModel
            {
                TotalHabits = habits.Count
            };

            int bestStreak = 0;
            string bestStreakHabit = "";

            var last7Days = new List<DailyProgressEntry>();
            var categoryProgress = new List<CategoryProgress>();

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                int completedOnDay = 0;

                foreach (var habit in habits)
                {
                    var log = habit.Logs?.FirstOrDefault(l => l.Date == date);
                    if (log != null && log.CompletionCount >= habit.DailyGoal)
                    {
                        completedOnDay++;
                    }
                }

                last7Days.Add(new DailyProgressEntry
                {
                    DayLabel = date.ToString("dd.MM"),
                    Completed = completedOnDay,
                    Total = habits.Count
                });
            }

            foreach (var habit in habits)
            {
                var todayLog = habit.Logs?.FirstOrDefault(l => l.Date == today);
                int todayCount = todayLog?.CompletionCount ?? 0;

                if (todayCount >= habit.DailyGoal)
                {
                    stats.CompletedToday++;
                }
                else if (todayCount > 0)
                {
                    stats.PendingToday++;
                }
                else
                {
                    stats.PendingToday++;
                }

                int streak = CalculateStreak(habit.Logs?.ToList() ?? new List<HabitLog>(), habit.DailyGoal);
                if (streak > bestStreak)
                {
                    bestStreak = streak;
                    bestStreakHabit = habit.Name;
                }

                categoryProgress.Add(new CategoryProgress
                {
                    HabitName = habit.Name,
                    TodayCount = todayCount,
                    DailyGoal = habit.DailyGoal,
                    Streak = streak
                });
            }

            stats.BestStreak = bestStreak;
            stats.BestStreakHabitName = bestStreakHabit;
            stats.CompletionRate = stats.TotalHabits > 0 ? Math.Round((double)stats.CompletedToday / stats.TotalHabits * 100, 0) : 0;
            stats.Last7Days = last7Days;
            stats.CategoryProgress = categoryProgress;

            return stats;
        }

        public async Task<UserProfileViewModel> GetUserProfileAsync(string userId)
        {
            var habits = await _repository.GetUserHabitsAsync(userId);
            var today = DateTime.Now.Date;

            int totalCompletions = habits.Sum(h => h.Logs?.Sum(l => l.CompletionCount) ?? 0);
            int longestStreak = 0;

            foreach (var habit in habits)
            {
                int streak = CalculateStreak(habit.Logs?.ToList() ?? new List<HabitLog>(), habit.DailyGoal);
                if (streak > longestStreak)
                    longestStreak = streak;
            }

            return new UserProfileViewModel
            {
                Email = "", // Заполняется из UserManager
                TotalHabits = habits.Count,
                TotalCompletions = totalCompletions,
                LongestStreak = longestStreak,
                Notifications = GetTodayReminders(userId)
            };
        }

        public async Task<HabitViewModel?> GetHabitDetailsWithLogsAsync(int habitId, string userId, HabitLogFilter filter)
        {
            var habit = await _repository.GetByIdWithUserIdAsync(habitId, userId);

            if (habit == null)
                return null;

            var today = DateTime.Now.Date;
            var todayLog = habit.Logs?.FirstOrDefault(l => l.Date == today);
            int todayCount = todayLog?.CompletionCount ?? 0;
            int streak = CalculateStreak(habit.Logs?.ToList() ?? new List<HabitLog>(), habit.DailyGoal);

            var allLogs = habit.Logs
                ?.OrderByDescending(l => l.Date)
                .Select(l => new HabitLogEntryViewModel
                {
                    Date = l.Date,
                    CreatedAt = l.CreatedAt,
                    CompletionCount = l.CompletionCount,
                    GoalMet = l.CompletionCount >= habit.DailyGoal
                })
                .ToList() ?? new List<HabitLogEntryViewModel>();

            return new HabitViewModel
            {
                Id = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                DailyGoal = habit.DailyGoal,
                TodayCount = todayCount,
                Streak = streak,
                TotalCompletions = habit.Logs?.Sum(l => l.CompletionCount) ?? 0,
                TargetDays = habit.TargetDays,
                StartDate = habit.StartDate,
                RecentLogs = allLogs,
                TotalLogCount = allLogs.Count,
                CurrentPage = 1,
                PageSize = allLogs.Count,
                HasMoreLogs = false
            };
        }
    }
}
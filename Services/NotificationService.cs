using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string message, NotificationType type, int? habitId = null);
        Task<List<Notification>> GetRecentNotificationsAsync(string userId, int days = 30);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
        Task GenerateRemindersForIncompleteHabitsAsync();
        Task<List<PendingToastNotification>> GetPendingToastNotificationsAsync(string userId);
    }

    public class PendingToastNotification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? HabitId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateNotificationAsync(string userId, string message, NotificationType type, int? habitId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                HabitId = habitId,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetRecentNotificationsAsync(string userId, int days = 30)
        {
            var since = DateTime.Now.Date.AddDays(-days);

            return await _context.Notifications
                .Where(n => n.UserId == userId && n.CreatedAt >= since)
                .OrderByDescending(n => n.CreatedAt)
                .Include(n => n.Habit)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task GenerateRemindersForIncompleteHabitsAsync()
        {
            var today = DateTime.Now.Date;
            var habits = await _context.Habits
                .Include(h => h.Logs)
                .ToListAsync();

            foreach (var habit in habits)
            {
                var todayLog = habit.Logs.FirstOrDefault(l => l.Date == today);
                int todayCount = todayLog?.CompletionCount ?? 0;

                if (todayCount < habit.DailyGoal)
                {
                    var lastReminder = await _context.Notifications
                        .Where(n => n.HabitId == habit.Id && n.Type == NotificationType.Reminder && n.CreatedAt >= today)
                        .OrderByDescending(n => n.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (lastReminder == null || (DateTime.Now - lastReminder.CreatedAt).TotalMinutes >= habit.NotificationIntervalMinutes)
                    {
                        var remaining = habit.DailyGoal - todayCount;

                        await CreateNotificationAsync(
                            habit.UserId,
                            $"«{habit.Name}» — осталось {remaining} выполнений на сегодня",
                            NotificationType.Reminder,
                            habit.Id
                        );

                        _logger.LogInformation("Создано напоминание для привычки {HabitId}: осталось {Remaining}", habit.Id, remaining);
                    }
                }
            }
        }

        public async Task<List<PendingToastNotification>> GetPendingToastNotificationsAsync(string userId)
        {
            var now = DateTime.Now;
            var today = now.Date;

            // Получаем все непрочитанные уведомления за сегодня
            var todayNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.CreatedAt >= today && !n.IsRead)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();

            var pending = new List<PendingToastNotification>();

            foreach (var notif in todayNotifications)
            {
                if (notif.Type == NotificationType.Reminder && notif.HabitId.HasValue)
                {
                    var habit = await _context.Habits.FindAsync(notif.HabitId.Value);
                    if (habit != null)
                    {
                        var interval = habit.NotificationIntervalMinutes;
                        if (interval <= 0) interval = 15;

                        var timeSinceCreation = (now - notif.CreatedAt).TotalMinutes;
                        if (timeSinceCreation >= 0.2)
                        {
                            pending.Add(new PendingToastNotification
                            {
                                Id = notif.Id,
                                Message = notif.Message,
                                Type = notif.Type.ToString(),
                                HabitId = notif.HabitId,
                                CreatedAt = notif.CreatedAt
                            });
                        }
                    }
                }
                else
                {
                    // Для Success, Achievement, Info — показываем сразу
                    var timeSinceCreation = (now - notif.CreatedAt).TotalSeconds;
                    if (timeSinceCreation >= 2)
                    {
                        pending.Add(new PendingToastNotification
                        {
                            Id = notif.Id,
                            Message = notif.Message,
                            Type = notif.Type.ToString(),
                            HabitId = notif.HabitId,
                            CreatedAt = notif.CreatedAt
                        });
                    }
                }
            }

            return pending;
        }
    }
}

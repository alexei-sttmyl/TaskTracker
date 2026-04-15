using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Habit
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        // Связь с пользователем
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public int? TargetDays { get; set; }

        public int DailyGoal { get; set; } = 1;

        // Интервал напоминаний в минутах (по умолчанию 15)
        public int NotificationIntervalMinutes { get; set; } = 15;

        // Количество дней с достигнутой целью
        public int CompletionDaysCount { get; set; }

        // Логи
        public ICollection<HabitLog> Logs { get; set; } = new List<HabitLog>();

        // Уведомления
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

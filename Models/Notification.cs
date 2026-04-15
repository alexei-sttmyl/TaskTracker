using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public enum NotificationType
    {
        Reminder,    // Напоминание о невыполненной привычке
        Success,     // Цель достигнута (минималистичное)
        Info,        // Общая информация
        Streak,      // Streak обновлён
        Achievement  // Красочное поздравление с большим достижением
    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? HabitId { get; set; }
        public Habit? Habit { get; set; }

        [Required]
        public required string Message { get; set; }

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

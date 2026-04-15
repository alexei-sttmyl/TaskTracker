namespace TaskTracker.Models
{
    public class HabitLog
    {
        public int Id { get; set; }

        // Связь с привычкой
        public int HabitId { get; set; }
        public Habit? Habit { get; set; }

        // Дата
        public DateTime Date { get; set; }

        // Сколько раз выполнено
        public int CompletionCount { get; set; } = 0;

        // Время создания записи
        public DateTime CreatedAt { get; set; }
    }
}
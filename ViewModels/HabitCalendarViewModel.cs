using TaskTracker.Models;

namespace TaskTracker.ViewModels
{
    public class HabitCalendarViewModel
    {
        public int HabitId { get; set; }

        public required string HabitName { get; set; }

        public int DailyGoal { get; set; }

        public List<HabitLog> Logs { get; set; } = new();

        public DateTime ViewDate { get; set; } = DateTime.UtcNow;
    }
}

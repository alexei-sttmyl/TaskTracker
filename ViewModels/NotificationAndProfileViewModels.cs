namespace TaskTracker.ViewModels
{
    public class NotificationItem
    {
        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string Type { get; set; } = "Info";
    }

    public class HabitStatsViewModel
    {
        public int TotalHabits { get; set; }

        public int CompletedToday { get; set; }

        public int PendingToday { get; set; }

        public double CompletionRate { get; set; }

        public int BestStreak { get; set; }

        public string BestStreakHabitName { get; set; } = string.Empty;

        public List<DailyProgressEntry> Last7Days { get; set; } = new();

        public List<CategoryProgress> CategoryProgress { get; set; } = new();
    }

    public class DailyProgressEntry
    {
        public string DayLabel { get; set; } = string.Empty;

        public int Completed { get; set; }

        public int Total { get; set; }
    }

    public class CategoryProgress
    {
        public string HabitName { get; set; } = string.Empty;

        public int TodayCount { get; set; }

        public int DailyGoal { get; set; }

        public int Streak { get; set; }

        public double CompletionPercent => DailyGoal > 0 ? Math.Min(100, (double)TodayCount / DailyGoal * 100) : 0;
    }

    public class UserProfileViewModel
    {
        public string Email { get; set; } = string.Empty;

        public int TotalHabits { get; set; }

        public int TotalCompletions { get; set; }

        public int LongestStreak { get; set; }

        public DateTime MemberSince { get; set; }

        public List<NotificationItem> Notifications { get; set; } = new();
    }
}

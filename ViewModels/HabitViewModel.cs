namespace TaskTracker.ViewModels
{
    public class HabitViewModel
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public int DailyGoal { get; set; }

        public int TodayCount { get; set; }

        public int Streak { get; set; }

        public int TotalCompletions { get; set; }

        public int? TargetDays { get; set; }

        public DateTime StartDate { get; set; }

        public List<HabitLogEntryViewModel> RecentLogs { get; set; } = new();

        // Для пагинации истории
        public int TotalLogCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool HasMoreLogs { get; set; }
    }

    public class HabitLogEntryViewModel
    {
        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CompletionCount { get; set; }

        public bool GoalMet { get; set; }
    }

    public class HabitLogFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? GoalMetOnly { get; set; }
    }
}
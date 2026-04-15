namespace TaskTracker.Services
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime ConvertFromUtc(DateTime utcDateTime);
        DateTime ConvertToUtc(DateTime localDateTime);
        string GetTimeZoneDisplayName();
    }

    public class TimeService : ITimeService
    {
        private readonly TimeZoneInfo _timeZone;

        public TimeService()
        {
            // Определяем часовой пояс системы
            try
            {
                _timeZone = TimeZoneInfo.Local;
            }
            catch
            {
                _timeZone = TimeZoneInfo.Utc;
            }
        }

        public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);

        public DateTime Today => Now.Date;

        public DateTime ConvertFromUtc(DateTime utcDateTime) =>
            TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _timeZone);

        public DateTime ConvertToUtc(DateTime localDateTime) =>
            TimeZoneInfo.ConvertTimeToUtc(localDateTime, _timeZone);

        public string GetTimeZoneDisplayName()
        {
            var offset = _timeZone.BaseUtcOffset;
            var sign = offset >= TimeSpan.Zero ? "+" : "-";
            return $"UTC{sign}{Math.Abs(offset.Hours):D2}:{Math.Abs(offset.Minutes):D2} ({_timeZone.DisplayName})";
        }
    }
}

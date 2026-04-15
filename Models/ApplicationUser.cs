using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public string? Nickname { get; set; }
        public string? TimeZoneId { get; set; }

        public ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(Nickname)) return Nickname;
            if (!string.IsNullOrWhiteSpace(FirstName)) return FirstName;
            return Email ?? "Пользователь";
        }
    }
}

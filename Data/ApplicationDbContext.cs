using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Habit> Habits { get; set; } = null!;
        public DbSet<HabitLog> HabitLogs { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Habit>()
                .HasOne(h => h.User)
                .WithMany(u => u.Habits)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HabitLog>()
                .HasOne(l => l.Habit)
                .WithMany(h => h.Logs)
                .HasForeignKey(l => l.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HabitLog>()
                .HasIndex(l => new { l.HabitId, l.Date })
                .IsUnique();

            // Notification → User
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification → Habit (nullable, NO ACTION для избежания цикла)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Habit)
                .WithMany(h => h.Notifications)
                .HasForeignKey(n => n.HabitId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

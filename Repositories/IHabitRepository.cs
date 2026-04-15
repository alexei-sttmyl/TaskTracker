using TaskTracker.Models;

namespace TaskTracker.Repositories
{
    public interface IHabitRepository
    {
        Task<List<Habit>> GetUserHabitsAsync(string userId);

        Task<Habit?> GetByIdAsync(int id);

        Task<Habit?> GetByIdWithUserIdAsync(int id, string userId);

        Task AddAsync(Habit habit);

        Task UpdateAsync(Habit habit);

        Task DeleteAsync(Habit habit);

        Task SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Repositories
{
    public class HabitRepository : IHabitRepository
    {
        private readonly ApplicationDbContext _context;

        public HabitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Habit>> GetUserHabitsAsync(string userId)
        {
            return await _context.Habits
            .Where(h => h.UserId == userId)
            .Include(h => h.Logs)
            .ToListAsync();
        }

        public async Task<Habit?> GetByIdAsync(int id)
        {
            return await _context.Habits
                .Include(h => h.Logs)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<Habit?> GetByIdWithUserIdAsync(int id, string userId)
        {
            return await _context.Habits
                .Include(h => h.Logs)
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
        }

        public async Task AddAsync(Habit habit)
        {
            await _context.Habits.AddAsync(habit);
        }

        public Task UpdateAsync(Habit habit)
        {
            _context.Habits.Update(habit);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Habit habit)
        {
            _context.Habits.Remove(habit);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
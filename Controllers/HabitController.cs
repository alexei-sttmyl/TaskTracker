using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.ViewModels;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class HabitController : Controller
    {
        private readonly IHabitService _service;
        private readonly INotificationService _notificationService;
        private readonly ILogger<HabitController> _logger;

        public HabitController(
            IHabitService service,
            INotificationService notificationService,
            ILogger<HabitController> logger)
        {
            _service = service;
            _notificationService = notificationService;
            _logger = logger;
        }

        //  Список привычек
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var habits = await _service.GetUserHabitsViewAsync(userId);

            return View(habits);
        }

        //  Создание (GET)
        public IActionResult Create()
        {
            return View();
        }

        //  Создание (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Habit habit)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Создание привычки не прошло валидацию: {Errors}", string.Join("; ", errors));
                TempData["Error"] = "Пожалуйста, исправьте ошибки в форме.";
                return View(habit);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                habit.UserId = userId;

                await _service.CreateHabitAsync(habit);

                _logger.LogInformation("Пользователь {UserId} создал привычку {HabitName}", userId, habit.Name);

                TempData["Success"] = $"Привычка «{habit.Name}» успешно создана!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании привычки");
                TempData["Error"] = "Произошла ошибка при создании привычки. Попробуйте ещё раз.";
                return View(habit);
            }
        }

        //  Отметка выполнения
        public async Task<IActionResult> Complete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var success = await _service.CompleteHabitAsync(id, userId);

            if (!success)
            {
                _logger.LogWarning("Пользователь {UserId} попытался отметить чужую привычку ID:{HabitId}", userId, id);
                return NotFound();
            }

            // Проверка достижений
            var achievement = await _service.CheckAchievementsAsync(id, userId);
            if (achievement != null)
            {
                if (achievement.BigGoalMet)
                {
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        $"🏆 НЕВЕРОЯТНО! Вы достигли главной цели «{achievement.HabitName}» — {achievement.CompletionDaysCount} дней подряд! Вы настоящий герой! 🎉🔥✨",
                        NotificationType.Achievement,
                        id
                    );
                    _logger.LogInformation("🏆 БОЛЬШОЕ ДОСТИЖЕНИЕ: {HabitName} — {Days} дней", achievement.HabitName, achievement.CompletionDaysCount);
                }
                else if (achievement.DailyGoalMet)
                {
                    await _notificationService.CreateNotificationAsync(
                        userId,
                        $"🎯 «{achievement.HabitName}» — дневная цель достигнута!",
                        NotificationType.Success,
                        id
                    );
                }
            }

            _logger.LogInformation("Пользователь {UserId} отметил привычку ID:{HabitId}", userId, id);

            TempData["Success"] = "Выполнение отмечено! 🔥";

            return RedirectToAction("Index");
        }

        //  Детали привычки
        public async Task<IActionResult> Details(int id, int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var filter = new HabitLogFilter { Page = page, PageSize = pageSize };

            var details = await _service.GetHabitDetailsWithLogsAsync(id, userId, filter);

            if (details == null)
            {
                return NotFound();
            }

            ViewData["Filter"] = filter;

            return View(details);
        }

        //  Редактирование (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var details = await _service.GetHabitDetailsAsync(id, userId);

            if (details == null)
            {
                return NotFound();
            }

            return View(details);
        }

        //  Редактирование (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Habit habit)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (!ModelState.IsValid)
            {
                var details = await _service.GetHabitDetailsAsync(id, userId);
                return details != null ? View(details) : NotFound();
            }

            var success = await _service.UpdateHabitAsync(id, habit, userId);

            if (!success)
            {
                return NotFound();
            }

            TempData["Success"] = $"Привычка «{habit.Name}» обновлена!";

            return RedirectToAction("Details", new { id });
        }

        //  Удаление (GET — подтверждение)
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var details = await _service.GetHabitDetailsAsync(id, userId);

            if (details == null)
            {
                return NotFound();
            }

            return View(details);
        }

        //  Удаление (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var details = await _service.GetHabitDetailsAsync(id, userId);
            var habitName = details?.Name ?? "Привычка";

            var success = await _service.DeleteHabitAsync(id, userId);

            if (!success)
            {
                return NotFound();
            }

            _logger.LogInformation("Пользователь {UserId} удалил привычку ID:{HabitId}", userId, id);

            TempData["Success"] = $"Привычка «{habitName}» удалена.";

            return RedirectToAction("Index");
        }

        //  Календарь
        public async Task<IActionResult> Calendar(int id, int? year, int? month)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var viewDate = new DateTime(
                year ?? DateTime.Now.Year,
                month ?? DateTime.Now.Month,
                1);

            var habit = await _service.GetUserHabitsAsync(userId);
            var targetHabit = habit.FirstOrDefault(h => h.Id == id);

            if (targetHabit == null)
            {
                return NotFound();
            }

            var logs = targetHabit.Logs
                .Where(l => l.Date.Year == viewDate.Year && l.Date.Month == viewDate.Month)
                .ToList();

            var viewModel = new HabitCalendarViewModel
            {
                HabitId = id,
                HabitName = targetHabit.Name,
                DailyGoal = targetHabit.DailyGoal,
                Logs = logs,
                ViewDate = viewDate
            };

            return View(viewModel);
        }
    }
}

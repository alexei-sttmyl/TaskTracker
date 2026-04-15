using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.ViewModels;

namespace TaskTracker.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHabitService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IHabitService service,
            UserManager<ApplicationUser> userManager,
            ILogger<ProfileController> logger)
        {
            _service = service;
            _userManager = userManager;
            _logger = logger;
        }

        //  Страница профиля
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var profile = await _service.GetUserProfileAsync(userId);
            profile.Email = user.Email ?? "";
            profile.MemberSince = user.CreatedAt();

            var stats = await _service.GetHabitStatsAsync(userId);

            ViewData["Stats"] = stats;

            return View(profile);
        }

        //  Раскрывающийся список уведомлений (для AJAX)
        [HttpGet]
        public IActionResult GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var notifications = _service.GetTodayReminders(userId);
            return Json(notifications.Select(n => new
            {
                message = n.Message,
                timestamp = n.Timestamp.ToString("dd.MM.yyyy HH:mm"),
                type = n.Type.ToString()
            }));
        }

        //  Тестовое уведомление
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendTestNotification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var notifications = _service.GetTodayReminders(userId);

            var summary = string.Join("\n", notifications.Select(n => $"• {n.Message}"));

            TempData["Success"] = $"📬 Тестовое уведомление:\n{summary}";

            _logger.LogInformation("Тестовое уведомление для пользователя {UserId}: {Count} напоминаний", userId, notifications.Count);

            return RedirectToAction("Index");
        }

        //  Экспорт в Excel
        public async Task<IActionResult> ExportToExcel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var habits = await _service.GetUserHabitsAsync(userId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Прогресс привычек");

            // Заголовок
            worksheet.Cell(1, 1).Value = "Отчёт по привычкам";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Cell(1, 1).Style.Font.FontName = "Inter";
            worksheet.Range(1, 1, 1, 5).Merge();

            // Шапка таблицы
            var headers = new[] { "Привычка", "Сегодня", "Цель", "Streak 🔥", "Всего" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(3, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0077cc");
                cell.Style.Font.FontColor = XLColor.White;
            }

            // Данные
            var today = DateTime.UtcNow.Date;
            int row = 4;
            foreach (var habit in habits)
            {
                var todayLog = habit.Logs?.FirstOrDefault(l => l.Date == today);
                int todayCount = todayLog?.CompletionCount ?? 0;
                int streak = _service.CalculateStreak(habit.Logs?.ToList() ?? new List<HabitLog>(), habit.DailyGoal);
                int total = habit.Logs?.Sum(l => l.CompletionCount) ?? 0;

                worksheet.Cell(row, 1).Value = habit.Name;
                worksheet.Cell(row, 2).Value = todayCount;
                worksheet.Cell(row, 3).Value = habit.DailyGoal;
                worksheet.Cell(row, 4).Value = streak;
                worksheet.Cell(row, 5).Value = total;

                // Подсветка выполненных
                if (todayCount >= habit.DailyGoal)
                {
                    for (int c = 1; c <= 5; c++)
                        worksheet.Cell(row, c).Style.Fill.BackgroundColor = XLColor.FromArgb(212, 237, 218);
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"HabitReport_{DateTime.UtcNow:yyyy-MM-dd}.xlsx");
        }
    }

    // Extension method для безопасного получения даты создания
    public static class IdentityUserExtensions
    {
        public static DateTime CreatedAt(this ApplicationUser user)
        {
            // IdentityUser не имеет CreationDate, используем текущую дату как fallback
            // В реальном проекте можно добавить поле CreatedAt в ApplicationUser
            return DateTime.UtcNow;
        }
    }
}

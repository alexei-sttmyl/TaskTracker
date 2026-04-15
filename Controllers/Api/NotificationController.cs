using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Services;

namespace TaskTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Получить непрочитанные + последние за 30 дней
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var notifications = await _notificationService.GetRecentNotificationsAsync(userId);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

            return Ok(new
            {
                unreadCount,
                notifications = notifications.Select(n => new
                {
                    id = n.Id,
                    message = n.Message,
                    type = n.Type.ToString(),
                    isRead = n.IsRead,
                    createdAt = n.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    habitId = n.HabitId,
                    habitName = n.Habit?.Name
                })
            });
        }

        // Отметить как прочитанное
        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok();
        }

        // Отметить все как прочитанные
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok();
        }

        // Получить количество непрочитанных
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        // Получить уведомления для toast-показа (по интервалу)
        [HttpGet("pending-toasts")]
        public async Task<IActionResult> GetPendingToasts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var pending = await _notificationService.GetPendingToastNotificationsAsync(userId);

            return Ok(pending.Select(n => new
            {
                id = n.Id,
                message = n.Message,
                type = n.Type,
                habitId = n.HabitId,
                createdAt = n.CreatedAt.ToString("dd.MM.yyyy HH:mm")
            }));
        }
    }
}

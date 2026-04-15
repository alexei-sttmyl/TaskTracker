namespace TaskTracker.Services
{
    /// <summary>
    /// Фоновый сервис — проверяет привычки каждые 5 минут и создаёт напоминания
    /// </summary>
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновый сервис напоминаний запущен (интервал: {Interval} мин)", _checkInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await notificationService.GenerateRemindersForIncompleteHabitsAsync();

                    _logger.LogDebug("Проверка напоминаний выполнена в {Time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при проверке напоминаний");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Фоновый сервис напоминаний остановлен");
        }
    }
}

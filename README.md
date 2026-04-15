# 📋 TaskTracker

> Веб-приложение для отслеживания привычек и формирования полезных ежедневных практик

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=aspdotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0.5-512BD4)](https://docs.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)](Dockerfile)

---

## 📖 О проекте

**TaskTracker** — это многопользовательское веб-приложение, которое помогает формировать полезные привычки, отслеживать прогресс и достигать поставленных целей.

### Возможности

| Функция | Описание |
|---------|----------|
| 🔐 **Аутентификация** | Регистрация и вход через ASP.NET Core Identity |
| ✅ **Управление привычками** | Создание, редактирование, удаление привычек с настройками |
| 🎯 **Ежедневные цели** | Настраиваемое количество выполнений в день |
| 🏆 **Большие цели** | Глобальная цель в днях с отслеживанием прогресса |
| 🔥 **Streak-система** | Автоматический расчёт серии дней подряд |
| 📅 **Календарь** | Визуальное представление истории выполнения по дням |
| 🔔 **Уведомления** | Напоминания, достижения, toast-оповещения в браузере |
| ⏰ **Фоновые напоминания** | Автоматическая проверка каждые 5 минут |
| 📊 **Статистика** | Прогресс за 7 дней, графики, аналитика |
| 📤 **Экспорт в Excel** | Генерация отчёта через ClosedXML |
| 👤 **Профиль** | Общая статистика, уведомления, настройки |
| 🌍 **Часовые пояса** | Поддержка временных зон пользователей |

---

## 🖥️ Скриншоты

> Добавьте скриншоты вашего приложения в папку `screenshots/` и укажите ссылки здесь.

---

## 🛠️ Стек технологий

### Backend

| Технология | Версия | Назначение |
|------------|--------|------------|
| [.NET 10.0](https://dotnet.microsoft.com/) | 10.0 | Платформа выполнения |
| [ASP.NET Core MVC](https://docs.microsoft.com/aspnet/core/mvc/) | 10.0 | Веб-фреймворк |
| [Entity Framework Core](https://docs.microsoft.com/ef/core/) | 10.0.5 | ORM |
| [SQL Server](https://www.microsoft.com/sql-server) | — | База данных |
| [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity) | 10.0.5 | Аутентификация и авторизация |
| [ClosedXML](https://github.com/ClosedXML/ClosedXML) | 0.105.0 | Экспорт в Excel |

### Frontend

| Технология | Версия | Назначение |
|------------|--------|------------|
| [Bootstrap](https://getbootstrap.com/) | 5.x | Адаптивный дизайн |
| [jQuery](https://jquery.com/) | 3.x | DOM-манипуляции, AJAX |
| [Chart.js](https://www.chartjs.org/) | 4.x | Графики и диаграммы |
| [Google Fonts](https://fonts.google.com/) | — | Шрифты Inter, Playfair Display |

### Инструменты

| Технология | Назначение |
|------------|------------|
| [Docker](https://www.docker.com/) | Контейнеризация |
| [Visual Studio 2026](https://visualstudio.microsoft.com/) | IDE |

---

## 🏗️ Архитектура

Проект построен по принципам **Clean Architecture** с разделением на слои:

```
┌─────────────────────────────────────────────────────┐
│              PRESENTATION LAYER                      │
│  Controllers │ Razor Pages │ Views │ REST API       │
├─────────────────────────────────────────────────────┤
│                SERVICE LAYER                         │
│  HabitService │ NotificationService │ TimeService   │
│  ReminderBackgroundService (HostedService)           │
├─────────────────────────────────────────────────────┤
│              REPOSITORY LAYER                        │
│  IHabitRepository │ HabitRepository                 │
├─────────────────────────────────────────────────────┤
│                DATA LAYER                            │
│  ApplicationDbContext │ EF Core │ Migrations        │
├─────────────────────────────────────────────────────┤
│                DOMAIN LAYER                          │
│  ApplicationUser │ Habit │ HabitLog │ Notification   │
└─────────────────────────────────────────────────────┘
```

### Паттерны проектирования

- **Repository Pattern** — абстракция доступа к данным
- **Service Layer** — бизнес-логика
- **Dependency Injection** — внедрение зависимостей
- **Unit of Work** — управление транзакциями
- **Background Service** — фоновые процессы
- **DTO/ViewModel** — разделение моделей и представлений

---

## 📂 Структура проекта

```
TaskTracker/
├── 📁 Controllers/          # MVC-контроллеры
│   ├── Api/
│   │   └── NotificationController.cs    # REST API уведомлений
│   ├── HomeController.cs                # Главная страница
│   ├── HabitController.cs               # CRUD привычек
│   └── ProfileController.cs             # Профиль и статистика
├── 📁 Models/               # Доменные сущности
│   ├── ApplicationUser.cs               # Пользователь (Identity)
│   ├── Habit.cs                         # Привычка
│   ├── HabitLog.cs                      # Журнал выполнений
│   ├── Notification.cs                  # Уведомления
│   └── ErrorViewModel.cs
├── 📁 ViewModels/           # DTO для представлений
│   ├── HabitViewModel.cs
│   ├── HabitCalendarViewModel.cs
│   └── NotificationAndProfileViewModels.cs
├── 📁 Repositories/         # Слой доступа к данным
│   ├── IHabitRepository.cs
│   └── HabitRepository.cs
├── 📁 Services/             # Бизнес-логика
│   ├── IHabitService.cs
│   ├── HabitService.cs
│   ├── INotificationService.cs
│   ├── NotificationService.cs
│   ├── ITimeService.cs
│   ├── TimeService.cs
│   └── ReminderBackgroundService.cs     # Фоновый сервис
├── 📁 Data/                 # EF Core
│   ├── ApplicationDbContext.cs
│   └── Migrations/                      # Миграции БД
├── 📁 Areas/Identity/       # ASP.NET Core Identity (Razor Pages)
│   └── Pages/Account/
│       ├── Login.cshtml
│       ├── Register.cshtml
│       ├── Logout.cshtml
│       └── Manage/Index.cshtml
├── 📁 Views/                # Razor-представления
│   ├── Home/
│   ├── Habit/
│   ├── Profile/
│   └── Shared/
├── 📁 wwwroot/              # Статические файлы
│   ├── css/site.css
│   ├── js/site.js
│   ├── lib/                 # Библиотеки (Bootstrap, jQuery, Chart.js)
│   └── favicon.ico
├── 📁 Properties/           # Настройки запуска
├── Program.cs               # Точка входа
├── appsettings.json         # Конфигурация
├── Dockerfile               # Docker-образ
├── .gitignore
└── TaskTracker.csproj       # Файл проекта
```

---

## 🚀 Быстрый старт

### Предварительные требования

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (локальный)
- [Visual Studio 2026](https://visualstudio.microsoft.com/) или [VS Code](https://code.visualstudio.com/) (опционально)

### Установка

1. **Клонируйте репозиторий**

   ```bash
   git clone https://github.com/your-username/TaskTracker.git
   cd TaskTracker
   ```

2. **Восстановите зависимости**

   ```bash
   dotnet restore
   ```

3. **Настройте строку подключения**

   Откройте `appsettings.json` и измените строку подключения:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ttdb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

   > Для SQL Server Authentication используйте:
   > ```
   > Server=localhost;Database=ttdb;User Id=your_username;Password=your_password;TrustServerCertificate=True;
   > ```

4. **Примените миграции**

   ```bash
   dotnet ef database update
   ```

5. **Запустите приложение**

   ```bash
   dotnet run
   ```

6. **Откройте браузер**

   - HTTP: http://localhost:5282
   - HTTPS: https://localhost:7049

---

## 🐳 Docker

### Сборка образа

```bash
docker build -t tasktracker .
```

### Запуск контейнера

```bash
docker run -d -p 8080:8080 -p 8081:8081 --name tasktracker tasktracker
```

> ⚠️ Убедитесь, что SQL Server доступен из контейнера или используйте Docker Compose с сервисом SQL Server.

---

## 📊 База данных

### Миграции

| Миграция | Описание |
|----------|----------|
| `CreateIdentitySchema` | Таблицы ASP.NET Identity |
| `AddHabitEntities` | Привычки и журналы выполнений |
| `AddUserProfileFields` | Поля профиля (имя, псевдоним, часовой пояс) |
| `AddHabitLogCreatedAt` | Время создания записи лога |
| `AddNotificationsAndHabitInterval` | Уведомления и интервал напоминаний |
| `AddCompletionDaysCount` | Счётчик дней с достигнутой целью |

### Создание новой миграции

```bash
dotnet ef migrations add MigrationName
```

### Откат миграции

```bash
dotnet ef database update PreviousMigrationName
```

### Удаление последней миграции

```bash
dotnet ef migrations remove
```

---

## ⚙️ Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ttdb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Настройки Identity

- Подтверждение email: **отключено**
- Минимальная длина пароля: **6 символов**
- Требования к сложности: **отключены** (без цифр, спецсимволов, регистра)

### Фоновый сервис напоминаний

- Интервал проверки: **каждые 5 минут**
- Создаёт напоминания для привычек, не достигших дневной цели
- Учитывает пользовательский интервал (`NotificationIntervalMinutes`)

---

## 🔑 API Endpoints

### REST API

| Метод | Путь | Описание |
|-------|------|----------|
| `GET` | `/api/Notification/recent` | Последние уведомления + счётчик непрочитанных |
| `POST` | `/api/Notification/{id}/read` | Отметить уведомление как прочитанное |
| `POST` | `/api/Notification/mark-all-read` | Прочитать все уведомления |
| `GET` | `/api/Notification/unread-count` | Количество непрочитанных |
| `GET` | `/api/Notification/pending-toasts` | Toast-уведомления для показа |

### MVC Routes

| Путь | Описание |
|------|----------|
| `/` | Главная страница |
| `/Habit` | Список привычек |
| `/Habit/Create` | Создание привычки |
| `/Habit/Details/{id}` | Детали привычки |
| `/Habit/Edit/{id}` | Редактирование |
| `/Habit/Calendar/{id}` | Календарь выполнений |
| `/Profile` | Профиль и статистика |
| `/Profile/ExportToExcel` | Экспорт в Excel |

---

## 🧪 Тестирование

> Автоматические тесты в проекте отсутствуют. Рекомендуется добавить:
>
> - Unit-тесты для `HabitService` (расчёт streak, достижения, выполнение)
> - Unit-тесты для `NotificationService`
> - Integration-тесты для API-эндпоинтов

---

## 🤝 Вклад в проект

1. Сделайте форк репозитория
2. Создайте ветку для новой функциональности (`git checkout -b feature/AmazingFeature`)
3. Зафиксируйте изменения (`git commit -m 'Add some AmazingFeature'`)
4. Отправьте в ветку (`git push origin feature/AmazingFeature`)
5. Откройте Pull Request

---

## 📝 Лицензия

Этот проект распространяется под лицензией MIT. См. файл [LICENSE](LICENSE) для подробностей.

---

## 👤 Автор

**AOtchik**

---

## 📚 Дополнительные ресурсы

- [Документация ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Документация Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Документация .NET](https://docs.microsoft.com/dotnet/)
- [Chart.js Documentation](https://www.chartjs.org/docs/latest/)
- [ClosedXML Documentation](https://closedxml.readthedocs.io/)

---

<div align="center">

**⭐ Если проект понравился — поставьте звезду!**

</div>

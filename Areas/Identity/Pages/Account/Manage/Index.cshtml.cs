using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskTracker.Models;

namespace TaskTracker.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public string? Username { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        public class InputModel
        {
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Имя")]
            public string? FirstName { get; set; }

            [Display(Name = "Фамилия")]
            public string? LastName { get; set; }

            [Display(Name = "Отчество")]
            public string? Patronymic { get; set; }

            [Display(Name = "Ник")]
            public string? Nickname { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Текущий пароль")]
            public string? CurrentPassword { get; set; }

            [StringLength(100, ErrorMessage = "{0} должен содержать минимум {2} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Новый пароль")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите новый пароль")]
            [Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
            public string? ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound();

            Username = user.Email;
            Input = new InputModel
            {
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                Nickname = user.Nickname
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                Username = user.Email;
                return Page();
            }

            // Обновляем профиль
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Patronymic = Input.Patronymic;
            user.Nickname = Input.Nickname;

            var profileResult = await _userManager.UpdateAsync(user);
            if (!profileResult.Succeeded)
            {
                foreach (var error in profileResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                Username = user.Email;
                return Page();
            }

            // Меняем пароль если указан
            if (!string.IsNullOrEmpty(Input.CurrentPassword))
            {
                var passwordResult = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword!);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    Username = user.Email;
                    return Page();
                }

                await _signInManager.RefreshSignInAsync(user);
            }

            _logger.LogInformation("Пользователь {UserId} обновил настройки аккаунта", user.Id);
            SuccessMessage = "Настройки аккаунта успешно сохранены!";

            return RedirectToPage();
        }

        private Task<ApplicationUser?> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}

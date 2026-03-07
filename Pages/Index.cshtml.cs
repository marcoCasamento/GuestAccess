using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using GuestAccess.Models;
using Microsoft.AspNetCore.Authorization;

namespace GuestAccess.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public bool CanOpenCancello { get; set; }
    public bool CanOpenCancelletto { get; set; }
    public bool CanOpenPortone { get; set; }
    public bool HasAnyPermission => CanOpenCancello || CanOpenCancelletto || CanOpenPortone;
    public string UserEmail { get; set; } = string.Empty;

    [TempData]
    public string? Message { get; set; }

    public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        UserEmail = user.Email ?? "Unknown";
        CanOpenCancello = user.CanOpenCancello;
        CanOpenCancelletto = user.CanOpenCancelletto;
        CanOpenPortone = user.CanOpenPortone;

        return Page();
    }

    public async Task<IActionResult> OnPostOpenCancelloAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.CanOpenCancello)
        {
            _logger.LogWarning("Unauthorized access attempt to Cancello by user {UserId}", user?.Id ?? "Unknown");
            return Forbid();
        }

        _logger.LogInformation("USER: {Email} (ID: {UserId}) - ACTION: Opening Cancello", user.Email, user.Id);
        Console.WriteLine($"🚪 APRI CANCELLO - User: {user.Email} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        Message = "Cancello aperto con successo!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostOpenCancelettoAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.CanOpenCancelletto)
        {
            _logger.LogWarning("Unauthorized access attempt to Cancelletto by user {UserId}", user?.Id ?? "Unknown");
            return Forbid();
        }

        _logger.LogInformation("USER: {Email} (ID: {UserId}) - ACTION: Opening Cancelletto", user.Email, user.Id);
        Console.WriteLine($"🚪 APRI CANCELLETTO - User: {user.Email} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        Message = "Cancelletto aperto con successo!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostOpenPortoneAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.CanOpenPortone)
        {
            _logger.LogWarning("Unauthorized access attempt to Portone by user {UserId}", user?.Id ?? "Unknown");
            return Forbid();
        }

        _logger.LogInformation("USER: {Email} (ID: {UserId}) - ACTION: Opening Portone", user.Email, user.Id);
        Console.WriteLine($"🚪 APRI PORTONE - User: {user.Email} - Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        Message = "Portone aperto con successo!";
        return RedirectToPage();
    }
}

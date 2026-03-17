using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using GuestAccess.Models;
using Microsoft.AspNetCore.Authorization;
using GuestAccess.Services;

namespace GuestAccess.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditLogService _auditLogService;
    private readonly IGateService _gateService;
    private readonly HomeAssistantSettings _haSettings;

    public bool CanOpenCancello { get; set; }
    public bool CanOpenCancelletto { get; set; }
    public bool CanOpenPortone { get; set; }
    public bool HasAnyPermission => CanOpenCancello || CanOpenCancelletto || CanOpenPortone;
    public string UserEmail { get; set; } = string.Empty;

    [TempData]
    public string? Message { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public IndexModel(
        ILogger<IndexModel> logger,
        UserManager<ApplicationUser> userManager,
        IAuditLogService auditLogService,
        IGateService gateService,
        HomeAssistantSettings haSettings)
    {
        _logger = logger;
        _userManager = userManager;
        _auditLogService = auditLogService;
        _gateService = gateService;
        _haSettings = haSettings;
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

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var email = user.Email ?? "Unknown";

        var success = await _gateService.TriggerAsync(_haSettings.Cancello, email);

        await _auditLogService.LogActionAsync(user.Id, email, "Apri Cancello",
            success ? "Main gate opened via webhook" : "Main gate webhook failed", ipAddress);

        Message = success ? "Cancello aperto!" : null;
        ErrorMessage = success ? null : "Errore: impossibile aprire il cancello. Riprovare.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostOpenCancellettoAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !user.CanOpenCancelletto)
        {
            _logger.LogWarning("Unauthorized access attempt to Cancelletto by user {UserId}", user?.Id ?? "Unknown");
            return Forbid();
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var email = user.Email ?? "Unknown";

        var success = await _gateService.TriggerAsync(_haSettings.Cancelletto, email);

        await _auditLogService.LogActionAsync(user.Id, email, "Apri Cancelletto",
            success ? "Small gate opened via webhook" : "Small gate webhook failed", ipAddress);

        Message = success ? "Cancelletto aperto!" : null;
        ErrorMessage = success ? null : "Errore: impossibile aprire il cancelletto. Riprovare.";
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

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var email = user.Email ?? "Unknown";

        var success = await _gateService.TriggerAsync(_haSettings.Portone, email);

        await _auditLogService.LogActionAsync(user.Id, email, "Apri Portone",
            success ? "Main door opened via webhook" : "Main door webhook failed", ipAddress);

        Message = success ? "Portone aperto!" : null;
        ErrorMessage = success ? null : "Errore: impossibile aprire il portone. Riprovare.";
        return RedirectToPage();
    }
}

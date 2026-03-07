using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuestAccess.Models;
using GuestAccess.Services;

namespace GuestAccess.Pages;

[Authorize]
public class LogsModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditLogService _auditLogService;

    public List<AuditLog> Logs { get; set; } = new();

    public LogsModel(UserManager<ApplicationUser> userManager, IAuditLogService auditLogService)
    {
        _userManager = userManager;
        _auditLogService = auditLogService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        Logs = await _auditLogService.GetUserLogsAsync(user.Id);

        return Page();
    }
}

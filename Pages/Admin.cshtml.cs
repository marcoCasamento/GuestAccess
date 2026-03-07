using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GuestAccess.Models;

namespace GuestAccess.Pages;

[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminModel> _logger;

    public List<UserPermissionsViewModel> Users { get; set; } = new();
    public List<string> AdminUserIds { get; set; } = new();

    [TempData]
    public string? Message { get; set; }

    public AdminModel(UserManager<ApplicationUser> userManager, ILogger<AdminModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        
        foreach (var user in users)
        {
            Users.Add(new UserPermissionsViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "Unknown",
                CanOpenCancello = user.CanOpenCancello,
                CanOpenCancelletto = user.CanOpenCancelletto,
                CanOpenPortone = user.CanOpenPortone
            });

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                AdminUserIds.Add(user.Id);
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostUpdatePermissionsAsync(
        string userId,
        bool canOpenCancello = false,
        bool canOpenCancelletto = false,
        bool canOpenPortone = false)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.CanOpenCancello = canOpenCancello;
        user.CanOpenCancelletto = canOpenCancelletto;
        user.CanOpenPortone = canOpenPortone;

        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("Admin updated permissions for user {Email}. Cancello: {Cancello}, Cancelletto: {Cancelletto}, Portone: {Portone}",
                user.Email, canOpenCancello, canOpenCancelletto, canOpenPortone);
            
            Message = $"Permessi aggiornati per {user.Email}";
        }
        else
        {
            _logger.LogError("Failed to update permissions for user {Email}", user.Email);
            Message = "Errore nell'aggiornamento dei permessi";
        }

        return RedirectToPage();
    }
}

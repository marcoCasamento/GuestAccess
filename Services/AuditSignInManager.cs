using GuestAccess.Data;
using GuestAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GuestAccess.Services;

public class AuditSignInManager : SignInManager<ApplicationUser>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSignInManager(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation,
        ApplicationDbContext context)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
        _context = context;
        _httpContextAccessor = contextAccessor;
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        
        if (result.Succeeded)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                await LogLoginAsync(user);
            }
        }
        
        return result;
    }

    public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
    {
        var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
        
        if (result.Succeeded)
        {
            var info = await GetExternalLoginInfoAsync();
            if (info != null)
            {
                var user = await UserManager.FindByLoginAsync(loginProvider, providerKey);
                if (user != null)
                {
                    await LogLoginAsync(user, $"via {loginProvider}");
                }
            }
        }
        
        return result;
    }

    public override async Task SignOutAsync()
    {
        if (_httpContextAccessor.HttpContext?.User != null)
        {
            var user = await UserManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                await LogLogoutAsync(user);
            }
        }
        
        await base.SignOutAsync();
    }

    private async Task LogLoginAsync(ApplicationUser user, string? details = null)
    {
        try
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var log = new AuditLog
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "Unknown",
                Action = "Login",
                Details = details ?? "User logged in",
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to log login for user {UserId}", user.Id);
        }
    }

    private async Task LogLogoutAsync(ApplicationUser user)
    {
        try
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var log = new AuditLog
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "Unknown",
                Action = "Logout",
                Details = "User logged out",
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to log logout for user {UserId}", user.Id);
        }
    }
}

using GuestAccess.Data;
using GuestAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestAccess.Services;

public interface IAuditLogService
{
    Task LogActionAsync(string userId, string userEmail, string action, string? details = null, string ipAddress = "");
    Task<List<AuditLog>> GetUserLogsAsync(string userId, int daysToKeep = 30);
    Task CleanupOldLogsAsync(int daysToKeep = 30);
}

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(ApplicationDbContext context, ILogger<AuditLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogActionAsync(string userId, string userEmail, string action, string? details = null, string ipAddress = "")
    {
        try
        {
            var log = new AuditLog
            {
                UserId = userId,
                UserEmail = userEmail,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Audit: {Email} - {Action} - {Details}", userEmail, action, details ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log for user {UserId}", userId);
        }
    }

    public async Task<List<AuditLog>> GetUserLogsAsync(string userId, int daysToKeep = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        
        return await _context.AuditLogs
            .Where(log => log.UserId == userId && log.Timestamp >= cutoffDate)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task CleanupOldLogsAsync(int daysToKeep = 30)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var oldLogs = await _context.AuditLogs
                .Where(log => log.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} old audit logs", oldLogs.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old audit logs");
        }
    }
}

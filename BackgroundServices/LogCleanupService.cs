using GuestAccess.Services;

namespace GuestAccess.BackgroundServices;

public class LogCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LogCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily

    public LogCleanupService(IServiceProvider serviceProvider, ILogger<LogCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Log Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
                    await auditLogService.CleanupOldLogsAsync(30);
                }

                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during log cleanup");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Log Cleanup Service stopped");
    }
}

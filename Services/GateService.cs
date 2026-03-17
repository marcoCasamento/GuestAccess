namespace GuestAccess.Services;

public class GateCommandSettings
{
    public string WebhookPath { get; set; } = string.Empty;
    public string SecretToken { get; set; } = string.Empty;
}

public class HomeAssistantSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public GateCommandSettings Cancello { get; set; } = new();
    public GateCommandSettings Cancelletto { get; set; } = new();
    public GateCommandSettings Portone { get; set; } = new();
}

public interface IGateService
{
    Task<bool> TriggerAsync(GateCommandSettings command, string userEmail);
}

public class GateService : IGateService
{
    private readonly HomeAssistantSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GateService> _logger;

    public GateService(HomeAssistantSettings settings, IHttpClientFactory httpClientFactory, ILogger<GateService> logger)
    {
        _settings = settings;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> TriggerAsync(GateCommandSettings command, string userEmail)
    {
        if (string.IsNullOrWhiteSpace(_settings.BaseUrl) ||
            string.IsNullOrWhiteSpace(command.WebhookPath) ||
            string.IsNullOrWhiteSpace(command.SecretToken))
        {
            _logger.LogWarning(
                "HomeAssistant is not configured. Webhook call skipped for user {User}. " +
                "Configure HomeAssistant:BaseUrl and per-command WebhookPath/SecretToken.", userEmail);
            return false;
        }

        var url = $"{_settings.BaseUrl.TrimEnd('/')}/api/webhook/{command.WebhookPath}";

        var payload = new
        {
            secret_token = command.SecretToken,
            user = userEmail
        };

        try
        {
            var client = _httpClientFactory.CreateClient("HomeAssistant");
            var response = await client.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Webhook triggered successfully: {Url} for user {User}", url, userEmail);
                return true;
            }

            _logger.LogWarning(
                "Webhook returned non-success status {Status} for {Url} (user: {User})",
                (int)response.StatusCode, url, userEmail);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call webhook {Url} for user {User}", url, userEmail);
            return false;
        }
    }
}

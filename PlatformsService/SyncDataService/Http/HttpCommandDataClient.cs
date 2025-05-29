using System.Text;
using System.Text.Json;
using PlatformsService.Dtos;

namespace PlatformsService.SyncDataService.Http;

public class HttpCommandsDataClient(HttpClient httpClient, IConfiguration configuration, ILogger<ICommandsDataClient> logger) : ICommandsDataClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<ICommandsDataClient> _logger = logger;

    public async Task SendPlatformToCommands(PlatformReadDto platform)
    {
        var commandServiceUrl = _configuration["CommandsService"];

        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(commandServiceUrl, httpContent);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Sync Post to Command Service was OK");
        }
        else
        {
            _logger.LogError("Sync Post to Command Service was NOT Ok");
        }
    }
}
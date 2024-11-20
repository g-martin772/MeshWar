using App.Schema;

namespace App.Services;

public class WarMachine(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<WarMachine> logger,
    WarStateProvider warStateProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var targets = configuration.GetSection("Targets").Get<string[]>()!;

        var tasks = targets.Select(target => Task.Run(async () => await AttackTask(target), stoppingToken)).ToList();

        await Task.WhenAll(tasks);
        return;

        async Task AttackTask(string target)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(target);
            while (true)
            {
                try
                {
                    var response = await client.GetAsync("status", stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Status of {Target}: {Status}", target,
                            await response.Content.ReadAsStringAsync(stoppingToken));
                    }
                    else
                    {
                        logger.LogWarning("Failed to get status of {Target}: {StatusCode}", target,
                            response.StatusCode);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Failed to get status of {Target}", target);
                    continue;
                }


                if (warStateProvider.Data.State != WarState.Running)
                    continue;
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
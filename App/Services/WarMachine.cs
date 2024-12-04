using App.Constants;
using App.Schema;

namespace App.Services;

public class WarMachine(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<WarMachine> logger,
    WarStateProvider warStateProvider,
    WarConfig config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var targets = configuration.GetSection("Targets").Get<string[]>()!;

        var tasks = new List<Task>();

        for (var i = 0; i < 1; i++)
            tasks.AddRange(targets
                .Select(target => Task.Run(async () => await AttackTask(target), stoppingToken)).ToList());

        await Task.WhenAll(tasks);
        return;

        async Task AttackTask(string target)
        {
            var client = httpClientFactory.CreateClient(HttpClients.HackingClient);
            client.BaseAddress = new Uri(target);
            while (true)
            {
                await Task.Delay(1000, stoppingToken);
                StatusModel status;

                if (warStateProvider.Status.State != WarState.Running)
                    continue;

                try
                {
                    var response = await client.GetAsync("status", stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Status of {Target}: {Status}", target,
                            await response.Content.ReadAsStringAsync(stoppingToken));
                        status = (await response.Content.ReadFromJsonAsync<StatusModel>(stoppingToken))!;
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

                if (warStateProvider.Status.Attack < status.Defense)
                    continue;

                var attack = warStateProvider.Status.Attack;
                attack *= 0.9 + (Random.Shared.NextDouble() * 0.2);

                var postModel = new HackingAttemptPostModel
                {
                    Attack = attack
                };

                try
                {
                    var response = await client.PostAsJsonAsync("hacking-attempt", postModel, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<HackingResultModel>(stoppingToken);

                        warStateProvider.AttackCount++;

                        switch (result!.HackingResult)
                        {
                            case HackingResult.Hacked:
                                warStateProvider.Status.Points += config.PointsGainedForSuccessfulHack;
                                warStateProvider.Status.Attack += config.AttackValueGainedForSuccessfulHack;
                                warStateProvider.AttackSuccessCount++;
                                warStateProvider.AttackStreak++;

                                if (warStateProvider.AttackStreak > config.NumberOfSuccessfulHacksForExtraDefense)
                                {
                                    warStateProvider.Status.Defense += config.NumberOfDefensePointsGainedForExtraDefense;
                                    warStateProvider.AttackStreak = 0;
                                }

                                logger.LogInformation("Hacking attempt on {Target} was successful", target);
                                break;
                            case HackingResult.Defended:
                                warStateProvider.Status.Points -= config.PointsLostForUnsuccessfulHack;
                                warStateProvider.Status.Attack -= config.AttackValueLostForUnsuccessfulHack;
                                if (warStateProvider.Status.Points < 0)
                                {
                                    warStateProvider.Status.State = WarState.Stopped;
                                    warStateProvider.Status.Points = 0;
                                }
                                logger.LogInformation("Hacking attempt on {Target} was defended", target);
                                break;
                            default:
                                logger.LogWarning("Unknown hacking result: {Result}", result.HackingResult);
                                break;
                        }

                        warStateProvider.StateHasChanged?.Invoke();
                    }
                    else
                    {
                        logger.LogWarning("Hacking attempt on {Target} failed: {StatusCode}", target,
                            response.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Hacking attempt on {Target} crashed", target);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
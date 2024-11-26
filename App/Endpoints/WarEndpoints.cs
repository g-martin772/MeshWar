using App.Schema;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class WarEndpoints
{
    public static void MapWarEndpoints(this WebApplication app)
    {
        app.MapGet("/status", async (WarStateProvider warStateProvider) =>
        {
            //await Task.Delay(2000);
            return Results.Json(warStateProvider.Status);
        });

        app.MapPost("/hacking-attempt", (
            [FromServices] WarStateProvider warStateProvider,
            [FromBody] HackingAttemptPostModel model,
            [FromServices] ILogger<WarMachine> logger,
            [FromHeader] string attacker,
            [FromServices] WarConfig config) =>
        {
            if (warStateProvider.Status.State != WarState.Running)
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

            warStateProvider.DefenseCount++;

            if (!(model.Attack > warStateProvider.Status.Defense))
            {
                warStateProvider.DefenseSuccessCount++;
                warStateProvider.Status.Defense += config.DefenseValueGainedForSuccessfulDefense;
                warStateProvider.Status.Points += config.PointsGainedForSuccessfulDefense;
                logger.LogInformation("Hacking attempt from {Attacker} was defended", attacker);
                return Results.Json(HackingResultModel.Defended);
            }

            warStateProvider.Status.Points -= config.PointsLostForUnsuccessfulDefense;
            warStateProvider.Status.Defense -= config.DefenseValueLostForUnsuccessfulDefense;

            if (warStateProvider.Status.Points < 0)
            {
                warStateProvider.Status.State = WarState.Stopped;
                warStateProvider.Status.Points = 0;
            }

            logger.LogInformation("Hacking attempt from {Attacker} was successful", attacker);
            
            warStateProvider.Status.State = WarState.Disabled;
            Task.Run(async () => await Task.Delay(config.DisabledStateDurationSeconds * 1000))
                .ContinueWith(_ => warStateProvider.Status.State = WarState.Running);

            warStateProvider.StateHasChanged?.Invoke();

            return Results.Json(HackingResultModel.Hacked);
        });
    }
}
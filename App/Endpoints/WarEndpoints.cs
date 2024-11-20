using App.Schema;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Endpoints;

public static class WarEndpoints
{
    public static void MapWarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/status", (WarStateProvider warStateProvider) => Results.Json(warStateProvider.Data));

        endpoints.MapPost("/hacking-attempt", (
            [FromServices] WarStateProvider warStateProvider,
            [FromBody] HackingAttemptPostModel model,
            [FromServices] ILogger logger,
            [FromHeader] string attacker) =>
        {
            if (warStateProvider.Data.State != WarState.Running)
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

            if (!(model.Attack > warStateProvider.Data.Defense))
                return Results.Json(new HackingResultModel()
                {
                    HackingResult = HackingResult.Defended
                });

            logger.LogInformation("Hacking attempt from {Attacker} was successful", attacker);

            return Results.Json(new HackingResultModel()
            {
                HackingResult = HackingResult.Hacked
            });

        });
    }
}
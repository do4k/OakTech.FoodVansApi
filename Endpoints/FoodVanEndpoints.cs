using Microsoft.AspNetCore.Mvc;
using OakTech.FoodVansApi.Services;

namespace OakTech.FoodVansApi.Endpoints;

public static class FoodVanEndpoints
{
    public static IEndpointRouteBuilder MapFoodVanEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async ([FromServices] ITradersCache tradersCache) =>
        {
            var traders = await tradersCache.GetTradersAsync(DateTime.Now);
            var dto = traders.Match(t => t.Links.Select(x => new { x.Link, x.Name }).ToArray(), _ => []);
            return dto;
        });

        return app;
    }
}
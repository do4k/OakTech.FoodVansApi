using Microsoft.AspNetCore.Mvc;
using OakTech.FoodVansApi.Endpoints;
using OakTech.FoodVansLib.Options;
using OakTech.FoodVansLib.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<ITradersCache, TradersCache>()
    .AddSingleton<ITradersService, TradersService>()
    .AddOpenApi();

builder.Services.AddOptions<FoodVanOptions>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFoodVanEndpoints();

app.Run();
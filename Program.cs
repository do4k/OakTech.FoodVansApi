using Microsoft.AspNetCore.Mvc;
using OakTech.FoodVansApi.Endpoints;
using OakTech.FoodVansApi.Options;
using OakTech.FoodVansApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddSingleton<ITradersCache, TradersCache>()
    .AddSingleton<ITradersService, TradersService>()
    .AddOpenApi();

builder.Services.AddOptions<FoodVanOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFoodVanEndpoints();

app.Run();
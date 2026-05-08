using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OakTech.FoodVansLib.Services;
using OakTech.FoodVansSlackNotifier;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Debug));
builder.Services.AddSingleton(new SlackWebhookUrl(builder.Configuration["SlackWebhookPath"] ?? ""));

builder.Services.AddSingleton<ITradersCache, TradersCache>();
builder.Services.AddSingleton<ITradersService, TradersService>();

builder.Services.AddScheduler();
builder.Services.AddTransient<FoodVanSlackNotifierJob>();

var host = builder.Build();
host.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<FoodVanSlackNotifierJob>()
        .DailyAt(8, 0)
        .Weekday();
});

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("FoodVanSlackNotifier scheduled to run daily at 8:00 AM on weekdays");

host.Run();
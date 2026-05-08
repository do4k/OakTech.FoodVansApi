using Coravel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OakTech.FoodVansLib.Services;
using OakTech.FoodVansSlackNotifier;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(new SlackWebhookUrl(builder.Configuration["SlackWebhookPath"] ?? ""));

builder.Services.AddSingleton<ITradersCache, TradersCache>();
builder.Services.AddSingleton<ITradersService, TradersService>();

builder.Services.AddScheduler();
builder.Services.AddTransient<FoodVanSlackNotifierJob>();

var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Coravel.Scheduling.Schedule.Interfaces.IScheduler>>();
host.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<FoodVanSlackNotifierJob>()
        .DailyAt(8, 0)
        .Weekday();
})
.LogScheduledTaskProgress(logger);

host.Run();
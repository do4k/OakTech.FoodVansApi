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
var nextRun = GetNextWeekdayMorningRun();
logger.LogInformation("FoodVanSlackNotifier scheduled to run daily at 8:00 AM on weekdays. Next run: {NextRun}", nextRun);

host.Run();

static DateTime GetNextWeekdayMorningRun()
{
    var now = DateTime.Now;
    var today = now.Date.AddHours(8);
    
    if (today > now && today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday)
        return today;
    
    for (int i = 1; i <= 7; i++)
    {
        var day = now.Date.AddDays(i);
        if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
            return day.AddHours(8);
    }
    
    return today;
}
using System.Reflection;
using System.Text.Json;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OakTech.FoodVansLib.Services;
using OakTech.FoodVansSlackNotifier;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Debug));
builder.Services.AddSingleton(new SlackWebhookUrl(builder.Configuration["SlackWebhookPath"] ?? ""));

builder.Services.AddSingleton<ITradersCache, TradersCache>();
builder.Services.AddSingleton<ITradersService, TradersService>();

builder.Services.AddScheduler();
builder.Services.AddTransient<FoodVanSlackNotifierJob>();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<FoodVanSlackNotifierJob>()
        .DailyAt(8, 0)
        .Weekday();
});

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var scheduler = app.Services.GetRequiredService<IScheduler>();
LogAllScheduledTasks(scheduler, logger);

app.MapPost("/notify", async (ILogger<Program> log) =>
{
    log.LogInformation("Manual trigger of food van notification");
    var job = app.Services.GetRequiredService<FoodVanSlackNotifierJob>();
    await job.Invoke();
    return Results.Ok("Notification job completed");
});

app.Run();

void LogAllScheduledTasks(IScheduler scheduler, ILogger logger)
{
    var schedulerType = scheduler.GetType();
    var tasksField = schedulerType.GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);
    
    if (tasksField == null)
    {
        logger.LogWarning("Could not find _tasks field on scheduler");
        return;
    }

    var tasks = tasksField.GetValue(scheduler) as System.Collections.Concurrent.ConcurrentDictionary<string, object>;
    if (tasks == null)
    {
        logger.LogWarning("Could not get tasks from scheduler");
        return;
    }

    foreach (var kvp in tasks)
    {
        logger.LogInformation("Scheduled task: {Name}", kvp.Key);
        
        var scheduledTaskType = kvp.Value.GetType();
        var scheduledEventField = scheduledTaskType.GetField("ScheduledEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        var scheduledEvent = scheduledEventField?.GetValue(kvp.Value);
        
        if (scheduledEvent != null)
        {
            var dueAtProperty = scheduledEvent.GetType().GetProperty("DueAt");
            var dueAt = dueAtProperty?.GetValue(scheduledEvent);
            logger.LogInformation("  DueAt: {DueAt}", dueAt);
        }
    }
}
using System.Net.Http.Json;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using OakTech.FoodVansLib.Services;
using OakTech.FoodVansLib.Models;

namespace OakTech.FoodVansSlackNotifier;

public record SlackMessage(string Text);
public record SlackWebhookUrl(string Value);

public class FoodVanSlackNotifierJob : IInvocable
{
    private readonly ITradersCache _tradersCache;
    private readonly SlackWebhookUrl _slackWebhookUrl;
    private readonly ILogger<FoodVanSlackNotifierJob> _logger;

    public FoodVanSlackNotifierJob(
        ITradersCache tradersCache,
        SlackWebhookUrl slackWebhookUrl,
        ILogger<FoodVanSlackNotifierJob> logger)
    {
        _tradersCache = tradersCache;
        _slackWebhookUrl = slackWebhookUrl;
        _logger = logger;
    }

    public async Task Invoke()
    {
        if (string.IsNullOrWhiteSpace(_slackWebhookUrl.Value))
        {
            _logger.LogWarning("No Slack webhook path configured");
            return;
        }

        _logger.LogInformation("Starting food van notification job");

        var traders = await _tradersCache.GetTradersAsync(DateTime.Now);
        var foodVans = traders.Match(t => t.Links.ToList(), _ => new List<TraderUrl>());

        _logger.LogInformation("Retrieved {Count} food vans", foodVans.Count);

        if (foodVans.Count == 0)
        {
            _logger.LogInformation("No food vans for today");
            return;
        }

        var message = $"There are {foodVans.Count} food vans to pick from today!\n" +
            string.Join("\n", foodVans.Select((x, i) =>
                $"{i + 1}. {x.Name} - {x.Link.Replace("https://www.", "")}"));

        _logger.LogInformation("Sending Slack message: {Message}", message);

        using var client = new HttpClient();
        var content = JsonContent.Create(new SlackMessage(message));
        var result = await client.PostAsync($"https://hooks.slack.com/triggers/{_slackWebhookUrl.Value}", content);

        if (result.IsSuccessStatusCode)
        {
            _logger.LogInformation("Slack message sent successfully");
        }
        else
        {
            _logger.LogError("Failed to send Slack message. Status: {StatusCode}", result.StatusCode);
        }
    }
}
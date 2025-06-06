using System.Net;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using OakTech.FoodVansApi.Extensions;
using OakTech.FoodVansApi.Models;
using OakTech.FoodVansApi.Options;
using OneOf.Types;

namespace OakTech.FoodVansApi.Services;

public interface ITradersService
{
    public Task<OneOf.OneOf<Traders, NotFound>> GetTradersAsync(DateTime date);
}

public class TradersService(ILogger<TradersService> logger, IOptions<FoodVanOptions> options) : ITradersService
{
    public async Task<OneOf.OneOf<Traders, NotFound>> GetTradersAsync(DateTime date)
    {
        var formatted = date.ToFoodVanDateString();
        
        var html = new HtmlWeb();
        var doc = await html.LoadFromWebAsync(options.Value.MarketUrl);

        var todaysTradersDiv = doc.DocumentNode.SelectNodes("//div[@data-date='" + formatted + "']");
        if (todaysTradersDiv is null)
        {
            logger.LogInformation("No traders found for date: {Date}. Raw Html: {Html}", formatted, doc.DocumentNode.OuterHtml);
            return new NotFound();
        }
    
        var vendors = todaysTradersDiv
            .SelectMany(node => node.SelectNodes(".//a[contains(@class, 'grid-link')]") ?? Enumerable.Empty<HtmlNode>())
            .ToList();
    
        var links = vendors.Select(x =>
        {
            var title = WebUtility.HtmlDecode(x.SelectSingleNode(".//h4[@class='link']")?.InnerText.Trim());
            if (title is null)
            {
                logger.LogError("Failed to get h4 element containing trader name. Raw Html: {Html}", x.OuterHtml);
                return null;
            }
            
            var url = x.Attributes["href"].Value;
            return new TraderUrl(title, url);
        })
        .Where(x => x is not null)
        .Select(x => x!)
        .Distinct()
        .ToArray();

        return new Traders(links, formatted);
    }
}
using System.Net;
using HtmlAgilityPack;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    var today = DateTime.Today;
    var formatted = $"{today:dddd} {today:dd}{(today.Day % 10 == 1 && today.Day != 11 ? "st" : today.Day % 10 == 2 && today.Day != 12 ? "nd" : today.Day % 10 == 3 && today.Day != 13 ? "rd" : "th")} {today:MMMM} {today:yyyy}";

    var html = new HtmlWeb();
    var doc = html.Load("https://www.buoyevents.co.uk/markets/harbourside-street-food-market/");

    var todaysTradersDiv = doc.DocumentNode.SelectNodes("//div[@data-date='" + formatted + "']");
    if (todaysTradersDiv is null)
    {
        return [];
    }
    
    var vendors = todaysTradersDiv
        .SelectMany(node => node.SelectNodes(".//a[contains(@class, 'grid-link')]") ?? Enumerable.Empty<HtmlNode>())
        .ToList();
    
    var vendorsDto = vendors.Select(x =>
        new 
        {
            Link = x.Attributes["href"].Value,
            Name = WebUtility.HtmlDecode(x.SelectSingleNode(".//h4[@class='link']")?.InnerText.Trim())
        }
    );

    return vendorsDto.Distinct();
});

app.Run();
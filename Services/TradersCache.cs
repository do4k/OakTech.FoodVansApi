using OakTech.FoodVansApi.Extensions;
using OakTech.FoodVansApi.Models;
using OneOf.Types;

namespace OakTech.FoodVansApi.Services;

public interface ITradersCache
{
    public Task<OneOf.OneOf<Traders, NotFound>> GetTradersAsync(DateTime date);
}

public class TradersCache(ITradersService tradersService, ILogger<TradersCache> logger) : ITradersCache
{
    private Traders? _traders;
    
    public async Task<OneOf.OneOf<Traders, NotFound>> GetTradersAsync(DateTime date)
    {
        var key = date.ToFoodVanDateString();
        
        if (_traders is null || _traders.Date != key)
        {
            logger.LogDebug("Cache miss for key: {Key}", key);
            var fromService = await tradersService.GetTradersAsync(date);
            _traders = fromService.Match(traders => traders, _ => new Traders([], key));
        }
        else
        {
            logger.LogDebug("Cache hit for key: {Key}", key);
        }

        return _traders switch
        {
            null => new NotFound(),
            var traders => traders,
        };
    }
}
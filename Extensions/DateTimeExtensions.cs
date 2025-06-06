namespace OakTech.FoodVansApi.Extensions;

public static class DateTimeExtensions
{
    public static string ToFoodVanDateString(this DateTime dateTime)
    {
        var formatted = $"{dateTime:dddd} {dateTime.Day}{(dateTime.Day % 10 == 1 && dateTime.Day != 11 ? "st" : dateTime.Day % 10 == 2 && dateTime.Day != 12 ? "nd" : dateTime.Day % 10 == 3 && dateTime.Day != 13 ? "rd" : "th")} {dateTime:MMMM} {dateTime:yyyy}";
        return formatted;
    }
}
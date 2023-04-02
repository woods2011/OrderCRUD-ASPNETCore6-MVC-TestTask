namespace MvcAppTest.Infrastructure.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime RoundToSeconds(this DateTime dateTime) => new(
        dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
    
    public static DateTime RoundToDay(this DateTime dateTime) => new(
        dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
}
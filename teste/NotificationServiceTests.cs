using FGC_Notification.Services;
using Microsoft.Extensions.Logging;

namespace FGC_Notification.Tests;

public class NotificationServiceTests
{
    [Fact]
    public async Task SendWelcomeEmailAsync_LogsRecipientAndUserName()
    {
        var logger = new CapturingLogger<NotificationService>();
        var service = new NotificationService(logger);

        await service.SendWelcomeEmailAsync("ana@example.com", 42, "Ana");

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Equal("ana@example.com", log.Values["Email"]);
        Assert.Equal("Ana", log.Values["User"]);
    }

    [Fact]
    public async Task SendPurchaseConfirmationAsync_LogsPurchaseDetails()
    {
        var logger = new CapturingLogger<NotificationService>();
        var service = new NotificationService(logger);

        await service.SendPurchaseConfirmationAsync(42, 7, 19.95m);

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Equal(42, log.Values["UserId"]);
        Assert.Equal(7, log.Values["GameId"]);
        Assert.Equal(19.95m, log.Values["Price"]);
    }
}

internal sealed class CapturingLogger<T> : ILogger<T>
{
    public List<CapturedLog> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var values = state is IEnumerable<KeyValuePair<string, object?>> properties
            ? properties.ToDictionary(property => property.Key, property => property.Value)
            : new Dictionary<string, object?>();

        Entries.Add(new CapturedLog(logLevel, formatter(state, exception), values));
    }

    internal sealed record CapturedLog(
        LogLevel Level,
        string Message,
        IReadOnlyDictionary<string, object?> Values);
}

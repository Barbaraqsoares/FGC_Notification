namespace FCG_Notification.Contracts;

public sealed record PaymentProcessedEvent(Guid OrderId, int UserId, int GameId, decimal Price, string Status, DateTime ProcessedAtUtc, string? Reason)
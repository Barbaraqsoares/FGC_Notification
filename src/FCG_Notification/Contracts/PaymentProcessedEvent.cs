using MassTransit;

namespace FCG_Notification.Contracts;

[EntityName("FCG_PaymentsAPI:PaymentProcessedEvent")]
public sealed record PaymentProcessedEvent(Guid OrderId, int UserId, int GameId, decimal Price, string Status, DateTime ProcessedAtUtc, string? Reason)
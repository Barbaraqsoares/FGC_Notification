using MassTransit;

namespace FCG_Notification.Contracts;

[EntityName("FCG_PaymentsAPI:PaymentProcessedEvent")]
[MessageUrn("urn:message:Fcg.Contracts:PaymentProcessedEvent", useDefaultPrefix: false)]
public sealed record PaymentProcessedEvent
{
    public Guid OrderId { get; init; }
    public int UserId { get; init; }
    public int GameId { get; init; }
    public decimal Price { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime ProcessedAtUtc { get; init; }
    public string? Reason { get; init; }
}
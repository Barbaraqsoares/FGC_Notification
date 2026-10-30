namespace FGC_Notification.Contracts;

public sealed record PaymentProcessedEvent
{
    public int UserId { get; init; }
    public int GameId { get; init; }
    public decimal Price { get; init; }
    public string Status { get; init; } = string.Empty;
}
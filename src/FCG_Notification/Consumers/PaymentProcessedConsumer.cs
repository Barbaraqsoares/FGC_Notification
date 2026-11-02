using FCG_Notification.Contracts;
using FCG_Notification.Services.Interfaces;
using MassTransit;

namespace FCG_Notification.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly INotificationService _notificationService;

    public PaymentProcessedConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var message = context.Message;

        if (message.Status != "Approved")
            return;

        await _notificationService.SendPurchaseConfirmationAsync(message.UserId, message.GameId, message.Price);
    }
}
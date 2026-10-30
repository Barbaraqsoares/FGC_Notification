using FGC_Notification.Contracts;
using FGC_Notification.Services.Interfaces;
using MassTransit;
using System;

namespace FGC_Notification.Consumers;

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

        await _notificationService
            .SendPurchaseConfirmationAsync(message.UserId, message.GameId, message.Price);
    }
}
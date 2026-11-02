using FCG_Notification.Contracts;
using FCG_Notification.Services.Interfaces;
using MassTransit;

namespace FCG_Notification.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly INotificationService _notificationService;

    public UserCreatedConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var message = context.Message;

        await _notificationService
            .SendWelcomeEmailAsync(message.Email, message.UserId, message.Name);
    }
}
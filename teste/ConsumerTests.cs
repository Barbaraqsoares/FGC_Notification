using FGC_Notification.Consumers;
using FGC_Notification.Contracts;
using FGC_Notification.Services.Interfaces;
using MassTransit;
using Moq;

namespace FGC_Notification.Tests;

public class ConsumerTests
{
    [Fact]
    public async Task UserCreatedConsumer_SendsWelcomeEmail()
    {
        var notifications = new Mock<INotificationService>();
        var context = new Mock<ConsumeContext<UserCreatedEvent>>();
        context.SetupGet(value => value.Message).Returns(new UserCreatedEvent
        {
            UserId = 42,
            Email = "ana@example.com",
            Name = "Ana"
        });

        await new UserCreatedConsumer(notifications.Object).Consume(context.Object);

        notifications.Verify(
            service => service.SendWelcomeEmailAsync("ana@example.com", 42, "Ana"),
            Times.Once);
    }

    [Fact]
    public async Task PaymentProcessedConsumer_SendsConfirmationForApprovedPayment()
    {
        var notifications = new Mock<INotificationService>();
        var context = CreatePaymentContext("Approved");

        await new PaymentProcessedConsumer(notifications.Object).Consume(context.Object);

        notifications.Verify(
            service => service.SendPurchaseConfirmationAsync(42, 7, 19.95m),
            Times.Once);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Rejected")]
    [InlineData("approved")]
    [InlineData("")]
    public async Task PaymentProcessedConsumer_DoesNotSendConfirmationForOtherStatuses(string status)
    {
        var notifications = new Mock<INotificationService>();
        var context = CreatePaymentContext(status);

        await new PaymentProcessedConsumer(notifications.Object).Consume(context.Object);

        notifications.Verify(
            service => service.SendPurchaseConfirmationAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<decimal>()),
            Times.Never);
    }

    private static Mock<ConsumeContext<PaymentProcessedEvent>> CreatePaymentContext(string status)
    {
        var context = new Mock<ConsumeContext<PaymentProcessedEvent>>();
        context.SetupGet(value => value.Message).Returns(new PaymentProcessedEvent
        {
            UserId = 42,
            GameId = 7,
            Price = 19.95m,
            Status = status
        });
        return context;
    }
}

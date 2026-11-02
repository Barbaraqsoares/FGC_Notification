namespace FCG_Notification.Services.Interfaces;

public interface INotificationService
{
    Task SendWelcomeEmailAsync(string email, int UserId, string name);

    Task SendPurchaseConfirmationAsync(int UserId, int GameId, decimal Price);
}

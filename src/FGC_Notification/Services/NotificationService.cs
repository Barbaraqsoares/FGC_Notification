using FGC_Notification.Services.Interfaces;

namespace FGC_Notification.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendWelcomeEmailAsync(string email,  int userId, string name)
    {
        _logger.LogInformation("""
        
        =====================================
        EMAIL DE BOAS VINDAS
        Destinatário: {Email}
        Usuário: {User}
        =====================================
        
        """, email, name);

        return Task.CompletedTask;
    }

    public Task SendPurchaseConfirmationAsync(int userId, int gameId, decimal price)
    {
        _logger.LogInformation("""
        
        =====================================
        COMPRA APROVADA
        UserId: {UserId}
        GameId: {GameId}
        Valor: {Price}
        =====================================
        
        """, userId, gameId, price);

        return Task.CompletedTask;
    }
}

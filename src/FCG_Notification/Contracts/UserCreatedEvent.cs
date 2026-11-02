using MassTransit;

namespace FCG_Notification.Contracts;

[EntityName("FCG_UserApi:user-created-event")]
[MessageUrn("urn:message:FCG.Contracts.Events:UserCreatedEvent", useDefaultPrefix: false)]
public sealed record UserCreatedEvent
{
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
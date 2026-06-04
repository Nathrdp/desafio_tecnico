namespace TesteVeste.Application.Notifications;

public interface INotificationService
{
    bool HasNotifications { get; }
    IReadOnlyList<string> Notifications { get; }
    void AddNotification(string message);
    void Clear();
}

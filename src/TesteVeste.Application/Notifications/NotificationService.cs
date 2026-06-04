namespace TesteVeste.Application.Notifications;

public class NotificationService : INotificationService
{
    private readonly List<string> _notifications = new();

    public bool HasNotifications => _notifications.Count > 0;
    public IReadOnlyList<string> Notifications => _notifications.AsReadOnly();

    public void AddNotification(string message) => _notifications.Add(message);
    public void Clear() => _notifications.Clear();
}

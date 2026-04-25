namespace MediSync.Application.Services;

public interface INotificationService
{
    Task SendPushAsync(string fcmToken, string title, string body, Dictionary<string,string>? data = null);
    Task SendToUserAsync(Guid userId, string title, string body);
}
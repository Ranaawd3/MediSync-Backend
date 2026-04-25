using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MediSync.Application.Services;
using MediSync.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Infrastructure.Services;

public class FirebaseNotificationService(AppDbContext db) : INotificationService
{
    static FirebaseNotificationService()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
            });
        }
    }

    public async Task SendPushAsync(
        string fcmToken, string title, string body,
        Dictionary<string, string>? data = null)
    {
        var message = new Message
        {
            Token = fcmToken,
            Notification = new Notification { Title = title, Body = body },
            Data = data ?? new Dictionary<string, string>()
        };

        await FirebaseMessaging.DefaultInstance.SendAsync(message);
    }

    public async Task SendToUserAsync(Guid userId, string title, string body)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user?.PushToken != null)
            await SendPushAsync(user.PushToken, title, body);
    }
}
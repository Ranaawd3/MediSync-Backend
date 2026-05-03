using MediSync.Application.Services;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Infrastructure.BackgroundJobs;

public class CaregiverAlertJob(
    IApplicationDbContext  db,
    INotificationService   pushService,
    IEmailService          emailService)
{
    // بيشتغل كل 30 دقيقة — مسجّل في Program.cs
    public async Task CheckAndAlertCaregiversAsync()
    {
        var now   = TimeOnly.FromDateTime(DateTime.UtcNow);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var threshold = now.AddMinutes(-30); // جرعات فاتت أكتر من 30 دقيقة

        // جيب كل الجرعات المتأخرة اللي pending
        var lateReminders = await db.Reminders
            .Include(r => r.Medication)
            .Include(r => r.User)
            .Where(r =>
                r.ScheduledDate == today
             && r.Status        == "pending"
             && r.ScheduledTime <= threshold
             && !r.CaregiverAlerted)             // مبعتناش تنبيه قبل كده
            .ToListAsync();

        foreach (var reminder in lateReminders)
        {
            // جيب الـ Caregivers النشطين للمريض ده
            var links = await db.FamilyLinks
                .Include(f => f.Caregiver)
                .Where(f =>
                    f.PatientId     == reminder.UserId
                 && f.Status        == "active"
                 && f.ReceiveAlerts == true)
                .ToListAsync();

            foreach (var link in links)
            {
                // Firebase Push Notification للـ Caregiver
                if (link.Caregiver?.PushToken != null)
                {
                    await pushService.SendPushAsync(
                        link.Caregiver.PushToken,
                        $"⚠️ {reminder.User.FullName} لم يأخذ دواءه",
                        $"{reminder.Medication.BrandName} — كان وقته {reminder.ScheduledTime:HH:mm}",
                        new Dictionary<string, string>
                        {
                            ["type"]       = "caregiver_alert",
                            ["patientId"]  = reminder.UserId.ToString(),
                            ["reminderId"] = reminder.Id.ToString()
                        });
                }

                // Email تنبيه
                await emailService.SendMissedDoseAlertAsync(
                    link.CaregiverEmail,
                    reminder.User.FullName,
                    reminder.Medication.BrandName,
                    reminder.ScheduledTime.ToString("HH:mm"));
            }

            // علّم التذكير ده إنه اتبعت تنبيه عنه
            reminder.CaregiverAlerted = true;
        }

        await db.SaveChangesAsync();
    }
}
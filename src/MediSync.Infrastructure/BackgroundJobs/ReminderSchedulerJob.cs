using MediSync.Infrastructure.Persistence;
using MediSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Infrastructure.BackgroundJobs;

public class ReminderSchedulerJob(AppDbContext db)
{
    // بيتشغل كل يوم الساعة 12 AM عشان يولد تذكيرات اليوم
    public async Task GenerateTodayRemindersAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // جيب كل الأدوية الفعّالة
        var medications = await db.Medications
            .Where(m => m.IsActive
                     && m.StartDate <= today
                     && (m.EndDate == null || m.EndDate >= today))
            .ToListAsync();

        foreach (var med in medications)
        {
            // تأكد ما اتعملتش تذكيرات لليوم ده
            var exists = await db.Reminders
                .AnyAsync(r => r.MedicationId == med.Id && r.ScheduledDate == today);
            if (exists) continue;

            // ولّد تذكير لكل مرة في اليوم
            var baseTime  = new TimeOnly(8, 0); // 8 الصبح
            var interval  = 24 / med.TimesPerDay;

            for (int i = 0; i < med.TimesPerDay; i++)
            {
                db.Reminders.Add(new Reminder
                {
                    MedicationId  = med.Id,
                    UserId        = med.UserId,
                    ScheduledDate = today,
                    ScheduledTime = baseTime.Add(TimeSpan.FromHours(interval * i)),
                    Status        = "pending",
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
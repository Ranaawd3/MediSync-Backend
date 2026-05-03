using MailKit.Net.Smtp;
using MailKit.Security;
using MediSync.Application.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace MediSync.Infrastructure.Services;

public class EmailService(IConfiguration config) : IEmailService
{
    private string Host        => config["Smtp:Host"]!;
    private int    Port        => int.Parse(config["Smtp:Port"]!);
    private string Username    => config["Smtp:Username"]!;
    private string Password    => config["Smtp:Password"]!;
    private string FromEmail   => config["Smtp:FromEmail"]!;

    public async Task SendCaregiverInviteAsync(
        string caregiverEmail,
        string patientName,
        string acceptUrl)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(FromEmail));
        message.To.Add(MailboxAddress.Parse(caregiverEmail));
        message.Subject = $"💊 {patientName} دعاك لمتابعة أدويته في MediSync";

        message.Body = new TextPart("html")
        {
            Text = $"""
            <div dir="rtl" style="font-family: Arial; max-width: 500px; margin: auto;">
              <h2 style="color:#1a56a0;">💊 MediSync — دعوة متابعة الأدوية</h2>
              <p>مرحباً،</p>
              <p>قام <strong>{patientName}</strong> بدعوتك لمتابعة أدويته عبر تطبيق MediSync.</p>
              <p>بصفتك مقرباً، ستتمكن من:</p>
              <ul>
                <li>مراقبة جدول الجرعات اليومية</li>
                <li>استقبال تنبيهات لو فاته دواء</li>
                <li>متابعة نسبة الالتزام</li>
              </ul>
              <a href="{acceptUrl}" style="
                display:inline-block; background:#1a56a0; color:white;
                padding:12px 28px; border-radius:8px; text-decoration:none;
                font-weight:bold; margin: 16px 0;">
                ✅ قبول الدعوة
              </a>
              <p style="color:#718096; font-size:13px;">الدعوة صالحة لمدة 7 أيام.</p>
              <hr>
              <p style="color:#718096; font-size:12px;">MediSync 💊 — الصيدلي الذكي في جيبك</p>
            </div>
            """
        };

        await SendAsync(message);
    }

    public async Task SendMissedDoseAlertAsync(
        string caregiverEmail,
        string patientName,
        string medicationName,
        string scheduledTime)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(FromEmail));
        message.To.Add(MailboxAddress.Parse(caregiverEmail));
        message.Subject = $"⚠️ تنبيه: {patientName} لم يأخذ دواءه في الوقت المحدد";

        message.Body = new TextPart("html")
        {
            Text = $"""
            <div dir="rtl" style="font-family: Arial; max-width: 500px; margin: auto;">
              <h2 style="color:#c53030;">⚠️ تنبيه جرعة فائتة</h2>
              <p>مرحباً،</p>
              <p>لم يقم <strong>{patientName}</strong> بتأكيد تناول دوائه حتى الآن:</p>
              <div style="background:#fff8f8; border:2px solid #fc8181; border-radius:8px; padding:14px; margin:14px 0;">
                <strong>💊 الدواء:</strong> {medicationName}<br>
                <strong>⏰ الوقت المحدد:</strong> {scheduledTime}
              </div>
              <p>يرجى التواصل معه والتأكد من أنه بخير.</p>
              <hr>
              <p style="color:#718096; font-size:12px;">MediSync 💊 — تنبيه تلقائي</p>
            </div>
            """
        };

        await SendAsync(message);
    }

    private async Task SendAsync(MimeMessage message)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(Host, Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(Username, Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
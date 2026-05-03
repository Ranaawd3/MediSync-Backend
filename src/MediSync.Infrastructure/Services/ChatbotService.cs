using System.Text;
using System.Text.Json;
using MediSync.Application.Persistence;
using MediSync.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MediSync.Infrastructure.Services;

public class ChatbotService(
    IApplicationDbContext db,
    IHttpClientFactory    httpClientFactory,
    IConfiguration        config)
    : IChatbotService
{
    public async Task<string> AskAsync(string question, Guid userId)
    {
        // جيب بيانات المريض عشان الـ Context يكون دقيق
        var user = await db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("USER_NOT_FOUND");

        var meds = await db.Medications
            .Where(m => m.UserId == userId && m.IsActive)
            .Select(m => $"{m.BrandName} ({m.ActiveIngredient} {m.DosageValue}{m.DosageUnit})")
            .ToListAsync();

        var systemPrompt = $"""
            أنت مساعد صيدلاني ذكي اسمه MediSync Assistant.
            المريض: {user.FullName}
            أدويته الحالية: {(meds.Any() ? string.Join(", ", meds) : "لا يوجد")}
            حساسياته: {(user.Allergies.Any() ? string.Join(", ", user.Allergies) : "لا توجد")}
            
            قواعد مهمة:
            - أجب بالعربية دائماً بشكل بسيط ومفهوم للمريض
            - لا تقدم تشخيصاً طبياً أبداً
            - وجّه للطبيب أو الصيدلاني عند الحاجة
            - إجاباتك قصيرة ومركزة (3-5 جمل)
            - لو السؤال عن تفاعل دوائي — قول له يراجع طبيبه
            """;

        var client    = httpClientFactory.CreateClient();
        var openAiKey = config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API Key غير موجود");

        var payload = new
        {
            model = "gpt-4o-mini",  // الأرخص والأسرع
            max_tokens = 500,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = question }
            }
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/chat/completions")
        {
            Headers  = { Authorization = new("Bearer", openAiKey) },
            Content  = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"OpenAI Error: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var doc  = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "عذراً، لم أتمكن من الإجابة حالياً.";
    }
}
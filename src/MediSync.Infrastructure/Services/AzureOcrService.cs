using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MediSync.Infrastructure.Services;

// Stub implementation — هيتكمل في أسبوع 5 مع Azure Vision
public class AzureOcrService(
    IHttpClientFactory httpClientFactory,
    IConfiguration     config)
{
    private readonly string _fastApiUrl =
        config["ExternalServices:FastApiUrl"] ?? "http://localhost:8000";

    public async Task<object> ScanAsync(
        byte[]            imageBytes,
        string            fileName,
        string            contentType,
        CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient();
        using var form    = new MultipartFormDataContent();
        using var content = new ByteArrayContent(imageBytes);
        content.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        form.Add(content, "image", fileName);

        try
        {
            var response = await client.PostAsync($"{_fastApiUrl}/ocr", form, ct);
            var json     = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<object>(json)!;
        }
        catch
        {
            return new { status = "failed", confidence = 0 };
        }
    }
}
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MediSync.Infrastructure.Services;

public class AzureOcrService(
    IHttpClientFactory httpClientFactory,
    IConfiguration     config)
    : IOcrService
{
    private readonly string _fastApiUrl =
        config["ExternalServices:FastApiUrl"] ?? "http://localhost:8000";

    public async Task<OcrResultDto> ScanAsync(
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
            return JsonSerializer.Deserialize<OcrResultDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch
        {
            return new OcrResultDto(Guid.NewGuid(), 0, "failed", true, null, [], 0);
        }
    }
}
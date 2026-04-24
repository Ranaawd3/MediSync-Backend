using MediSync.Application.DTOs.Medications;

namespace MediSync.Application.Services;

public interface IOcrService
{
    /// بيستقبل الصورة كـ bytes — مش IFormFile (Application layer independent)
    Task<OcrResultDto> ScanAsync(
        byte[]          imageBytes,
        string          fileName,
        string          contentType,
        CancellationToken ct = default);
}
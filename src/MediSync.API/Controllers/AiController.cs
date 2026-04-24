using System.Text;
using System.Text.Json;
using MediatR;
using MediSync.Application.Features.Medications.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AiController(IMediator mediator)  // ✅ بدل IHttpClientFactory
    : ControllerBase
{
    /// <summary>OCR — Flutter ترفع صورة</summary>
    [HttpPost("ocr")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> OcrProxy([FromForm] OcrRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(
            System.Security.Claims.ClaimTypes.NameIdentifier)!);

        // ✅ تحويل IFormFile → byte[] هنا في الـ API Layer
        using var ms = new MemoryStream();
        await request.Image.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        var result = await mediator.Send(new ScanPrescriptionCommand(
            imageBytes,
            request.Image.FileName,
            request.Image.ContentType,
            userId));

        return Ok(result);
    }

    /// <summary>Drug Interaction Proxy → FastAPI</summary>
    [HttpPost("drug-interaction")]
    public async Task<IActionResult> DrugInteractionProxy([FromBody] JsonElement req)
    {
        // TODO: أسبوع 4 — هنحول ده لـ MediatR Command كمان
        return Ok(new { message = "Drug interaction proxy — coming soon" });
    }
}

/// DTO للـ OCR Upload — لازم يكون هنا مش في Application Layer
public class OcrRequest
{
    public IFormFile Image { get; set; } = null!;
}
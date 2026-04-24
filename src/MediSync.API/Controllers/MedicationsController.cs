using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Medications;
using MediSync.Application.Features.Medications.Commands;
using MediSync.Application.Features.Medications.Queries;

namespace MediSync.API.Controllers;

[ApiController]
[Route("api/v1/medications")]
[Authorize]
public class MedicationsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>كل أدوية المستخدم</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetMedicationsQuery(GetUserId()));
        return Ok(ApiResponse<List<MedicationDto>>.Ok(result));
    }

    /// <summary>تفاصيل دواء واحد</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetMedicationByIdQuery(id, GetUserId()));
        return Ok(ApiResponse<MedicationDto>.Ok(result));
    }

    /// <summary>إضافة دواء — بيفحص التفاعلات تلقائياً</summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddMedicationDto dto)
    {
        var result = await mediator.Send(new AddMedicationCommand(dto, GetUserId()));

        return StatusCode(201, new
        {
            success  = true,
            data     = result.Medication,
            warnings = result.HasWarnings ? result.Warnings : null
        });
    }

    /// <summary>تعديل دواء</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationDto dto)
    {
        var result = await mediator.Send(new UpdateMedicationCommand(id, GetUserId(), dto));
        return Ok(ApiResponse<MedicationDto>.Ok(result));
    }

    /// <summary>حذف دواء (Soft Delete)</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteMedicationCommand(id, GetUserId()));
        return StatusCode(204);
    }

    /// <summary>تأكيد أخذ الجرعة</summary>
    [HttpPost("{id}/taken")]
    public async Task<IActionResult> TakeDose(Guid id)
    {
        await mediator.Send(new TakeDoseCommand(id, GetUserId()));
        return Ok(ApiResponse<object>.Ok(new {}, "تم تسجيل الجرعة"));
    }

    /// <summary>بدائل آمنة للدواء</summary>
    [HttpGet("{id}/alternatives")]
    public async Task<IActionResult> GetAlternatives(Guid id)
    {
        var result = await mediator.Send(new GetAlternativesQuery(id, GetUserId()));
        return Ok(ApiResponse<List<string>>.Ok(result));
    }

    /// <summary>فحص تفاعلات المستخدم الحالية</summary>
    [HttpGet("interactions")]
    public async Task<IActionResult> GetInteractions()
    {
        var result = await mediator.Send(
            new MediSync.Application.Features.Interactions.Queries
                .CheckInteractionsQuery(GetUserId()));
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>OCR — Flutter بترفع صورة روشتة</summary>
    [HttpPost("scan")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Scan([FromForm] OcrRequest request)
    {
    using var ms = new MemoryStream();
    await request.Image.CopyToAsync(ms);

    var result = await mediator.Send(new ScanPrescriptionCommand(
        ms.ToArray(),
        request.Image.FileName,
        request.Image.ContentType,
        GetUserId()));

    return Ok(ApiResponse<object>.Ok(result));
    }
} 

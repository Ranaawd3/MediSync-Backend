using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;
using MediSync.Application.Features.Family.Commands;
using MediSync.Application.Persistence;
using MediSync.Application.Services;
using MediSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Family.Handlers;

public class InviteCaregiverHandler(
    IApplicationDbContext db,
    IEmailService         emailService
) : IRequestHandler<InviteCaregiverCommand, ApiResponse<FamilyLinkDto>>
{
    public async Task<ApiResponse<FamilyLinkDto>> Handle(
        InviteCaregiverCommand request,
        CancellationToken      ct)
    {
        // تأكد إن المريض موجود
        var patient = await db.Users.FindAsync(request.PatientId, ct)
            ?? throw new KeyNotFoundException("المستخدم غير موجود");

        // تأكد مفيش دعوة نشطة مسبقاً لنفس الـ Email
        var existing = await db.FamilyLinks.AnyAsync(
            f => f.PatientId == request.PatientId
              && f.CaregiverEmail == request.Dto.CaregiverEmail
              && f.Status != "revoked", ct);

        if (existing)
            throw new InvalidOperationException("دعوة مرسلة بالفعل لهذا البريد");

        // إنشاء رابط الأسرة
        var link = new FamilyLink
        {
            PatientId      = request.PatientId,
            CaregiverEmail = request.Dto.CaregiverEmail,
            CanEditMeds    = request.Dto.CanEditMeds,
            ReceiveAlerts  = request.Dto.ReceiveAlerts,
            Status         = "pending"
        };

        db.FamilyLinks.Add(link);
        await db.SaveChangesAsync(ct);

        // بعت Email الدعوة
        var acceptUrl = $"https://medisync.app/family/accept/{link.InviteToken}";
        await emailService.SendCaregiverInviteAsync(
            request.Dto.CaregiverEmail,
            patient.FullName,
            acceptUrl);

        var dto = MapToDto(link, patient.FullName);
        return ApiResponse<FamilyLinkDto>.Ok(dto, "تم إرسال الدعوة بنجاح");
    }

    private static FamilyLinkDto MapToDto(FamilyLink link, string patientName) =>
        new(link.Id, link.PatientId, patientName,
            link.CaregiverId, link.CaregiverEmail,
            link.Status, link.CanEditMeds, link.ReceiveAlerts,
            link.CreatedAt, link.AcceptedAt);
}
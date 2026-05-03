using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.DTOs.Family;
using MediSync.Application.Features.Family.Commands;
using MediSync.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediSync.Application.Features.Family.Handlers;

public class AcceptInviteHandler(
    IApplicationDbContext db
) : IRequestHandler<AcceptInviteCommand, ApiResponse<FamilyLinkDto>>
{
    public async Task<ApiResponse<FamilyLinkDto>> Handle(
        AcceptInviteCommand request,
        CancellationToken   ct)
    {
        var link = await db.FamilyLinks
            .Include(f => f.Patient)
            .FirstOrDefaultAsync(
                f => f.InviteToken == request.Token && f.Status == "pending", ct)
            ?? throw new KeyNotFoundException("الدعوة غير موجودة أو منتهية الصلاحية");

        // تحقق من انتهاء صلاحية الـ Token
        if (link.InviteExpiry < DateTime.UtcNow)
            throw new InvalidOperationException("انتهت صلاحية الدعوة — اطلب دعوة جديدة");

        // ربط الـ Caregiver بالـ Link
        link.CaregiverId = request.CaregiverId;
        link.Status      = "active";
        link.AcceptedAt  = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        var dto = new FamilyLinkDto(
            link.Id, link.PatientId, link.Patient.FullName,
            link.CaregiverId, link.CaregiverEmail,
            link.Status, link.CanEditMeds, link.ReceiveAlerts,
            link.CreatedAt, link.AcceptedAt);

        return ApiResponse<FamilyLinkDto>.Ok(dto, "تم قبول الدعوة بنجاح");
    }
}
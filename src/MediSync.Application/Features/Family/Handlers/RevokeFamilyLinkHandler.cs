using MediatR;
using MediSync.Application.DTOs.Common;
using MediSync.Application.Features.Family.Commands;
using MediSync.Application.Persistence;

namespace MediSync.Application.Features.Family.Handlers;

public class RevokeFamilyLinkHandler(
    IApplicationDbContext db
) : IRequestHandler<RevokeFamilyLinkCommand, ApiResponse<object>>
{
    public async Task<ApiResponse<object>> Handle(
        RevokeFamilyLinkCommand request,
        CancellationToken       ct)
    {
        var link = await db.FamilyLinks.FindAsync(request.LinkId, ct)
            ?? throw new KeyNotFoundException("الربط غير موجود");

        // تأكد إن صاحب الطلب هو الـ Patient نفسه
        if (link.PatientId != request.RequesterId)
            throw new UnauthorizedAccessException("AUTH_FORBIDDEN");

        link.Status = "revoked";
        await db.SaveChangesAsync(ct);

        return ApiResponse<object>.Ok(null!, "تم إلغاء صلاحية المقرب");
    }
}
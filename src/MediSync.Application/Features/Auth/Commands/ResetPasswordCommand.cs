using MediatR;
namespace MediSync.Application.Features.Auth.Commands;
public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;
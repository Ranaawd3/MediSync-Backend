using MediatR;
namespace MediSync.Application.Features.Auth.Commands;
public record ForgotPasswordCommand(string Email) : IRequest;
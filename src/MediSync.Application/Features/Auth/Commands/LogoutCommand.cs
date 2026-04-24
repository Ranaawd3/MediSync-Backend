using MediatR;

namespace MediSync.Application.Features.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;

using MediatR;
using MediSync.Application.DTOs.Auth;

namespace MediSync.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterDto Dto) : IRequest<TokenDto>;
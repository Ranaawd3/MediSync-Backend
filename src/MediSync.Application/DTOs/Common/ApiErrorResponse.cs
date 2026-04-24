namespace MediSync.Application.DTOs.Common;

public class ApiErrorResponse
{
    public bool        Success { get; init; } = false;
    public ErrorDetail Error   { get; init; } = new();
}

public class ErrorDetail
{
    public string       Code    { get; init; } = "";
    public string       Message { get; init; } = "";
    public List<string> Details { get; init; } = [];
}
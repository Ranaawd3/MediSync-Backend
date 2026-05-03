namespace MediSync.Application.Services;

public interface IChatbotService
{
    Task<string> AskAsync(string question, Guid userId);
}
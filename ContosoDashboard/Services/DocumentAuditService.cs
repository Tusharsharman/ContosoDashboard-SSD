using Microsoft.Extensions.Logging;

namespace ContosoDashboard.Services;

public class DocumentAuditService
{
    private readonly ILogger<DocumentAuditService> _logger;

    public DocumentAuditService(ILogger<DocumentAuditService> logger)
    {
        _logger = logger;
    }

    public Task AuditAsync(int userId, string action, int documentId, string? details = null)
    {
        _logger.LogInformation("DocumentAudit: User {UserId} performed {Action} on Document {DocumentId}. Details: {Details}", userId, action, documentId, details);
        return Task.CompletedTask;
    }
}

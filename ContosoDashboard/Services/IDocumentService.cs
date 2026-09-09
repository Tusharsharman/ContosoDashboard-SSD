using ContosoDashboard.Models;
using Microsoft.AspNetCore.Http;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<Document> UploadAsync(IFormFile file, int uploaderId, string title, string category, string? description, int? projectId, string? tags);
    Task<IEnumerable<Document>> GetMyDocumentsAsync(int userId, int page = 1, int pageSize = 50);
}

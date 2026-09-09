using ContosoDashboard.Data;
using ContosoDashboard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _db;
    private readonly IFileStorageService _storage;
    private readonly IVirusScanner _scanner;
    private readonly DocumentAuditService _audit;

    public DocumentService(ApplicationDbContext db, IFileStorageService storage, IVirusScanner scanner, DocumentAuditService audit)
    {
        _db = db;
        _storage = storage;
        _scanner = scanner;
        _audit = audit;
    }

    public async Task<Document> UploadAsync(IFormFile file, int uploaderId, string title, string category, string? description, int? projectId, string? tags)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file.Length <= 0) throw new ArgumentException("Empty file");
        if (file.Length > 25 * 1024 * 1024) throw new InvalidOperationException("File too large");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        var clean = await _scanner.ScanAsync(ms, file.FileName);
        if (!clean) throw new InvalidOperationException("File failed virus scan");

        ms.Position = 0;
        var storagePath = await _storage.UploadAsync(ms, file.FileName, file.ContentType);

        var doc = new Document
        {
            Title = title,
            Category = category,
            Description = description,
            Tags = tags,
            UploaderId = uploaderId,
            UploadDateUtc = DateTime.UtcNow,
            FileSizeBytes = file.Length,
            ContentType = file.ContentType ?? string.Empty,
            StoragePath = storagePath,
            ProjectId = projectId
        };

        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();

        await _audit.AuditAsync(uploaderId, "upload", doc.DocumentId, $"Title={title}");

        return doc;
    }

    public async Task<IEnumerable<Document>> GetMyDocumentsAsync(int userId, int page = 1, int pageSize = 50)
    {
        return await _db.Documents
            .Where(d => d.UploaderId == userId)
            .OrderByDescending(d => d.UploadDateUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}

using ContosoDashboard.Data;
using ContosoDashboard.Models;
using ContosoDashboard.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DocumentListing.Tests;

public class DocumentServiceListingTests
{
    [Fact]
    public async Task GetMyDocumentsAsync_ReturnsOnlyDocumentsOwnedByRequestedUser()
    {
        await using var context = CreateContext();
        context.Documents.AddRange(
            CreateDocument(1, 10, "Alpha plan", "Project Documents", "release notes"),
            CreateDocument(2, 20, "Other user's plan", "Project Documents", "release notes"));
        await context.SaveChangesAsync();

        var documents = await CreateService(context).GetMyDocumentsAsync(10);

        var document = Assert.Single(documents);
        Assert.Equal(1, document.DocumentId);
        Assert.Equal(10, document.UploaderId);
    }

    [Fact]
    public async Task GetMyDocumentsAsync_ReturnsNewestDocumentsFirstAndAppliesPaging()
    {
        await using var context = CreateContext();
        for (var index = 1; index <= 3; index++)
        {
            context.Documents.Add(CreateDocument(index, 10, $"Document {index}", "Project Documents", null,
                DateTime.UtcNow.AddMinutes(-index)));
        }
        await context.SaveChangesAsync();

        var documents = (await CreateService(context).GetMyDocumentsAsync(10, page: 1, pageSize: 2)).ToList();

        Assert.Equal(2, documents.Count);
        Assert.Equal(new[] { 1, 2 }, documents.Select(document => document.DocumentId));
    }

    [Fact]
    public async Task GetMyDocumentsAsync_CompletesWithinTwoSecondsForSmallDataset()
    {
        await using var context = CreateContext();
        for (var index = 1; index <= 500; index++)
        {
            context.Documents.Add(CreateDocument(index, 10, $"Project document {index}", "Project Documents", "planning tags"));
        }
        await context.SaveChangesAsync();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var documents = (await CreateService(context).GetMyDocumentsAsync(10, pageSize: 50)).ToList();
        stopwatch.Stop();

        Assert.Equal(50, documents.Count);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2), $"Listing took {stopwatch.Elapsed}");

        var searchMatches = documents.Where(document =>
            document.Title.Contains("planning", StringComparison.OrdinalIgnoreCase) ||
            (document.Description?.Contains("planning", StringComparison.OrdinalIgnoreCase) ?? false) ||
            (document.Tags?.Contains("planning", StringComparison.OrdinalIgnoreCase) ?? false));
        Assert.All(searchMatches, document => Assert.Contains("planning", document.Tags, StringComparison.OrdinalIgnoreCase));
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static DocumentService CreateService(ApplicationDbContext context)
    {
        return new DocumentService(context, new TestFileStorageService(), new TestVirusScanner(), new DocumentAuditService(new TestLogger<DocumentAuditService>()));
    }

    private static Document CreateDocument(int id, int uploaderId, string title, string category, string? tags, DateTime? uploadDate = null)
    {
        return new Document
        {
            DocumentId = id,
            UploaderId = uploaderId,
            Title = title,
            Category = category,
            Description = "Document description",
            Tags = tags,
            UploadDateUtc = uploadDate ?? DateTime.UtcNow,
            FileSizeBytes = 100,
            ContentType = "application/pdf",
            StoragePath = $"document-{id}.pdf"
        };
    }

    private sealed class TestFileStorageService : IFileStorageService
    {
        public Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default) => Task.FromResult(fileName);
        public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default) => Task.FromResult<Stream>(new MemoryStream());
        public Task<string> GetUrlAsync(string storagePath, TimeSpan expiration) => Task.FromResult(storagePath);
    }

    private sealed class TestVirusScanner : IVirusScanner
    {
        public Task<bool> ScanAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }

    private sealed class TestLogger<T> : Microsoft.Extensions.Logging.ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => false;
        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}

using Microsoft.Extensions.Configuration;
using System.IO;

namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration.GetValue<string>("FileStorage:LocalPath") ?? "AppData/uploads";
        // Ensure absolute path
        if (!Path.IsPathRooted(_basePath))
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), _basePath);
        }
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueName = Guid.NewGuid().ToString();
        var ext = Path.GetExtension(fileName);
        var filePath = Path.Combine(_basePath, uniqueName + ext);
        using var outStream = File.Create(filePath);
        await fileStream.CopyToAsync(outStream, cancellationToken);
        return filePath;
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(storagePath)) File.Delete(storagePath);
        return Task.CompletedTask;
    }

    public Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        Stream stream = File.OpenRead(storagePath);
        return Task.FromResult(stream);
    }

    public Task<string> GetUrlAsync(string storagePath, TimeSpan expiration)
    {
        // Local storage: return file path. In production this would return a signed URL.
        return Task.FromResult(storagePath);
    }
}

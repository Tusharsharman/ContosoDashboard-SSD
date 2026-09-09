namespace ContosoDashboard.Services;

public interface IVirusScanner
{
    Task<bool> ScanAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}

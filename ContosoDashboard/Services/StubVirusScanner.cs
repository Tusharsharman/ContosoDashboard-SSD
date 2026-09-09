namespace ContosoDashboard.Services;

public class StubVirusScanner : IVirusScanner
{
    public Task<bool> ScanAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        // Training stub: always return clean. Real implementation should call an external scanner.
        return Task.FromResult(true);
    }
}

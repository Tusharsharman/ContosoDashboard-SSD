using ContosoDashboard.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContosoDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;

    public DocumentsController(IDocumentService documents)
    {
        _documents = documents;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string title, [FromForm] string category, [FromForm] string? description, [FromForm] int? projectId, [FromForm] string? tags)
    {
        if (file == null) return BadRequest("No file uploaded");

        // For training, extract uploader id from claims if present; otherwise use 1
        var userId = 1;
        if (User?.Identity?.IsAuthenticated == true)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
            if (claim != null && int.TryParse(claim.Value, out var parsed)) userId = parsed;
        }

        try
        {
            var doc = await _documents.UploadAsync(file, userId, title, category, description, projectId, tags);
            return CreatedAtAction(nameof(GetMyDocuments), new { id = doc.DocumentId }, new { doc.DocumentId, doc.Title, doc.StoragePath, doc.UploadDateUtc });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyDocuments(int page = 1, int pageSize = 50)
    {
        var userId = 1;
        if (User?.Identity?.IsAuthenticated == true)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
            if (claim != null && int.TryParse(claim.Value, out var parsed)) userId = parsed;
        }

        var docs = await _documents.GetMyDocumentsAsync(userId, page, pageSize);
        return Ok(new { items = docs, total = docs.Count() });
    }
}

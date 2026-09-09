using System.Security.Claims;
using ContosoDashboard.Controllers;
using ContosoDashboard.Models;
using ContosoDashboard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DocumentListing.Tests;

public class DocumentsControllerListingTests
{
    [Fact]
    public async Task GetMyDocumentsAsync_UsesAuthenticatedNameIdentifierAndReturnsOkJson()
    {
        var service = new RecordingDocumentService(new[]
        {
            new Document { DocumentId = 7, UploaderId = 42, Title = "User document", Category = "Project Documents" }
        });
        var controller = new DocumentsController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.NameIdentifier, "42") }, "Cookies"))
                }
            }
        };

        var result = await controller.GetMyDocuments();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(42, service.RequestedUserId);
        Assert.NotNull(ok.Value);
        Assert.Contains("items", ok.Value!.GetType().GetProperties().Select(property => property.Name));
        Assert.Contains("total", ok.Value.GetType().GetProperties().Select(property => property.Name));
    }

    [Fact]
    public async Task GetMyDocumentsAsync_DoesNotMixDocumentsFromAnotherUser()
    {
        var service = new RecordingDocumentService(new[]
        {
            new Document { DocumentId = 1, UploaderId = 10, Title = "User 10 document" }
        });
        var controller = new DocumentsController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.NameIdentifier, "20") }, "Cookies"))
                }
            }
        };

        await controller.GetMyDocuments();

        Assert.Equal(20, service.RequestedUserId);
    }

    private sealed class RecordingDocumentService : IDocumentService
    {
        private readonly IEnumerable<Document> documents;
        public int RequestedUserId { get; private set; }

        public RecordingDocumentService(IEnumerable<Document> documents)
        {
            this.documents = documents;
        }

        public Task<IEnumerable<Document>> GetMyDocumentsAsync(int userId, int page = 1, int pageSize = 50)
        {
            RequestedUserId = userId;
            return Task.FromResult(documents.Where(document => document.UploaderId == userId));
        }

        public Task<Document> UploadAsync(Microsoft.AspNetCore.Http.IFormFile file, int uploaderId, string title, string category, string? description, int? projectId, string? tags) =>
            throw new NotSupportedException();
    }
}

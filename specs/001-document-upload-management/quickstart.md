# Quickstart: Validate Document Upload Feature Locally

Prerequisites
- .NET 8 SDK installed
- LocalDB or configured app database (app uses existing application DB)
- Ensure `AppData/uploads` writable directory exists (or configured local storage path)

Steps

1. Restore and build the app:

```powershell
dotnet restore
dotnet build
```

2. Ensure database is clean for tests (optional):

```powershell
# Optional: Drop and recreate local DB (see spec for instructions)
dotnet ef database drop --force
dotnet ef database update
```

3. Run the app locally:

```powershell
dotnet run --project ContosoDashboard/ContosoDashboard.csproj
```

4. Open the app in a browser, log in as a test user, and navigate to the Documents page.

5. Upload a test PDF (<25MB), fill required metadata, and submit. Expected outcome: upload completes, metadata persisted, document appears in "My Documents" list.

6. Test download/preview: click preview for PDF/image; verify inline preview. Click download to verify binary stream.

7. Test delete: delete a document and confirm it is removed from listing and storage.

Expected results
- Uploads saved to local storage path and metadata persisted to DB.  
- UI lists and search operate within performance targets for small datasets.  
- Audit logs emitted for upload/download/delete actions.

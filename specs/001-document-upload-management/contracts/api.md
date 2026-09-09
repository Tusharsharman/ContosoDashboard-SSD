# API Contracts: Document Upload and Management (minimal)

## Upload

POST /api/documents/upload
- Request: multipart/form-data with fields: `file` (binary), `title`, `description` (optional), `category`, `projectId` (optional), `tags` (optional)
- Response 201: { documentId, title, storagePath, uploadDateUtc }
- Errors: 400 (validation), 413 (file too large), 415 (unsupported media type), 401/403 (auth)

## List My Documents

GET /api/documents/my
- Query: `page`, `pageSize`, `sort`, `filterCategory`, `projectId`, `dateFrom`, `dateTo`
- Response 200: { items: [ { documentId, title, category, uploadDateUtc, fileSize } ], total }

## Project Documents

GET /api/projects/{projectId}/documents
- Response 200: similar to My Documents but scoped to project and authorization enforced

## Download / Preview

GET /api/documents/{id}/download
- Authorization: verifies project membership or share
- Response: 200 stream with proper Content-Type and Content-Disposition

GET /api/documents/{id}/preview
- Returns HTML viewer for PDFs/images or redirects to download for unsupported previews

## Edit Metadata

PUT /api/documents/{id}
- Body: { title?, description?, category?, tags? }
- Response: 200 updated metadata

## Delete

DELETE /api/documents/{id}
- Response: 204 on success (permanent delete as specified)

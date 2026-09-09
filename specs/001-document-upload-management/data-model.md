# Data Model: Document Upload and Management

## Entities

- **Document**
  - `DocumentId` (int, PK)
  - `Title` (string, required)
  - `Description` (string, optional)
  - `Category` (string, required)
  - `Tags` (string, optional) — stored as comma-separated values or a normalized link table in later iterations
  - `UploaderId` (int, FK -> User)
  - `UploadDateUtc` (datetime)
  - `FileSizeBytes` (long)
  - `ContentType` (string, length 255)
  - `StoragePath` (string) — path or storage key used by storage implementation
  - `ProjectId` (int, nullable, FK -> Project)

- **DocumentShare**
  - `DocumentShareId` (int, PK)
  - `DocumentId` (int, FK -> Document)
  - `SharedWithUserId` (int, nullable)
  - `SharedWithTeamId` (int, nullable)
  - `SharedByUserId` (int, FK -> User)
  - `SharedDateUtc` (datetime)

## Indexes & Constraints

- Index on `UploaderId` + `UploadDateUtc` for efficient "My Documents" queries
- Index on `ProjectId` for project document listings
- Index on `Tags` may be needed for search; consider a separate tag table for scale
- `ContentType` length must support long MIME strings (255)

## Validation Rules

- `Title`: required, max length 256
- `Category`: required, must be one of predefined values (Project Documents, Team Resources, Personal Files, Reports, Presentations, Other)
- `FileSizeBytes`: <= 25 * 1024 * 1024
- `ContentType`: allowed list for upload validation; stored length 255

## Migration Notes

- Add `Documents` table with integer PK and required fields. Provide migration script to backfill if needed.
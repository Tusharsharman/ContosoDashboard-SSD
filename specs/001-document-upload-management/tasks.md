# Tasks: Document Upload and Management

Phase 1 — Setup

- [x] T001 [P] Create feature folder scaffolding and add plan/spec artifacts in `specs/001-document-upload-management`
- [x] T002 Initialize local uploads directory and configuration entry in `appsettings.json` (e.g., `FileStorage:LocalPath`)
- [x] T003 [P] Add dependency registration for storage abstraction in `Program.cs` (DI placeholder)
- [x] T004 Create database migration scaffold for `Documents` table in `Data/Migrations/` (EF migration)

Phase 2 — Foundational (blocking prerequisites)

- [x] T005 Implement `Data/Document.cs` entity and add `DbSet<Document>` to `Data/ApplicationDbContext.cs`
- [x] T006 [P] Add data model validation attributes in `Data/Document.cs` (title length, required category, max file size constraint constant)
- [x] T007 Implement storage abstraction interface in `Services/IFileStorageService.cs`
- [x] T008 Implement local storage backend `Services/LocalFileStorageService.cs` (save/delete/download semantics, path generation)
- [x] T009 Implement audit logging for document actions in `Services/DocumentAuditService.cs` (or integrate into `DocumentService`)
- [x] T010 Create `Services/DocumentService.cs` with orchestrated upload workflow (validate, authorize, store file, save metadata, emit audit + notifications)
- [x] T011 Add unit test project tests/DocumentService.UnitTests for core business logic

Phase 3 — User Story US1 (Upload Document) (Priority: P1)

- [x] T012 [US1] [P] Implement upload API endpoint `Controllers/DocumentsController.UploadAsync` (route: `POST /api/documents/upload`)
- [x] T013 [US1] Implement client upload component `_DocumentUpload.razor` and `Pages/Documents.razor` UI hooks
- [x] T014 [US1] Implement server-side validation (file type whitelist, size limit) in `Services/DocumentService.cs`
- [x] T015 [US1] Integrate virus-scan stub or ClamAV hook in `Services/Scanning/IVirusScanner.cs` and tests
- [x] T016 [US1] Add integration tests tests/DocumentService.IntegrationTests for successful upload and rejected uploads (size/type)

Phase 4 — User Story US2 (Browse & Search) (Priority: P1)

- [x] T017 [US2] Implement `GET /api/documents/my` endpoint in `Controllers/DocumentsController.cs` with paging, sorting, filtering
- [x] T018 [US2] Implement front-end `Pages/Documents.razor` listing with sort/filter UI and search input
- [x] T019 [US2] Add unit/integration tests for listing/search performance and authorization

Phase 5 — User Story US3 (Project Documents & Sharing) (Priority: P2)

- [ ] T020 [US3] Implement project-scoped listing `GET /api/projects/{id}/documents` and authorization checks in `Controllers/ProjectsController.cs` or `DocumentsController.cs`
- [ ] T021 [US3] Implement `DocumentShare` data model and share API `POST /api/documents/{id}/share`
- [ ] T022 [US3] Implement notifications for shares in `Services/NotificationService.cs` and tests

Phase 6 — User Story US4 (Preview, Edit Metadata, Delete) (Priority: P2)

- [ ] T023 [US4] Implement preview endpoint `GET /api/documents/{id}/preview` and client preview UI `DocumentDetails.razor`
- [ ] T024 [US4] Implement metadata edit API `PUT /api/documents/{id}` and UI edits in `DocumentDetails.razor`
- [ ] T025 [US4] Implement delete API `DELETE /api/documents/{id}` that permanently removes file and metadata and emits audit event
- [ ] T026 [US4] Add integration tests covering preview, edit, and delete flows including authorization checks

Final Phase — Polish & Cross-Cutting Concerns

- [ ] T027 [P] Add structured logging and metrics for uploads, downloads, errors (integrate with existing logging)
- [ ] T028 [P] Add documentation and quickstart `specs/001-document-upload-management/quickstart.md` and inline README in `ContosoDashboard/Docs/`
- [ ] T029 Run performance and load checks for listing/search to validate p95 targets
- [ ] T030 Create migration and rollback scripts and ensure CI runs DB migrations in test pipeline

Dependencies (story completion order)

- US1 (Upload) and foundational tasks T005-T011 must be completed before US2 (Browse & Search) and US4 (Preview/Edit/Delete).
- US2 should follow US1; US3 (sharing) depends on US1 and user/project authorization components.

Parallel execution examples

- Frontend components (`T013`, `T018`, `T023`) can be developed in parallel with backend endpoints for different stories if API contracts are mocked (`T012`, `T017`, `T023`).
- Storage implementation (`T008`) and DB schema (`T005`) can be worked on in parallel by different engineers with agreed schema contract.

Independent test criteria per story

- US1: Upload a supported file with required metadata; file exists on disk; DB record present; response 201.
- US2: Query "My Documents" with filters and search; results are correct and returned within 2s (small dataset).
- US3: Share a document with a user; recipient receives notification and can access the document.
- US4: Preview a PDF inline; edit metadata persists; delete removes file and DB record.

Suggested MVP scope

- MVP = Phase 1 + Phase 2 + Phase 3 + Phase 4 (basic upload, listing/search, project scope can be P2 after initial release). Prioritize US1 and US2 for first release.

Format validation: This tasks.md follows the checklist format with sequential Task IDs, [P] for parallelizable tasks, and [USn] labels for story tasks.

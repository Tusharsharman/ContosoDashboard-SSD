# Research: Document Upload and Management (Phase 0)

Decision: Store binaries on local filesystem outside `wwwroot` for training, with a storage abstraction to support later Azure Blob Storage migration.

Rationale: The project must work offline for training. Local filesystem is simple to set up and secure when used with controller-based authorized download endpoints. A storage abstraction decouples business logic from storage implementation for future migration.

Alternatives considered:
- Azure Blob Storage (production): pros — scalable and managed; cons — not available offline for training, added complexity and credentials.
- Database BLOB storage: pros — transactional; cons — heavy DB load, not necessary for training scale.

Decision: Use non-guessable filenames and per-user/per-project directory organization.

Rationale: Prevents path traversal and guessing; enables simple cleanup policies; matches future blob naming strategies.

Alternatives considered:
- Keep original user filenames on disk (rejected): security risk and collision-prone.

Decision: Virus scanning — provide an integration point and a training fallback stub.

Rationale: Security requirement mandates scanning; ClamAV CLI can be used in training, or a stubbed scanner that logs an event and returns success for offline training. Production must use a real scanning pipeline.

Decision: Previewing — render PDFs and images in-browser; for Office docs, provide download and optionally convert to PDF in later phases.

Rationale: Browser PDF/image preview provides immediate UX. Office conversions introduce additional dependencies.

Decision: Deletion policy — permanent delete immediately (as clarified by stakeholder).

Rationale: Simpler lifecycle for training and aligns with stakeholder choice; document deletion will remove file and metadata immediately and emit an audit event.

Open Questions (NEEDS CLARIFICATION):
- None remaining; stakeholder confirmed deletion policy.

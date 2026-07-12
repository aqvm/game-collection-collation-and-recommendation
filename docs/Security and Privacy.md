# Security and Privacy

## Threat posture

Protect account access, personal play history, local database integrity, and user trust against credential leakage, malicious imports/content, compromised dependencies, unsafe browser callbacks, and overbroad diagnostic exports.

## Baseline controls

- Store no passwords.
- Put tokens, keys, and session secrets in the signed-in OS account's credential vault.
- Keep secret references—not values—in SQLite.
- Use system browser authorization, PKCE/device flow, state/nonce validation, loopback callback hardening, and least scopes where supported.
- Encrypt network traffic and validate endpoints/certificates normally; do not disable TLS verification.
- Redact logs by allowlist, not a fragile blacklist.
- Make diagnostic bundles previewable before export.
- Make crash reporting explicitly opt-in and document transmitted fields.
- Validate JSON imports with size/depth/count limits and transactional preview.
- Treat remote text, markup, URLs, and images as untrusted content.
- Sign release artifacts and publish checksums/update metadata when distribution begins.

## Privacy defaults

- No account and no cloud are required.
- No behavioral analytics by default.
- Metadata queries reveal only the game/provider request necessary for enrichment.
- Optional AI services receive game evidence and task instructions, not credentials or a whole identifiable library.
- Local logs have retention and size limits.
- Disconnecting a platform offers clear choices for deleting secrets, cached observations, and normalized records.
- Export and backup are explicit user actions; optional Google Drive backup is a later feature with client-side encryption considered before implementation.

## Security work products

- Per-connector threat model and requested scopes.
- Data-flow diagram and data inventory.
- Secret-redaction tests and log review.
- Dependency and secret scanning in CI.
- Software bill of materials for releases.
- Vulnerability disclosure policy before public release.
- Recovery tests for interrupted migrations and corrupted/partial sync data.

## Authentication caveat

“Browser-assisted and persistent” is a desired experience, not permission to capture private cookies or automate unsupported login forms. Each platform feasibility report must establish an acceptable durable authorization mechanism. Where none exists, prefer read-only local discovery or user imports over unsafe credential handling.


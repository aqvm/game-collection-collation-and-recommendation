# Sprint 1 reassessment after Sprint 0

The original Sprint 1 implementation established useful domain, SQLite, and Steam API experiments, but it is not Sprint 1-complete after Sprint 0 because its interaction surface was a console program. Sprint 0 selected and validated the Uno C# Markup shell, so all user workflows must now be provided in that shell.

## Retained work

- `SteamWebApiSource`: documented `GetOwnedGames` observation path, cancellation, and no browser/password handling.
- `SqliteLibraryStore`: immutable raw snapshots, platform-copy uniqueness, transactional reconciliation, durable exclusion/state changes.
- Domain preferred-copy policy and SQLite/purgatory tests.

## Work to replace or extend

| Prior element | Reassessment | Sprint 1 action |
|---|---|---|
| CLI commands | Not an accessible desktop workflow | Replace with connection, sync, library, detail, correction, and purgatory UI actions |
| API key environment variable | Not durable or user-facing | Store a user-supplied key through `ISecretVault`; show capability limits and connection health |
| Console progress | Does not meet activity-panel requirement | Bind job phase/count/error/retry state into the Uno activity panel |
| Per-product game rows | Does not fully model canonical/copy relationships | Make canonical games and copies explicit in the migration and detail view |
| Minimal tests | Do not prove desktop journeys or connector fixtures | Add fixture-backed connector, migration, search, correction, and UI journey coverage |

No Sprint 1 checklist item is currently complete. Its retained lower-layer work will be reused, but the sprint must be delivered through the validated desktop architecture and reverified against the offline demo.

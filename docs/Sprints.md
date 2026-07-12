# Sprints

Sprints are ordered capability increments, not calendar boxes. Each ends in working software or a decision that unblocks working software. A sprint may be split into smaller implementation tasks without changing its outcome.

## Sprint 0 — Foundations and evidence

**Outcome:** A buildable Windows-first solution with enforceable architecture, CI, a code-defined desktop shell, and resolved first-order technical unknowns.

- [x] Initialize Git.
- [x] Choose Apache-2.0 or MIT, create a non-`main` implementation branch, and add contribution/security basics.
- [x] Pin current .NET LTS and dependencies; enable nullable types, analyzers, deterministic builds, formatting, and warnings policy.
- [x] Create Domain/Application/Infrastructure/Desktop/test project boundaries.
- [x] Render a small Uno C# Markup library shell from fake data; check keyboard/focus, virtualization approach, startup/package size, testability, and OS integration.
- [x] Confirm or replace Uno using ADR-003 exit criteria—no parallel production prototypes.
- [x] Establish SQLite migration/recovery test and OS credential-vault abstraction.
- [x] Add structured redacted logging, background job state model, and activity-panel skeleton.
- [x] Write Steam capability/authorization report with sanitized fixtures.
- [x] Create initial threat model and data inventory.

**Demo:** The app opens quickly, browses a large fake library smoothly, shows a detailed fake background job, and passes CI.

## Sprint 1 — Trustworthy local library

**Outcome:** A useful offline library from a real Steam account.

- [x] Implement the narrowest responsible Steam connection/import path established in Sprint 0.
- [x] Store immutable sync observations, platform copies, canonical games, and connection health.
- [x] Implement idempotent snapshot/diff/reconciliation with cancellation and recovery.
- [x] Browse, search, filter, and open a game detail view from cached local data.
- [x] Implement preferred-copy selection and explain why a copy is preferred.
- [x] Implement purgatory and the explicit recovery filter; test exclusion invariants.
- [x] Allow durable correction of editable metadata and lifecycle state.
- [x] Surface detailed synchronization phases, changes, errors, and retry state.

**Demo:** Disconnect the network, launch immediately, browse the previously imported real library, inspect copies, exclude/restore a game, and rerun sync without duplicates.

> Live-account demo status: pending user-provided SteamID64 and Web API key. Fixture-backed connector, reconciliation, exclusion, recovery, and offline-cache tests pass locally.

## Sprint 2 — Identity, manual ownership, and backlog

**Outcome:** The library remains correct when automation is incomplete and begins helping the user choose intentionally.

- [ ] Implement versioned JSON import with schema, preview, validation, and transactional errors.
- [ ] Implement UI-driven one-off manual game/copy entry.
- [ ] Add confidence-based identity matching and merge/split review workflow.
- [ ] Add edition/remaster/inferior/supersedes relationships.
- [ ] Add explicit lifecycle states and conservative activity-derived suggestions.
- [ ] Put every active game in a context-agnostic calculated backlog.
- [ ] Support removal, restoration, manual priority, and reordering without losing calculated rank.
- [ ] Add basic cover-art cache and responsive placeholders.

**Demo:** Import mixed manual data, resolve an ambiguous identity, relate a remaster, and curate a backlog without corrupting platform facts.

## Sprint 3 — Metadata that knows the games

**Outcome:** Games acquire useful recommendation traits passively, transparently, and without harming foreground performance.

- [ ] Evaluate metadata providers for coverage, license, attribution, caching, limits, and cost.
- [ ] Implement provider interfaces, evidence/provenance storage, and confidence aggregation.
- [ ] Implement low-priority prioritized research queue with resource budgets, pause/cancel, checkpoint, retry, and provider backoff.
- [ ] Populate the six initial recommendation dimensions and tags.
- [ ] Always retain a provisional assessment; flag very low confidence.
- [ ] Create game-level evidence and correction UI.
- [ ] Ensure user overrides persist across re-research and retain audit history.
- [ ] Add recommendation-readiness and low-confidence coverage reporting.

**Demo:** Add an obscure game, continue using the app smoothly while research runs, inspect its sources/confidence, correct a trait, and re-run research without losing the override.

## Sprint 4 — One game tonight

**Outcome:** The app gives one responsive, defensible recommendation rather than another overwhelming list.

- [ ] Define calibrated slider semantics and deterministic scoring formula.
- [ ] Implement eligibility gates for purgatory, never-recommend, lifecycle constraints, and inferior editions.
- [ ] Combine slider fit, backlog intent, neglect, confidence, and revealed preferences.
- [ ] Show one recommendation with trait metrics, evidence confidence, score contribution, and natural-language explanation.
- [ ] Recompute when sliders change without displaying an alternative queue.
- [ ] Implement rejection/cooldown and metadata-correction-aware feedback.
- [ ] Attribute subsequent play as positive signal conservatively.
- [ ] Store algorithm version and complete recommendation event for debugging.
- [ ] Validate with golden libraries and adversarial edge cases.

**Demo:** Move an individual slider, see the single recommendation and explanation change for the stated reason, correct bad metadata, reject it, and receive a new eligible game.

## Sprint 5 — Delight and self-discovery

**Outcome:** Opening the application regularly reveals something accurate, surprising, and motivating.

- [ ] Implement versioned, evidence-backed insight templates and freshness/diversity rules.
- [ ] Build the home surface around one fresh insight and the current one-game recommendation.
- [ ] Add genre radar by playtime and by count.
- [ ] Add backlog, recommendation-dimension coverage, and ownership-by-platform visualizations.
- [ ] Add accessible tables, filters, definitions, confidence, and duplicate policies for charts.
- [ ] Establish visual language, motion primitives, optional sound design, and satisfying interaction states.
- [ ] Add reduced-motion, mute/volume, keyboard, focus, contrast, and screen-reader acceptance checks.
- [ ] Measure scrolling, animation frame time, memory, artwork cache, and time-to-interactive.

**Demo:** Repeated launches rotate non-repetitive, sourced insights; charts agree with filtered library facts; all delight features respect accessibility controls.

## Sprint 6 — Epic connection

**Outcome:** Add Epic ownership responsibly if the feasibility gate permits it; otherwise ship the best honest fallback.

- [ ] Complete Epic feasibility, terms, threat model, capability matrix, and fixture strategy.
- [ ] Implement supported connection/local import/manual fallback according to the decision.
- [ ] Reconcile Epic copies with existing identities and preferred-copy policy.
- [ ] Add Epic-specific ownership nature, playtime/achievement limitations, health, and errors.
- [ ] Detect and display wishlist flags only where responsibly available.
- [ ] Exercise duplicates, purgatory, backlog, statistics, and recommendations across platforms.

**Demo:** A Steam/Epic duplicate becomes one related library identity with two copies and a deterministic preferred copy—or the UI clearly explains the supported Epic fallback and missing capabilities.

## Sprint 7 — GOG connection

**Outcome:** Add GOG under the same evidence and degradation standards.

- [ ] Complete GOG feasibility, terms, threat model, capability matrix, and fixture strategy.
- [ ] Implement the supported connection/local import/manual fallback.
- [ ] Reconcile GOG copies and product-separated editions.
- [ ] Add GOG-specific limitations and connection health.
- [ ] Validate three-platform statistics, duplicate policy, and wishlist flags.

**Demo:** A three-platform library remains responsive, explainable, and correct across refresh, disconnect, exclusion, and identity correction.

## Sprint 8 — Installable preview

**Outcome:** A non-developer can install, connect, understand, diagnose, and safely remove the Windows application.

- [ ] Build guided first-run launcher discovery and explicit connection selection.
- [ ] Package/sign Windows installer and clean uninstall behavior.
- [ ] Add update policy and release-channel design.
- [ ] Implement previewable diagnostic bundle with redaction tests.
- [ ] Implement explicitly opt-in crash reporting with a published data contract.
- [ ] Add backup/export and disaster-recovery workflow; cloud backup remains deferred.
- [ ] Run security, accessibility, performance, migration, and fresh-machine release checks.
- [ ] Publish privacy, security, contribution, support, and vulnerability-reporting documentation.

**Demo:** A fresh Windows user installs, imports, understands active background work, exports diagnostics, restores a backup, and uninstalls cleanly.

## Later increments

- [ ] macOS build, vault, packaging, launcher discovery, accessibility, and release validation.
- [ ] Linux build, Wayland/X11 validation, vault, packaging, and launcher discovery.
- [ ] Amazon Games feasibility and connector/fallback.
- [ ] itch.io feasibility and connector/fallback.
- [ ] Xbox/Microsoft, PlayStation, Nintendo, physical/emulated collection investigations.
- [ ] Optional client-side-encrypted cloud backup such as Google Drive.
- [ ] Deeper achievement/history insights and preference learning after sufficient local feedback.

## Release gates

Do not call a platform/connector supported until its happy path, offline behavior, failure recovery, disconnect, migration, diagnostics, and privacy behavior are tested. Do not promote a sprint merely because its checklist is long; promote it when its outcome is demonstrable.

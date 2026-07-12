# Risks and Decisions

## Risk register

| Risk | Likelihood / impact | Mitigation and trigger |
|---|---|---|
| Epic/GOG lack durable consumer-library APIs | High / Critical | Feasibility gates; capability model; local/manual fallback; never capture passwords |
| Provider terms prohibit desired collection/caching | Medium / Critical | Terms review before connector/provider implementation; record allowed use and attribution |
| Canonical identity merges wrong games | High / High | Platform IDs, confidence, review queue, immutable observations, reversible merge/split history |
| Metadata research is costly, slow, or low quality | High / High | Tiered providers, low-priority budgets, provenance, confidence, overrides, incremental queue |
| Recommendation feels arbitrary | Medium / High | Deterministic baseline, complete score explanation, golden scenarios, feedback semantics |
| Background work harms responsiveness | Medium / High | Fast cached shell, bounded scheduler, cancellation, instrumentation, measured budgets |
| Cross-platform UI/packaging friction | Medium / High | Isolated UI/OS adapters; Windows first; only claim platforms covered by CI/release tests |
| “UI juice” harms accessibility or performance | Medium / Medium | Reduced motion/sound controls, interruptible animation, semantic UI, frame-time checks |
| Documentation becomes token-heavy/stale | Medium / Medium | Hub-and-spoke notes, sprint-scoped reading, link checks, compact decisions, milestone pruning |
| External services change without notice | High / Medium | Contract fixtures, capability health, graceful degradation, connector versioning |

## Decision log

### ADR-001 — Modular local-first desktop application

- **Status:** Accepted
- **Decision:** Use a modular monolith with local SQLite persistence and OS vault secrets. No hosted backend is required.
- **Reason:** Privacy, offline utility, simpler operations, and a single-person product model.
- **Consequences:** Backup/sync is deferred; device compromise remains within the OS-account threat boundary.

### ADR-002 — C#/.NET core

- **Status:** Proposed
- **Decision:** Use current .NET LTS and C# unless the first implementation sprint reveals a blocking constraint.
- **Reason:** Existing experience, maintainability, performance, tooling, testing, and platform APIs.
- **Consequences:** Cross-platform desktop UI requires a third-party framework.

### ADR-003 — Uno Platform with C# Markup

- **Status:** Proposed, validate during the first vertical slice
- **Decision:** Use Uno code-defined UI without XAML. Keep UI replaceable through project boundaries.
- **Reason:** Meets C#, cross-platform, non-Electron, and no-XML preferences.
- **Revisit if:** Desktop accessibility, library virtualization, charting, startup/package size, Windows integration, Linux/macOS viability, or testing is materially inadequate.
- **Fallback:** Avalonia code-only; accepting its documentation ecosystem rather than adopting XAML by default.

**Validation evidence (2026-07-11):** Uno 6.5.36 compiles against .NET 10 desktop. A C#-only shell renders a 2,500-item local `ListView`, keyboard-focusable search, and activity panel. The domain/application/infrastructure projects have no Uno dependency. No exit criterion was triggered; no competing production prototype was created.

### ADR-004 — Permissive open-source license

- **Status:** Proposed
- **Decision:** Use Apache License 2.0.
- **Reason:** Permissive reuse like MIT, plus an explicit patent grant and clearer contribution protections. This is helpful for a connector-heavy public project. MIT remains acceptable if extreme brevity is preferred.
- **Consequence:** Others may distribute proprietary derivatives; copyleft guarantees are not a stated goal.

### ADR-005 — Explainable recommendation baseline

- **Status:** Accepted
- **Decision:** Begin with deterministic weighted scoring and explicit gates, not opaque machine learning.
- **Reason:** Debuggability, user trust, sparse initial feedback, and explainable slider response.

## Open decisions

- [ ] Confirm Uno after the first real screen and architecture checks; record concrete evidence, not taste alone.
- [ ] Select SQLite library, migration mechanism, and OS vault abstraction.
- [ ] Complete Steam authentication/data capability decision.
- [ ] Complete Epic and GOG feasibility reports.
- [ ] Select metadata sources after license/coverage evaluation.
- [ ] Define exact manual-import JSON schema and compatibility policy.
- [ ] Define recommendation slider scales and calibration examples.
- [ ] Define opt-in crash-report provider and transmitted-data contract.
- [ ] Confirm Apache-2.0 before first public source release.

## Superseded ideas

- ~~Run an A/B implementation across UI frameworks.~~ Rejected as excessive; use one provisional framework with objective exit criteria.
- ~~Require all three initial connectors for the first usable build.~~ Replaced by a Steam vertical slice followed by connector feasibility/increments.


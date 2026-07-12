# Engineering Handbook

## Quality principles

- Optimize first for clarity, correctness, observability, and measured performance.
- Use small cohesive types, explicit names, nullable-reference checking, and compiler/analyzer enforcement.
- Comments explain intent, invariants, provider quirks, and non-obvious tradeoffs—not what readable code already says.
- Public/domain APIs receive documentation where it aids use; avoid ceremonial comment coverage.
- Favor composition, immutable values, dependency inversion at real boundaries, and boring technology.
- Avoid speculative abstractions and generic “plugin systems.” Connector interfaces are internal extension points.

## Testing strategy

Use test-driven development where it improves design and confidence:

- Unit tests for domain policies, identity rules, state inference, scoring, exclusions, and copy priority.
- Property/boundary tests for normalization, rankings, thresholds, and import validation.
- Contract tests for connector/provider capabilities using sanitized fixtures.
- Integration tests with temporary SQLite databases, migrations, vault fakes, and HTTP stubs.
- Architecture tests preventing infrastructure/UI dependencies from leaking inward.
- UI tests for startup, import, search, correction, purgatory recovery, and recommendation journeys.
- A small number of end-to-end tests; do not demand test-first work for purely cosmetic iteration.

No live third-party service is required for the ordinary test suite. Live smoke tests are manual/secured and rate-limited.

## Definition of done

A change is done when applicable:

- Acceptance behavior is demonstrated and tests pass.
- Failure, cancellation, empty, offline, and low-confidence states are handled.
- Accessibility and responsiveness are checked for affected UI.
- Logs contain useful non-secret context.
- Migrations are forward-tested and recovery considered.
- Relevant living documentation and decision status are updated.
- No new warnings, secrets, or unexplained dependencies are introduced.

## Source control and CI

- Initialize Git with `main` protected conceptually from direct work.
- Never commit directly to `main`; use helpfully named branches.
- Resynchronize with `main` before every commit, per repository guidance.
- Keep commits small, intentional, and independently buildable.
- CI should restore reproducibly, format/lint, build with warnings enforced, test, scan secrets/dependencies, and produce diagnostic artifacts.
- Add platform build jobs as those platforms become supported; do not claim support without CI and release validation.

## Observability

Use structured events with correlation IDs for sync/research jobs, durations, outcomes, retry categories, and affected counts. Never log tokens, cookies, authorization codes, raw sensitive responses, or unnecessary personal identifiers.

## Living documentation policy

- [[../PROJECT-PLAN|Project plan]] is the navigation hub.
- Each implementation task reads the sprint note and directly linked domain note, not the whole vault.
- Update current truth in place; retain a compact “superseded” record only when the reasoning remains useful.
- Check completed checklist items rather than deleting them.
- Record architectural/product decisions in [[Risks and Decisions]] with date, status, reason, and consequences.
- Archive verbose investigations under `docs/research/` only when needed; summarize their conclusion in the relevant durable note.
- Prefer links over duplicated prose. Split a note only when it has a distinct audience or change cadence.
- Review documentation size and stale links at each release milestone.

## Pull-request checklist

- [ ] Scope maps to a sprint outcome or documented decision.
- [ ] Tests added/updated at the right level.
- [ ] External data and secrets are sanitized.
- [ ] UI remains responsive and accessible.
- [ ] Connector/provider assumptions are backed by fixtures or documentation.
- [ ] Relevant notes/checklists/ADRs updated.
- [ ] Branch was resynchronized with `main` before commit.


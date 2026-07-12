# Data Model

## Modeling rule

Do not flatten truth into one mutable game row. Preserve three layers:

1. **Observed:** what a platform or imported file reported, with source and observation time.
2. **Inferred:** normalized identities, traits, states, and relationships produced by the tool, with confidence and evidence.
3. **Overridden:** user judgments, which take precedence in presentation and recommendation without destroying observed history.

## Principal records

| Record | Purpose |
|---|---|
| `CanonicalGame` | One application identity corresponding normally to one platform-separated product |
| `PlatformCopy` | Platform product ID, ownership/access nature, account, playtime, achievements, visibility, dates |
| `PlatformConnection` | Non-secret connection state, capabilities, last synchronization, health |
| `LibraryState` | Unplayed, playing, paused, completed, abandoned, endless/not-applicable, unknown |
| `Exclusion` | Purgatory status, reason, time, and optional note |
| `BacklogEntry` | Membership, calculated priority, user order/priority override, status |
| `TraitAssessment` | Dimension, value, confidence, provenance, model/version, override |
| `TagAssertion` | Tag plus observed/inferred/user source and confidence |
| `GameRelationship` | Edition/remaster/inferior-to/supersedes relationship |
| `WishlistFlag` | Platform and last-observed wishlist state; visible only on owned game details |
| `RecommendationEvent` | Inputs, candidates, score breakdown, result, rejection/acceptance/play outcome |
| `Insight` | Generated fact, supporting query/evidence, freshness, display history |
| `SyncSnapshot` | Connector observation batch and reconciliation result |

## Identity rules

- If a platform lists products separately, model them as separate canonical games by default.
- Relate editions/remasters rather than merging them automatically.
- A user may mark one game as inferior to or superseded by another; inferior games are excluded from recommendations while remaining in the active library and statistics unless separately sent to purgatory.
- Matching uses platform IDs first, then curated external IDs, normalized title/publisher/date signals, and finally reviewable fuzzy matching.
- Never silently merge ambiguous matches. Preserve aliases and split/merge history.

## Copy priority

The preferred copy is selected deterministically:

1. Greatest playtime for that game.
2. If all copies have zero/unknown playtime, the platform with greatest total active-library playtime.
3. Stable configured platform order as the final tie-breaker.
4. User override wins and persists.

## Exclusion invariant

If a canonical game or all of its relevant copies are in purgatory, it is excluded from:

- Default browsing and search
- Backlog membership and priority
- Recommendation candidates and preference learning
- All aggregate statistics, charts, and generated insights
- Platform-total playtime used for copy priority

It remains available through an explicit “include excluded” filter and can be restored.

## Derived state heuristics

Heuristics are suggestions, not irreversible facts:

- More than one hour establishes at least “played,” not necessarily a lifecycle state.
- Recent alternating activity can make multiple games “playing.”
- High playtime (initial heuristic: over 100 hours) may suggest “favorite,” but favorites should ultimately be a separate user-adjustable signal rather than a lifecycle state.
- Completion and abandonment usually require achievements, explicit user input, or stronger evidence than playtime alone.

Thresholds belong in versioned policy/configuration and must be tested at boundaries.

## Manual import

Support both one-off UI entry and versioned JSON import. The schema must include source platform, platform product ID when available, ownership/access nature, state, exclusion, playtime provenance, and user metadata. Imports validate fully, preview changes, and report per-record errors before committing transactionally.


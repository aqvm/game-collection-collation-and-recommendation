# Experience and Visualizations

## Design direction

Slick, visual, responsive, and stats-forward—not a dense administrative database by default. Cover art, typography, motion, spatial transitions, responsive hover/press states, subtle sound, and rewarding microinteractions should make exploration pleasurable.

“UI juice” must remain purposeful:

- Never delay input or conceal work.
- Support reduced motion, mute, volume, and high-contrast/keyboard use.
- Avoid sound for routine background completion unless enabled.
- Keep animations interruptible and frame-time monitored.
- Treat accessibility semantics and focus behavior as acceptance criteria.

## Primary surfaces

1. **Home:** one fresh, sourced insight; one-game recommendation; background activity summary.
2. **Library:** cover-forward browse, fast search/filter, platform/copy badges, explicit purgatory filter.
3. **Game detail:** copies, preferred copy, ownership nature, play history, achievements, traits, evidence, overrides, wishlist flags, relationships.
4. **Recommendation:** one answer, slider controls, score/trait breakdown, explanation, reject and correction actions.
5. **Backlog:** calculated order plus clear manual priority/reordering semantics.
6. **Stats:** interactive charts with definitions, filters, and accessible tabular equivalents.
7. **Connections and activity:** launcher discovery, capability/health status, sync controls, detailed jobs.

## Insight engine

Insights are versioned queries/templates over trustworthy local facts, not free-form claims. Examples:

- Near-complete achievements in a long-neglected game.
- A game played in every month of a prior year.
- An unusual genre/platform/playtime pattern.
- A forgotten acquisition that strongly matches current preferences.
- Duplicate ownership or a wishlist conflict.

Each insight stores its evidence, freshness requirements, eligibility constraints, last-shown time, and diversity category. Purgatory data is never used. Avoid repeating recent insights, overclaiming causality, or exposing unreliable dates as fact.

## Initial visualizations

- Genre radar weighted by playtime.
- Genre radar weighted by game count.
- Backlog composition and priority.
- Recommendation-dimension coverage, including missing/low-confidence regions.
- Ownership by platform, with clear treatment of duplicates.

Every chart defines its denominator and duplicate policy, supports filtering, explains uncertainty, and provides a non-visual data view. Radar charts require careful normalization and category-count limits to remain readable.

## Background activity experience

The global indicator opens a detailed activity panel showing:

- Exact job and current phase (for example, “Matching 34 new Steam products”).
- Completed/total counts where knowable.
- Start time, last progress, rate-limit/backoff state, and estimated time only when defensible.
- CPU/network policy and pause/cancel/retry controls.
- Errors with actionable explanations and preserved cached data.
- Recently completed work and resulting library changes.

## Artwork and cache

Artwork loads progressively from a disk cache with placeholders and cancellation. Record source/license requirements. Bound cache size and provide clear storage controls. Never allow image decoding or network fetching to block scrolling or startup.


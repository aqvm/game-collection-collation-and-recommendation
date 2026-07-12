# Product Vision

## Product promise

The application is a personal picture window into every game the user owns. It should answer:

- What do I own, and where?
- Which copy is the meaningful one?
- What have I forgotten?
- What does my library say about how I actually play?
- Given how I feel tonight, what single game should I play?

## Experience principles

1. **Local first.** Account secrets, normalized library data, preferences, and recommendation history stay on the device by default.
2. **Useful before comprehensive.** Ship a working Steam vertical slice before multiplying connectors.
3. **One answer, not another catalog.** The primary recommendation surface shows exactly one game.
4. **Explain the magic.** Recommendations, inferences, and background work expose their reasons and state.
5. **Automation with recourse.** The tool does the research; the user can inspect and correct it.
6. **Respect exclusion.** A purgatory game is recoverable through an explicit filter but absent from ordinary views, statistics, insights, backlog, learning, and recommendations.
7. **Joy is functional.** Motion, sound, tactility, art, and small surprises are product requirements, subject to accessibility and reduced-motion/sound controls.
8. **Fast shell, patient workers.** The interactive UI becomes usable before synchronization or research begins.

## Audiences

The initial user is the project owner. The architecture should nevertheless lead to a guided installer and account setup suitable for ordinary users. Windows is the first validation target; macOS and Linux remain architectural targets.

## Core capabilities

- Launcher discovery with explicit opt-in connection.
- Steam, Epic, and GOG library collation; Amazon Games, itch.io, Xbox/Microsoft, PlayStation, Nintendo, and manual sources are planned extensions.
- Canonical game identities with platform-specific copies, playtime, achievements, state, ownership nature, and wishlist flags.
- JSON batch import and UI-driven manual entry.
- Duplicate resolution favoring the copy with greatest game playtime, then the platform with greatest total playtime.
- Background metadata enrichment with provenance and confidence.
- User-editable traits, tags, relationships, and lifecycle state.
- Backlog management and one-game “play tonight” recommendation.
- Stats-forward home screen and library visualizations.

## Success measures

Early measures are local and product-oriented rather than telemetry-dependent:

- Import completion and reconciliation accuracy on a real library.
- Percentage of platform items matched to the correct canonical title.
- Time to interactive launch and UI responsiveness during background work.
- Percentage of active-library games with recommendation-ready metadata.
- Recommendation acceptance, rejection, and subsequent-play rate.
- User corrections per researched game and confidence calibration.
- Number of useful insights generated without factual correction.

## Terminology

- **Canonical game:** the application's identity for one platform-listed game/product.
- **Copy:** ownership/access for a canonical game on one platform/account.
- **Active library:** all non-excluded canonical games and copies.
- **Purgatory:** excluded but recoverable records.
- **Trait:** a scored recommendation dimension such as intensity or session length.
- **Evidence:** sourced material supporting inferred metadata.
- **Insight:** a factual, explainable observation derived from library history.


# Recommendations and Research

## Recommendation promise

The primary surface answers “What should I play tonight?” with exactly one game. Changing a slider recomputes that one answer. The explanation shows why the winner fits, what evidence supports its traits, and how revealed preferences affected it.

## Initial sliders

- Familiar ↔ novel
- Thinking ↔ vibing
- Short session ↔ long session
- Relaxing ↔ intense
- Short completion ↔ long commitment
- Recently acquired ↔ long neglected

Each trait has a normalized value, confidence, provenance, and optional user override. Slider semantics and endpoints require user-facing definitions and calibration examples.

## Baseline scoring

Start deterministic and explainable. A candidate score combines:

- Distance from current slider targets, weighted by confidence.
- Backlog priority and explicit user ordering.
- Recency/neglect and acquisition context.
- Learned affinity from past choices without overwhelming explicit tonight settings.
- Diversity/cooldown so repeated rejection does not cycle immediately.
- Eligibility gates: purgatory, explicit “never recommend,” incompatible state, and inferior/superseded edition.

Persist the complete score breakdown and algorithm version for every recommendation. Do not begin with an opaque ML ranker. Personalization can later adjust documented weights and priors.

## Feedback semantics

- **Play after recommendation:** positive outcome, subject to a reasonable attribution window.
- **Reject without editing metadata:** negative tonight-context signal; surface another game.
- **Correct metadata then reject:** primarily a data-quality correction; do not overlearn dislike.
- **Never recommend:** hard eligibility exclusion, reversible in settings.
- **Inferior to another game:** relationship-based eligibility exclusion.
- **Manual backlog edits:** durable explicit preference stronger than inferred ordering.

The UI never queues several visible alternatives. Recommendation history can remain inspectable elsewhere for transparency.

## Backlog priority

Every active-library game begins in the backlog. A context-agnostic score orders it using intent, neglect, completion commitment, revealed preference, confidence, and user adjustments. Users can remove, restore, reorder, or explicitly prioritize games. Preserve both calculated score and user override so recomputation never erases intent.

## Metadata research pipeline

1. Create a low-priority job for a new or changed canonical game.
2. Resolve authoritative identity and store-page metadata first.
3. Collect permitted evidence from configured providers.
4. Extract candidate tags and trait assessments.
5. Aggregate values with source reliability and confidence.
6. Always produce a provisional assessment; visibly flag very low confidence.
7. Persist citations, retrieval time, raw-source reference/cache policy, and inference version.
8. Recalculate only affected recommendation and visualization features.

Research prioritizes games currently viewed, likely recommendation candidates, new acquisitions, and missing metadata. It runs with strict concurrency/CPU/network budgets and pauses under user activity or power/resource constraints where practical.

## Provider strategy

- The core experience must work without a paid key.
- Prefer authoritative store/product pages and licensed/open structured databases.
- Optional user-supplied API or AI keys may enrich obscure games.
- Never send credentials or the full personal library to an AI provider.
- Provider licenses, attribution, caching, robots directives, and redistribution terms must be recorded before integration.
- Forums and reviews are supporting evidence, not unquestioned facts.

## User overrides

Users may change subjective traits, tags, relationships, lifecycle state, and interpretation metadata. They may not rewrite observed playtime or achievements; corrections there are annotations or connector reconciliation. Overrides retain prior value, source, time, and optional reason.

## Evaluation

Maintain anonymized/synthetic test libraries and golden scoring scenarios. Test eligibility, boundary values, explanation/score agreement, correction semantics, deterministic ties, low-confidence behavior, and migration compatibility. Recommendation quality is evaluated separately from metadata coverage.


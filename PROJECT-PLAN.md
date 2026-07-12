# Game Library Project Plan

> [!abstract] Mission
> Build a joyful, local-first desktop application that unifies a person's game ownership, reveals surprising truths about their library, and recommends exactly one game worth playing tonight.

## Start here

- [[docs/Product Vision|Product vision and principles]]
- [[docs/Architecture|Architecture and technology decisions]]
- [[docs/Data Model|Domain and data model]]
- [[docs/Integrations|Platform integration strategy]]
- [[docs/Recommendations and Research|Recommendations and metadata research]]
- [[docs/Experience and Visualizations|Experience, statistics, and visualizations]]
- [[docs/Security and Privacy|Security and privacy]]
- [[docs/Engineering Handbook|Engineering and documentation practices]]
- [[docs/Risks and Decisions|Risks, open questions, and decision log]]
- [[docs/Sprints|Incremental sprint roadmap]]

## Current status

| Area | Status | Next decision or result |
|---|---|---|
| Product discovery | Complete for initial planning | Validate assumptions with working software |
| Repository | Git initialized; planning only | Begin Sprint 0 on an implementation branch |
| UI stack | Proposed | Uno Platform with C# Markup; validate with a minimal vertical slice |
| First connector | Proposed | Steam feasibility and import |
| Epic and GOG | Unproven | Complete documented feasibility investigations |
| Recommendation system | Designed conceptually | Establish explainable baseline scoring |

## Product outcomes

1. A user can see a trustworthy unified library without surrendering credentials or library data to a hosted service.
2. Duplicate ownership and cross-platform wishlist flags are visible on canonical game records.
3. Excluded games remain recoverable but influence nothing else.
4. The application recommends one game at a time and explains the evidence, slider fit, and learned preferences behind it.
5. Background synchronization and research never block startup and always report meaningful progress.
6. The home screen repeatedly creates moments of discovery through accurate, second-order library insights.

## Near-term definition of usable

The first usable deliverable imports a real Steam library into a local database, normalizes it, supports search and browsing, displays a canonical game detail page, and permits durable user corrections. It does not need Epic, GOG, an installer, or a sophisticated recommendation engine yet.

## Non-goals

- A storefront, purchasing flow, or competing launcher.
- Launching games directly; platform deep links may be offered later.
- Storefront/browser purchase interception.
- Multi-user or household profiles.
- Mandatory cloud accounts, cloud storage, or paid metadata services.
- Perfect automated identity matching without review or correction.

## Working rules

- Each sprint must leave the application working and demonstrably more useful.
- Sprint boundaries express coherent increments, not fixed calendar commitments.
- External integrations are adapters around a stable core and may degrade independently.
- User edits outrank inferred metadata; observed platform facts remain traceable.
- Completed work and superseded decisions are marked, not silently erased. Git retains fine-grained history; these notes retain decision-relevant history.
- Implementation chats should read this index plus only the notes linked by their assigned sprint.

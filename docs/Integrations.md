# Integrations

## Connector contract

Each platform adapter advertises capabilities rather than pretending all platforms are equal:

- Detect installed launcher
- Connect/disconnect account
- Enumerate owned/access-granted products
- Read playtime, achievements, acquisition/last-played data
- Read wishlist membership
- Produce deep link to the platform client/library
- Incrementally synchronize

Unsupported capabilities are explicit. Connector failures never prevent cached library use.

## Connection policy

- Discover installed launchers locally, then ask which services to connect.
- Also list supported services whose launchers are absent.
- Prefer official OAuth/device authorization and APIs.
- Never collect or store account passwords.
- Persist refresh tokens or session material only in the OS credential vault.
- Browser-assisted authentication should use the system browser and verified callback/device flows where supported.
- Local launcher databases/configuration may supplement official APIs when lawful, documented, read-only, and resilient.
- Scraping or reuse of private session cookies requires a separate legal/security decision and is not assumed.

## Platform plan

### Steam — first implementation

The official `IPlayerService/GetOwnedGames` API can return visible owned games and playtime but requires a Web API key and sufficient profile visibility. The implementation investigation must compare:

- User-provided Web API key and SteamID.
- Public-profile limitations and guided setup.
- Local Steam installation metadata as a read-only supplement.
- Achievement, wishlist, free-game, family-sharing, hidden-product, and acquisition-date coverage.

Deliver a capability matrix and fixture-backed contract tests before relying on fields.

### Epic — feasibility gate

Epic Online Services exposes ownership concepts for applications integrating EOS, but that does not establish a supported API for enumerating a consumer's complete Epic Games Store library. Investigate official account authorization, local launcher manifests/databases, entitlement scope, wishlist availability, terms, and token handling. If no responsible durable route exists, support explicit local/manual import and label limitations honestly.

### GOG — feasibility gate

The public GOG developer SDK is oriented toward developers integrating their own games and stats, not necessarily consumer-wide library enumeration. Investigate supported account flows, local GOG Galaxy data, product metadata, wishlist and playtime coverage, and terms. Avoid recreating Galaxy's brittle third-party-plugin model inside the application.

### Planned later

Amazon Games, itch.io, Xbox/Microsoft Store, PlayStation, Nintendo, DRM-free/manual, and emulated/physical collections each receive a capability and risk assessment before implementation.

## Synchronization pipeline

1. Connector captures an immutable observation snapshot.
2. Validate and normalize platform records.
3. Diff against the last successful snapshot.
4. Reconcile identities and copies transactionally.
5. Queue research only for new or materially changed games.
6. Recompute affected aggregates, backlog scores, insights, and recommendations.
7. Publish a detailed summary; retain enough history to diagnose changes.

Jobs are idempotent and use retry with jitter/backoff. Respect documented rate limits and provider terms. Never delete local/user data merely because one transient response omits it.

## Wishlist behavior

Wishlist-only games do not appear as browsable library items. If an owned canonical identity is observed on any connected wishlist, its detail view shows which platform(s) contain the wishlist entry. Ambiguous identity matches are reviewable and do not produce confident warnings.

## Required feasibility artifact

Every connector begins with a short report containing authentication method, available fields, rate limits, terms constraints, offline/local sources, failure modes, test-fixture strategy, and graceful fallback.

## Primary references

- [Steam Web API overview](https://partner.steamgames.com/doc/webapi_overview)
- [Steam `IPlayerService`, including `GetOwnedGames`](https://partner.steamgames.com/doc/webapi/iplayerservice)
- [Epic Online Services ownership status](https://dev.epicgames.com/docs/api-ref/enums/eos-e-ownership-status)
- [GOG Galaxy SDK statistics and achievements](https://docs.gog.com/sdk-stats-and-achievements/)

These references establish available developer-facing capabilities, not permission or proof that a consumer-wide library API exists. The feasibility reports must verify the actual user-library use case and current terms.

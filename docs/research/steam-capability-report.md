# Steam capability and authorization report

**Decision:** Sprint 1 may use Steam's documented `IPlayerService/GetOwnedGames` endpoint with a user-supplied Web API key and SteamID. The key is stored only through `ISecretVault`; it is never placed in SQLite or logs.

| Capability | Status | Evidence / limitation |
|---|---|---|
| Owned games and playtime | Supported | `GetOwnedGames`, subject to profile visibility and API-key access |
| Product ID and title | Supported | `appid`, `name`, and `playtime_forever` in the response |
| Incremental sync | Derived | Snapshot/diff locally; endpoint is a full observation |
| Achievements, wishlist, acquisition date | Not in this path | Explicitly deferred until separately authorized and fixture-backed |

The connector uses HTTPS, the documented endpoint, cancellation, and sanitized JSON fixtures. It does not automate Steam login, collect passwords, reuse cookies, or scrape a browser session. A failed or partial response retains the prior cached library.

References: [Steam Web API overview](https://partner.steamgames.com/doc/webapi_overview) and [IPlayerService](https://partner.steamgames.com/doc/webapi/iplayerservice).

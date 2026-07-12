# Steam API key guide

## Why the app asks for this

Steam requires an API key to let the app request the complete visible game library and playtime. Think of it as a private pass for this one connection; it is not a Steam password and it does not let the app buy games, trade items, or change the account.

## Create a key

1. Open [Steam's API-key page](https://steamcommunity.com/dev/apikey) in a normal browser and sign in to Steam.
2. Read and accept Steam's API terms.
3. When Steam asks for a domain name, use information that is accurate for your own API-key registration. Game Library cannot choose this for you and does not recommend a particular value.
4. Create the key, copy it, and paste it once into Game Library's Steam connection panel.
5. The entry box masks the key. After saving, it is held by Windows Credential Manager and the app only reports that a key is saved—it never displays it again.

## Replace or remove a key

Use **Disconnect Steam** in the connection panel. The confirmation lets you independently remove the key from Windows Credential Manager and delete cached Steam observations, copies, and games. Paste a replacement only when you next want to sync.

If Steam rejects a key, the app keeps cached data, explains that the key may be revoked or invalid, and directs the user to remove and replace it. Retrying unchanged credentials is not presented as a fix.

## Limits and privacy

- Steam privacy settings, private games, Family Sharing, and API changes can affect what is returned.
- The app uses HTTPS, never logs the key, and does not put it in SQLite, exports, or diagnostics.
- A key is protected by the signed-in Windows account. Someone who can fully control that Windows account can act as that account; use normal device lock and account-security practices.
- Steam may rate-limit or temporarily fail requests. The app preserves the last successful cache and permits a later retry.

Official references: [Steam API-key authentication](https://partner.steamgames.com/doc/webapi_overview/auth?l=english&language=english), [IPlayerService](https://partner.steamgames.com/doc/webapi/iplayerservice?language=english), and [Steam API response codes](https://partner.steamgames.com/doc/webapi_overview/responses?l=dutch&language=english).

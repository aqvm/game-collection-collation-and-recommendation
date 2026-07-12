# Privacy notice

Game Library is local-first. It does not require an account with us, send analytics by default, or upload a library to our servers.

## Steam connection data

When the user requests a Steam sync, the app sends the user’s SteamID and personal Steam Web API key over HTTPS to Steam. The key is stored only in Windows Credential Manager. The local SQLite database stores Steam observations, normalized games/copies, sync status, exclusions, and user lifecycle corrections on the user’s device.

The app does not display a saved key, write it to logs, include it in exports/diagnostics, or share it with third parties. Steam data is stored only on the user’s local device, in the country or region where that device is located.

## User choices

The **Disconnect Steam** control can remove the saved key and, independently, delete Steam observations, copies, and games. Removing either does not send a deletion request to Steam.

## Source-data notice

Steam data is provided as is and as available. It may be incomplete, delayed, unavailable, or inaccurate. Game Library is not affiliated with or endorsed by Valve or Steam.

# Initial threat model and data inventory

| Asset / flow | Threat | Control |
|---|---|---|
| Steam API key | Disclosure in database, logs, or source | Windows Credential Manager through `ISecretVault`; allowlist-redacted logs; secret scanning in CI |
| Steam response | Personal library disclosure | Local SQLite only, sanitized fixtures, no telemetry |
| JSON/database | Corruption or interrupted write | Transactional migrations, immutable snapshots, recovery test |
| Browser authorization | Cookie capture or callback abuse | No browser login/cookie route; future flows require documented OAuth/device flow review |
| Diagnostics | Oversharing | Logs contain only allowlisted operational fields; export is deferred pending preview UX |

Data stored locally in Sprint 0: fake shell data, SQLite schema/migration records, test fixtures, and structured operational events. Sprint 1 adds raw Steam observations and normalized copies; secrets remain outside SQLite.

# Contributing

Use a focused non-`main` branch. Before committing, fast-forward from `origin/main`, then run `dotnet format --verify-no-changes`, `dotnet build GameLibrary.sln -c Release`, and `dotnet test GameLibrary.sln -c Release`.

Do not add credentials, Steam API responses, account IDs, or diagnostic exports to the repository. Connector changes require sanitized fixtures and a capability report.

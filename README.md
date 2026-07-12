# Game Library

Local-first game-library desktop application. Sprint 0 establishes the Uno C# Markup shell, local persistence seams, safe logging, connector evidence, and CI.

## Run and verify

```powershell
dotnet build GameLibrary.sln -c Release
dotnet test GameLibrary.sln -c Release
dotnet run --project src/GameLibrary.Desktop -f net10.0-desktop
```

The shell starts from its local cached library and activity state; it does not contact Steam on startup.

To import a complete Steam library, follow the beginner-friendly [Steam API key guide](docs/Steam%20API%20Key%20Guide.md). The key is stored in Windows Credential Manager and is never displayed after saving.

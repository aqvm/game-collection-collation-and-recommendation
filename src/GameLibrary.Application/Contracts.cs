using GameLibrary.Domain;

namespace GameLibrary.Application;

public interface ILibraryStore
{
    Task SaveSnapshotAsync(SyncObservation observation, CancellationToken cancellationToken);
    Task<SyncResult> ReconcileAsync(SyncObservation observation, IProgress<SyncProgress>? progress, CancellationToken cancellationToken);
    Task<IReadOnlyList<Game>> SearchAsync(string? query, bool includeExcluded, CancellationToken cancellationToken);
    Task<IReadOnlyList<PlatformCopy>> GetCopiesAsync(Guid gameId, CancellationToken cancellationToken);
    Task SetExcludedAsync(Guid gameId, bool excluded, string? reason, CancellationToken cancellationToken);
    Task SetLifecycleStateAsync(Guid gameId, LifecycleState state, CancellationToken cancellationToken);
    Task RecordSyncFailureAsync(string platform, string message, CancellationToken cancellationToken);
    Task RecordSyncSuccessAsync(string platform, string message, CancellationToken cancellationToken);
    Task<string?> GetLatestSyncStatusAsync(string platform, CancellationToken cancellationToken);
    Task DeleteSteamDataAsync(CancellationToken cancellationToken);
}

public interface ISteamSource
{
    Task<SyncObservation> GetOwnedGamesAsync(string steamId, CancellationToken cancellationToken);
}

public interface ISecretVault
{
    Task StoreAsync(string reference, string secret, CancellationToken cancellationToken);
    Task<string?> GetAsync(string reference, CancellationToken cancellationToken);
    Task RemoveAsync(string reference, CancellationToken cancellationToken);
}

public interface IStructuredLogger
{
    void Write(string eventName, IReadOnlyDictionary<string, object?> fields);
}

public sealed class LibraryService(ILibraryStore store, ISteamSource steam)
{
    public async Task<SyncResult> SyncSteamAsync(string steamId, IProgress<SyncProgress>? progress, CancellationToken cancellationToken)
    {
        try
        {
            progress?.Report(new("Observing", 0, 0, "Reading owned games from Steam."));
            var observation = await steam.GetOwnedGamesAsync(steamId, cancellationToken);
            await store.SaveSnapshotAsync(observation, cancellationToken);
            var result = await store.ReconcileAsync(observation, progress, cancellationToken);
            await store.RecordSyncSuccessAsync("Steam", $"{result.Added} added, {result.Updated} updated, {result.Unchanged} unchanged.", cancellationToken);
            return result;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await store.RecordSyncFailureAsync("Steam", exception.Message, CancellationToken.None);
            throw;
        }
    }

    public Task<IReadOnlyList<Game>> BrowseAsync(string? query, bool includeExcluded, CancellationToken cancellationToken) => store.SearchAsync(query, includeExcluded, cancellationToken);
    public Task<IReadOnlyList<PlatformCopy>> GetCopiesAsync(Guid gameId, CancellationToken cancellationToken) => store.GetCopiesAsync(gameId, cancellationToken);
    public Task SetPurgatoryAsync(Guid gameId, bool excluded, string? reason, CancellationToken cancellationToken) => store.SetExcludedAsync(gameId, excluded, reason, cancellationToken);
    public Task CorrectStateAsync(Guid gameId, LifecycleState state, CancellationToken cancellationToken) => store.SetLifecycleStateAsync(gameId, state, cancellationToken);
    public Task<string?> GetSteamHealthAsync(CancellationToken cancellationToken) => store.GetLatestSyncStatusAsync("Steam", cancellationToken);
}

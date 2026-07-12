namespace GameLibrary.Domain;

public enum ConnectionHealth { Unknown, Healthy, Degraded, Failed }
public enum LifecycleState { Unknown, Unplayed, Playing, Paused, Completed, Abandoned, Endless }
public enum BackgroundJobState { Queued, Running, Paused, Retrying, Completed, Failed, Cancelled }

public sealed record PlatformCopy(string Platform, string ProductId, string Title, int PlaytimeMinutes, DateTimeOffset ObservedAt);
public sealed record Game(Guid Id, string Title, LifecycleState State, bool Excluded, DateTimeOffset UpdatedAt);
public sealed record SyncObservation(string Platform, string AccountId, IReadOnlyList<PlatformCopy> Copies, DateTimeOffset ObservedAt, string RawPayload);
public sealed record SyncProgress(string Phase, int Completed, int Total, string Message);
public sealed record SyncResult(int Added, int Updated, int Unchanged, DateTimeOffset CompletedAt);
public sealed record BackgroundJobStatus(Guid Id, string Kind, BackgroundJobState State, string Phase, int Completed, int? Total, DateTimeOffset UpdatedAt, string? RetryReason = null);

public static class PreferredCopyPolicy
{
    public static PlatformCopy? Select(IEnumerable<PlatformCopy> copies, IReadOnlyDictionary<string, int> platformMinutes, IReadOnlyList<string> platformOrder)
        => copies.OrderByDescending(copy => copy.PlaytimeMinutes)
            .ThenByDescending(copy => platformMinutes.GetValueOrDefault(copy.Platform))
            .ThenBy(copy => { var index = Enumerable.Range(0, platformOrder.Count).FirstOrDefault(i => string.Equals(platformOrder[i], copy.Platform, StringComparison.Ordinal)); return string.Equals(platformOrder.ElementAtOrDefault(index), copy.Platform, StringComparison.Ordinal) ? index : int.MaxValue; })
            .ThenBy(copy => copy.Platform, StringComparer.Ordinal)
            .FirstOrDefault();
}

using GameLibrary.Application;
using GameLibrary.Domain;
using Microsoft.Data.Sqlite;

namespace GameLibrary.Infrastructure;

public sealed class SqliteLibraryStore : ILibraryStore
{
    private readonly string _connectionString;
    public SqliteLibraryStore(string databasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath) ?? ".");
        _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
        Initialize();
    }

    public async Task SaveSnapshotAsync(SyncObservation observation, CancellationToken cancellationToken)
    {
        await using var connection = Open();
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO SyncSnapshots(Platform, AccountId, ObservedAt, Payload) VALUES($platform,$account,$at,$payload);";
        command.Parameters.AddWithValue("$platform", observation.Platform); command.Parameters.AddWithValue("$account", observation.AccountId); command.Parameters.AddWithValue("$at", observation.ObservedAt.ToString("O")); command.Parameters.AddWithValue("$payload", observation.RawPayload);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<SyncResult> ReconcileAsync(SyncObservation observation, IProgress<SyncProgress>? progress, CancellationToken cancellationToken)
    {
        var added = 0; var updated = 0; var unchanged = 0;
        await using var connection = Open(); await connection.OpenAsync(cancellationToken); await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        for (var index = 0; index < observation.Copies.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested(); var copy = observation.Copies[index];
            var find = connection.CreateCommand(); find.Transaction = (SqliteTransaction)transaction; find.CommandText = "SELECT GameId, Title, Playtime FROM Copies WHERE Platform=$platform AND ProductId=$product;"; find.Parameters.AddWithValue("$platform", copy.Platform); find.Parameters.AddWithValue("$product", copy.ProductId);
            await using var reader = await find.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var gameId = reader.GetString(0); var same = reader.GetString(1) == copy.Title && reader.GetInt32(2) == copy.PlaytimeMinutes; reader.Close();
                if (same) unchanged++; else { await UpdateCopyAsync(connection, (SqliteTransaction)transaction, copy, gameId, cancellationToken); updated++; }
            }
            else { var gameId = Guid.NewGuid().ToString(); await InsertGameAsync(connection, (SqliteTransaction)transaction, gameId, copy, cancellationToken); added++; }
            progress?.Report(new("Reconciling", index + 1, observation.Copies.Count, copy.Title));
        }
        await transaction.CommitAsync(cancellationToken);
        progress?.Report(new("Completed", observation.Copies.Count, observation.Copies.Count, "Steam library is cached locally."));
        return new SyncResult(added, updated, unchanged, DateTimeOffset.UtcNow);
    }

    public async Task<IReadOnlyList<Game>> SearchAsync(string? query, bool includeExcluded, CancellationToken cancellationToken)
    { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "SELECT Id,Title,State,Excluded,UpdatedAt FROM Games WHERE ($excluded=1 OR Excluded=0) AND ($query='' OR Title LIKE '%' || $query || '%') ORDER BY Title;"; command.Parameters.AddWithValue("$excluded", includeExcluded ? 1 : 0); command.Parameters.AddWithValue("$query", query?.Trim() ?? ""); await using var reader = await command.ExecuteReaderAsync(cancellationToken); var result = new List<Game>(); while (await reader.ReadAsync(cancellationToken)) result.Add(new(Guid.Parse(reader.GetString(0)), reader.GetString(1), Enum.Parse<LifecycleState>(reader.GetString(2)), reader.GetBoolean(3), DateTimeOffset.Parse(reader.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind))); return result; }
    public async Task<IReadOnlyList<PlatformCopy>> GetCopiesAsync(Guid gameId, CancellationToken cancellationToken)
    { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "SELECT Platform,ProductId,Title,Playtime,ObservedAt FROM Copies WHERE GameId=$id;"; command.Parameters.AddWithValue("$id", gameId.ToString()); await using var reader = await command.ExecuteReaderAsync(cancellationToken); var result = new List<PlatformCopy>(); while (await reader.ReadAsync(cancellationToken)) result.Add(new(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), DateTimeOffset.Parse(reader.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind))); return result; }
    public async Task SetExcludedAsync(Guid gameId, bool excluded, string? reason, CancellationToken cancellationToken) { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "UPDATE Games SET Excluded=$excluded, UpdatedAt=$at WHERE Id=$id; INSERT INTO Exclusions(GameId,Excluded,Reason,ChangedAt) VALUES($id,$excluded,$reason,$at);"; command.Parameters.AddWithValue("$id", gameId.ToString()); command.Parameters.AddWithValue("$excluded", excluded ? 1 : 0); command.Parameters.AddWithValue("$reason", reason ?? string.Empty); command.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("O")); await command.ExecuteNonQueryAsync(cancellationToken); }
    public async Task SetLifecycleStateAsync(Guid gameId, LifecycleState state, CancellationToken cancellationToken) { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "UPDATE Games SET State=$state, UpdatedAt=$at WHERE Id=$id;"; command.Parameters.AddWithValue("$id", gameId.ToString()); command.Parameters.AddWithValue("$state", state.ToString()); command.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("O")); await command.ExecuteNonQueryAsync(cancellationToken); }
    public async Task RecordSyncFailureAsync(string platform, string message, CancellationToken cancellationToken) { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "INSERT INTO SyncRuns(Platform,Status,Message,OccurredAt) VALUES($platform,'Failed',$message,$at);"; command.Parameters.AddWithValue("$platform", platform); command.Parameters.AddWithValue("$message", message); command.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("O")); await command.ExecuteNonQueryAsync(cancellationToken); }
    public async Task RecordSyncSuccessAsync(string platform, string message, CancellationToken cancellationToken) { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "INSERT INTO SyncRuns(Platform,Status,Message,OccurredAt) VALUES($platform,'Healthy',$message,$at);"; command.Parameters.AddWithValue("$platform", platform); command.Parameters.AddWithValue("$message", message); command.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("O")); await command.ExecuteNonQueryAsync(cancellationToken); }
    public async Task<string?> GetLatestSyncStatusAsync(string platform, CancellationToken cancellationToken) { await using var connection = Open(); await connection.OpenAsync(cancellationToken); var command = connection.CreateCommand(); command.CommandText = "SELECT Status || ': ' || Message || ' (' || OccurredAt || ')' FROM SyncRuns WHERE Platform=$platform ORDER BY Id DESC LIMIT 1;"; command.Parameters.AddWithValue("$platform", platform); return (string?)await command.ExecuteScalarAsync(cancellationToken); }
    public async Task DeleteSteamDataAsync(CancellationToken cancellationToken) { await using var connection = Open(); await connection.OpenAsync(cancellationToken); await using var transaction = await connection.BeginTransactionAsync(cancellationToken); var command = connection.CreateCommand(); command.Transaction = (SqliteTransaction)transaction; command.CommandText = "DELETE FROM SyncSnapshots WHERE Platform='Steam'; DELETE FROM SyncRuns WHERE Platform='Steam'; DELETE FROM Copies WHERE Platform='Steam'; DELETE FROM Games WHERE Id NOT IN (SELECT GameId FROM Copies); DELETE FROM Exclusions WHERE GameId NOT IN (SELECT Id FROM Games);"; await command.ExecuteNonQueryAsync(cancellationToken); await transaction.CommitAsync(cancellationToken); }
    private SqliteConnection Open() => new(_connectionString);
    private void Initialize()
    {
        using var connection = Open(); connection.Open(); using var transaction = connection.BeginTransaction();
        var command = connection.CreateCommand(); command.Transaction = transaction;
        command.CommandText = "CREATE TABLE IF NOT EXISTS SchemaMigrations(Version INTEGER PRIMARY KEY, AppliedAt TEXT NOT NULL); CREATE TABLE IF NOT EXISTS Games(Id TEXT PRIMARY KEY, Title TEXT NOT NULL, State TEXT NOT NULL, Excluded INTEGER NOT NULL DEFAULT 0, UpdatedAt TEXT NOT NULL); CREATE TABLE IF NOT EXISTS Copies(Platform TEXT NOT NULL, ProductId TEXT NOT NULL, GameId TEXT NOT NULL REFERENCES Games(Id), Title TEXT NOT NULL, Playtime INTEGER NOT NULL, ObservedAt TEXT NOT NULL, PRIMARY KEY(Platform,ProductId)); CREATE TABLE IF NOT EXISTS SyncSnapshots(Id INTEGER PRIMARY KEY, Platform TEXT NOT NULL, AccountId TEXT NOT NULL, ObservedAt TEXT NOT NULL, Payload TEXT NOT NULL); CREATE TABLE IF NOT EXISTS Exclusions(Id INTEGER PRIMARY KEY, GameId TEXT NOT NULL, Excluded INTEGER NOT NULL, Reason TEXT NOT NULL, ChangedAt TEXT NOT NULL); CREATE TABLE IF NOT EXISTS SyncRuns(Id INTEGER PRIMARY KEY, Platform TEXT NOT NULL, Status TEXT NOT NULL, Message TEXT NOT NULL, OccurredAt TEXT NOT NULL); INSERT OR IGNORE INTO SchemaMigrations(Version,AppliedAt) VALUES(1,$applied);";
        command.Parameters.AddWithValue("$applied", DateTimeOffset.UtcNow.ToString("O")); command.ExecuteNonQuery(); transaction.Commit();
    }
    private static async Task InsertGameAsync(SqliteConnection connection, SqliteTransaction transaction, string gameId, PlatformCopy copy, CancellationToken token) { var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = "INSERT INTO Games(Id,Title,State,Excluded,UpdatedAt) VALUES($id,$title,'Unknown',0,$at); INSERT INTO Copies(Platform,ProductId,GameId,Title,Playtime,ObservedAt) VALUES($platform,$product,$id,$title,$playtime,$at);"; command.Parameters.AddWithValue("$id", gameId); command.Parameters.AddWithValue("$title", copy.Title); command.Parameters.AddWithValue("$platform", copy.Platform); command.Parameters.AddWithValue("$product", copy.ProductId); command.Parameters.AddWithValue("$playtime", copy.PlaytimeMinutes); command.Parameters.AddWithValue("$at", copy.ObservedAt.ToString("O")); await command.ExecuteNonQueryAsync(token); }
    private static async Task UpdateCopyAsync(SqliteConnection connection, SqliteTransaction transaction, PlatformCopy copy, string gameId, CancellationToken token) { var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = "UPDATE Copies SET Title=$title,Playtime=$playtime,ObservedAt=$at WHERE Platform=$platform AND ProductId=$product; UPDATE Games SET Title=$title,UpdatedAt=$at WHERE Id=$id;"; command.Parameters.AddWithValue("$id", gameId); command.Parameters.AddWithValue("$title", copy.Title); command.Parameters.AddWithValue("$platform", copy.Platform); command.Parameters.AddWithValue("$product", copy.ProductId); command.Parameters.AddWithValue("$playtime", copy.PlaytimeMinutes); command.Parameters.AddWithValue("$at", copy.ObservedAt.ToString("O")); await command.ExecuteNonQueryAsync(token); }
}

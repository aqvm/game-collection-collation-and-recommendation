using GameLibrary.Domain;
using GameLibrary.Infrastructure;
using System.Net;
using System.Text;
using Xunit;

namespace GameLibrary.Tests;

public sealed class LibraryFoundationTests
{
    [Fact]
    public void PreferredCopyUsesPlaytimeThenPlatformPolicy()
    {
        var now = DateTimeOffset.UtcNow;
        var selected = PreferredCopyPolicy.Select([new("GOG", "1", "A", 8, now), new("Steam", "2", "A", 12, now)], new Dictionary<string, int>(), ["Steam", "GOG"]);
        Assert.Equal("Steam", selected?.Platform);
    }

    [Fact]
    public async Task ReconciliationIsIdempotentAndPurgatoryIsRecoverableAfterReopen()
    {
        var path = Path.Combine(Path.GetTempPath(), $"game-library-{Guid.NewGuid():N}.db");
        var observation = new SyncObservation("Steam", "fixture", [new("Steam", "42", "Fixture Game", 15, DateTimeOffset.UtcNow)], DateTimeOffset.UtcNow, "{sanitized:true}");
        var store = new SqliteLibraryStore(path);
        await store.SaveSnapshotAsync(observation, CancellationToken.None);
        var first = await store.ReconcileAsync(observation, null, CancellationToken.None);
        var second = await store.ReconcileAsync(observation, null, CancellationToken.None);
        Assert.Equal(1, first.Added); Assert.Equal(1, second.Unchanged);
        var game = Assert.Single(await store.SearchAsync(null, false, CancellationToken.None));
        await store.SetExcludedAsync(game.Id, true, "fixture", CancellationToken.None);
        Assert.Empty(await store.SearchAsync(null, false, CancellationToken.None));
        var reopened = new SqliteLibraryStore(path);
        Assert.True(Assert.Single(await reopened.SearchAsync(null, true, CancellationToken.None)).Excluded);
        await reopened.SetExcludedAsync(game.Id, false, null, CancellationToken.None);
        Assert.Single(await reopened.SearchAsync(null, false, CancellationToken.None));
        await reopened.RecordSyncSuccessAsync("Steam", "fixture completed", CancellationToken.None);
        Assert.Contains("Healthy", await reopened.GetLatestSyncStatusAsync("Steam", CancellationToken.None));
        await reopened.DeleteSteamDataAsync(CancellationToken.None);
        Assert.Empty(await reopened.SearchAsync(null, true, CancellationToken.None));
        Assert.Null(await reopened.GetLatestSyncStatusAsync("Steam", CancellationToken.None));
    }

    [Fact]
    public void StructuredLoggerDiscardsUnapprovedFields()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"game-library-log-{Guid.NewGuid():N}");
        new RedactedFileLogger(directory).Write("sync", new Dictionary<string, object?> { ["count"] = 2, ["apiKey"] = "do-not-log" });
        var output = File.ReadAllText(Path.Combine(directory, "game-library.ndjson"));
        Assert.Contains("count", output); Assert.DoesNotContain("do-not-log", output); Assert.DoesNotContain("apiKey", output);
    }

    [Fact]
    public async Task SteamConnectorParsesSanitizedOwnedGamesFixture()
    {
        const string fixture = "{\"response\":{\"games\":[{\"appid\":10,\"name\":\"Fixture Game\",\"playtime_forever\":42}]}}";
        using var client = new HttpClient(new FixtureHandler(fixture)) { BaseAddress = new Uri("https://api.steampowered.com/") };
        var observation = await new SteamWebApiSource(client, "test-key").GetOwnedGamesAsync("fixture-id", CancellationToken.None);
        var game = Assert.Single(observation.Copies); Assert.Equal("10", game.ProductId); Assert.Equal("Fixture Game", game.Title); Assert.Equal(42, game.PlaytimeMinutes);
    }

    private sealed class FixtureHandler(string payload) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(payload, Encoding.UTF8, "application/json") });
    }
}

using System.Text.Json;
using GameLibrary.Application;
using GameLibrary.Domain;

namespace GameLibrary.Infrastructure;

public sealed class SteamAuthorizationException : Exception
{
    public SteamAuthorizationException() : base("Steam rejected this API key.") { }
}

public sealed class SteamWebApiSource(HttpClient client, string apiKey) : ISteamSource
{
    public async Task<string> ResolveProfileAsync(string profile, CancellationToken cancellationToken)
    {
        if (ulong.TryParse(profile.Trim(), out _)) return profile.Trim();
        if (!Uri.TryCreate(profile.Trim(), UriKind.Absolute, out var uri) || !string.Equals(uri.Host, "steamcommunity.com", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Enter a Steam profile URL or a SteamID64 number.");
        var parts = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) throw new ArgumentException("That Steam profile URL is not recognized.");
        if (string.Equals(parts[0], "profiles", StringComparison.OrdinalIgnoreCase) && ulong.TryParse(parts[1], out _)) return parts[1];
        if (!string.Equals(parts[0], "id", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Use a Steam /profiles/ or /id/ profile URL.");
        using var response = await client.GetAsync($"ISteamUser/ResolveVanityURL/v0001/?key={Uri.EscapeDataString(apiKey)}&vanityurl={Uri.EscapeDataString(parts[1])}&format=json", cancellationToken);
        if (response.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden) throw new SteamAuthorizationException();
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        if (!document.RootElement.TryGetProperty("response", out var result) || !result.TryGetProperty("steamid", out var steamId)) throw new ArgumentException("Steam could not find that public profile URL.");
        return steamId.GetString() ?? throw new ArgumentException("Steam did not return an account identifier.");
    }

    public async Task<SyncObservation> GetOwnedGamesAsync(string steamId, CancellationToken cancellationToken)
    {
        var url = $"IPlayerService/GetOwnedGames/v0001/?key={Uri.EscapeDataString(apiKey)}&steamid={Uri.EscapeDataString(steamId)}&include_appinfo=1&include_played_free_games=1&format=json";
        using var response = await client.GetAsync(url, cancellationToken);
        if (response.StatusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden) throw new SteamAuthorizationException();
        response.EnsureSuccessStatusCode();
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(raw);
        var games = document.RootElement.GetProperty("response").TryGetProperty("games", out var list) ? list.EnumerateArray() : Enumerable.Empty<JsonElement>();
        var now = DateTimeOffset.UtcNow;
        var copies = games.Select(game => new PlatformCopy("Steam", game.GetProperty("appid").GetInt32().ToString(System.Globalization.CultureInfo.InvariantCulture), game.GetProperty("name").GetString() ?? "Untitled game", game.TryGetProperty("playtime_forever", out var minutes) ? minutes.GetInt32() : 0, now)).ToList();
        return new SyncObservation("Steam", steamId, copies, now, raw);
    }
}

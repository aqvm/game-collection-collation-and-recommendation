using GameLibrary.Application;
using GameLibrary.Domain;
using GameLibrary.Infrastructure;

namespace GameLibrary.Desktop;

#pragma warning disable CA1001 // The page cancels/disposes the operation after each sync; framework page lifetime is not IDisposable.
public sealed class MainPage : Page
{
    private readonly SqliteLibraryStore _store;
    private readonly WindowsCredentialVault _vault = new();
    private readonly ListView _library = new() { SelectionMode = ListViewSelectionMode.Single, IsTabStop = true, TabNavigation = KeyboardNavigationMode.Local };
    private readonly TextBlock _detail = new() { TextWrapping = TextWrapping.Wrap };
    private readonly TextBlock _activity = new() { TextWrapping = TextWrapping.Wrap };
    private readonly TextBox _profile = new() { PlaceholderText = "Steam profile URL (or SteamID64)" };
    private readonly PasswordBox _apiKey = new() { PlaceholderText = "Steam Web API key" };
    private readonly CheckBox _includeExcluded = new() { Content = "Include purgatory" };
    private readonly CheckBox _termsAcknowledged = new() { Content = "I understand this is my personal Steam API key and that I must follow Steam’s API Terms." };
    private readonly ComboBox _state = new() { ItemsSource = Enum.GetNames<LifecycleState>() };
    private readonly Grid _libraryView = new();
    private readonly ScrollViewer _connectionsView = new() { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Visibility = Visibility.Collapsed };
    private IReadOnlyList<Game> _games = [];
    private CancellationTokenSource? _syncCancellation;

    public MainPage()
    {
        var dataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GameLibrary", "library.db");
        _store = new SqliteLibraryStore(dataPath);
        var title = new TextBlock { Text = "Game Library", FontSize = 32, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        var search = new AutoSuggestBox { PlaceholderText = "Search your cached library", QueryIcon = new SymbolIcon(Symbol.Find) };
        var sync = new Button { Content = "Save key and sync Steam" }; var cancel = new Button { Content = "Cancel sync" }; var disconnect = new Button { Content = "Disconnect Steam…" }; var exclude = new Button { Content = "Send to purgatory" }; var restore = new Button { Content = "Restore" }; var saveState = new Button { Content = "Save lifecycle state" };
        sync.Click += async (_, _) => await SyncAsync(); cancel.Click += (_, _) => _syncCancellation?.Cancel(); disconnect.Click += async (_, _) => await DisconnectSteamAsync(); exclude.Click += async (_, _) => await SetExcludedAsync(true); restore.Click += async (_, _) => await SetExcludedAsync(false); saveState.Click += async (_, _) => await SaveStateAsync();
        search.TextChanged += async (_, _) => await LoadLibraryAsync(search.Text); _includeExcluded.Checked += async (_, _) => await LoadLibraryAsync(search.Text); _includeExcluded.Unchecked += async (_, _) => await LoadLibraryAsync(search.Text);
        _library.SelectionChanged += async (_, _) => await ShowDetailAsync();

        var credentials = new StackPanel { Spacing = 8 }; credentials.Children.Add(new TextBlock { Text = "Steam connection", FontSize = 20, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold }); credentials.Children.Add(new TextBlock { Text = "Steam uses an API key like a private pass that lets this app request your library.\n\n1. In a web browser, visit steamcommunity.com/dev/apikey and sign in to Steam.\n2. Read Steam’s terms. Steam asks you to associate a domain with your key; use information accurate for your own registration. Game Library cannot choose this for you.\n3. Copy the key and paste it here once. It is masked while typing and saved in Windows Credential Manager. This app never displays it again.", TextWrapping = TextWrapping.Wrap }); credentials.Children.Add(_termsAcknowledged); credentials.Children.Add(_profile); credentials.Children.Add(_apiKey); credentials.Children.Add(sync); credentials.Children.Add(cancel); credentials.Children.Add(disconnect);
        credentials.Children.Add(new TextBlock { Text = "Steam data is provided as is and as available; it can be incomplete, delayed, unavailable, or inaccurate. Game Library is not affiliated with or endorsed by Valve or Steam.", TextWrapping = TextWrapping.Wrap, Opacity = 0.72 });
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 }; actions.Children.Add(exclude); actions.Children.Add(restore); actions.Children.Add(_state); actions.Children.Add(saveState);
        var left = new Grid { RowSpacing = 12 }; left.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); left.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); left.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); left.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); Grid.SetRow(search, 1); Grid.SetRow(_includeExcluded, 2); Grid.SetRow(_library, 3); Grid.SetRow(actions, 4); left.Children.Add(title); left.Children.Add(search); left.Children.Add(_includeExcluded); left.Children.Add(_library); left.Children.Add(actions);
        var detailPanel = new StackPanel { Spacing = 12, Padding = new Thickness(16), Background = new SolidColorBrush(Windows.UI.Color.FromArgb(20, 127, 127, 127)) }; detailPanel.Children.Add(new TextBlock { Text = "Selected game", FontSize = 20, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold }); detailPanel.Children.Add(_detail);
        _libraryView.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); _libraryView.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(340) }); var detailScroll = new ScrollViewer { Content = detailPanel, VerticalScrollBarVisibility = ScrollBarVisibility.Auto }; Grid.SetColumn(detailScroll, 1); _libraryView.Children.Add(left); _libraryView.Children.Add(detailScroll);
        var connectionPanel = new StackPanel { Spacing = 12, Padding = new Thickness(24), MaxWidth = 640 }; connectionPanel.Children.Add(new TextBlock { Text = "Connections", FontSize = 32, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold }); connectionPanel.Children.Add(credentials); connectionPanel.Children.Add(new TextBlock { Text = "Activity", FontSize = 20, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold }); connectionPanel.Children.Add(_activity); _connectionsView.Content = connectionPanel;
        var libraryTab = new ToggleButton { Content = "Library", IsChecked = true }; var connectionsTab = new ToggleButton { Content = "Connections" }; libraryTab.Click += (_, _) => { libraryTab.IsChecked = true; connectionsTab.IsChecked = false; _libraryView.Visibility = Visibility.Visible; _connectionsView.Visibility = Visibility.Collapsed; }; connectionsTab.Click += (_, _) => { libraryTab.IsChecked = false; connectionsTab.IsChecked = true; _libraryView.Visibility = Visibility.Collapsed; _connectionsView.Visibility = Visibility.Visible; };
        var navigation = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Padding = new Thickness(24, 16, 24, 8) }; navigation.Children.Add(libraryTab); navigation.Children.Add(connectionsTab);
        var root = new Grid { Padding = new Thickness(24) }; root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); Grid.SetRow(_libraryView, 1); Grid.SetRow(_connectionsView, 1); root.Children.Add(navigation); root.Children.Add(_libraryView); root.Children.Add(_connectionsView); Content = root;
        Loaded += async (_, _) => { await LoadLibraryAsync(); var health = await _store.GetLatestSyncStatusAsync("Steam", CancellationToken.None); var key = await _vault.GetAsync("GameLibrary.Steam.WebApiKey", CancellationToken.None); _activity.Text = $"Steam: {(health ?? "not connected yet.")}\nAPI key: {(key is null ? "not saved" : "saved securely")}"; };
    }

    private async Task SyncAsync()
    {
        if (!_termsAcknowledged.IsChecked.GetValueOrDefault()) { _activity.Text = "Please acknowledge Steam’s API terms before connecting."; return; }
        if (string.IsNullOrWhiteSpace(_profile.Text)) { _activity.Text = "Enter your Steam profile URL. The key remains masked and is never displayed after saving."; return; }
        try
        {
            var key = _apiKey.Password;
            if (!string.IsNullOrWhiteSpace(key)) await _vault.StoreAsync("GameLibrary.Steam.WebApiKey", key, CancellationToken.None);
            else key = await _vault.GetAsync("GameLibrary.Steam.WebApiKey", CancellationToken.None) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key)) { _activity.Text = "Paste a newly created Steam Web API key to connect for the first time."; return; }
            using var client = new HttpClient { BaseAddress = new Uri("https://api.steampowered.com/") };
            var source = new SteamWebApiSource(client, key); var steamId = await source.ResolveProfileAsync(_profile.Text, CancellationToken.None); var service = new LibraryService(_store, source);
            var progress = new Progress<SyncProgress>(item => _activity.Text = $"{item.Phase}: {item.Completed}/{item.Total}\n{item.Message}");
            _syncCancellation = new CancellationTokenSource(); var result = await service.SyncSteamAsync(steamId, progress, _syncCancellation.Token);
            _apiKey.Password = string.Empty; _activity.Text = $"Completed: {result.Added} added, {result.Updated} updated, {result.Unchanged} unchanged. Cached data remains available offline."; await LoadLibraryAsync();
        }
        catch (OperationCanceledException) { _activity.Text = "Sync cancelled safely. Your prior cached library remains available."; }
        catch (SteamAuthorizationException) { _activity.Text = "Steam rejected the saved key. It may have been revoked, expired, or entered incorrectly. Your cached library is safe. Remove the saved key, create a new one at steamcommunity.com/dev/apikey, and paste it here."; }
        catch (Exception exception) { _activity.Text = $"Sync failed safely: {exception.Message}\nYour prior cached library was not changed. Select Connect and sync Steam to retry."; }
        finally { _syncCancellation?.Dispose(); _syncCancellation = null; }
    }

    private async Task DisconnectSteamAsync()
    {
        var removeKey = new CheckBox { Content = "Remove saved API key", IsChecked = true }; var removeData = new CheckBox { Content = "Delete cached Steam observations, copies, and games", IsChecked = false };
        var content = new StackPanel { Spacing = 8 }; content.Children.Add(new TextBlock { Text = "Choose what to remove. Your choices cannot be undone.", TextWrapping = TextWrapping.Wrap }); content.Children.Add(removeKey); content.Children.Add(removeData);
        var dialog = new ContentDialog { XamlRoot = XamlRoot, Title = "Disconnect Steam?", Content = content, PrimaryButtonText = "Disconnect", CloseButtonText = "Cancel", DefaultButton = ContentDialogButton.Close };
        if (await dialog.ShowAsync() == ContentDialogResult.Primary) { if (removeKey.IsChecked == true) await _vault.RemoveAsync("GameLibrary.Steam.WebApiKey", CancellationToken.None); if (removeData.IsChecked == true) await _store.DeleteSteamDataAsync(CancellationToken.None); _apiKey.Password = string.Empty; _activity.Text = "Steam disconnected. Selected local data was removed."; await LoadLibraryAsync(); }
    }

    private async Task LoadLibraryAsync(string? query = null)
    {
        _games = await _store.SearchAsync(query, _includeExcluded.IsChecked == true, CancellationToken.None);
        _library.ItemsSource = _games.Select(game => $"{game.Title}  •  {game.State}{(game.Excluded ? "  •  PURGATORY" : string.Empty)}").ToArray();
        if (_games.Count == 0) _detail.Text = "No cached games yet. Connect Steam to import, then this library works without a network connection.";
    }

    private async Task ShowDetailAsync()
    {
        if (_library.SelectedIndex < 0 || _library.SelectedIndex >= _games.Count) return; var game = _games[_library.SelectedIndex]; var copies = await _store.GetCopiesAsync(game.Id, CancellationToken.None);
        var preferred = PreferredCopyPolicy.Select(copies, copies.GroupBy(copy => copy.Platform).ToDictionary(group => group.Key, group => group.Sum(copy => copy.PlaytimeMinutes)), ["Steam"]);
        var copySummary = copies.Count == 0 ? "No platform copies were recorded for this game." : string.Join("\n", copies.Select(copy => $"{copy.Platform}: {FormatPlaytime(copy.PlaytimeMinutes)}"));
        _detail.Text = $"{game.Title}\nLifecycle: {game.State}\n{(game.Excluded ? "In purgatory" : "Active library")}\n\nCopies:\n{copySummary}\n\nPreferred copy: {preferred?.Platform ?? "None"} (highest playtime, then platform policy).";
    }

    private async Task SetExcludedAsync(bool excluded)
    {
        if (_library.SelectedIndex < 0 || _library.SelectedIndex >= _games.Count) { _activity.Text = "Select a game first."; return; }
        await _store.SetExcludedAsync(_games[_library.SelectedIndex].Id, excluded, excluded ? "User action" : null, CancellationToken.None); _activity.Text = excluded ? "Moved to purgatory. It is removed from normal browsing." : "Restored to the active library."; await LoadLibraryAsync();
    }

    private async Task SaveStateAsync()
    {
        if (_library.SelectedIndex < 0 || _library.SelectedIndex >= _games.Count || _state.SelectedItem is not string name) { _activity.Text = "Select a game and lifecycle state first."; return; }
        await _store.SetLifecycleStateAsync(_games[_library.SelectedIndex].Id, Enum.Parse<LifecycleState>(name), CancellationToken.None); _activity.Text = "Lifecycle correction saved locally."; await LoadLibraryAsync();
    }

    private static string FormatPlaytime(int minutes)
    {
        var duration = TimeSpan.FromMinutes(minutes);
        if (minutes < 60) return $"{minutes} minutes";
        if (duration.TotalHours < 24) return $"{duration.Hours}h {duration.Minutes}m";
        return $"{Math.Floor(duration.TotalDays)} days {duration.Hours}h ({duration.TotalHours:0.#} hours)";
    }
}
#pragma warning restore CA1001

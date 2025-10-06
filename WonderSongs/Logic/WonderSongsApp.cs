namespace WonderSongs.Logic;

static class WonderSongsApp
{
    public static async IAsyncEnumerable<StorageFolder> GetSongsFolderHistoryAsync()
    {
        var historyAsObject = ApplicationData.Current.LocalSettings.Values["History"];
        if (historyAsObject is string historyString)
        {
            foreach (var item in historyString.Split("\n"))
            {
                StorageFolder? folder = null;
                try
                {
                    folder = await StorageFolder.GetFolderFromPathAsync(item);
                }
                catch
                {

                }
                if (folder is not null)
                {
                    yield return folder;
                }
            }
        }
    }
    public static async Task<WonderSongsPlayable> OpenFromCollectionAsync(StorageFolder directory)
    {
        // Update history: move current directory to first position
        var historyKey = "History";
        var localSettings = ApplicationData.Current.LocalSettings;
        var dirPath = directory.Path;

        // Get current history as list
        var historyAsObject = localSettings.Values[historyKey];
        List<string> historyList = new();
        if (historyAsObject is string historyString)
        {
            historyList = historyString.Split('\n')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            // Remove current directory if exists
            historyList.RemoveAll(p => string.Equals(p, dirPath, StringComparison.OrdinalIgnoreCase));
        }
        // Insert current directory at the top
        historyList.Insert(0, dirPath);

        // Save updated history
        localSettings.Values[historyKey] = string.Join('\n', historyList);

        var files = await directory.GetFilesAsync();
        var audioFiles = files.Where(f =>
            f.ContentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
            f.ContentType.StartsWith("music/", StringComparison.OrdinalIgnoreCase)
        ).ToArray();

        var songTasks = audioFiles.Select(Song.CreateAsync).ToArray();
        var songs = await Task.WhenAll(songTasks);

        return new WonderSongsPlayable(songs);
    }
}

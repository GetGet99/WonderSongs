#if ANDROID
using System.Reflection;
using Android.Content;
using WonderSongs.Droid;
#endif

namespace WonderSongs.Core;

static class WonderSongsApp
{
#if ANDROID
    static MethodInfo androidFolderGetter { get; } = typeof(StorageFolder).GetMethod("GetFromSafUri", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
    static StorageFolder? GetAndroidFolder(string folderUri)
    {
        var uri = Android.Net.Uri.Parse(folderUri);
        if (uri is null) return null;
        return androidFolderGetter.Invoke(null, [uri]) as StorageFolder;
    }
#endif
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
#if ANDROID
                    folder = GetAndroidFolder(item);
#else
                    folder = await StorageFolder.GetFolderFromPathAsync(item);
#endif
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
#if ANDROID
        var dirPath = StorageExtensions.TryGetAndroidUri(directory).ToString() ?? "";
#else
        var dirPath = directory.Path;
#endif

        // Get current history as list
        var historyAsObject = localSettings.Values[historyKey];
        List<string> historyList = new();
        if (historyAsObject is string historyString)
        {
            historyList = historyString.Split('\n')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            // Remove current directory if exists
            if (historyList.RemoveAll(p => string.Equals(p, dirPath, StringComparison.OrdinalIgnoreCase)) is 0)
            {
                // new folder
#if ANDROID
                ContextWrapper c = new(Android.App.Application.Context);
                c.ContentResolver?.TakePersistableUriPermission(Android.Net.Uri.Parse(dirPath)!, ActivityFlags.GrantReadUriPermission);
#endif
            }
        } else
        {
            // new folder
#if ANDROID
                ContextWrapper c = new(Android.App.Application.Context);
                c.ContentResolver?.TakePersistableUriPermission(Android.Net.Uri.Parse(dirPath)!, ActivityFlags.GrantReadUriPermission);
#endif
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

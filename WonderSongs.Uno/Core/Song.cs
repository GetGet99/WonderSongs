namespace WonderSongs.Core;

class Song
{
    private Song(StorageFile File)
    {
        this.File = File;
    }
    StorageFile File { get; }
    public required string Title { get; init; }
    /// <remarks>Can be empty string</remarks>
    public required string Artist { get; init; }
    public static async Task<Song> CreateAsync(StorageFile sf)
    {
#if HAS_UNO
        var title = default(string);
        var artist = "";
#else
        var props = await sf.Properties.GetMusicPropertiesAsync();
        var title = props.Title;
        var artist = props.Artist ?? "";
#endif
        if (string.IsNullOrWhiteSpace(title))
        {
            var dot = sf.Name.LastIndexOf('.');
            if (dot >= 0)
                title = sf.Name[..dot];
            else
                title = sf.Name;
        }
        return new Song(sf)
        {
            Title = title,
            Artist = artist
        };
    }
#if ANDROID
    public Task<StorageFile> GetSourceAsync()
    {
        return Task.FromResult(File);
    }
#elif DESKTOP
    public Task<MediaSource> GetSourceAsync()
    {
        Debug.WriteLine(File.Path);
        Debug.WriteLine(new Uri(File.Path).ToString());
        return Task.FromResult(MediaSource.CreateFromUri(new(File.Path)));
    }
#else
    public Task<MediaSource> GetSourceAsync()
    {
        return Task.FromResult(MediaSource.CreateFromStorageFile(File));
    }
#endif
}

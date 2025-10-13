namespace WonderSongs.Core;

class Song
{
    private Song(StorageFile File)
    {
        this.File = File;
    }
    StorageFile File { get; }
    public required string Title { get; init; }
    public static async Task<Song> CreateAsync(StorageFile sf)
    {
#if HAS_UNO
        var title = default(string);
#else
        var props = await sf.Properties.GetMusicPropertiesAsync();
        var title = props.Title;
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
            Title = title
        };
    }
#if ANDROID
    public Task<StorageFile> GetSourceAsync()
    {
        return Task.FromResult(File);
        //var uri = File.TryGetAndroidUri();
        //return Task.FromResult(MediaSource.CreateFromUri(new(uri!.ToString()!)));
    }
#else
    public Task<MediaSource> GetSourceAsync()
    {
        return Task.FromResult(MediaSource.CreateFromStorageFile(File));
    }
#endif
}

namespace WonderSongs.Logic;

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
        var props = await sf.Properties.GetMusicPropertiesAsync();
        var title = props.Title;
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
    public Task<MediaSource> GetSourceAsync()
    {
        return Task.FromResult(MediaSource.CreateFromStorageFile(File));
    }
}
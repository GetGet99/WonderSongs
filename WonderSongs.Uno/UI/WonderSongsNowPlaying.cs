using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    using WonderSongs.Core;
    private Song CurrentSong = `default!`;

    <root Padding=12
          RowDefinitions=<>
            <RowDefinition Height=`GridLength.Auto` />
            <RowDefinition Height=`GridLength.Auto` />
          </>
    >
        <TextBlock Text="Now Playing"
                   FontSize=14
                   Opacity=0.7 />

        <TextBlock Grid.Row=1
                   Text=`CurrentSong?.Title ?? string.Empty`
                   FontSize=20
                 //    FontWeight=`FontWeights.SemiBold`
                   TextWrapping=Wrap />
    </root>
    """)]
partial class WonderSongsNowPlaying : Grid
{
    public WonderSongsNowPlaying(Song? song = null)
    {
        Init();
        Song = song;
    }

    public Song? Song
    {
        get => CurrentSong;
        set => CurrentSong = value!;
    }
}

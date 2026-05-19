using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root Padding=12
          RowDefinitions=<>
            <RowDefinition Height=`GridLength.Auto` />
            <RowDefinition Height=`GridLength.Auto` />
          </>
    >
        <TextBlock Text="Now Playing"
                   FontSize=14
                   Opacity=0.7 />

        <TextBlock Grid_Row=1
                   Text=`s.Title`
                   FontSize=20
                //    FontWeight=`FontWeights.SemiBold`
                   TextWrapping=Wrap />
    </root>
    """)]
partial class WonderSongsNowPlaying : Grid
{
    Song s;
    public WonderSongsNowPlaying(Song s)
    {
        this.s = s;
        Init();
    }
}

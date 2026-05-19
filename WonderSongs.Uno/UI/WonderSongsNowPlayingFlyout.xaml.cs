#if WINAPPSDK_PACKAGED
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root IsBackdropEnabled BackdropKind=Acrylic Placement=Top !HideOnLostFocus>
        <TrayIconFlyoutIsland>
            contentHost = <Grid Margin=16 />
        </TrayIconFlyoutIsland>
    </root>
    """)]
partial class WonderSongsNowPlayingFlyout : TrayIconFlyout
{
    bool deferShow;

    public WonderSongsNowPlayingFlyout()
    {
        Init();
        Loaded += delegate
        {
            if (deferShow)
            {
                Show();
                deferShow = false;
            }
        };
    }

    public void ShowSong(Song song)
    {
        if (!DispatcherQueue.HasThreadAccess)
        {
            DispatcherQueue.TryEnqueue(() => ShowSong(song));
            return;
        }

        contentHost.Children.Clear();
        contentHost.Children.Add(new WonderSongsNowPlaying(song));

        Show();
        if (!IsOpen)
            deferShow = true;
    }
}
#endif

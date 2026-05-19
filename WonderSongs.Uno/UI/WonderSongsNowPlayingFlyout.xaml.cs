#if WINAPPSDK_PACKAGED
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root IsBackdropEnabled BackdropKind=Acrylic Placement=RightEdgeAlignedTop !HideOnLostFocus ActivationMode=NeverActivate>
        <TrayIconFlyoutIsland>
            nowPlaying = <WonderSongsNowPlaying Margin=8 />
        </TrayIconFlyoutIsland>
    </root>
    """)]
partial class WonderSongsNowPlayingFlyout : TrayIconFlyout
{
    bool deferShow;
    int showVersion;

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

        nowPlaying.Song = song;

        Show();
        if (!IsOpen)
            deferShow = true;

        var version = ++showVersion;
        _ = HideAfterDelayAsync(version);
    }

    async Task HideAfterDelayAsync(int version)
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        if (!DispatcherQueue.HasThreadAccess)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (version == showVersion)
                    Hide();
            });
            return;
        }

        if (version == showVersion)
            Hide();
    }
}
#endif

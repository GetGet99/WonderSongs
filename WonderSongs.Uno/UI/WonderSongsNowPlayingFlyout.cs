#if WINAPPSDK_PACKAGED
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <TrayIconFlyout IsBackdropEnabled BackdropKind=Acrylic Placement=TopEdgeAlignedRight !HideOnLostFocus ActivationMode=NeverActivate PopupDirection=RightToLeft>
        <TrayIconFlyoutIsland>
            nowPlaying = <WonderSongsNowPlaying Margin=8 />
        </TrayIconFlyoutIsland>
    </TrayIconFlyout>
    """)]
partial class WonderSongsNowPlayingFlyout : IQuickMarkupComponent<TrayIconFlyout>
{
    bool deferShow;
    int showVersion;
    
    public WonderSongsNowPlayingFlyout()
    {
        Init();
        MarkupNode.Loaded += delegate
        {
            if (deferShow)
            {
                MarkupNode.Show();
                deferShow = false;
            }
        };
    }

    public void ShowSong(Song song)
    {
        if (!MarkupNode.DispatcherQueue.HasThreadAccess)
        {
            MarkupNode.DispatcherQueue.TryEnqueue(() => ShowSong(song));
            return;
        }

        nowPlaying.Song = song;

        MarkupNode.Show();
        if (!MarkupNode.IsOpen)
            deferShow = true;

        var version = ++showVersion;
        _ = HideAfterDelayAsync(version);
    }

    async Task HideAfterDelayAsync(int version)
    {
        await Task.Delay(TimeSpan.FromSeconds(7));
        if (!MarkupNode.DispatcherQueue.HasThreadAccess)
        {
            MarkupNode.DispatcherQueue.TryEnqueue(() =>
            {
                if (version == showVersion)
                    MarkupNode.Hide();
            });
            return;
        }

        if (version == showVersion)
            MarkupNode.Hide();
    }
}
#endif

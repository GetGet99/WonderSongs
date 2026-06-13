#if !HAS_UNO
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <TrayIconFlyout IsBackdropEnabled BackdropKind=Acrylic>
        <TrayIconFlyoutIsland Height=300>
            <WonderSongsMediaControlUI(`playable`) SmallMode />
        </TrayIconFlyoutIsland>
    </TrayIconFlyout>
    """)]
partial class WonderSongsTrayIconFlyout : IQuickMarkupComponent<TrayIconFlyout>
{
    WonderSongsPlayable playable;
    public WonderSongsTrayIconFlyout(WonderSongsPlayable playable)
    {
        App.CurrentTray = this;
        this.playable = playable;
        Init();
    }
}
#endif

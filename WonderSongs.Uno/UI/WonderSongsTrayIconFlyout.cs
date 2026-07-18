#if !HAS_UNO || DESKTOP
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <DesktopFlyout IsBackdropEnabled BackdropKind=DesktopAcrylic>
        <DesktopFlyoutIsland Height=300>
            <WonderSongsMediaControlUI(`playable`) SmallMode />
        </DesktopFlyoutIsland>
    </DesktopFlyout>
    """)]
partial class WonderSongsTrayIconFlyout : IQuickMarkupComponent<DesktopFlyout>
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

#if !HAS_UNO
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <DesktopFlyout IsBackdropEnabled BackdropKind=DesktopAcrylic>
        <DesktopFlyoutIsland Height=300>
            <WonderSongsMediaControlUI(`playable`) SmallMode />
        </DesktopFlyoutIsland>
    </DesktopFlyout>
    """)]
partial class WonderSongsDesktopFlyout : IQuickMarkupComponent<DesktopFlyout>
{
    WonderSongsPlayable playable;
    public WonderSongsDesktopFlyout(WonderSongsPlayable playable)
    {
        App.CurrentTray = this;
        this.playable = playable;
        Init();
    }
}
#endif

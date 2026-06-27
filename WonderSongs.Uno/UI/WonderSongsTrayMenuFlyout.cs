#if !HAS_UNO
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <DesktopMenuFlyout>
        <MenuFlyoutItem Text="Exit" Icon=<SymbolIcon Symbol=Cancel /> @Click+=`Environment.Exit(0)` />
    </DesktopMenuFlyout>
    """)]
partial class WonderSongsTrayMenuFlyout : IQuickMarkupComponent<DesktopMenuFlyout>;
#endif

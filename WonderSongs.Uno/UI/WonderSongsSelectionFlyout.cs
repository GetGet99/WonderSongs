#if WINAPPSDK_PACKAGED || DESKTOP
using System.Reflection;
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <DesktopFlyout IsBackdropEnabled BackdropKind=DesktopAcrylic Placement=TopCenter !HideOnLostFocus ActivationMode=NoActivateOnOpen>
        <DesktopFlyoutIsland>
            page = <WonderSongsSelectionPage(`playable`) Margin=16 ShowFocusHint />
        </DesktopFlyoutIsland>
    </DesktopFlyout>
    """)]
partial class WonderSongsSelectionFlyout : IQuickMarkupComponent<DesktopFlyout>
{
    WonderSongsPlayable playable;
    public WonderSongsSelectionFlyout(WonderSongsPlayable playable)
    {
        this.playable = playable;
        Init();

        bool deferShow = false;
        page.NextSongAvaliable += async () =>
        {
            var a = MarkupNode.IsLoaded;
            MarkupNode.Show();
            if (!MarkupNode.IsOpen) deferShow = true;
        };
        MarkupNode.Loaded += delegate
        {
            if (deferShow)
            {
                MarkupNode.Show();
            }
        };

        page.NextSongSelected += MarkupNode.Hide;
#if WINAPPSDK_PACKAGED
        KeyboardHook.Instance.ModifierPressed += delegate
        {
            if (MarkupNode.IsOpen)
            {
                WinWrapper.Windowing.Window.FromWindowHandle(
                    (nint)page.XamlRoot.ContentIslandEnvironment.AppWindowId.Value
                ).SetAsForegroundWindow();
                MarkupNode.NavigateFocus(XamlSourceFocusNavigationReason.Programmatic);
                return true;
            }
            return false;
        };
#endif
    }
    public Task<Song> SelectNextSongAsync() => page.SelectNextSongAsync();
}
#endif

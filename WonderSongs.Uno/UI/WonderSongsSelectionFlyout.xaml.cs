#if WINAPPSDK_PACKAGED
using System.Reflection;
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root IsBackdropEnabled BackdropKind=Acrylic Placement=Top !HideOnLostFocus ActivationMode=NoActivateOnOpen>
        <TrayIconFlyoutIsland>
            page = <WonderSongsSelectionPage(`playable`) Margin=16 ShowFocusHint />
        </TrayIconFlyoutIsland>
    </root>
    """)]
partial class WonderSongsSelectionFlyout : TrayIconFlyout
{
    WonderSongsPlayable playable;
    public WonderSongsSelectionFlyout(WonderSongsPlayable playable)
    {
        this.playable = playable;
        Init();

        bool deferShow = false;
        page.NextSongAvaliable += async () =>
        {
            var a = this.IsLoaded;
            Show();
            if (!IsOpen) deferShow = true;
        };
        Loaded += delegate
        {
            if (deferShow)
            {
                Show();
            }
        };

        page.NextSongSelected += Hide;
        var hostField = typeof(TrayIconFlyout).GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance);
        var field = hostField!.GetValue(this);
        var method = field!.GetType().GetMethod("NavigateFocus", BindingFlags.NonPublic | BindingFlags.Instance);
        void NavigateFocus()
        {
            method!.Invoke(field, []);
        }
        KeyboardHook.Instance.ModifierPressed += delegate
        {
            if (IsOpen)
            {
                WinWrapper.Windowing.Window.FromWindowHandle(
                    (nint)page.XamlRoot.ContentIslandEnvironment.AppWindowId.Value
                ).SetAsForegroundWindow();
                NavigateFocus();
                return true;
            }
            return false;
        };
    }
    public Task<Song> SelectNextSongAsync() => page.SelectNextSongAsync();
}
#endif

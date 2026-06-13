#if WINAPPSDK_PACKAGED
using System.Reflection;
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <TrayIconFlyout IsBackdropEnabled BackdropKind=Acrylic Placement=Top !HideOnLostFocus ActivationMode=NoActivateOnOpen>
        <TrayIconFlyoutIsland>
            page = <WonderSongsSelectionPage(`playable`) Margin=16 ShowFocusHint />
        </TrayIconFlyoutIsland>
    </TrayIconFlyout>
    """)]
partial class WonderSongsSelectionFlyout : IQuickMarkupComponent<TrayIconFlyout>
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
        var hostField = typeof(TrayIconFlyout).GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance);
        var field = hostField!.GetValue(MarkupNode);
        var method = field!.GetType().GetMethod("NavigateFocus", BindingFlags.NonPublic | BindingFlags.Instance);
        void NavigateFocus()
        {
            method!.Invoke(field, []);
        }
        KeyboardHook.Instance.ModifierPressed += delegate
        {
            if (MarkupNode.IsOpen)
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

#if WINAPPSDK_PACKAGED
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root IsBackdropEnabled BackdropKind=Acrylic Placement=Top !HideOnLostFocus ActivationMode=NoActivateOnOpen>
        <TrayIconFlyoutIsland>
            page = <WonderSongsSelectionPage(`playable`) Margin=16 />
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
        KeyboardHook.Instance.ModifierPressed += delegate
        {
            if (IsOpen)
            {
                page.Focus(FocusState.Keyboard);
                return true;
            }
            return false;
        };
    }
    public Task<Song> SelectNextSongAsync() => page.SelectNextSongAsync();
}
#endif

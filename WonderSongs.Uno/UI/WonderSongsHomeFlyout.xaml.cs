#if WINAPPSDK_PACKAGED
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root IsBackdropEnabled BackdropKind=Acrylic Placement=Top !HideOnLostFocus>
        <TrayIconFlyoutIsland Width=500>
            homePage = <WonderSongsHomePage Margin=`new(16,32,16,32)` />
        </TrayIconFlyoutIsland>
    </root>
    """)]
partial class WonderSongsHomeFlyout : TrayIconFlyout
{
    public WonderSongsHomeFlyout()
    {
        Init();
        bool a = false;
        homePage.UIInitialized += () =>
        {
            if (a) Show();
            a = true;
        };
        Loaded += delegate
        {
            if (a) Show();
            a = true;
        };
        homePage.OnSelected += Hide;
        homePage.OnSuccess += (playable, swa) =>
        {
            Hide();
            if (swa)
            {
                var selection = new SingleWindowPage(playable);
                new WonderSongsWindow() { Content = selection }.AppWindow.Show();
            }
            else
            {
                Action<Song> newSongPlaying = delegate { };

#if WINAPPSDK_PACKAGED
                var selection = new WonderSongsSelectionFlyout(playable);
                var nowPlaying = new WonderSongsNowPlayingFlyout();
                newSongPlaying = nowPlaying.ShowSong;
                var tray = new WonderSongsTrayIconFlyout(playable);

                var trayIcon = new SystemTrayIcon(
                    @"D:\Programming\VS\WonderSongs.Uno\WonderSongs.Uno\Assets\icon.ico",
                    "WonderSongs",
                    Guid.NewGuid()
                );
                trayIcon.LeftClicked += Icon_LeftClicked;
                trayIcon.IsVisible = true;
                trayIcon.Show();
                App.TrayIcon = trayIcon;

                void Icon_LeftClicked(object? sender, MouseEventReceivedEventArgs e)
                {
                    if (tray.IsOpen)
                        // this branch is never taken
                        tray.Hide();
                    else
                        // this is called, but RootGrid is null so it does nothing
                        tray.Show();
                }
#else
                var selection = new WonderSongsSelectionWindow(playable);
#endif
                playable.LifeCycle(selection.SelectNextSongAsync, newSongPlaying);
            }
        };
    }
}
#endif

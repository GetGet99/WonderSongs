#if WINAPPSDK_PACKAGED || DESKTOP
using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <DesktopFlyout IsBackdropEnabled BackdropKind=DesktopAcrylic Placement=TopCenter !HideOnLostFocus>
        <DesktopFlyoutIsland Width=500>
            homePage = <WonderSongsHomePage Margin=`new(16,32,16,32)` />
        </DesktopFlyoutIsland>
    </DesktopFlyout>
    """)]
partial class WonderSongsHomeFlyout : IQuickMarkupComponent<DesktopFlyout>
{
    public WonderSongsHomeFlyout()
    {
        Init();
        bool a = false;
        homePage.UIInitialized += () =>
        {
            if (a) MarkupNode.Show();
            a = true;
        };
        MarkupNode.Loaded += delegate
        {
            if (a) MarkupNode.Show();
            a = true;
        };
        homePage.OnSelected += MarkupNode.Hide;
        homePage.OnSuccess += (playable, swa) =>
        {
            MarkupNode.Hide();
            if (swa)
            {
                var selection = new SingleWindowPage(playable);
                new WonderSongsWindow() { Content = selection }.AppWindow.Show();
            }
            else
            {
                Action<Song> newSongPlaying = delegate { };

#if WINAPPSDK_PACKAGED || DESKTOP
                var selection = new WonderSongsSelectionFlyout(playable);
                var nowPlaying = new WonderSongsNowPlayingFlyout();
                newSongPlaying = nowPlaying.ShowSong;
                var tray = new WonderSongsTrayIconFlyout(playable);

                var trayIcon = new SystemTrayIcon(
                    @"D:\Programming\VS\WonderSongs.Uno\WonderSongs.Uno\Assets\wondersongs.ico",
                    "WonderSongs",
                    Guid.NewGuid()
                );
                trayIcon.LeftClicked += Icon_LeftClicked;
                trayIcon.RightClicked += Icon_LeftClicked;
                trayIcon.IsVisible = true;
                trayIcon.Show();
                App.TrayIcon = trayIcon;
                App.CurrentTrayMenuFlyout = new();

                void Icon_LeftClicked(object? sender, MouseEventReceivedEventArgs e)
                {
                    //if (tray.MarkupNode.IsOpen)
                    //    // this branch is never taken
                    //    tray.MarkupNode.Hide();
                    //else
                    //    // this is called, but RootGrid is null so it does nothing
                    //    tray.MarkupNode.Show();
                    App.CurrentTrayMenuFlyout.MarkupNode.Show(e.Point);
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

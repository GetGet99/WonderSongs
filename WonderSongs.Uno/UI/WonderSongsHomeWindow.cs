using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

partial class WonderSongsHomeWindow : WonderSongsWindow
{
    public WonderSongsHomeWindow()
    {
#if !HAS_UNO
        var width = 500; var height = 400;
        var primary = DisplayArea.GetFromPoint(default, DisplayAreaFallback.Primary);
        AppWindow.Resize(new()
        {
            Width = width,
            Height = height
        });
        AppWindow.MoveAndResize(new()
        {
            X = (int)((primary.WorkArea.X + primary.WorkArea.Width - width) / 2),
            Y = (int)((primary.WorkArea.Y + primary.WorkArea.Height - height) / 2),
            Width = (int)width,
            Height = (int)height
        });
#endif
        var homePage = new WonderSongsHomePage();
        Content = homePage;
        homePage.OnSuccess += (playable, swa) =>
        {
#if !HAS_UNO
            AppWindow.Hide();
#else
            Close();
#endif
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
                    @"D:\Programming\VS\WonderSongs.Uno\WonderSongs.Uno\Assets\wondersongs.ico",
                    "WonderSongs",
                    Guid.NewGuid()
                );
                trayIcon.LeftClicked += Icon_LeftClicked;
                trayIcon.IsVisible = true;
                trayIcon.Show();
                App.TrayIcon = trayIcon;

                void Icon_LeftClicked(object? sender, MouseEventReceivedEventArgs e)
                {
                    if (tray.MarkupNode.IsOpen)
                        // this branch is never taken
                        tray.MarkupNode.Hide();
                    else
                        // this is called, but RootGrid is null so it does nothing
                        tray.MarkupNode.Show();
                }
#else
                var selection = new WonderSongsSelectionWindow(playable);
#endif
                playable.LifeCycle(selection.SelectNextSongAsync, newSongPlaying);
            }
        };
    }
}

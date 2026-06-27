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

                var selection = new WonderSongsSelectionWindow(playable);
                playable.LifeCycle(selection.SelectNextSongAsync, newSongPlaying);
            }
        };
    }
}

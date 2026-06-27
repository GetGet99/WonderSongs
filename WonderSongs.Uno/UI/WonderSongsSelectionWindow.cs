using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

partial class WonderSongsSelectionWindow : WonderSongsWindow
{
    WonderSongsSelectionPage page;
    public WonderSongsSelectionWindow(WonderSongsPlayable playable)
    {
#if !HAS_UNO
        (AppWindow.Presenter as OverlappedPresenter)!.IsAlwaysOnTop = true;
#endif
        page = new WonderSongsSelectionPage(playable);
        Content = page;
        page.NextSongAvaliable += delegate
        {
#if !HAS_UNO
            // Center screen
            var primary = DisplayArea.GetFromPoint(default, DisplayAreaFallback.Primary);
            var DesiredSize = new Size(400, 200);
            //Measure(new(primary.WorkArea.Width, primary.WorkArea.Height));
            //Translation = new(0, (float)-DesiredSize.Height, 0);
            AppWindow.MoveAndResize(new()
            {
                X = (int)((primary.WorkArea.X + primary.WorkArea.Width - DesiredSize.Width) / 2),
                Y = primary.WorkArea.Y,
                Width = (int)DesiredSize.Width,
                Height = (int)DesiredSize.Height
            });
            AppWindow.Show();
#endif
        };
        page.NextSongSelected += delegate
        {
#if !HAS_UNO
            AppWindow.Hide();
#else
            (AppWindow.Presenter as OverlappedPresenter)?.Minimize();
#endif
        };
    }
    public Task<Song> SelectNextSongAsync() => page.SelectNextSongAsync();
}

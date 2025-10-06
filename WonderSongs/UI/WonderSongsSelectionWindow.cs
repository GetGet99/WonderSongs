using Microsoft.UI.Windowing;
using WonderSongs.Logic;

namespace WonderSongs.UI;

partial class WonderSongsSelectionWindow : WonderSongsWindow
{
    Button[] buttons = [new(), new(), new()];
    WonderSongsPlayable Playable { get; }
    public WonderSongsSelectionWindow(WonderSongsPlayable playable)
    {
        Playable = playable;
        Window.SystemBackdrop = new MicaBackdrop();
    }
    TaskCompletionSource<Song>? currentTCS;
    public async Task<Song> SelectNextSongAsync()
    {
        if (currentTCS is not null)
            return await currentTCS.Task;
        if (!DispatcherQueue.HasThreadAccess)
        {
            var tcs = new TaskCompletionSource<Song>();
            DispatcherQueue.TryEnqueue(async () =>
            {
                tcs.SetResult(await SelectNextSongAsync());
            });
            return await tcs.Task;
        }
        currentTCS = new();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Tag = Playable.NextCandidates[i];
            buttons[i].Content = Playable.NextCandidates[i].Title;
        }
        // Center screen
        var primary = DisplayArea.GetFromPoint(default, DisplayAreaFallback.Primary);
        var DesiredSize = new Size(400, 200);
        //Measure(new(primary.WorkArea.Width, primary.WorkArea.Height));
        //Translation = new(0, (float)-DesiredSize.Height, 0);
        Window.AppWindow.MoveAndResize(new()
        {
            X = (int)((primary.WorkArea.X + primary.WorkArea.Width - DesiredSize.Width) / 2),
            Y = primary.WorkArea.Y,
            Width = (int)DesiredSize.Width,
            Height = (int)DesiredSize.Height
        });
        Window.AppWindow.Show();

        var result = await currentTCS.Task;
        currentTCS = null;
        Window.AppWindow.Hide();
        return result;
    }
    protected override void Initialize(Page rootElement)
    {
        rootElement.Padding = new(16);
        rootElement.HorizontalAlignment = HorizontalAlignment.Center;
        rootElement.VerticalAlignment = VerticalAlignment.Center;
        rootElement.Content = new VStack(spacing: 16)
        {
            Text("Select your next song!").Center_Horizontal(),
            new VStack(spacing: 16) { Margin = new(16, 0, 16, 0)}
            .WithCustomCode(x =>
            {
                foreach (var button in buttons)
                {
                    button.HorizontalAlignment = HorizontalAlignment.Stretch;
                    OrientedStack.LengthProperty.SetValue(button, Star(1));
                    button.ClickEv(x => currentTCS?.SetResult((Song)x.Tag));
                    x.Children.Add(button);
                }
            })
        };
    }
}

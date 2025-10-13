using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

partial class WonderSongsSelectionPage : TemplateControl<Page>
{
    Button[] buttons = [new(), new(), new()];
    WonderSongsPlayable Playable { get; }
    public WonderSongsSelectionPage(WonderSongsPlayable playable)
    {
        Playable = playable;
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

        // Set a random message
        var rand = new Random();
        nextSongTextBlock.Text = NextSongMessages[rand.Next(NextSongMessages.Length)];

        NextSongAvaliable?.Invoke();
        var result = await currentTCS.Task;
        currentTCS = null;
        NextSongSelected?.Invoke();
        return result;
    }
    public event Action? NextSongAvaliable;
    public event Action? NextSongSelected;
    // Add at the top of the class
    static readonly string[] NextSongMessages = new[]
    {
        "Pick your next jam!",
        "What tune shall we vibe to next?",
        "Choose wisely, music master!",
        "Spin the wheel of songs!",
        "Time for another banger!",
        "Select your next sonic adventure!",
        "Let the music play—choose a song!",
        "What's your next earworm?",
        "Surprise us with your pick!",
        "Hit me with your best song!"
    };

    TextBlock nextSongTextBlock = new();

    // Update Initialize to use the global TextBlock
    protected override void Initialize(Page rootElement)
    {
        rootElement.Padding = new(16);
        rootElement.HorizontalAlignment = HorizontalAlignment.Center;
        rootElement.VerticalAlignment = VerticalAlignment.Center;
        nextSongTextBlock.Text = NextSongMessages[0];
        rootElement.Content = new VStack(spacing: 16)
        {
            nextSongTextBlock.Center_Horizontal(),
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

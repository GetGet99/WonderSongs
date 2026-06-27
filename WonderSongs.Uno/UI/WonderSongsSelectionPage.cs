using Microsoft.UI.Windowing;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    using Get.Symbols;
    bool ShowFocusHint = false;

    <root Padding=16 CenterH CenterV XYFocusKeyboardNavigation=Enabled>
        <VStack Spacing=16>
            nextSongTextBlock = <TextBlock Text=`NextSongMessages[0]` CenterH />
            foreach (var button in `buttons`)
                `button`
            if (`ShowFocusHint`) {
                <HStack Spacing=8 CenterH>
                    <HStack Spacing=4>
                        <KeyBlock Key="R-ALT" />
                        <SymbolExIcon(Add) FontSize=12 />
                        <KeyBlock Key="1" />
                        <KeyBlock Key="2" />
                        <KeyBlock Key="3" />
                    </HStack>
                    <TextBlock Text="Select song" CenterV Opacity=0.75 Tight />
                </HStack>
            }
        </VStack>
    </root>
    """)]
partial class WonderSongsSelectionPage : WonderSongsPage
{
    Button[] buttons = [new(), new(), new()];
    WonderSongsPlayable Playable { get; }
    public WonderSongsSelectionPage(WonderSongsPlayable playable)
    {
        Playable = playable;
        foreach (var button in buttons)
        {
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            OrientedStack.LengthProperty.SetValue(button, Star(1));
            button.ClickEv(x => currentTCS?.SetResult((Song)x.Tag));
        }
        Init();
#if !HAS_UNO
        this.FirstLoadedEv(x =>
        {
            playable.InitializeWithWindow((nint)x.XamlRoot.ContentIslandEnvironment.AppWindowId.Value);
        });
#endif
        KeyDown += WonderSongsSelectionPage_KeyDown;
    }

    private void WonderSongsSelectionPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is >=VirtualKey.Number1 and <=VirtualKey.Number3)
        {
            currentTCS?.SetResult((Song)(buttons[e.Key - VirtualKey.Number1]).Tag);
        }
        else if (e.Key is >= VirtualKey.NumberPad1 and <= VirtualKey.NumberPad3)
        {
            currentTCS?.SetResult((Song)(buttons[e.Key - VirtualKey.NumberPad1]).Tag);
        }
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
        "Choose wisely... your vibe depends on it.",
        "Spin the wheel of songs!",
        "Time for another banger!",
        "Select your next sonic adventure!",
        "Let the music play — choose a song!",
        "Surprise us with your pick!",
        "Hit me with your best song!",
        "Pick your next favorite… or maybe your next second favorite 🤔",
        "What’s calling your name this time?",
        "Destiny awaits — three tracks enter, one track leaves.",
        "Your next vibe is just a click away.",
        "Which one’s feeling right for the moment?",
        "Trust your instincts… or pure chaos. Your call.",
        "You’re the DJ — make it count!",
        "Pick the soundtrack to your next 3 minutes.",
        "Follow your heart. Or your shuffle finger.",
        "Three options. Infinite swagger.",
        "Choose the next chapter of your groove story.",
        "What’s the next move, maestro?",
        "Feeling lucky, or just feeling vibey?",
        "A tough choice, but someone’s gotta drop the beat.",
        "Ready for round two of absolute tunes?",
        "One of these songs has main-character energy. Find it.",
        "No wrong answers — only different flavors of awesome.",
        "Make your pick… destiny’s listening.",
        "Which one’s whispering ‘play me’? 🎶",
        "Your ears will thank you for this one.",
        "Choose your champion — may the best track win."
    };
}
[QuickMarkup("""
    string Key;
    <root>
        <Border Padding=`new(8,2,8,2)`
            CornerRadius=4
            BorderThickness=1
            BorderBrush=`new SolidColorBrush(Microsoft.UI.Colors.Gray)`>
            <TextBlock Text=`Key`
                FontFamily="Consolas"
                FontSize=12 />
        </Border>
    </root>
    """)]
partial class KeyBlock : UserControl;

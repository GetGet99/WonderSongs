using WonderSongs.Core;
#if ANDROID
using MainActivity = WonderSongs.Droid.MainActivity;
#endif

namespace WonderSongs.UI;

[QuickMarkup("""
    bool SmallMode = false;
    private string FunnyMessage = `FunnyPlayingMessages[0]`;
    private bool IsClockEnabled = false;
    private bool IsPlaying = false;
    <root>
        <VStack Spacing=16 CenterV>
            <TextBlock
                Text=`FunnyMessage`
                HorizontalAlignment=Stretch
                TextAlignment=Center
                TextWrapping=WrapWholeWords
                FontSize=`SmallMode ? 16 : 32`
            />
            <Button Width=48 Height=48 CornerRadius=24 @Click+=`
                if (IsPlaying) playable.Play();
                else playable.Pause();
            `>
                if (`!IsPlaying`)
                    <SymbolIcon Symbol=Play />
                else
                    <SymbolIcon Symbol=Pause />
            </Button>
        </VStack>
        clockPlace = <Border Top Right Margin=16>
            if (`IsClockEnabled`)
                <HStack Spacing=16>
                    <Button Content="Hide Clock" @Click+=`IsClockEnabled = false` />
                    `Clock ??= ClockFactory.CreateClock()`
                </HStack>
            else
                <HStack Spacing=16>
                    <Button Content="Show Clock" @Click+=`IsClockEnabled = true` />
                </HStack>
        </Border>
    </root>
    """)]
partial class WonderSongsMediaControlUI : Grid
{
    WonderSongsPlayable playable;
    TextBlock? Clock;
    public void RefreshMessage()
    {
        FunnyMessage = FunnyPlayingMessages[_random.Next(FunnyPlayingMessages.Length)];
    }
    public WonderSongsMediaControlUI(WonderSongsPlayable playable)
    {
        this.playable = playable;
        playable.IsPlayingProperty.ApplyAndRegisterForNewValue((_, x) =>
        {
            if (DispatcherQueue.HasThreadAccess)
            {
                Action();
            }
            else
            {
                DispatcherQueue.TryEnqueue(Action);
            }
            void Action()
            {
                IsPlaying = x;
            }
        });
        Init();
#if ANDROID
        MainActivity.Resume += ClockDisplayUpdate;
        MainActivity.Pause += ClockDisplayUpdate;
        void ClockDisplayUpdate()
        {
            clockPlace.Visibility = MainActivity.IsPinned ? Visibility.Visible : Visibility.Collapsed;
        }
        ClockDisplayUpdate();
#endif
    }

    private static readonly string[] FunnyPlayingMessages = new string[]
    {
        "Nice choice — your collection has great taste.",
        "And the beat goes on…",
        "Let’s see if this one hits harder than the last!",
        "Another banger? You’re on fire!",
        "Your collection delivers again.",
        "Certified vibe continuation.",
        "Transitioning to your next sonic adventure.",
        "Plot twist: this track might just top the last one.",
        "Cue the next chapter of your personal soundtrack.",
        "Music never sleeps… and apparently, neither do you.",
        "Next up: something your speakers might fall in love with.",
        "Brace yourself — another groove is coming.",
        "Great pick!",
        "Your collection never misses",
        "You’re the DJ now. No pressure.",
        "Hope your neighbors like this one too.",
        "Continuing your main-character energy.",
        "The vibe check continues…",
        "Don’t worry, this next one also slaps.",
        "Stay tuned — your rhythm journey continues.",
        "You really trust that random picker, huh?",
        "One song ends, another legend begins.",
        "Playlist destiny unfolding in real time.",
        "RNGesus has blessed your next track.",
        "Music’s rolling — just vibe with it.",
        "Can’t stop, won’t stop… playing your collection’s finest."
    };

    private static readonly Random _random = new();
}

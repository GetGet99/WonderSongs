using WonderSongs.Core;
#if ANDROID
using MainActivity = WonderSongs.Droid.MainActivity;
#endif

namespace WonderSongs.UI;

partial class SingleWindowPage : Page
{
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

    public SingleWindowPage(WonderSongsPlayable? playable = null)
    {
        if (playable is null)
        {
            var homePage = new WonderSongsHomePage();
            Content = homePage;
            homePage.OnSuccess += (p, _) => PickCollectionSuccess(p);
        }
        else
        {
            PickCollectionSuccess(playable);
        }
        void PickCollectionSuccess(WonderSongsPlayable playable)
        {
            var selection = new WonderSongsSelectionPage(playable);
            Content = selection;
            var (refresh, waitingUI) = CreateUI();
            selection.NextSongAvaliable += delegate
            {
                Content = selection;
            };
            selection.NextSongSelected += delegate
            {
                refresh();
                Content = waitingUI;
            };
            playable.LifeCycle(selection.SelectNextSongAsync, delegate { });
            (Action, UIElement) CreateUI()
            {
                var message = FunnyPlayingMessages[_random.Next(FunnyPlayingMessages.Length)];
                var tb = Text(message);
                tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                tb.TextAlignment = TextAlignment.Center;
                tb.TextWrapping = TextWrapping.WrapWholeWords;
                tb.FontSize = 32;
                void RefreshMessage()
                {
                    var message = FunnyPlayingMessages[_random.Next(FunnyPlayingMessages.Length)];
                    tb.Text = message;
                }
                var pause = new Button {
                    Content = new SymbolIcon { Symbol = Symbol.Pause },
                    Width = 48,
                    Height = 48,
                    CornerRadius = new CornerRadius(24)
                };
                var play = new Button { Content = new SymbolIcon { Symbol = Symbol.Play },
                    Width = 48,
                    Height = 48,
                    CornerRadius = new CornerRadius(24),
                    Style = (Style)Application.Current.Resources["AccentButtonStyle"]
                };
                play.ClickEv(_ => playable.Play());
                pause.ClickEv(_ => playable.Pause());


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
                        play.Visibility = !x ? Visibility.Visible : Visibility.Collapsed;
                        pause.Visibility = x ? Visibility.Visible : Visibility.Collapsed;
                    }
                });

                Border clockPlace = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(16)
                };

                {
                    Button showClock = Button("Show Clock");
                    clockPlace.Child = showClock;
                    HStack clockWithButton = new(spacing: 16)
                    {
                        Button("Hide Clock").ClickEv(_ =>
                        {
                            clockPlace.Child = showClock;
                        }),
                        ClockFactory.Clock()
                    };
                    showClock.ClickEv(_ =>
                    {
                        clockPlace.Child = clockWithButton;
                    });
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

                return (RefreshMessage, new Grid
                {
                    Children =
                    {
                        new VStack(spacing: 16)
                        {
                            tb,
                            new HStack(spacing: 16)
                            {
                                play, pause
                            }.Center_Horizontal()
                        }.Center_Vertical(),
                        clockPlace
                    }
                });
            }
        }
    }
}

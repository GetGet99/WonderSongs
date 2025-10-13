using WonderSongs.Core;

namespace WonderSongs.UI;

partial class SingleWindowPage : Page
{
    private static readonly string[] FunnyPlayingMessages = new[]
    {
        "Your song is spinning up! Grab a snack.",
        "Jamming out... Please hold your applause.",
        "The music elves are working overtime!",
        "Song loading... Dance break recommended.",
        "Your tune is brewing. Patience, maestro!",
        "Rocking out in the background. Air guitar optional.",
        "Music magic in progress. Come back with jazz hands.",
        "Your song is on a secret mission. Stand by.",
        "Melody incoming... Prepare your ears!",
        "The beat drops soon. Don't touch that dial!"
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
                var pause = new Button { Content = new SymbolIcon { Symbol = Symbol.Pause } };
                var play = new Button { Content = new SymbolIcon { Symbol = Symbol.Play } };
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
                return (RefreshMessage, new VStack(spacing: 16)
                {
                    tb,
                    new HStack(spacing: 16)
                    {
                        play, pause
                    }.Center_Horizontal()
                }.Center_Vertical());
            }
        }
    }
}

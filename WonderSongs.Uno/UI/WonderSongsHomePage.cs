using Windows.Storage.Pickers;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <root Padding=16 CenterH CenterV>
        <VStack Spacing=16 XYFocusKeyboardNavigation=Enabled>
            <TextBlock Text="WonderSongs" FontSize=48 CenterH />
            <TextBlock Text=`RandomMessage()` CenterH />
            songsPlace = <VStack Spacing=16 />
            <TextBlock Text="or" CenterH />
            <Button Content="Select folder of music" @Click+=`SelectFolderOfMusic()` CenterH />
            if (`RunningInMultiWindow`)
                SWA_Mode = <CheckBox Content="Single Window Application Mode" IsChecked=`!RunningInMultiWindow` CenterH />
            <Button Content="Exit" @Click+=`Environment.Exit(0)` CenterH />
        </VStack>
    </root>
    """)]
partial class WonderSongsHomePage : WonderSongsPage
{
    const bool RunningInMultiWindow =
#if HAS_UNO || DESKTOP
        true
#else
        false
#endif
        ;
    Action? _UIInitialized;
    public event Action? UIInitialized
    {
        add
        {
            _UIInitialized += value;
            if (initializedInner)
            {
                value?.Invoke();
            }
        }
        remove
        {
            _UIInitialized -= value;
        }
    }
    public event Action? OnSelected;
    bool initializedInner;
    public WonderSongsHomePage()
    {
        QuickMarkup.Infra.ReactiveScheduler.AddTickCallbackForCurrentThread(
            () => DispatcherQueue.TryEnqueue(QuickMarkup.Infra.ReactiveScheduler.Tick)
        );
        Init();
        InitInner();
        async void InitInner()
        {
            await foreach (var folder in WonderSongsApp.GetSongsFolderHistoryAsync())
            {
                songsPlace.Children.Add(new Button
                {
                    Content = folder.Name,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                }.ClickEv(async delegate
                {
                    OnSelected?.Invoke();
                    var collection = await WonderSongsApp.OpenFromCollectionAsync(folder);
                    OnSuccess?.Invoke(collection, SWA_Mode?.IsChecked ?? false);
                }));
            }
            initializedInner = true;
            _UIInitialized?.Invoke();
        }
    }
    string RandomMessage()
    {
        var random = new Random();
        return CollectionMessages[random.Next(CollectionMessages.Length)];
    }
    async void SelectFolderOfMusic()
    {
        var picker = new FolderPicker()
        {
            CommitButtonText = "Open Folder",
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.MusicLibrary
        };
#if !HAS_UNO
        InitializeWithWindow.Initialize(picker, (nint)this.XamlRoot.ContentIslandEnvironment.AppWindowId.Value);
#endif
        var folderP = await picker.PickSingleFolderAsync();
        if (folderP is null) return;
        var collection = await WonderSongsApp.OpenFromCollectionAsync(folderP);
        OnSuccess?.Invoke(collection, SWA_Mode.IsChecked ?? false);
    }
    public event Action<WonderSongsPlayable, bool>? OnSuccess;


    // Global array of funny/inspiring messages for "Select your collections"
    private static readonly string[] CollectionMessages = new[]
    {
        "Pick your music paradise!",
        "Open your musical treasure trove!",
        "Dive into your song stash!",
        "Choose your groove vault!",
        "Select your sonic adventure!",
        "Open your vault of musical wonders!",
        "Explore your tune trove!",
        "Open your groove collection!",
        "Find your harmony hub!",
        "Select your sound stash!",
        "Where does your next vibe live?",
        "Choose your realm of rhythm!",
        "Let’s see what your collection’s hiding.",
        "Find your next sound sanctuary.",
        "Your next musical journey starts here.",
        "Open the gates to groove city.",
        "Dig into your archive of awesome.",
        "Let the beats out of their folder.",
        "Select your library of legends.",
        "Unlock your sound universe.",
        "Pick the collection that defines your mood today.",
        "Where your bangers are born.",
        "Ready to summon some good tunes?",
        "Choose your sonic playground.",
        "Music lives here — which world will you enter?",
        "Open your personal hall of fame.",
        "Pick your soundtrack vault.",
        "Show us where the magic’s stored.",
        "Your vibe begins with a folder — choose wisely.",
        "Enter your collection of pure rhythm energy.",
        "Choose your soundtrack source!",
        "Pick the collection that matches your mood.",
        "Step into your world of sound.",
        "Ready to spin your own selection?",
        "Where do your best beats live?",
        "Open the door to your next vibe.",
        "Your next jam starts here.",
        "Find your folder of fame.",
        "Where your rhythms rest — until now.",
        "Show us where the music magic happens.",
        "Uncover your hidden bangers vault.",
        "Summon your symphony of sound.",
        "Select your realm of rhythm.",
        "Let’s see what your collection’s hiding.",
        "Enter your personal sound sanctuary.",
        "Unlock your archive of awesome.",
        "Dig into your library of legends.",
        "Discover where your best tunes call home.",
        "Where melodies meet destiny — pick wisely.",
        "Time to awaken your collection of classics."
    };
}

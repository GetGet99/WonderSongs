using System.Runtime.InteropServices;
using Windows.Devices.Haptics;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WonderSongs.Core;

namespace WonderSongs.UI;

partial class WonderSongsHomePage : TemplateControl<Page>
{
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

    protected override void Initialize(Page rootElement)
    {
        var random = new Random();
        var message = CollectionMessages[random.Next(CollectionMessages.Length)];
        CheckBox SWA_Mode = new() { Content = "Single Window Application Mode" };
#if HAS_UNO
        SWA_Mode.IsChecked = true;
#endif

        rootElement.Padding = new(16);
        rootElement.HorizontalAlignment = HorizontalAlignment.Center;
        rootElement.VerticalAlignment = VerticalAlignment.Center;
        rootElement.Content = new VStack(spacing: 16)
        {
            Text("WonderSongs").WithCustomCode(x => x.FontSize = 48).Center_Horizontal(),
            Text(message).Center_Horizontal(),
            new VStack(spacing: 16).WithCustomCode(async x =>
            {
                await foreach (var folder in WonderSongsApp.GetSongsFolderHistoryAsync())
                {
                    x.Children.Add(new Button
                    {
                        Content = folder.Name,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    }.ClickEv(async delegate
                    {
                        var collection = await WonderSongsApp.OpenFromCollectionAsync(folder);
                        OnSuccess?.Invoke(collection, SWA_Mode.IsChecked ?? false);
                    }));
                }
            }).Center_Horizontal(),
            Text("or").Center_Horizontal(),
            new Button
            {
                Content = "Select folder of music"
            }.ClickEv(async delegate
            {
                var picker = new FolderPicker()
                {
                    CommitButtonText = "Open Folder",
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.MusicLibrary
                };
                //#if !HAS_UNO
                //                picker.As<IInitializeWithWindow>().Initialize((nint)Window.AppWindow.Id.Value);
                //#endif
#if !HAS_UNO
                InitializeWithWindow.Initialize(picker, (nint)this.XamlRoot.ContentIslandEnvironment.AppWindowId.Value);
#endif
                var folderP = await picker.PickSingleFolderAsync();
                if (folderP is null) return;
                var collection = await WonderSongsApp.OpenFromCollectionAsync(folderP);
                OnSuccess?.Invoke(collection, SWA_Mode.IsChecked ?? false);
            }).Center_Horizontal(),
#if !HAS_UNO
            SWA_Mode.Center_Horizontal()
#endif
        };
    }
    public event Action<WonderSongsPlayable, bool>? OnSuccess;
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    internal interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }
}

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
        "Pick your playlist paradise!",
        "Unleash your musical treasure trove!",
        "Dive into your song stash!",
        "Choose your jam vault!",
        "Select your sonic adventure!",
        "Open your box of musical wonders!",
        "Time to raid your tune trove!",
        "Grab your groove collection!",
        "Find your harmony haven!",
        "Select your symphony stash!"
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
                Content = "Select folder of musics"
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

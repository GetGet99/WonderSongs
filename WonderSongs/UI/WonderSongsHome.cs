using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT;
using WinRT.Interop;
using WonderSongs.Logic;
using WonderSongs.UI;

namespace WonderSongs;

partial class WonderSongsHome : WonderSongsWindow
{
    public WonderSongsHome()
    {
        var width = 500; var height = 400;
        var primary = DisplayArea.GetFromPoint(default, DisplayAreaFallback.Primary);
        Window.AppWindow.Resize(new(width, height));
        Window.AppWindow.MoveAndResize(new()
        {
            X = (int)((primary.WorkArea.X + primary.WorkArea.Width - width) / 2),
            Y = (int)((primary.WorkArea.Y + primary.WorkArea.Height - height) / 2),
            Width = (int)width,
            Height = (int)height
        });
    }
    protected override void Initialize(Page rootElement)
    {
        rootElement.Padding = new(16);
        rootElement.HorizontalAlignment = HorizontalAlignment.Center;
        rootElement.VerticalAlignment = VerticalAlignment.Center;
        rootElement.Content = new VStack(spacing: 16)
        {
            Text("WonderSongs").WithCustomCode(x => x.FontSize = 48).Center_Horizontal(),
            Text("Select your collections").Center_Horizontal(),
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
                        Success(collection);
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
                picker.As<IInitializeWithWindow>().Initialize((nint)Window.AppWindow.Id.Value);

                var folderP = await picker.PickSingleFolderAsync();
                if (folderP is null) return;
                var collection = await WonderSongsApp.OpenFromCollectionAsync(folderP);
                Success(collection);
            }).Center_Horizontal()
        };
        void Success(WonderSongsPlayable playable)
        {
            var selection = new WonderSongsSelectionWindow(playable);
            playable.LifeCycle(selection.SelectNextSongAsync, delegate { });
            Window.AppWindow.Hide();
        }
    }
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    internal interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }

}

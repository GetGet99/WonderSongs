using System.Diagnostics.Metrics;
using WonderSongs.Core;
#if ANDROID
using MainActivity = WonderSongs.Droid.MainActivity;
#endif

namespace WonderSongs.UI;

partial class SingleWindowPage : Page
{
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
            //var (refresh, waitingUI) = CreateUI();
            var waitingUI = new WonderSongsMediaControlUI(playable);
            selection.NextSongAvaliable += delegate
            {
                Content = selection;
            };
            selection.NextSongSelected += delegate
            {
                waitingUI.RefreshMessage();
                Content = waitingUI;
            };
            playable.LifeCycle(selection.SelectNextSongAsync, delegate { });
        }
    }
}

using System.Runtime.InteropServices;
using Windows.Media;
using WinRT;

namespace WonderSongs.Core;
[AutoProperty]
partial class WonderSongsPlayable
{
    MediaPlayer MediaPlayer { get; } = new();
    Dictionary<Song, int> PlayCounts { get; } = new(); // song → #times played
    List<Song> Songs { get; } = new();
    Song? PlayingSong { get; set; }

    public IReadOnlyProperty<bool> IsPlayingProperty { get; }

    public WonderSongsPlayable(Song[] initialList)
    {
        var isplaying = Auto(false);
        IsPlayingProperty = isplaying;
#if ANDROID || WINAPPSDK_PACKAGED
        MediaPlayer.CurrentStateChanged += delegate
        {
            isplaying.CurrentValue = MediaPlayer.CurrentState is MediaPlayerState.Playing;
        };
#else
        MediaPlayer.PlaybackSession.PlaybackStateChanged += delegate
        {
            isplaying.CurrentValue = MediaPlayer.PlaybackSession.PlaybackState is Windows.Media.Playback.MediaPlaybackState.Playing;
        };
#endif

#if ANDROID
        InitializeWithWindow(default);
#endif
        MediaPlayer.IsLoopingEnabled = false;
        MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        Songs.AddRange(initialList);
        foreach (var s in initialList)
            PlayCounts[s] = 0;

        FillNextCandidate();
        MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;

    }
    bool hasCrossed;
    public void InitializeWithWindow(nint hwnd)
    {
#if WINDOWS
        
        var mediaControls = SystemMediaTransportControlsInterop.GetForWindow(hwnd);
#elif ANDROID
        var mediaControls = SystemMediaTransportControls.Instance;
#endif
#if WINDOWS || ANDROID
        MediaPlayer.CommandManager.IsEnabled = false;
        mediaControls.IsEnabled = true;
        mediaControls.IsPlayEnabled = true;
        mediaControls.IsPauseEnabled = true;
        IsPlayingProperty.ApplyAndRegisterForNewValue((_, x) =>
        {
            mediaControls.PlaybackStatus = x ? MediaPlaybackStatus.Playing : MediaPlaybackStatus.Paused;
        });
        mediaControls.ButtonPressed += (_, e) =>
        {
            switch (e.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Pause();
                    break;
            }
        };
#endif
    }

    private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
    {
#if DESKTOP
        // Uno Platform gives a bug where `sender.NaturalDuration`
        // and `sender.Position` could be `-00:00:00.0010000`
        if (sender.NaturalDuration <= TimeSpan.Zero) return;
        if (sender.Position <= TimeSpan.Zero) return;
#endif
        var position = sender.Position;
        if (!hasCrossed && sender.NaturalDuration - position <= TimeSpan.FromSeconds(30))
        {
            hasCrossed = true;
            var nextSong = await (NextSongsSelectionDialogTrigger?.Invoke() ?? Task.FromResult<Song?>(default)!);
            if (nextSong is not null)
            {
                ScheduleNextSong(nextSong);
            }
        }
    }


    double GetWeight(Song s) => 1.0 / (1 + PlayCounts[s]);

    void FillNextCandidate()
    {
        int count = Math.Min(3, Songs.Count);
        var available = new List<Song>(Songs);
        var candidates = new List<Song>(count);

        for (int i = 0; i < count; i++)
        {
            double totalWeight = available.Sum(s => GetWeight(s));
            double r = Random.Shared.NextDouble() * totalWeight;

            double cumulative = 0;
            Song? chosen = null;
            foreach (var s in available)
            {
                cumulative += GetWeight(s);
                if (r <= cumulative)
                {
                    chosen = s;
                    break;
                }
            }

            if (chosen != null)
            {
                candidates.Add(chosen);
                available.Remove(chosen); // no duplicates in this round
            }
        }

        NextCandidates = [.. candidates];
    }

    private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        PlayingSong = null;
        Play();
    }


    public Song[] NextCandidates { get; private set; } = [];
    public Song? NextSong { get; private set; }
    Action<Song>? NewSongPlaying;
    Func<Task<Song>>? NextSongsSelectionDialogTrigger;
    public void ScheduleNextSong(Song song)
    {
        NextSong = song;
        PlayCounts[song]++;        // increment only when actually scheduled
        FillNextCandidate();       // refresh candidates for the next round
        if (PlayingSong is null)
        {
            Play();
        }
    }
    public async void Play()
    {
        Debug.WriteLine($"Play()");
        if (PlayingSong is not null) goto Play;
        PlayingSong = NextSong;
        NextSong = null;
        if (PlayingSong is not null)
        {
            NewSongPlaying?.Invoke(PlayingSong);
            var src = await PlayingSong.GetSourceAsync();
            MediaPlayer.Source = src;
            hasCrossed = false;
            goto Play;
        }
        // possibly that there is no next song scheduled yet
        return;
    Play:
#if DESKTOP
    if (MediaPlayer.PlaybackSession.PlaybackState is not Windows.Media.Playback.MediaPlaybackState.Playing)
#endif
        MediaPlayer.Play();
    }

    public void Pause()
    {
#if DESKTOP
        Debug.WriteLine($"Pause()");
        if (MediaPlayer.PlaybackSession.PlaybackState is Windows.Media.Playback.MediaPlaybackState.Playing)
#endif
        MediaPlayer.Pause();
    }
    public void LifeCycle(Func<Task<Song>> NextSongsSelectionDialogTrigger, Action<Song> NewSongPlaying)
    {
        this.NextSongsSelectionDialogTrigger = () =>
        {
#if ANDROID
            Droid.VibrationHelper.VibrateTwice();
#endif
#if !DESKTOP
            Task.Run(async () =>
            {
                MediaPlayer.Volume = 0.5;
                await Task.Delay(2000);
                MediaPlayer.Volume = 1;
            });
#endif
            return NextSongsSelectionDialogTrigger();
        };
        this.NewSongPlaying = NewSongPlaying;

        InternalLifeCycle();

    }
    async void InternalLifeCycle()
    {
        // pick
        var song = await NextSongsSelectionDialogTrigger!();
        // play
        ScheduleNextSong(song);
        Play(); // triggers next song dialog
    }
}
//#if WINDOWS
//[ComImport]
//[Guid("DDB0472D-C911-4A1F-86D9-DC3D71A95F5A")]
//[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
//interface ISystemMediaTransportControlsInterop
//{
//    IntPtr GetForWindow(IntPtr hwnd, [In] ref Guid riid);
//}
//class MediaTransportControlsHelper
//{
//    private static Guid IID_ISystemMediaTransportControls = new Guid("DDB0472D-C911-4A1F-86D9-DC3D71A95F5A");

//    public static SystemMediaTransportControls GetSystemMediaTransportControls(IntPtr hwnd)
//    {
//        //var interop = (ISystemMediaTransportControlsInterop)Activator.CreateInstance(
//        //    Type.GetTypeFromCLSID(new Guid("DDB0472D-C911-4A1F-86D9-DC3D71A95F5A"))
//        //);
//        var interop = ActivationFactory.Get("Windows.Media.SystemMeriaTransportControls").As<ISystemMediaTransportControlsInterop>();
//        IntPtr controlsPtr = interop.Vftbl.GetForWindow(hwnd, ref IID_ISystemMediaTransportControls);
//        return Marshal.GetObjectForIUnknown(controlsPtr) as SystemMediaTransportControls;
//    }
//}

//#endif

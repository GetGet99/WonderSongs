global using MediaPlayer = WonderSongs.Droid.MediaPlayer;
global using MediaPlaybackSession = WonderSongs.Droid.MediaPlaybackSession;
using AndroidMediaPlayer = Android.Media.MediaPlayer;
using Application = Android.App.Application;

namespace WonderSongs.Droid;

class MediaPlayer
{
    readonly AndroidMediaPlayer mp = new();

    public bool IsLoopingEnabled
    {
        get => mp.Looping;
        set => mp.Looping = value;
    }

    public event Action<MediaPlayer, object>? MediaEnded;

    static object EmptyArgs { get; } = new();

    public MediaPlaybackSession PlaybackSession { get; }

    public MediaPlaybackCommandManager CommandManager { get; } = new();

    StorageFile? _source;
    public StorageFile Source
    {
        get => _source!;
        set
        {
            _source = value;
            InitializeSource(value);
        }
    }

    public MediaPlayer()
    {
        
        mp.Completion += delegate
        {
            CurrentStateChanged?.Invoke(this, EmptyArgs);
            MediaEnded?.Invoke(this, EmptyArgs);
        };
        PlaybackSession = new(this, mp);
        
    }

    void InitializeSource(StorageFile file)
    {
        try
        {
            // Release any existing source
            mp.Reset();

            // Try reflection to get SAF Uri
            var uri = file.TryGetAndroidUri();

            if (uri != null)
            {
                mp.SetDataSource(Application.Context, uri);
            }
            else
            {
                // Fallback: try normal path
                var path = file.Path;
                mp.SetDataSource(path);
            }

            mp.Prepare();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MediaPlayer] Failed to initialize source: {ex}");
        }
    }

    public void Play()
    {
        if (!mp.IsPlaying)
        {
            mp.Start();
            PlaybackSession.StartPositionUpdates();
            CurrentStateChanged?.Invoke(this, EmptyArgs);
        }
    }

    public void Pause()
    {
        if (mp.IsPlaying)
        {
            mp.Pause();
            PlaybackSession.StopPositionUpdates();
            CurrentStateChanged?.Invoke(this, EmptyArgs);
        }
    }

    double _volume = 1.0;
    public double Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0, 1);
            mp.SetVolume((float)_volume, (float)_volume);
        }
    }
    public event Action<MediaPlayer, object>? CurrentStateChanged;
    public MediaPlayerState CurrentState
    {
        get => mp.IsPlaying ? MediaPlayerState.Playing : MediaPlayerState.Paused;
    }
}
class MediaPlaybackCommandManager
{
#pragma warning disable CA1822 // Mark members as static
    public bool IsEnabled
#pragma warning restore CA1822 // Mark members as static
    {
        set
        {
            if (value)
            {
                throw new NotImplementedException("Enabling Media Playback is not implemented on Android.");
            }
        }
    }
}
class MediaPlaybackSession
{
    readonly AndroidMediaPlayer mp;
    readonly MediaPlayer owner;
    Timer? positionTimer;

    public MediaPlaybackSession(MediaPlayer owner, AndroidMediaPlayer mp)
    {
        this.owner = owner;
        this.mp = mp;
    }

    public event Action<MediaPlaybackSession, object>? PositionChanged;

    public TimeSpan Position
        => TimeSpan.FromMilliseconds(mp.CurrentPosition);

    public TimeSpan NaturalDuration
        => TimeSpan.FromMilliseconds(mp.Duration);

    public void StartPositionUpdates()
    {
        StopPositionUpdates();
        positionTimer = new Timer(_ =>
        {
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }, null, 0, 500); // every 0.5s
    }

    public void StopPositionUpdates()
    {
        positionTimer?.Dispose();
        positionTimer = null;
    }

    public MediaPlaybackState PlaybackState
    {
        get => mp.IsPlaying ? MediaPlaybackState.Playing : MediaPlaybackState.Paused;
    }
}

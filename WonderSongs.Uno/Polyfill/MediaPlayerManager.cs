#if ANDROID
using Android.Content;

namespace WonderSongs.Polyfill;

class MediaPlayerManager
{
    public static MediaPlayerManager Instance { get; } = new();
    public MediaPlayer? CurrentPlayer { get; private set; }

    public void Register(MediaPlayer player)
    {
        if (CurrentPlayer is null)
        {
            var context = Android.App.Application.Context;
            var intent = new Intent(context, typeof(MediaPlaybackService));
            context.StartForegroundService(intent);
        }
        if (CurrentPlayer is { } oldPlayer)
        {
            oldPlayer.CurrentStateChanged -= MediaPlayer_CurrentStateChanged;
        }
        player.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
        CurrentPlayer = player;
        CurrentStateChanegd?.Invoke();
    }

    private void MediaPlayer_CurrentStateChanged(MediaPlayer arg1, object arg2)
    {
        CurrentStateChanegd?.Invoke();
    }
    public event Action? CurrentStateChanegd;

    public void Play() => CurrentPlayer?.Play();
    public void Pause() => CurrentPlayer?.Pause();
}

#endif

#if ANDROID
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using AndroidX.Media.App;
using Java.Lang;
using NotificationCompat = AndroidX.Core.App.NotificationCompat;
namespace WonderSongs.Polyfill;

[Service(ForegroundServiceType = ForegroundService.TypeMediaPlayback, Exported = true)]
class MediaPlaybackService : Service
{
    public const int NotificationId = 1001;
    const string ChannelId = "wonder_songs_playback";
    MediaSessionCompat? _mediaSession;
    NotificationCompat.Builder? _builder;

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();

        _mediaSession = new MediaSessionCompat(this, "WonderSongsSession");
        _mediaSession.SetCallback(new MediaSessionCallback(this));
        _mediaSession.Active = true;
    }

    public override IBinder? OnBind(Intent? intent) => null;

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        var action = intent?.Action;

        if (action == "ACTION_PLAY")
        {
            MediaPlayerManager.Instance.Play();
            UpdateNotification(true);
        }
        else if (action == "ACTION_PAUSE")
        {
            MediaPlayerManager.Instance.Pause();
            UpdateNotification(false);
        }
        else
        {
            // Initial startup
            UpdateNotification(MediaPlayerManager.Instance.CurrentPlayer?.PlaybackSession.PlaybackState is MediaPlaybackState.Playing);
            MediaPlayerManager.Instance.CurrentStateChanegd += delegate
            {
                UpdateNotification(MediaPlayerManager.Instance.CurrentPlayer?.PlaybackSession.PlaybackState is MediaPlaybackState.Playing);
            };
        }

        return StartCommandResult.Sticky;
    }

    public void UpdateNotification(bool isPlaying)
    {
        var appName = Android.App.Application.Context.ApplicationInfo.LoadLabel(PackageManager)?.ToString() ?? "WonderSongs";
        var playPauseAction = BuildPlayPauseAction(isPlaying);

        var mediaStyle = new AndroidX.Media.App.NotificationCompat.MediaStyle();
        
        _builder = new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle(appName)
            .SetContentText(isPlaying ? "Playing" : "Paused")
            .SetSmallIcon(Android.Resource.Drawable.IcMediaPlay)
            .SetOnlyAlertOnce(true)
            .SetOngoing(true)
            .SetStyle(mediaStyle.SetMediaSession(_mediaSession?.SessionToken))
            .AddAction(playPauseAction);

        var notification = _builder.Build();
        StartForeground(NotificationId, notification);
    }

    NotificationCompat.Action BuildPlayPauseAction(bool isPlaying)
    {
        var intent = new Intent(this, typeof(MediaPlaybackService));
        intent.SetAction(isPlaying ? "ACTION_PAUSE" : "ACTION_PLAY");
        var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.Immutable);

        return new NotificationCompat.Action(
            isPlaying ? Android.Resource.Drawable.IcMediaPause : Android.Resource.Drawable.IcMediaPlay,
            isPlaying ? "Pause" : "Play",
            pendingIntent);
    }

    void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(ChannelId, "Media Playback", NotificationImportance.Low)
            {
                Description = "Media playback controls",
                LockscreenVisibility = NotificationVisibility.Public
            };
            var manager = (NotificationManager)GetSystemService(NotificationService);
            manager.CreateNotificationChannel(channel);
        }
    }

    // Simple vibration helper
    public void VibrateTwice()
    {
        long[] pattern = { 0, 100, 100, 100 };

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            var vm = (VibratorManager)GetSystemService(VibratorManagerService);
            vm.DefaultVibrator.Vibrate(VibrationEffect.CreateWaveform(pattern, -1));
        }
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var v = (Vibrator)GetSystemService(VibratorService);
            v?.Vibrate(VibrationEffect.CreateWaveform(pattern, -1));
        }
        else
        {
            var v = (Vibrator)GetSystemService(VibratorService);
            v?.Vibrate(pattern, -1);
        }
    }

    public class MediaSessionCallback : MediaSessionCompat.Callback
    {
        private readonly MediaPlaybackService _service;

        public MediaSessionCallback(MediaPlaybackService service)
        {
            _service = service;
        }

        public override void OnPlay()
        {
            MediaPlayerManager.Instance.Play();
            _service.UpdateNotification(true);
        }

        public override void OnPause()
        {
            MediaPlayerManager.Instance.Pause();
            _service.UpdateNotification(false);
        }
    }
    public override void OnTaskRemoved(Intent? rootIntent)
    {
        base.OnTaskRemoved(rootIntent);

        // Stop your playback
        MediaPlayerManager.Instance.CurrentPlayer?.Pause();

        // Stop the foreground service completely
        StopForeground(true);
        StopSelf();
        JavaSystem.Exit(0);
    }

}


#endif

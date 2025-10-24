using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media.Session;
using Java.Lang;
using Windows.Media;
using AndroidApp = Android.App.Application;
using AndroidResource = global::Android.Resource;
using NotificationCompat = AndroidX.Core.App.NotificationCompat;
namespace WonderSongs.Droid;

[Service(ForegroundServiceType = ForegroundService.TypeMediaPlayback, Exported = true)]
class MediaPlaybackService : Service
{
    public const int NotificationId = 1001;
    const string ChannelId = "wonder_songs_playback";
    MediaSessionCompat? _mediaSession;
    private AudioManager _audioManager;
    private AudioDeviceListener _audioCallback;
    NotificationCompat.Builder? _builder;

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();

        _mediaSession = new MediaSessionCompat(this, "WonderSongsSession");
        _mediaSession.SetCallback(new MediaSessionCallback(this));
        _mediaSession.Active = true;
        var stateBuilder = new PlaybackStateCompat.Builder()
            .SetActions(
                PlaybackStateCompat.ActionPlay |
                PlaybackStateCompat.ActionPause |
                PlaybackStateCompat.ActionPlayPause |
                PlaybackStateCompat.ActionStop)
            !.SetState(PlaybackStateCompat.StatePaused, 0, 1.0f);
        _mediaSession.SetPlaybackState(stateBuilder!.Build());

        var _audioCallback = new AudioDeviceListener();

        // Initialize AudioManager using service context
        _audioManager = (AudioManager)GetSystemService(AudioService);

        _audioCallback = new AudioDeviceListener();
        _audioManager.RegisterAudioDeviceCallback(_audioCallback, null);
        _audioCallback.DeviceDisconnected += delegate
        {
            var p = hasPausedByUser;
            MediaPlayerManager.Instance.Pause();
            SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Pause);
            UpdateNotification(false);

            // do not change hasPausedByUser as this is an automatic pause
            hasPausedByUser = p;
        };
        _audioCallback.SameDeviceConnected += delegate
        {
            if (!hasPausedByUser)
            {
                MediaPlayerManager.Instance.Play();
                SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Play);
                UpdateNotification(true);
            }
        };
    }

    public override IBinder? OnBind(Intent? intent) => null;

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        var action = intent?.Action;

        if (action == "ACTION_PLAY")
        {
            MediaPlayerManager.Instance.Play();
            //UpdateNotification(true); // will be updated via event
            SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Play);
        }
        else if (action == "ACTION_PAUSE")
        {
            MediaPlayerManager.Instance.Pause();
            //UpdateNotification(false); // will be updated via event
            SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Pause);
        }
        else
        {
            // Initial startup
            //UpdateNotification(MediaPlayerManager.Instance.CurrentPlayer?.PlaybackSession.PlaybackState is MediaPlaybackState.Playing);
            MediaPlayerManager.Instance.CurrentStateChanegd += delegate
            {
                UpdateNotification(MediaPlayerManager.Instance.CurrentPlayer?.PlaybackSession.PlaybackState is MediaPlaybackState.Playing);
            };
            UpdateNotification(SystemMediaTransportControls.Instance.PlaybackStatus is MediaPlaybackStatus.Playing);
            SystemMediaTransportControls.ServiceBindings.PlaybackStatusFromUIChanged += delegate
            {
                UpdateNotification(SystemMediaTransportControls.Instance.PlaybackStatus is MediaPlaybackStatus.Playing);
            };
        }

        return StartCommandResult.Sticky;
    }
    bool hasPausedByUser = false;
    public void UpdateNotification(bool isPlaying)
    {
        hasPausedByUser = !isPlaying;
        var appName = AndroidApp.Context.ApplicationInfo!.LoadLabel(PackageManager!)?.ToString() ?? "WonderSongs";
        UpdatePlaybackState(isPlaying);

        var playPauseAction = BuildPlayPauseAction(isPlaying);
        
        var mediaStyle = new AndroidX.Media.App.NotificationCompat.MediaStyle();

        _builder = new NotificationCompat.Builder(this, ChannelId)
            !.SetContentTitle(appName)
            !.SetContentText(isPlaying ? "Playing" : "Paused")
            !.SetSmallIcon(global::Android.Resource.Drawable.IcMediaPlay)
            !.SetOnlyAlertOnce(true)
            !.SetOngoing(true)
            !.SetStyle(mediaStyle.SetMediaSession(_mediaSession?.SessionToken))
            !.AddAction(playPauseAction)!;

        var notification = _builder.Build();
        StartForeground(NotificationId, notification);
    }

    NotificationCompat.Action BuildPlayPauseAction(bool isPlaying)
    {
        var intent = new Intent(this, typeof(MediaPlaybackService));
        intent.SetAction(isPlaying ? "ACTION_PAUSE" : "ACTION_PLAY");
        var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.Immutable);

        return new NotificationCompat.Action(
            isPlaying ? AndroidResource.Drawable.IcMediaPause : AndroidResource.Drawable.IcMediaPlay,
            isPlaying ? "Pause" : "Play",
            pendingIntent);
    }

    void CreateNotificationChannel()
    {
#pragma warning disable CA1416 // NotificationChannel is Android O and above
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(ChannelId, "Media Playback", NotificationImportance.Low)
            {
                Description = "Media playback controls",
                LockscreenVisibility = NotificationVisibility.Public
            };
            var manager = (NotificationManager)GetSystemService(NotificationService)!;
            manager.CreateNotificationChannel(channel);
        }
#pragma warning restore CA1416
    }

    void UpdatePlaybackState(bool isPlaying)
    {
        var state = new Android.Support.V4.Media.Session.PlaybackStateCompat.Builder()
            .SetActions(
                PlaybackStateCompat.ActionPlay |
                PlaybackStateCompat.ActionPause |
                PlaybackStateCompat.ActionPlayPause)
            !.SetState(
                isPlaying ? PlaybackStateCompat.StatePlaying : PlaybackStateCompat.StatePaused,
                PlaybackStateCompat.PlaybackPositionUnknown,
                1.0f)
            !.Build();

        _mediaSession?.SetPlaybackState(state);
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
            SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Play);
            _service.UpdateNotification(true);
        }

        public override void OnPause()
        {
            MediaPlayerManager.Instance.Pause();
            SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Pause);
            _service.UpdateNotification(false);
        }
    }
    public override void OnTaskRemoved(Intent? rootIntent)
    {
        base.OnTaskRemoved(rootIntent);

        // Stop your playback
        MediaPlayerManager.Instance.CurrentPlayer?.Pause();
        SystemMediaTransportControls.ServiceBindings.OnButtonPressed(SystemMediaTransportControlsButton.Pause);

        // Stop the foreground service completely
        StopForeground(true);
        StopSelf();
        JavaSystem.Exit(0);
    }
    public static void Start()
    {
        var context = AndroidApp.Context;
        var intent = new Intent(context, typeof(MediaPlaybackService));
        context.StartForegroundService(intent);
    }
}

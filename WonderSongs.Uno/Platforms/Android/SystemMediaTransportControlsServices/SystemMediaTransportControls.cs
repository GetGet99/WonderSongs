global using BackgroundMediaPlayer = WonderSongs.Droid.BackgroundMediaPlayer;
global using SystemMediaTransportControls = WonderSongs.Droid.SystemMediaTransportControls;
using Android.Content;
using Windows.Media;
namespace WonderSongs.Droid;
class SystemMediaTransportControls
{
    public static SystemMediaTransportControls Instance { get; } = new();
    private SystemMediaTransportControls()
    {
        MediaPlaybackService.Start();
    }
    MediaPlaybackStatus _PlaybackStatus;
    public MediaPlaybackStatus PlaybackStatus
    {
        get => _PlaybackStatus;
        set
        {
            _PlaybackStatus = value;
            ServiceBindings.InvokePlaybackStatusFromUIChanged(value);
        }
    }
    public bool IsEnabled
    {
        set
        {
            if (!value)
            {
                throw new NotImplementedException("Disabling SystemMediaTransportControls is not implemented on Android.");
            }
        }
    }
    public bool IsPlayEnabled
    {
        set
        {
            if (!value)
            {
                throw new NotImplementedException("Disabling SystemMediaTransportControls is not implemented on Android.");
            }
        }
    }
    public bool IsPauseEnabled
    {
        set
        {
            if (!value)
            {
                throw new NotImplementedException("Disabling SystemMediaTransportControls is not implemented on Android.");
            }
        }
    }
    public event Action<SystemMediaTransportControls, SystemMediaTransportControlsButtonPressedEventArgs>? ButtonPressed;
    public static class ServiceBindings
    {
        public static event Action? PlaybackStatusFromUIChanged;
        public static void InvokePlaybackStatusFromUIChanged(MediaPlaybackStatus PlaybackStatus) => PlaybackStatusFromUIChanged?.Invoke();
        public static void OnButtonPressed(SystemMediaTransportControlsButton button)
        {
            var mediaControls = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
            mediaControls.ButtonPressed?.Invoke(mediaControls, new SystemMediaTransportControlsButtonPressedEventArgs(button));
        }
    }
}
record SystemMediaTransportControlsButtonPressedEventArgs(SystemMediaTransportControlsButton Button);
static class BackgroundMediaPlayer
{
    public static class Current
    {
        public static SystemMediaTransportControls SystemMediaTransportControls { get; } = SystemMediaTransportControls.Instance;
    }
}

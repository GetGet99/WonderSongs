using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace WonderSongs.Droid;
[Activity(
    MainLauncher = true,
    ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
    WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
)]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        global::AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

        base.OnCreate(savedInstanceState);
    }
    protected override void OnResume()
    {
        Resume?.Invoke();
        base.OnResume();
    }
    protected override void OnPause()
    {
        Pause?.Invoke();
        base.OnPause();
    }
    public static event Action? Resume;
    public static event Action? Pause;
    public static bool IsPinned
    {
        get
        {
            var activityManager = (ActivityManager)Current!.ApplicationContext!.GetSystemService(Context.ActivityService)!;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                return activityManager.LockTaskModeState != LockTaskMode.None;
            }
            else
            {
#pragma warning disable CS0618 // Suppress deprecation warning
                return activityManager.IsInLockTaskMode;
#pragma warning restore CS0618
            }

        }
    }

}

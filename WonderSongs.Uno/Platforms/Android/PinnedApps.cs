using Android.App;
using Android.Content;
using Android.OS;

namespace WonderSongs.Droid;

public static class LockTaskHelper
{
    public static bool IsAppPinned(Context context)
    {
        var activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService)!;

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

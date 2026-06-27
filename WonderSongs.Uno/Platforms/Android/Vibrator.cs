using Android.Content;
using Android.OS;

namespace WonderSongs.Droid;

public static class VibrationHelper
{
    public static void VibrateTwice()
    {
        Context context = global::Android.App.Application.Context;
        long[] pattern = { 0, 100, 100, 100 };
        // Pattern explanation:
        // 0ms delay before starting
        // vibrate 100ms
        // pause 100ms
        // vibrate 100ms

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            // Android 12+ uses VibratorManager
            var vibratorManager = (VibratorManager)context.GetSystemService(Context.VibratorManagerService);
            var vibrator = vibratorManager.DefaultVibrator;
            vibrator.Vibrate(VibrationEffect.CreateWaveform(pattern, -1));
        }
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            // Android 8.0+ (API 26–31)
            var vibrator = (Vibrator)context.GetSystemService(Context.VibratorService);
            vibrator?.Vibrate(VibrationEffect.CreateWaveform(pattern, -1));
        }
        else
        {
            // Legacy devices
            var vibrator = (Vibrator)context.GetSystemService(Context.VibratorService);
            vibrator?.Vibrate(pattern, -1);
        }
    }
}

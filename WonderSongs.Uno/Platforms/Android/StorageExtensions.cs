#if ANDROID
using System.Reflection;
using Uri = Android.Net.Uri;
namespace WonderSongs.Droid;
public static class StorageExtensions
{
    public static Uri TryGetAndroidUri(this StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        //try
        //{
        // Step 1: Get internal 'Implementation' property
        var implProp = typeof(StorageFile).GetProperty(
            "Implementation",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var impl = implProp?.GetValue(file);
        if (impl == null)
            return null;

        // Step 2: Get private field '_fileUri' from implementation
        var uriField = impl.GetType().GetField(
            "_fileUri",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var uriValue = uriField?.GetValue(impl) as Uri;
        return uriValue;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Debug.WriteLine($"[StorageFileExtensions] Reflection failed: {ex}");
        //    return null;
        //}
    }
    public static Uri TryGetAndroidUri(this StorageFolder folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        //try
        //{
        // Step 1: Get internal 'Implementation' property
        var implProp = typeof(StorageFolder).GetProperty(
            "Implementation",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var impl = implProp?.GetValue(folder);
        if (impl == null)
            return null;

        // Step 2: Get private field '_fileUri' from implementation
        var uriField = impl.GetType().GetField(
            "_folderUri",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var uriValue = uriField?.GetValue(impl) as Uri;
        return uriValue;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Debug.WriteLine($"[StorageFileExtensions] Reflection failed: {ex}");
        //    return null;
        //}
    }
}
#endif

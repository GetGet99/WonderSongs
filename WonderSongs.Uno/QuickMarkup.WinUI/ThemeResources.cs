using Windows.UI.ViewManagement;
#if UWP
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
#else
using Microsoft.UI.Dispatching;
#endif

namespace QuickMarkup.WinUI;

public partial class ThemeResources
{
    static UISettings UISettings { get; } = new();

#if UWP
    static CoreDispatcher? _dispatcher;
#else
    static DispatcherQueue? _dispatcher;
#endif

    static void EnsureDispatcher()
    {
#if UWP
        _dispatcher ??= CoreApplication.GetCurrentView().Dispatcher;
#else
        _dispatcher ??= DispatcherQueue.GetForCurrentThread();
#endif
    }

    static void RunOnUIThread(Action action)
    {
#if UWP
        _ = _dispatcher!.TryRunAsync(CoreDispatcherPriority.High, () => action());
#else
        _ = _dispatcher!.TryEnqueue(() => action());
#endif
    }

    public static Reference<T?> Get<T>(string resourcesName, FrameworkElement? element)
    {
        if (element is null)
            return Get<T>(resourcesName);

        var prop = new Reference<T?>(Resolve<T>(resourcesName, element));
        element.ActualThemeChanged += delegate
        {
            prop.Value = Resolve<T>(resourcesName, element);
        };
        return prop;
    }

    public static Reference<T?> Get<T>(string resourcesName)
    {
        bool isDark = UISettings.GetColorValue(UIColorType.Background).R < 255 / 2;
        var prop = new Reference<T?>(Resolve<T>(resourcesName, isDark));
        EnsureDispatcher();
        UISettings.ColorValuesChanged += delegate
        {
            RunOnUIThread(() =>
            {
                bool isDark = UISettings.GetColorValue(UIColorType.Background).R < 255 / 2;
                prop.Value = Resolve<T>(resourcesName, isDark);
            });
        };
        return prop;
    }
}

using Get.Data.Properties;
using static Get.Data.Properties.AutoTyper;
namespace Get.UI.Data;
public partial class ThemeResources
{
    public static IReadOnlyProperty<T> Get<T>(string resourcesName, FrameworkElement ele)
    {
#if WINDOWS_UWP || UWPNET9
        ResourceDictionary AppDark() => (ResourceDictionary)Application.Current.Resources.ThemeDictionaries["Default"];
        ResourceDictionary AppLight() => (ResourceDictionary)Application.Current.Resources.ThemeDictionaries["Light"];
        ResourceDictionary AppTheme() => ele.ActualTheme is Platform.UI.Xaml.ElementTheme.Dark ? AppDark() : AppLight();
        ResourceDictionary? ElementDark() =>
            ele.Resources.ThemeDictionaries.TryGetValue("Default", out var d) ? (ResourceDictionary)d : null;
        ResourceDictionary? ElementLight() =>
            ele.Resources.ThemeDictionaries.TryGetValue("Light", out var d) ? (ResourceDictionary)d : null;
        ResourceDictionary? ElementTheme() => ele.ActualTheme is Platform.UI.Xaml.ElementTheme.Dark ? ElementDark() : ElementLight();
        var appLight = Application.Current.Resources.ThemeDictionaries["Light"];
        FrameworkElement ele2 = ele;
        string resourcesName2 = resourcesName;
        if (!(ElementTheme()?.TryGetValue(resourcesName2, out var value) ?? false))
        {
            value = AppTheme()[resourcesName2];
        }

        Property<T> prop = new((T)value);
        ele2.ActualThemeChanged += delegate
        {
            if (!(ElementTheme()?.TryGetValue(resourcesName2, out var value2) ?? false))
            {
                value2 = AppTheme()[resourcesName2];
            }

            prop.Value = (T)value2;
        };
        return AutoReadOnly((IProperty<T>)prop);
#else
        // TODO: Element themes
        if (!ele.Resources.TryGetValue(resourcesName, out var val)) val = Application.Current.Resources[resourcesName];
        var prop = new Property<T>((T)val);
        ele.ActualThemeChanged += delegate
        {
            if (!ele.Resources.TryGetValue(resourcesName, out var val)) val = Application.Current.Resources[resourcesName];
            prop.Value = (T)val;
        };
        return AutoReadOnly<T>(prop);
#endif
    }
    public static IReadOnlyProperty<T> Create<T>(FrameworkElement ele, T darkResource, T lightResource)
    {
        var prop = new Property<T>(ele.ActualTheme is ElementTheme.Dark ? darkResource : lightResource);
        ele.ActualThemeChanged += delegate
        {
            prop.Value = ele.ActualTheme is ElementTheme.Dark ? darkResource : lightResource;
        };
        return AutoReadOnly<T>(prop);
    }
}
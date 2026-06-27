namespace QuickMarkup.WinUI;

// Code copied from https://github.com/microsoft/microsoft-ui-reactor/blob/main/src/Reactor/Core/Theme.cs
// With minor modification on changing type to generic type T
public partial class ThemeResources
{
    /// <summary>
    /// Resolves this theme reference using the element's actual theme.
    /// Walks the ThemeDictionaries in Application.Resources and MergedDictionaries
    /// to find the brush matching the element's effective theme (which respects
    /// per-element RequestedTheme overrides, not just the app-level theme).
    /// </summary>
    private static T? Resolve<T>(string resourceKey, FrameworkElement fe)
    {
        var themeName = GetEffectiveThemeName(fe);
        return ResolveForTheme<T>(resourceKey, themeName);
    }
    // </snippet:theme-ref>

    /// <summary>
    /// Resolves a theme resource using an explicit isDark flag.
    /// Useful for resolving during Render() before controls are in the tree.
    /// </summary>
    private static T? Resolve<T>(string resourceKey, bool isDark)
    {
        return ResolveForTheme<T>(resourceKey, isDark ? "Dark" : "Light");
    }

    private static T? ResolveForTheme<T>(string resourceKey, string themeName)
    {
        var resources = Application.Current?.Resources;
        if (resources is null) return default;

        // WinUI's XamlControlsResources ThemeDictionary keys vary by app configuration:
        //   Keys observed: "Default", "Light", "HighContrast" (no "Dark")
        // "Default" contains the base/dark brushes; "Light" contains light overrides.
        // Try the exact theme name first, then "Default" as the universal fallback.
        if (TryResolveFromThemeDictionaries<T>(resources, resourceKey, themeName, out var value))
            return value;
        if (TryResolveFromThemeDictionaries<T>(resources, resourceKey, "Default", out value))
            return value;

        // Fallback: non-themed resource lookup (including MergedDictionaries)
        if (TryResolveNonThemed<T>(resources, resourceKey, out var fb))
            return fb;

        return default;
    }

    private static string GetEffectiveThemeName(FrameworkElement fe)
    {
        // Check the element's own RequestedTheme first
        if (fe.RequestedTheme != ElementTheme.Default)
            return fe.RequestedTheme == ElementTheme.Dark ? "Dark" : "Light";

        // Walk up the visual tree for the nearest override
        var parent = VisualTreeHelper.GetParent(fe) as FrameworkElement;
        while (parent is not null)
        {
            if (parent.RequestedTheme != ElementTheme.Default)
                return parent.RequestedTheme == ElementTheme.Dark ? "Dark" : "Light";
            parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
        }

        // No override found — check ActualTheme (reliable for elements already in the tree)
        if (fe.ActualTheme != ElementTheme.Default)
            return fe.ActualTheme == ElementTheme.Dark ? "Dark" : "Light";

        // Final fallback: application theme
        return Application.Current?.RequestedTheme == ApplicationTheme.Dark ? "Dark" : "Light";
    }

    private static bool TryResolveFromThemeDictionaries<T>(
        ResourceDictionary resources, string key, string themeName, out T? value)
    {
        // Check this dictionary's ThemeDictionaries
        if (resources.ThemeDictionaries.TryGetValue(themeName, out var themeObj)
            && themeObj is ResourceDictionary themeDict
            && themeDict.TryGetValue(key, out var themed)
            && themed is T themedValue)
        {
            value = themedValue;
            return true;
        }

        // Check MergedDictionaries (XamlControlsResources is added here)
        foreach (var merged in resources.MergedDictionaries)
        {
            if (TryResolveFromThemeDictionaries(merged, key, themeName, out value))
                return true;
        }

        value = default;
        return false;
    }

    private static bool TryResolveNonThemed<T>(ResourceDictionary resources, string key, out T? value)
    {
        if (resources.TryGetValue(key, out var value2) && value2 is T val)
        {
            value = val;
            return true;
        }

        foreach (var merged in resources.MergedDictionaries)
        {
            if (TryResolveNonThemed(merged, key, out value))
                return true;
        }

        value = default;
        return false;
    }
}

using System.Runtime.CompilerServices;

namespace QuickMarkup.WinUI;

static class ThemeResourcesExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThemeBrushes UseThemeBrushes(this FrameworkElement element) => new(element);
}
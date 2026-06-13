using Get.Data.Bindings;
using Get.Data.Bundles;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI;
using Get.Data.Bindings.Linq;
namespace Get.UI.Data;

public static partial class QuickCreate
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GridLength Pixel(double pixel)
        => new(pixel);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public static GridLength Auto() => GridLength.Auto;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GridLength Star(double star = 1)
        => new(star, GridUnitType.Star);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SolidColorBrush Solid(Color color)
        => new(color);
}

[QuickMarkup("""
        <root Orientation=Horizontal />
        """)]
public partial class HStack : StackPanel;
[QuickMarkup("""
        <root Orientation=Vertical />
        """)]
public partial class VStack : StackPanel;

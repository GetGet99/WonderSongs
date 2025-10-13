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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IContentBundle<UIElement> Content(UIElement element)
        => new ConstantContentBundle<UIElement>(element);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IContentBundle<UIElement> Content(string text)
        => new ConstantContentBundle<UIElement>(new TextBlock { Text = text });
    public static Orientation Flip(this Orientation orientation)
        => orientation is Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyBinding<Orientation> Flip(this IReadOnlyBinding<Orientation> @in)
        => from o in @in select o.Flip();
    public partial class HStack : StackPanel, IEnumerable<UIElement>
    {
        public HStack(double spacing = 0)
        {
            Orientation = Orientation.Horizontal;
            Spacing = spacing;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(UIElement item) => Children.Add(item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<UIElement> GetEnumerator() => Children.AsEnumerable().GetEnumerator();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => Children.AsEnumerable().GetEnumerator();
    }
    public partial class VStack : StackPanel, IEnumerable<UIElement>
    {
        public VStack(double spacing = 0)
        {
            Orientation = Orientation.Vertical;
            Spacing = spacing;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(UIElement item) => Children.Add(item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<UIElement> GetEnumerator() => Children.AsEnumerable().GetEnumerator();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => Children.AsEnumerable().GetEnumerator();
    }
}

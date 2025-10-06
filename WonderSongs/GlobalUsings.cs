global using Get.Data.Bindings;
global using Get.Data.Bindings.Linq;
global using Get.Data.Collections;
global using Get.Data.Collections.Linq;
global using Get.Data.Collections.Update;
global using Get.Data.Helpers;
global using Get.Data.Properties;
global using Get.Data.XACL;
global using Get.UI.Data;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Media.Imaging;
global using Microsoft.UI.Xaml.Navigation;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Runtime.InteropServices.WindowsRuntime;
global using Windows.Foundation;
global using Windows.UI;
global using Windows.UI.Xaml;
global using static Get.Data.Properties.AutoTyper;
global using static Get.Data.XACL.QuickBindingExtension;
global using static Get.UI.Data.QuickCreate;
global using static WonderSongs.UIFuncs;
global using System.Threading.Tasks;
global using Windows.Storage;
global using Windows.Media.Core;
global using Windows.Media.Playback;
namespace WonderSongs;
using Get.Symbols;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using System.Collections;
using System.Runtime.CompilerServices;
using Windows.UI.Text;
static class UIFuncs
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextBlock FlyoutDismissText(string text = "(Tap outside this dialog to dismiss)")
        => new TextBlock
        {
            Text = text,
            FontFamily = new FontFamily("Calibri"),
            FontStyle = FontStyle.Italic,
            TextWrapping = TextWrapping.WrapWholeWords
        }.Center_Horizontal();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextBlock Text(string text, TextWrapping wrap = TextWrapping.NoWrap, TextLineBounds bounds = TextLineBounds.Full)
        => new TextBlock
        {
            Text = text,
            TextWrapping = wrap,
            TextLineBounds = bounds
        };
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Button Button(string text)
        => new Button
        {
            Content = text
        };
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Button Button(SymbolEx icon)
        => new Button
        {
            Content = new SymbolExIcon(icon),
            Padding = new(5)
        };
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SolidColorBrush Solid(Color color, double opacity)
    {
        return new SolidColorBrush(color) { Opacity = opacity };
    }
    public static T Center_Horizontal<T>(this T element)
        where T : FrameworkElement
    {
        element.HorizontalAlignment = HorizontalAlignment.Center;
        return element;
    }
    public static T Center_Vertical<T>(this T element)
        where T : FrameworkElement
    {
        element.VerticalAlignment = VerticalAlignment.Center;
        return element;
    }
    public static T Top<T>(this T element) where T : FrameworkElement
    {
        element.VerticalAlignment = VerticalAlignment.Top;
        return element;
    }
    public static T Bottom<T>(this T element) where T : FrameworkElement
    {
        element.VerticalAlignment = VerticalAlignment.Bottom;
        return element;
    }
    public static T Left<T>(this T element) where T : FrameworkElement
    {
        element.HorizontalAlignment = HorizontalAlignment.Left;
        return element;
    }
    public static T Right<T>(this T element) where T : FrameworkElement
    {
        element.HorizontalAlignment = HorizontalAlignment.Right;
        return element;
    }
    public static T Top<T>(this T element, double pixel) where T : FrameworkElement
    {
        element.Margin = element.Margin with { Top = pixel };
        return element;
    }
    public static T Bottom<T>(this T element, double pixel) where T : FrameworkElement
    {
        element.Margin = element.Margin with { Bottom = pixel };
        return element;
    }
    public static T Left<T>(this T element, double pixel) where T : FrameworkElement
    {
        element.Margin = element.Margin with { Left = pixel };
        return element;
    }
    public static T Right<T>(this T element, double pixel) where T : FrameworkElement
    {
        element.Margin = element.Margin with { Right = pixel };
        return element;
    }
    public static T LoadedEv<T>(this T element, Action<T> act) where T : FrameworkElement
    {
        element.Loaded += (_, _) => act(element);
        return element;
    }
    public static Button ClickEv(this Button element, Action<Button> act)
    {
        element.Click += (_, _) => act(element);
        return element;
    }
    public static MenuFlyoutItem ClickEv(this MenuFlyoutItem element, Action<MenuFlyoutItem> act)
    {
        element.Click += (_, _) => act(element);
        return element;
    }
    public static T ClickEvMFI<T>(this T element, Action<T> act) where T : MenuFlyoutItem
    {
        element.Click += (_, _) => act(element);
        return element;
    }
    public static T ClickEvBtn<T>(this T element, Action<T> act) where T : Button
    {
        element.Click += (_, _) => act(element);
        return element;
    }
    public static T FirstLoadedEv<T>(this T element, Action<T> act) where T : FrameworkElement
    {

        void r(object _, RoutedEventArgs _1)
        {
            element.Loaded -= r;
            act(element);
        }
        element.Loaded += r;
        return element;
    }
    public static TextBlock Bold(this TextBlock element)
    {
        element.FontWeight = FontWeights.Bold;
        return element;
    }
    public static TextBlock Italic(this TextBlock element)
    {
        element.FontFamily = new FontFamily("Calibri");
        element.FontStyle = FontStyle.Italic;
        return element;
    }
}
public partial class VStack : StackPanel, IEnumerable<UIElement>, IEnumerable
{
    public VStack(double spacing = 0.0)
    {
        base.Orientation = Orientation.Vertical;
        base.Spacing = spacing;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(UIElement item)
    {
        base.Children.Add(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<UIElement> GetEnumerator()
    {
        return base.Children.AsEnumerable().GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return base.Children.AsEnumerable().GetEnumerator();
    }
}
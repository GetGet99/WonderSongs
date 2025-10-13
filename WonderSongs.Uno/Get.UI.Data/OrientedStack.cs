using Get.Data.Bindings;
using Get.Data.Properties;
using Get.UI.Data;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Diagnostics;

namespace Get.UI.Data;
[AutoProperty]
public partial class OrientedStack : NamedPanel
{
    public static AttachedProperty<DependencyObject, GridUnitType> LengthTypeProperty { get; } = new(default);
    public static AttachedProperty<DependencyObject, double> LengthValueProperty { get; } = new(default);
    public static AttachedProperty<DependencyObject, GridLength> LengthProperty { get; } = new(default);
    public static AttachedProperty<DependencyObject, bool> VisibilityTrackingProperty { get; } = new(false);
    static OrientedStack()
    {
        LengthTypeProperty.ValueChanged += OnLengthTypeChanged;
        LengthValueProperty.ValueChanged += OnLengthValueChanged;
        LengthProperty.ValueChanged += OnLengthChanged;
    }
    public IProperty<Orientation> OrientationProperty { get; } = Auto<Orientation>(default);
    public IProperty<double> SpacingProperty { get; } = Auto<double>(0);
    public OrientedStack()
    {
        OrientationProperty.ValueChanged += OnOrientationChanged;
        
    }
    public OrientedStack(Orientation orientation = default, double spacing = 0) : this()
    {
        Orientation = orientation;
        Spacing = spacing;
    }
    static void OnLengthTypeChanged(DependencyObject obj, GridUnitType oldValue, GridUnitType newValue)
    {
        var length = LengthProperty.GetValue(obj);
        length = new(length.Value, newValue);
        if (LengthProperty.GetValue(obj) != length)
            LengthProperty.SetValue(obj, length);
    }
    static void OnLengthValueChanged(DependencyObject obj, double oldValue, double newValue)
    {
        var length = LengthProperty.GetValue(obj);
        length = new(newValue, length.GridUnitType);
        if (LengthProperty.GetValue(obj) != length)
            LengthProperty.SetValue(obj, length);
    }
    static void OnLengthChanged(DependencyObject obj, GridLength oldValue, GridLength newValue)
    {
        if (LengthValueProperty.GetValue(obj) != newValue.Value)
            LengthValueProperty.SetValue(obj, newValue.Value);
        if (LengthTypeProperty.GetValue(obj) != newValue.GridUnitType)
            LengthTypeProperty.SetValue(obj, newValue.GridUnitType);
        (VisualTreeHelper.GetParent(obj) as OrientedStack)?.InvalidateArrange();
    }
    void OnOrientationChanged(Orientation oldValue, Orientation newValue)
    {
        InvalidateArrange();
        InvalidateMeasure();
    }
    void OnChildVisibilityChanged(Visibility _, Visibility _1)
    {
        InvalidateArrange();
        InvalidateMeasure();
    }
    readonly Dictionary<UIElement, IProperty<Visibility>> CachedChildrenVisibility = [];
    readonly HashSet<UIElement> CachedChildren = [];
    static readonly IPropertyDefinition<UIElement, Visibility> VisibilityPropertyDefinition = VisibilityProperty.AsPropertyDefinition<UIElement, Visibility>();
    protected override Size MeasureOverride(Size availableSize)
    {
#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        var orientation = Orientation;
        (double Along, double Opposite) SizeToOF(Size size) =>
            orientation is Orientation.Horizontal ?
            (size.Width, size.Height) : (size.Height, size.Width);
        Size OFToSize((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        var panelSize = SizeToOF(availableSize);
        var panelRemainingSize = panelSize;
        double totalUsed = 0;
        var count = Children.Count;
        List<(double pixel, UIElement ele)> pixelList = new(count);
        List<UIElement> autoList = new(count);
        List<(double star, UIElement ele)> starList = new(count);

        double totalAbsolutePixel = 0, totalStar = 0, maxOpposite = 0;
        int visibleChildren = 0;
        foreach (var child in Children)
        {
            if (VisibilityTrackingProperty.GetValue(child))
            {
                if (!CachedChildren.Remove(child))
                {
                    var prop = VisibilityPropertyDefinition.GetProperty(child);
                    CachedChildrenVisibility[child] = prop;
                    prop.ValueChanged += OnChildVisibilityChanged;
                }
            }
            if (child.Visibility is not Visibility.Visible) continue;
            visibleChildren++;
            var length = LengthProperty.GetValue(child);
            if (length.IsAuto)
            {
                autoList.Add(child);
            }
            else if (length.IsStar)
            {
                totalStar += length.Value;
                starList.Add((length.Value, child));
            }
            else
            {
                totalAbsolutePixel += length.Value;
            }
        }
        // these children are no longer here
        foreach (var child in CachedChildren)
        {
            if (CachedChildrenVisibility.Remove(child, out var property))
                property.ValueChanged -= OnChildVisibilityChanged;
            if (property is IDisposable disposable)
                disposable.Dispose();
        }
        CachedChildren.Clear();
        // ...
        // add spacing as part of absolute pixel
        if (visibleChildren > 1)
        {
            totalAbsolutePixel += (visibleChildren - 1) * Spacing;
        }
        foreach (var (pixel, child) in pixelList)
        {
            child.Measure(OFToSize((pixel, panelSize.Opposite)));
            var (_, Opposite) = SizeToOF(child.DesiredSize);
            if (Opposite > maxOpposite)
                maxOpposite = Opposite;
        }
        panelRemainingSize.Along -= totalAbsolutePixel;
        totalUsed += totalAbsolutePixel;
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        foreach (var child in autoList)
        {
            child.Measure(OFToSize(panelRemainingSize));
            var (Along, Opposite) = SizeToOF(child.DesiredSize);
            if (Opposite > maxOpposite)
                maxOpposite = Opposite;
            totalUsed += Along;
            panelRemainingSize.Along -= Along;
            panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        }
        double maxStarSize = 0;
        foreach (var (star, child) in starList)
        {
            child.Measure(OFToSize(panelRemainingSize));
            var (Along, Opposite) = SizeToOF(child.DesiredSize);
            if (Opposite > maxOpposite)
                maxOpposite = Opposite;
            var starSize = Along / star;
            if (starSize > maxStarSize)
                maxStarSize = starSize;
        }
        var computed = maxStarSize * totalStar;
        panelRemainingSize.Along -= computed;
        totalUsed += computed;
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        var toReturn = OFToSize((totalUsed, Math.Min(maxOpposite, panelSize.Opposite)));
#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        return toReturn;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        var orientation = Orientation;
        (double Along, double Opposite) SizeToOF(Size size) =>
            orientation is Orientation.Horizontal ?
            (size.Width, size.Height) : (size.Height, size.Width);
        Size OFToSize((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        Point OFToPoint((double Along, double Opposite) of) =>
            orientation is Orientation.Horizontal ?
            new(of.Along, of.Opposite) : new(of.Opposite, of.Along);
        var panelSize = SizeToOF(finalSize);
        var panelRemainingSize = panelSize;
        double totalAbsolutePixel = 0, totalStar = 0;
        int visibleChildren = 0;
        foreach (var child in Children)
        {
            if (child.Visibility is not Visibility.Visible) continue;
            visibleChildren++;
            var length = LengthProperty.GetValue(child);
            if (length.IsAuto)
            {
                var desiredSize = SizeToOF(child.DesiredSize);
                totalAbsolutePixel += desiredSize.Along;
            }
            else if (length.IsStar)
            {
                totalStar += length.Value;
            }
            else
            {
                totalAbsolutePixel += length.Value;
            }
        }
        // add spacing as part of absolute pixel
        if (visibleChildren > 1)
        {
            totalAbsolutePixel += (visibleChildren - 1) * Spacing;
        }
        panelRemainingSize.Along -= totalAbsolutePixel;
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);
        double alongOffset = 0;
        
        // To avoid divide by 0
        if (totalStar is 0) totalStar = 1;
        var starLength = panelRemainingSize.Along / totalStar;
        
        foreach (var child in Children)
        {
            if (child.Visibility is not Visibility.Visible) continue;
            var length = LengthProperty.GetValue(child);
            if (length.IsAuto)
            {
                var desiredSize = SizeToOF(child.DesiredSize);
                child.Arrange(new(
                    OFToPoint((alongOffset, 0)),
                    OFToSize((desiredSize.Along, panelRemainingSize.Opposite))
                ));
                alongOffset += desiredSize.Along + Spacing;
                panelRemainingSize.Along -= desiredSize.Along + Spacing;
            }
            else if (length.IsStar)
            {
                var computedLength = starLength * length.Value;
                child.Arrange(new(
                    OFToPoint((alongOffset, 0)),
                    OFToSize((computedLength, panelRemainingSize.Opposite))
                ));
                alongOffset += computedLength + Spacing;
                panelRemainingSize.Along -= computedLength + Spacing;
            }
            else
            {
                child.Arrange(new(
                    OFToPoint((alongOffset, 0)),
                    OFToSize((length.Value, panelRemainingSize.Opposite))
                ));
                alongOffset += length.Value + Spacing;
                panelRemainingSize.Along -= length.Value + Spacing;
            }
        }
        panelRemainingSize.Along = Math.Max(panelRemainingSize.Along, 0);

#if DEBUG
        if (Tag is "Debug")
            Debugger.Break();
#endif
        return OFToSize((panelSize.Along - panelRemainingSize.Along, Math.Min(panelRemainingSize.Opposite, panelSize.Opposite)));
    }
}
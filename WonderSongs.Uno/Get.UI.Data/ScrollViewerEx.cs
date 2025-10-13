using Get.Data.Bundles;
using Get.Data.Properties;

namespace Get.UI.Data;
[AutoProperty]
public partial class ScrollViewerEx : TemplateControl<ScrollViewer>
{
    public IProperty<IContentBundle<UIElement>?> ContentProperty { get; } = Auto<IContentBundle<UIElement>?>(null);
    public IProperty<ScrollMode> HorizontalScrollModeProperty { get; } = Auto(ScrollMode.Auto);
    public IProperty<ScrollMode> VerticalScrollModeProperty { get; } = Auto(ScrollMode.Auto);
    protected override void Initialize(ScrollViewer rootElement)
    {
        ScrollViewer.HorizontalScrollModeProperty.AsProperty<ScrollViewer, ScrollMode>(rootElement)
            .BindOneWay(HorizontalScrollModeProperty);
        ScrollViewer.VerticalScrollModeProperty.AsProperty<ScrollViewer, ScrollMode>(rootElement)
            .BindOneWay(VerticalScrollModeProperty);
        rootElement.Content = new ContentBundleControl
        {
            ContentBundleBinding = OneWay(ContentProperty)
        };
    }
}
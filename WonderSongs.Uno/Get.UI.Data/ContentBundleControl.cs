#nullable enable
using Get.Data.Bindings;
using Get.Data.Bindings.Linq;
using Get.Data.Bundles;
using Get.Data.Properties;
using static Get.Data.Properties.AutoTyper;
namespace Get.UI.Data;
[AutoProperty]
public partial class ContentBundleControl : TemplateControl<Border>
{
    public IProperty<IContentBundle<UIElement>?> ContentBundleProperty { get; } = Auto<IContentBundle<UIElement>?>(default);
    protected override void Initialize(Border rootElement)
    {
        if (Tag is "Debug")
            Debugger.Break();
        var prop = ContentBundleProperty.SelectPath(x => x!.OutputContent);
        prop.ValueChanged += (_, child) => rootElement.Child = child;
        rootElement.Child = prop.CurrentValue;
    }
    public ContentBundleControl()
    {
    }
}
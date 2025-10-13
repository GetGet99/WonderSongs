#nullable enable
using Get.Data.DataTemplates;
using Get.Data.Properties;
using Get.Data.Collections.Conversion;
using Get.Data.Bundles;

using static Get.Data.Properties.AutoTyper;
namespace Get.UI.Data;

[AutoProperty]
public partial class ItemsBundleControl<T> : TemplateControl<T> where T : Panel, new()
{
    public IProperty<IReadOnlyItemsBundle<UIElement>?> ItemsBundleProperty { get; } = Auto<IReadOnlyItemsBundle<UIElement>?>(default);
    IDisposable? disposable;
    protected override void Initialize(T rootElement)
    {
        disposable = ItemsBundle?.OutputContent.Bind(rootElement.Children.AsGDCollection());
        ItemsBundleProperty.ValueChanged += (_, @new) =>
        {
            disposable?.Dispose();
            disposable = @new?.OutputContent.Bind(rootElement.Children.AsGDCollection());
        };
    }
}
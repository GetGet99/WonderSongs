#nullable enable
using Get.Data.Bundles;
using Get.Data.Properties;
using static Get.Data.Properties.AutoTyper;

namespace Get.UI.Data;
[AutoProperty]
public abstract partial class TypedTemplateContentControl<TContent, TTargetElement, TRootElement>
    : TemplateControl<TRootElement>
    where TRootElement : UIElement, new()
    where TTargetElement : UIElement
{
    public IProperty<ContentBundle<TContent, TTargetElement>?> ContentBundleProperty { get; }
        = Auto<ContentBundle<TContent, TTargetElement>?>(null);

}
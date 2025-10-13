using Get.Data.Properties;

namespace Get.UI.Data;
[AutoProperty]
public partial class UIElementContentControl : TemplateControl<Border>
{
    public IProperty<UIElement?> ContentProperty { get; } = Auto<UIElement?>(null);

    protected override void Initialize(Border rootElement)
    {
        ContentProperty.ApplyAndRegisterForNewValue((_, x) =>
        {
            rootElement.Child = x;
        });
    }
}
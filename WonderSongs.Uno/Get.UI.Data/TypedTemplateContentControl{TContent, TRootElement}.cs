
namespace Get.UI.Data;

public abstract partial class TypedTemplateContentControl<TContent, TRootElement> :
    TypedTemplateContentControl<TContent, UIElement, TRootElement>
    where TRootElement : UIElement, new();
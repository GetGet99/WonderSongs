
namespace Get.UI.Data;

public abstract partial class CachedTemplateControl<T> : TemplateControl<UserControl> where T : UIElement, new()
{
    T? ele;
    protected sealed override void Initialize(UserControl rootElement)
    {
        if (ele == null)
        {
            ele = new();
            Initialize(ele);
        } else
        {
            Debugger.Break();
        }
        rootElement.Content = ele;
    }
    protected abstract void Initialize(T rootElement);
}
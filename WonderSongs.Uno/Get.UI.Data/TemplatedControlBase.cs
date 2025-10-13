
namespace Get.UI.Data;

public abstract partial class TemplatedControlBase : Control
{
    protected const string TemplateChildName = "TemplatedControlChild";
    protected static ControlTemplate BuildTemplate<T>()
    {
        var typeNamespace = (typeof(T).Namespace ?? "");
#if UWPNET9
        if (!typeNamespace.StartsWith("Windows.UI.Xaml.Controls"))
#else

        if (!(typeNamespace.StartsWith("Microsoft.UI.Xaml.Controls") || typeNamespace.StartsWith("Windows.UI.Xaml.Controls")))
#endif
            // XAML probably does not know the type T
            return BuildTemplate<UserControl>();

        return (ControlTemplate)XamlReader.Load(
        $"""
        <{nameof(ControlTemplate)}
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:templateControlRoot="using:{typeof(TemplatedControlBase).Namespace}"
            xmlns:userControlRoot="using:{typeof(T).Namespace}">
            <userControlRoot:{typeof(T).Name} x:Name="{TemplateChildName}" />
        </{nameof(ControlTemplate)}>
        """);
    }
    internal TemplatedControlBase(ControlTemplate template)
    {
        Template = template;
    }

}
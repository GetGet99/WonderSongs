namespace Get.UI.Data;

public partial class NamedPanel : Panel
{
    public NamedPanel()
    {
        Name = GetType().ToReadableString();
    }
}
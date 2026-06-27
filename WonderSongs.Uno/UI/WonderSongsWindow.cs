using Microsoft.UI;
using Windows.UI.ViewManagement;

namespace WonderSongs.UI;
class WonderSongsWindow : Window
{
#if !HAS_UNO
    public WonderSongsWindow()
    {
        SystemBackdrop = new MicaBackdrop();
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        UISettings.ColorValuesChanged += (_, _) => SetButtonBG();
        SetButtonBG();
    }
    void SetButtonBG()
    {
        if (UISettings.GetColorValue(UIColorType.Background).R < 255 / 2)
            AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
        else
            AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
    }
    static UISettings UISettings { get; } = new();
#endif
}

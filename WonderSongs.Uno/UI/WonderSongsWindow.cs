using Microsoft.UI;
using Windows.UI.ViewManagement;

namespace WonderSongs.UI;
class WonderSongsWindow : Window
{
    public WonderSongsWindow()
    {
#if !HAS_UNO
        SystemBackdrop = new MicaBackdrop();
#endif
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        UISettings.ColorValuesChanged += (_, _) => SetButtonBG();
        SetButtonBG();
    }
    void SetButtonBG()
    {
#if !HAS_UNO
        if (UISettings.GetColorValue(UIColorType.Background).R < 255 / 2)
            AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
        else
            AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
#endif
    }
    static UISettings UISettings { get; } = new();
}

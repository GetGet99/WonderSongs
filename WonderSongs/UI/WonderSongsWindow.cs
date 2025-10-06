using Microsoft.UI;
using Windows.UI.ViewManagement;

namespace WonderSongs.UI;

abstract class WonderSongsWindow : TemplateControl<Page>
{
    public Window Window { get; } = new();
    public WonderSongsWindow()
    {
        Window.Content = this;
        Window.SystemBackdrop = new MicaBackdrop();
        Window.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        UISettings.ColorValuesChanged += (_, _) => SetButtonBG();
        SetButtonBG();
    }
    void SetButtonBG()
    {
        if (UISettings.GetColorValue(UIColorType.Background).R < 255 / 2)
            Window.AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
        else
            Window.AppWindow.TitleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
    }
    static UISettings UISettings { get; } = new();
}

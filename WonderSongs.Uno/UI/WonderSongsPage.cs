using Windows.Storage.Pickers;
using WonderSongs.Core;

namespace WonderSongs.UI;

[QuickMarkup("""
    <setup>
    var theme = this.UseThemeBrushes();
    </setup>
    <root Background=`HasUno ? theme.SolidBackground : null` />
    """)]
partial class WonderSongsPage : Page
{
    const bool HasUno =
#if HAS_UNO
        true
#else
        false
#endif
        ;
}

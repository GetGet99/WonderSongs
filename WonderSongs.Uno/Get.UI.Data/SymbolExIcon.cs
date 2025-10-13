using System;
using System.Collections.Generic;
using System.Text;

namespace Get.Symbols;

public partial class SymbolExIcon : FontIcon
{
    public static int? DefaultIconSize = null;
    public SymbolExIcon()
    {
        if (DefaultIconSize.HasValue)
            FontSize = DefaultIconSize.Value;
        if (Resources["SymbolThemeFontFamily"] is FontFamily f)
            FontFamily = f;
        Loaded += delegate
        {
            if (Resources["SymbolThemeFontFamily"] is FontFamily f2)
                FontFamily = f2;
        };
    }
    public SymbolExIcon(SymbolEx SymbolEx) : this()
    {
        this.SymbolEx = SymbolEx;
    }
    public SymbolExIcon(Symbol Symbol) : this()
    {
        this.Symbol = Symbol;
    }
    public SymbolEx SymbolEx
    {
        get => (SymbolEx)Glyph[0];
        set => Glyph = ((char)value).ToString();
    }
    public Symbol Symbol
    {
        get => (Symbol)Glyph[0];
        set => Glyph = ((char)value).ToString();
    }
}

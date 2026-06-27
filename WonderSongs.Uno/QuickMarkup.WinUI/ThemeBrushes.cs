namespace QuickMarkup.WinUI;

// The ThemeBrushes class is modified and inspired from Theme class
// of https://github.com/microsoft/microsoft-ui-reactor/blob/main/src/Reactor/Core/Theme.cs

public class ThemeBrushes(FrameworkElement? element)
{
    public static ThemeBrushes Global { get; } = new(null);
    // ── Accent / Fill ────────────────────────────────────────────────
    public Reference<Brush?> AccentProp               => field ??= ThemeResources.Get<Brush>("AccentFillColorDefaultBrush", element);
    public Brush? Accent                              => AccentProp.Value;
    public Reference<Brush?> AccentSecondaryProp      => field ??= ThemeResources.Get<Brush>("AccentFillColorSecondaryBrush", element);
    public Brush? AccentSecondary                     => AccentSecondaryProp.Value;
    public Reference<Brush?> AccentTertiaryProp       => field ??= ThemeResources.Get<Brush>("AccentFillColorTertiaryBrush", element);
    public Brush? AccentTertiary                      => AccentTertiaryProp.Value;
    public Reference<Brush?> AccentDisabledProp       => field ??= ThemeResources.Get<Brush>("AccentFillColorDisabledBrush", element);
    public Brush? AccentDisabled                      => AccentDisabledProp.Value;

    // ── Text ─────────────────────────────────────────────────────────
    public Reference<Brush?> PrimaryTextProp          => field ??= ThemeResources.Get<Brush>("TextFillColorPrimaryBrush", element);
    public Brush? PrimaryText                         => PrimaryTextProp.Value;
    public Reference<Brush?> SecondaryTextProp        => field ??= ThemeResources.Get<Brush>("TextFillColorSecondaryBrush", element);
    public Brush? SecondaryText                       => SecondaryTextProp.Value;
    public Reference<Brush?> TertiaryTextProp         => field ??= ThemeResources.Get<Brush>("TextFillColorTertiaryBrush", element);
    public Brush? TertiaryText                        => TertiaryTextProp.Value;
    public Reference<Brush?> DisabledTextProp         => field ??= ThemeResources.Get<Brush>("TextFillColorDisabledBrush", element);
    public Brush? DisabledText                        => DisabledTextProp.Value;
    public Reference<Brush?> AccentTextProp           => field ??= ThemeResources.Get<Brush>("AccentTextFillColorPrimaryBrush", element);
    public Brush? AccentText                          => AccentTextProp.Value;

    // ── Surfaces / Fill ──────────────────────────────────────────────
    public Reference<Brush?> SolidBackgroundProp   => field ??= ThemeResources.Get<Brush>("SolidBackgroundFillColorBaseBrush", element);
    public Brush? SolidBackground                  => SolidBackgroundProp.Value;
    public Reference<Brush?> CardBackgroundProp    => field ??= ThemeResources.Get<Brush>("CardBackgroundFillColorDefaultBrush", element);
    public Brush? CardBackground                   => CardBackgroundProp.Value;
    public Reference<Brush?> SmokeFillProp         => field ??= ThemeResources.Get<Brush>("SmokeFillColorDefaultBrush", element);
    public Brush? SmokeFill                        => SmokeFillProp.Value;
    public Reference<Brush?> SubtleFillProp        => field ??= ThemeResources.Get<Brush>("SubtleFillColorSecondaryBrush", element);
    public Brush? SubtleFill                       => SubtleFillProp.Value;
    public Reference<Brush?> LayerFillProp         => field ??= ThemeResources.Get<Brush>("LayerFillColorDefaultBrush", element);
    public Brush? LayerFill                        => LayerFillProp.Value;

    // ── Control Fill ─────────────────────────────────────────────────
    public Reference<Brush?> ControlFillProp              => field ??= ThemeResources.Get<Brush>("ControlFillColorDefaultBrush", element);
    public Brush? ControlFill                             => ControlFillProp.Value;
    public Reference<Brush?> ControlFillSecondaryProp     => field ??= ThemeResources.Get<Brush>("ControlFillColorSecondaryBrush", element);
    public Brush? ControlFillSecondary                    => ControlFillSecondaryProp.Value;
    public Reference<Brush?> ControlFillTertiaryProp      => field ??= ThemeResources.Get<Brush>("ControlFillColorTertiaryBrush", element);
    public Brush? ControlFillTertiary                     => ControlFillTertiaryProp.Value;
    public Reference<Brush?> ControlFillDisabledProp      => field ??= ThemeResources.Get<Brush>("ControlFillColorDisabledBrush", element);
    public Brush? ControlFillDisabled                     => ControlFillDisabledProp.Value;
    public Reference<Brush?> ControlFillInputActiveProp   => field ??= ThemeResources.Get<Brush>("ControlFillColorInputActiveBrush", element);
    public Brush? ControlFillInputActive                  => ControlFillInputActiveProp.Value;

    // ── Stroke / Border ──────────────────────────────────────────────
    public Reference<Brush?> CardStrokeProp        => field ??= ThemeResources.Get<Brush>("CardStrokeColorDefaultBrush", element);
    public Brush? CardStroke                       => CardStrokeProp.Value;
    public Reference<Brush?> SurfaceStrokeProp     => field ??= ThemeResources.Get<Brush>("SurfaceStrokeColorDefaultBrush", element);
    public Brush? SurfaceStroke                    => SurfaceStrokeProp.Value;
    public Reference<Brush?> DividerStrokeProp     => field ??= ThemeResources.Get<Brush>("DividerStrokeColorDefaultBrush", element);
    public Brush? DividerStroke                    => DividerStrokeProp.Value;
    public Reference<Brush?> ControlStrokeProp     => field ??= ThemeResources.Get<Brush>("ControlStrokeColorDefaultBrush", element);
    public Brush? ControlStroke                    => ControlStrokeProp.Value;
    public Reference<Brush?> ControlStrokeSecondaryProp => field ??= ThemeResources.Get<Brush>("ControlStrokeColorSecondaryBrush", element);
    public Brush? ControlStrokeSecondary                => ControlStrokeSecondaryProp.Value;

    // ── Signal ───────────────────────────────────────────────────────
    public Reference<Brush?> SystemAttentionProp   => field ??= ThemeResources.Get<Brush>("SystemFillColorAttentionBrush", element);
    public Brush? SystemAttention                  => SystemAttentionProp.Value;
    public Reference<Brush?> SystemSuccessProp     => field ??= ThemeResources.Get<Brush>("SystemFillColorSuccessBrush", element);
    public Brush? SystemSuccess                    => SystemSuccessProp.Value;
    public Reference<Brush?> SystemCautionProp     => field ??= ThemeResources.Get<Brush>("SystemFillColorCautionBrush", element);
    public Brush? SystemCaution                    => SystemCautionProp.Value;
    public Reference<Brush?> SystemCriticalProp    => field ??= ThemeResources.Get<Brush>("SystemFillColorCriticalBrush", element);
    public Brush? SystemCritical                   => SystemCriticalProp.Value;
    public Reference<Brush?> SystemNeutralProp     => field ??= ThemeResources.Get<Brush>("SystemFillColorNeutralBrush", element);
    public Brush? SystemNeutral                    => SystemNeutralProp.Value;
    public Reference<Brush?> SystemSolidNeutralProp => field ??= ThemeResources.Get<Brush>("SystemFillColorSolidNeutralBrush", element);
    public Brush? SystemSolidNeutral                => SystemSolidNeutralProp.Value;

    public Reference<Brush?> SystemAttentionBackgroundProp => field ??= ThemeResources.Get<Brush>("SystemFillColorAttentionBackgroundBrush", element);
    public Brush? SystemAttentionBackground                => SystemAttentionBackgroundProp.Value;
    public Reference<Brush?> SystemSuccessBackgroundProp   => field ??= ThemeResources.Get<Brush>("SystemFillColorSuccessBackgroundBrush", element);
    public Brush? SystemSuccessBackground                  => SystemSuccessBackgroundProp.Value;
    public Reference<Brush?> SystemCautionBackgroundProp   => field ??= ThemeResources.Get<Brush>("SystemFillColorCautionBackgroundBrush", element);
    public Brush? SystemCautionBackground                  => SystemCautionBackgroundProp.Value;
    public Reference<Brush?> SystemCriticalBackgroundProp  => field ??= ThemeResources.Get<Brush>("SystemFillColorCriticalBackgroundBrush", element);
    public Brush? SystemCriticalBackground                 => SystemCriticalBackgroundProp.Value;
    public Reference<Brush?> SystemNeutralBackgroundProp   => field ??= ThemeResources.Get<Brush>("SystemFillColorNeutralBackgroundBrush", element);
    public Brush? SystemNeutralBackground                  => SystemNeutralBackgroundProp.Value;
    public Reference<Brush?> SystemSolidAttentionProp       => field ??= ThemeResources.Get<Brush>("SystemFillColorSolidAttentionBackgroundBrush", element);
    public Brush? SystemSolidAttention                      => SystemSolidAttentionProp.Value;
}

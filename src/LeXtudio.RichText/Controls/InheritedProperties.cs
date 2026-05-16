using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
using System.Windows.Documents;

namespace LeXtudio.UI.Xaml.Controls;

internal record InheritedProperties(
    FontFamily FontFamily,
    double FontSize,
    FontWeight FontWeight,
    FontStyle FontStyle,
    global::Windows.UI.Text.FontStretch FontStretch,
    int CharacterSpacing,
    Brush Foreground,
    TextDecorations TextDecorations)
{
    // The WPF-shim Inline/Block auto-property returns a non-null default Black brush even when
    // never set, and creates a fresh instance per element so reference-equality to a sentinel
    // doesn't work. As a pragmatic compromise we treat "SolidColorBrush(Black)" as "not set"
    // and inherit from the parent. The trade-off: explicitly setting Foreground = Brushes.Black
    // on a child to override a non-black inherited value won't be honored.
    internal static bool IsExplicitForeground(Brush? brush) =>
        brush is not null && !(brush is SolidColorBrush scb && scb.Color == Microsoft.UI.Colors.Black);

    // Probe the shim's default values once so we can detect "user explicitly set" per font prop.
    // Same trade-off as Foreground: a user explicitly assigning the default value won't override
    // a differing inherited value.
    private static readonly Run _shimProbe = new();
    internal static readonly FontFamily? DefaultShimInlineFontFamily = _shimProbe.FontFamily;
    internal static readonly double DefaultShimInlineFontSize = _shimProbe.FontSize;
    internal static readonly global::Windows.UI.Text.FontWeight DefaultShimInlineFontWeight = _shimProbe.FontWeight;
    internal static readonly global::Windows.UI.Text.FontStyle DefaultShimInlineFontStyle = _shimProbe.FontStyle;
    internal static readonly global::Windows.UI.Text.FontStretch DefaultShimInlineFontStretch = _shimProbe.FontStretch;
    internal static readonly int DefaultShimInlineCharacterSpacing = _shimProbe.CharacterSpacing;

    internal static bool IsExplicitFontFamily(FontFamily? f) =>
        f is not null && f.Source != (DefaultShimInlineFontFamily?.Source ?? string.Empty);
    internal static bool IsExplicitFontSize(double size) =>
        !double.IsNaN(size) && size != DefaultShimInlineFontSize;
    internal static bool IsExplicitFontWeight(global::Windows.UI.Text.FontWeight w) =>
        w.Weight != DefaultShimInlineFontWeight.Weight;
    internal static bool IsExplicitFontStyle(global::Windows.UI.Text.FontStyle s) =>
        s != DefaultShimInlineFontStyle;
    internal static bool IsExplicitFontStretch(global::Windows.UI.Text.FontStretch s) =>
        s != DefaultShimInlineFontStretch;
    internal static bool IsExplicitCharacterSpacing(int v) =>
        v != DefaultShimInlineCharacterSpacing;

    internal InheritedProperties Merge(Inline inline) => this with
    {
        FontFamily      = IsExplicitFontFamily(inline.FontFamily) ? inline.FontFamily! : FontFamily,
        FontSize        = IsExplicitFontSize(inline.FontSize) ? inline.FontSize : FontSize,
        FontWeight      = inline is Bold ? FontWeights.Bold
                          : IsExplicitFontWeight(inline.FontWeight) ? ConvertFontWeight(inline.FontWeight)
                          : FontWeight,
        FontStyle       = inline is Italic ? global::Windows.UI.Text.FontStyle.Italic
                          : IsExplicitFontStyle(inline.FontStyle) ? ConvertFontStyle(inline.FontStyle)
                          : FontStyle,
        FontStretch     = IsExplicitFontStretch(inline.FontStretch) ? inline.FontStretch : FontStretch,
        CharacterSpacing = IsExplicitCharacterSpacing(inline.CharacterSpacing) ? inline.CharacterSpacing : CharacterSpacing,
        Foreground      = IsExplicitForeground(inline.Foreground) ? inline.Foreground! : Foreground,
        TextDecorations = inline is Underline ? TextDecorations | global::Windows.UI.Text.TextDecorations.Underline : TextDecorations,
    };

    internal static global::Windows.UI.Text.FontWeight ConvertFontWeight(FontWeight wpfWeight)
    {
        return wpfWeight.Weight switch
        {
            100 => FontWeights.Thin,
            200 => FontWeights.ExtraLight,
            300 => FontWeights.Light,
            350 => FontWeights.SemiLight,
            400 => FontWeights.Normal,
            500 => FontWeights.Medium,
            600 => FontWeights.SemiBold,
            700 => FontWeights.Bold,
            800 => FontWeights.ExtraBold,
            900 => FontWeights.Black,
            950 => FontWeights.ExtraBlack,
            _ => FontWeights.Normal
        };
    }

    private static FontStyle ConvertFontStyle(FontStyle wpfStyle) =>
        wpfStyle == global::Windows.UI.Text.FontStyle.Italic ? global::Windows.UI.Text.FontStyle.Italic : global::Windows.UI.Text.FontStyle.Normal;

    internal string ToPretextFontString()
    {
        var sb = new System.Text.StringBuilder();
        if (FontStyle == FontStyle.Italic) sb.Append("italic ");
        var weight = FontWeight.Weight;
        if (weight >= 700) sb.Append("bold ");
        else if (weight != 400) sb.Append($"{weight} ");
        var stretch = ToCssFontStretch(FontStretch);
        if (stretch.Length > 0) sb.Append(stretch).Append(' ');
        sb.Append($"{FontSize}px ");
        sb.Append(FontFamily?.ToString() ?? "sans-serif");
        return sb.ToString().TrimEnd();
    }

    internal static double FontStretchScale(global::Windows.UI.Text.FontStretch stretch) => stretch switch
    {
        global::Windows.UI.Text.FontStretch.UltraCondensed => 0.5,
        global::Windows.UI.Text.FontStretch.ExtraCondensed => 0.625,
        global::Windows.UI.Text.FontStretch.Condensed => 0.75,
        global::Windows.UI.Text.FontStretch.SemiCondensed => 0.875,
        global::Windows.UI.Text.FontStretch.SemiExpanded => 1.125,
        global::Windows.UI.Text.FontStretch.Expanded => 1.25,
        global::Windows.UI.Text.FontStretch.ExtraExpanded => 1.5,
        global::Windows.UI.Text.FontStretch.UltraExpanded => 2.0,
        _ => 1.0
    };

    private static string ToCssFontStretch(global::Windows.UI.Text.FontStretch stretch) => stretch switch
    {
        global::Windows.UI.Text.FontStretch.UltraCondensed => "ultra-condensed",
        global::Windows.UI.Text.FontStretch.ExtraCondensed => "extra-condensed",
        global::Windows.UI.Text.FontStretch.Condensed => "condensed",
        global::Windows.UI.Text.FontStretch.SemiCondensed => "semi-condensed",
        global::Windows.UI.Text.FontStretch.SemiExpanded => "semi-expanded",
        global::Windows.UI.Text.FontStretch.Expanded => "expanded",
        global::Windows.UI.Text.FontStretch.ExtraExpanded => "extra-expanded",
        global::Windows.UI.Text.FontStretch.UltraExpanded => "ultra-expanded",
        _ => string.Empty
    };
}

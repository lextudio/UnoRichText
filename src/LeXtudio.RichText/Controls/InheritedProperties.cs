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
    Brush Foreground,
    TextDecorations TextDecorations)
{
    internal InheritedProperties Merge(Inline inline) => this with
    {
        FontFamily      = inline.FontFamily != null ? ConvertFontFamily(inline.FontFamily) : FontFamily,
        FontSize        = double.IsNaN(inline.FontSize) ? FontSize : inline.FontSize,
        FontWeight      = inline is Bold ? FontWeights.Bold : inline.FontWeight.HasValue ? ConvertFontWeight(inline.FontWeight.Value) : FontWeight,
        FontStyle       = inline is Italic ? global::Windows.UI.Text.FontStyle.Italic : inline.FontStyle.HasValue ? ConvertFontStyle(inline.FontStyle.Value) : FontStyle,
        Foreground      = inline.Foreground ?? Foreground,
        TextDecorations = inline is Underline ? TextDecorations | global::Windows.UI.Text.TextDecorations.Underline : TextDecorations,
    };

    private static FontFamily ConvertFontFamily(System.Windows.FontFamily wpfFamily) =>
        new(wpfFamily.ToString());

    private static FontWeight ConvertFontWeight(System.Windows.FontWeight wpfWeight)
    {
        // Convert from System.Windows.FontWeight to Windows.UI.Text.FontWeight
        var openTypeWeight = wpfWeight.ToOpenTypeWeight();
        return openTypeWeight switch
        {
            100 => FontWeights.Thin,
            200 => FontWeights.ExtraLight,
            300 => FontWeights.Light,
            400 => FontWeights.Normal,
            500 => FontWeights.Medium,
            600 => FontWeights.SemiBold,
            700 => FontWeights.Bold,
            800 => FontWeights.ExtraBold,
            900 => FontWeights.Black,
            _ => FontWeights.Normal
        };
    }

    private static FontStyle ConvertFontStyle(System.Windows.FontStyle wpfStyle) =>
        wpfStyle.ToString() == "Italic" ? global::Windows.UI.Text.FontStyle.Italic : global::Windows.UI.Text.FontStyle.Normal;

    internal string ToPretextFontString()
    {
        var sb = new System.Text.StringBuilder();
        if (FontStyle == FontStyle.Italic) sb.Append("italic ");
        var weight = FontWeight.Weight;
        if (weight >= 700) sb.Append("bold ");
        else if (weight != 400) sb.Append($"{weight} ");
        sb.Append($"{FontSize}px ");
        sb.Append(FontFamily?.ToString() ?? "sans-serif");
        return sb.ToString().TrimEnd();
    }
}

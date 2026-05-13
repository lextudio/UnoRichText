using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;
using LeXtudio.UI.Xaml.Documents;

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
        FontFamily      = inline.FontFamily ?? FontFamily,
        FontSize        = double.IsNaN(inline.FontSize) ? FontSize : inline.FontSize,
        FontWeight      = inline.FontWeight ?? FontWeight,
        FontStyle       = inline.FontStyle ?? FontStyle,
        Foreground      = inline.Foreground ?? Foreground,
        TextDecorations = inline.TextDecorations | TextDecorations,
    };

    internal string ToPretextFontString()
    {
        var sb = new System.Text.StringBuilder();
        if (FontStyle == FontStyle.Italic) sb.Append("italic ");
        var weight = FontWeight.Weight;
        if (weight >= 700) sb.Append("bold ");
        else if (weight != 400) sb.Append($"{weight} ");
        sb.Append($"{FontSize}px ");
        sb.Append(FontFamily?.Source ?? "sans-serif");
        return sb.ToString().TrimEnd();
    }
}

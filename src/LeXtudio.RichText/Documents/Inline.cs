using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace LeXtudio.UI.Xaml.Documents;

public abstract class Inline
{
    public FontFamily? FontFamily { get; set; }

    public double FontSize { get; set; } = double.NaN;

    public FontWeight? FontWeight { get; set; }

    public FontStyle? FontStyle { get; set; }

    public Brush? Foreground { get; set; }

    public TextDecorations TextDecorations { get; set; } = TextDecorations.None;
}

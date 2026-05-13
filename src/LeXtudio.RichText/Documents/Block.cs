using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace LeXtudio.UI.Xaml.Documents;

public abstract class Block
{
    public Thickness Margin { get; set; }
    public TextAlignment TextAlignment { get; set; }
    public double FontSize { get; set; } = double.NaN;
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
    public double LineHeight { get; set; } = double.NaN;
    public Brush? Foreground { get; set; }
    public FontFamily? FontFamily { get; set; }
}

using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace System.Windows.Documents;

public abstract partial class Block
{
    public double FontSize { get; set; } = double.NaN;
    public Brush? Foreground { get; set; }
    public FontFamily? FontFamily { get; set; }
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
}

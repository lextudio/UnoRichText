using Microsoft.UI.Xaml;

namespace LeXtudio.UI.Xaml.Controls;

internal sealed class RichTextBlockTextLayoutHost(RichTextBlock owner) : System.Windows.Documents.ITextLayoutHost
{
    public UIElement RenderScope => owner;

    public bool IsLayoutValid => owner.IsTextLayoutValid;

    public double ViewportWidth => owner.ActualWidth;

    public double ViewportHeight => owner.ActualHeight;

    public double ExtentHeight => owner.ExtentHeight;

    public void InvalidateLayout()
    {
        owner.InvalidateMeasure();
    }
}

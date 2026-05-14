using Microsoft.UI.Xaml;

namespace System.Windows.Documents;

public interface ITextLayoutHost
{
    UIElement RenderScope { get; }
    bool IsLayoutValid { get; }
    double ViewportWidth { get; }
    double ViewportHeight { get; }
    double ExtentHeight { get; }

    void InvalidateLayout();
}

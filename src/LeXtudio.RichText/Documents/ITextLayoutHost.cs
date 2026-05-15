namespace System.Windows.Documents;

public interface ITextLayoutHost
{
    object RenderScope { get; }
    bool IsLayoutValid { get; }
    double ViewportWidth { get; }
    double ViewportHeight { get; }
    double ExtentHeight { get; }

    void InvalidateLayout();
}

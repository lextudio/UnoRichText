using Microsoft.UI.Xaml;

namespace MS.Internal.Documents;

internal sealed class UIElementIsland : System.IDisposable
{
    public UIElementIsland(UIElement root) => Root = root;
    public UIElement Root { get; }
    public void Dispose()
    {
    }
}

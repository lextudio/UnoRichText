namespace LeXtudio.UI.Xaml.Documents;

public class Hyperlink : Span
{
    public event EventHandler? Click;

    internal void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
}

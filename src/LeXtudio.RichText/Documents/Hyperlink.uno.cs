namespace System.Windows.Documents;

public partial class Hyperlink
{
    public event EventHandler? Click;

    internal void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
}

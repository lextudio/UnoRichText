namespace System.Windows.Documents;

public partial class Hyperlink
{
    internal void RaiseClick() => OnClick();
}

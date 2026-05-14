namespace System.Windows.Documents;

public sealed class FlowDocument : TextElement
{
    private readonly BlockCollection _blocks;

    public FlowDocument()
    {
        _blocks = new BlockCollection(this, true);
    }

    public new object? Parent { get; set; }

    public BlockCollection Blocks => _blocks;

    public ITextLayoutHost? TextLayoutHost
    {
        get => LayoutHost;
        set => SetLayoutHostRecursive(value);
    }
}

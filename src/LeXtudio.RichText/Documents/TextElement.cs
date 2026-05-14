namespace System.Windows.Documents;

/// <summary>
/// Base class for rich text document elements.
/// Microsoft WPF source reference:
/// System/Windows/Documents/TextElement.cs (captured in docs/PROVENANCE.md).
/// </summary>
public abstract partial class TextElement : System.Windows.DependencyObject
{
    protected static readonly DependencyPropertyShim DefaultStyleKeyProperty = new();
    private readonly TextPointer _contentStart = new();
    private readonly TextPointer _contentEnd = new();
    private readonly TextContainer _textContainer = new();

    public TextPointer ContentStart => _contentStart;
    public TextPointer ContentEnd => _contentEnd;
    public object? Parent { get; internal set; }
    internal TextContainer TextContainer => _textContainer;
    internal TextElement? NextElement => null;
    internal TextElement? PreviousElement => null;

    internal void Reposition(TextPointer start, TextPointer end)
    {
    }

    internal static TextElement GetCommonAncestor(TextElement element1, TextElement element2)
    {
        return element1;
    }

    internal virtual void OnTextUpdated()
    {
    }

    internal virtual void BeforeLogicalTreeChange()
    {
    }

    internal virtual void AfterLogicalTreeChange()
    {
    }

    internal virtual int EffectiveValuesInitialSize => 0;

    internal virtual bool IsIMEStructuralElement => false;
}

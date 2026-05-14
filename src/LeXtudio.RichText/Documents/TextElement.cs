namespace System.Windows.Documents;

/// <summary>
/// Base class for rich text document elements.
/// Microsoft WPF source reference:
/// System/Windows/Documents/TextElement.cs (captured in docs/PROVENANCE.md).
/// </summary>
public abstract partial class TextElement : System.Windows.DependencyObject
{
    protected static readonly DependencyPropertyShim DefaultStyleKeyProperty = new();
    private readonly List<object> _children = [];
    private readonly TextPointer _contentStart;
    private readonly TextPointer _contentEnd;
    private readonly TextContainer _textContainer;

    protected TextElement()
    {
        _textContainer = new TextContainer(this);
        _contentStart = new TextPointer(this, ElementEdge.BeforeStart);
        _contentEnd = new TextPointer(this, ElementEdge.AfterEnd);
    }

    public TextPointer ContentStart => _contentStart;
    public TextPointer ContentEnd => _contentEnd;
    internal TextPointer ElementStart => new(this, ElementEdge.BeforeStart);
    internal TextPointer ElementEnd => new(this, ElementEdge.AfterEnd);
    public object? Parent { get; internal set; }
    internal TextContainer TextContainer => _textContainer;
    internal IReadOnlyList<object> LogicalChildren => _children;
    internal TextElement? NextElement => GetSibling(1);
    internal TextElement? PreviousElement => GetSibling(-1);
    internal ITextLayoutHost? LayoutHost => _textContainer.LayoutHost;

    internal void Reposition(TextPointer start, TextPointer end)
    {
    }

    internal void AddLogicalChild(object child)
    {
        InsertLogicalChild(_children.Count, child);
    }

    internal void InsertLogicalChild(int index, object child)
    {
        if (child is TextElement textElement)
        {
            textElement.Parent = this;
            textElement.SetLayoutHostRecursive(LayoutHost);
        }

        ContainerTextElementField.SetValue(child, this);
        _children.Insert(index, child);
        LayoutHost?.InvalidateLayout();
    }

    internal bool RemoveLogicalChild(object child)
    {
        var removed = _children.Remove(child);
        if (removed)
        {
            if (child is TextElement textElement && ReferenceEquals(textElement.Parent, this))
            {
                textElement.Parent = null;
                textElement.SetLayoutHostRecursive(null);
            }

            ContainerTextElementField.ClearValue(child);
            LayoutHost?.InvalidateLayout();
        }

        return removed;
    }

    internal void ClearLogicalChildren()
    {
        foreach (var child in _children.ToArray())
        {
            RemoveLogicalChild(child);
        }
    }

    internal int IndexOfLogicalChild(object child) => _children.IndexOf(child);

    internal void SetLayoutHostRecursive(ITextLayoutHost? host)
    {
        _textContainer.LayoutHost = host;

        foreach (var child in _children.OfType<TextElement>())
        {
            child.SetLayoutHostRecursive(host);
        }
    }

    private TextElement? GetSibling(int offset)
    {
        if (Parent is not TextElement parent)
        {
            return null;
        }

        var index = parent.IndexOfLogicalChild(this);
        if (index < 0)
        {
            return null;
        }

        index += offset;
        return index >= 0 && index < parent._children.Count ? parent._children[index] as TextElement : null;
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

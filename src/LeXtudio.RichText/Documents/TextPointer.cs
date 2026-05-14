namespace System.Windows.Documents;

/// <summary>
/// Placeholder for the WPF text position type while UnoRichText incrementally
/// ports the document stack. Public constructors that mirror WPF can reference
/// this type before the text container engine is implemented.
/// </summary>
public sealed class TextPointer : ITextPointer
{
    public TextContainer TextContainer { get; } = new();
    public object? Parent { get; set; }
    public System.Type? ParentType { get; set; }
    public Paragraph? Paragraph { get; set; }

    public void InsertInline(Inline inline)
    {
    }

    public int CompareTo(TextPointer other) => 0;
    public int CompareTo(ITextPointer position) => 0;

    public TextPointerContext GetPointerContext(LogicalDirection direction) => TextPointerContext.None;

    public TextPointer GetNextContextPosition(LogicalDirection direction) => this;

    public Inline? GetNonMergeableInlineAncestor() => null;

    public object? GetAdjacentElement(LogicalDirection direction) => null;

    public TextPointer CreatePointer() => this;

    public void MoveToNextContextPosition(LogicalDirection direction)
    {
    }

    public void InsertUIElement(object element)
    {
    }

    public void InsertTextInRun(string text)
    {
    }
}

public class TextContainer
{
    public object? Parent { get; set; }

    public void BeginChange()
    {
    }

    public void EndChange()
    {
    }

    public void DeleteContentInternal(TextPointer start, TextPointer end)
    {
    }
}

public enum LogicalDirection
{
    Backward,
    Forward
}

public enum TextPointerContext
{
    None,
    Text,
    EmbeddedElement,
    ElementStart,
    ElementEnd
}

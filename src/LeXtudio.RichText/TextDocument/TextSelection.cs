// Skeleton TextSelection implementation backing RichEditTextDocument.Selection.
// Inherits TextRange and adds the selection-only members.

namespace Microsoft.UI.Text;

internal sealed class TextSelection : TextRange, ITextSelection
{
    public TextSelection(RichEditTextDocument document) : base(document, 0, 0)
    {
    }

    public SelectionOptions Options { get; set; } = SelectionOptions.None;
    public SelectionType Type => Length == 0 ? SelectionType.InsertionPoint : SelectionType.Normal;

    public int EndKey(TextRangeUnit unit, bool extend)
    {
        int max = _document.Buffer.Length;
        if (extend) SetRange(StartPosition, max);
        else SetRange(max, max);
        return max;
    }

    public int HomeKey(TextRangeUnit unit, bool extend)
    {
        if (extend) SetRange(0, EndPosition);
        else SetRange(0, 0);
        return 0;
    }

    public void MoveDown(TextRangeUnit unit, int count, bool extend) { /* TODO: line nav once layout exists */ }
    public void MoveUp(TextRangeUnit unit, int count, bool extend) { /* TODO */ }

    public void MoveLeft(TextRangeUnit unit, int count, bool extend)
    {
        int target = System.Math.Max(0, StartPosition - count);
        SetRange(target, extend ? EndPosition : target);
    }

    public void MoveRight(TextRangeUnit unit, int count, bool extend)
    {
        int target = System.Math.Min(_document.Buffer.Length, EndPosition + count);
        if (extend) SetRange(StartPosition, target);
        else SetRange(target, target);
    }

    public void TypeText(string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        Text = value;
        // After typing, collapse to end-of-insertion.
        int newPos = StartPosition + value.Length;
        SetRange(newPos, newPos);
    }
}

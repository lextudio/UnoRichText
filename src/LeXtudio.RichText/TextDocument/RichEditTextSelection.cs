// RichEditTextSelection — implementation of Microsoft.UI.Text.ITextSelection.

using ITextSelection = Microsoft.UI.Text.ITextSelection;
using SelectionOptions = Microsoft.UI.Text.SelectionOptions;
using SelectionType = Microsoft.UI.Text.SelectionType;
using TextRangeUnit = Microsoft.UI.Text.TextRangeUnit;

namespace LeXtudio.UI.Text;

internal sealed class RichEditTextSelection : RichEditTextRange, ITextSelection
{
    public RichEditTextSelection(RichEditTextDocument document) : base(document, 0, 0) { }

    public SelectionOptions Options { get; set; } = 0;
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

    public int MoveDown(TextRangeUnit unit, int count, bool extend) => 0;
    public int MoveUp(TextRangeUnit unit, int count, bool extend) => 0;

    public int MoveLeft(TextRangeUnit unit, int count, bool extend)
    {
        int target = System.Math.Max(0, StartPosition - count);
        int moved = StartPosition - target;
        SetRange(target, extend ? EndPosition : target);
        return moved;
    }

    public int MoveRight(TextRangeUnit unit, int count, bool extend)
    {
        int target = System.Math.Min(_document.Buffer.Length, EndPosition + count);
        int moved = target - EndPosition;
        if (extend) SetRange(StartPosition, target);
        else SetRange(target, target);
        return moved;
    }

    public void TypeText(string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        int start = StartPosition;
        int end = EndPosition;

        TextCharacterFormat? replacementFormat = null;
        if (end > start)
            replacementFormat = _document.GetCharacterFormat(start, end);

        _document.DeleteRange(start, end);
        _document.InsertText(start, value, replacementFormat);

        int newPos = start + value.Length;
        SetRange(newPos, newPos);
    }
}

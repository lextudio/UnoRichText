
namespace Microsoft.UI.Text;

public interface ITextSelection : ITextRange
{
    SelectionOptions Options { get; set; }
    SelectionType Type { get; }

    int EndKey(TextRangeUnit unit, bool extend);
    int HomeKey(TextRangeUnit unit, bool extend);
    void MoveDown(TextRangeUnit unit, int count, bool extend);
    void MoveLeft(TextRangeUnit unit, int count, bool extend);
    void MoveRight(TextRangeUnit unit, int count, bool extend);
    void MoveUp(TextRangeUnit unit, int count, bool extend);
    void TypeText(string value);
}

public enum SelectionOptions
{
    None = 0,
    StartActive = 1,
    AtEol = 2,
    Overtype = 4,
    Active = 8,
    Replace = 16,
}

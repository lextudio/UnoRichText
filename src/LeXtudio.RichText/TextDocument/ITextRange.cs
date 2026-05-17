using global::Windows.Foundation;
using global::Windows.Storage.Streams;

namespace Microsoft.UI.Text;

public interface ITextRange
{
    ITextCharacterFormat CharacterFormat { get; set; }
    string Character { get; set; }
    ITextParagraphFormat ParagraphFormat { get; set; }
    string FormattedText { get; set; }
    string Link { get; set; }
    int StartOf { get; }
    int Length { get; }
    int StoryLength { get; }
    int EndPosition { get; set; }
    int StartPosition { get; set; }
    string Text { get; set; }

    bool CanPaste(int format);
    bool ChangeCase(LetterCase value);
    void Collapse(bool start);
    void Copy();
    void Cut();
    void Delete(TextRangeUnit unit, int count);
    int EndOf(TextRangeUnit unit, bool extend);
    bool Expand(TextRangeUnit unit);
    bool FindText(string value, int scanLength, FindOptions options);
    int GetCharacterUtf32(out int position, int offset);
    ITextRange GetClone();
    int GetIndex(TextRangeUnit unit);
    void GetPoint(HorizontalCharacterAlignment horizontalAlign, VerticalCharacterAlignment verticalAlign, PointOptions options, out Point point);
    void GetRect(PointOptions options, out Rect rect, out int hit);
    void GetText(TextGetOptions options, out string value);
    void InsertImage(int width, int height, int ascent, VerticalCharacterAlignment verticalAlign, string alternateText, IRandomAccessStream value);
    bool InRange(ITextRange range);
    bool InStory(ITextRange range);
    bool IsEqual(ITextRange range);
    int Move(TextRangeUnit unit, int count);
    int MoveEnd(TextRangeUnit unit, int count);
    int MoveStart(TextRangeUnit unit, int count);
    void Paste(int format);
    void ScrollIntoView(PointOptions value);
    void SetIndex(TextRangeUnit unit, int index, bool extend);
    void SetPoint(Point point, PointOptions options, bool extend);
    void SetRange(int startPosition, int endPosition);
    void SetText(TextSetOptions options, string value);
}

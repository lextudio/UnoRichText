using global::Windows.Storage.Streams;

namespace Microsoft.UI.Text;

public interface ITextDocument
{
    CaretType CaretType { get; set; }
    int DefaultTabStop { get; set; }
    bool UndoLimit { get; set; }
    ITextSelection Selection { get; }
    RichEditMathMode MathMode { get; set; }
    bool CanCopy();
    bool CanPaste();
    bool CanRedo();
    bool CanUndo();
    void ApplyDisplayUpdates();
    int BatchDisplayUpdates();
    void BeginUndoGroup();
    void EndUndoGroup();
    void GetText(TextGetOptions options, out string value);
    ITextRange GetRange(int startPosition, int endPosition);
    ITextRange GetRangeFromPoint(global::Windows.Foundation.Point point, PointOptions options);
    void LoadFromStream(TextSetOptions options, IRandomAccessStream value);
    void Redo();
    void SaveToStream(TextGetOptions options, IRandomAccessStream value);
    void SetDefaultCharacterFormat(ITextCharacterFormat value);
    void SetDefaultParagraphFormat(ITextParagraphFormat value);
    void SetText(TextSetOptions options, string value);
    void Undo();
    ITextCharacterFormat GetDefaultCharacterFormat();
    ITextParagraphFormat GetDefaultParagraphFormat();
}

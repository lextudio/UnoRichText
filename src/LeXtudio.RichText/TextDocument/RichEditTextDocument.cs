// Skeleton implementation of Microsoft.UI.Text.RichEditTextDocument.
// Real editing behavior (range / selection mutation, formatting, undo/redo,
// stream serialization) will be filled in alongside the RichEditBox engine.

using System;
using System.Text;
using global::Windows.Storage.Streams;

namespace Microsoft.UI.Text;

public sealed class RichEditTextDocument : ITextDocument
{
    private readonly StringBuilder _buffer = new();
    private readonly TextSelection _selection;
    private TextCharacterFormat _defaultCharacterFormat = new();
    private TextParagraphFormat _defaultParagraphFormat = new();
    private int _batchCount;

    public RichEditTextDocument()
    {
        _selection = new TextSelection(this);
    }

    public CaretType CaretType { get; set; } = CaretType.Normal;
    public int DefaultTabStop { get; set; } = 36;
    public bool UndoLimit { get; set; } = true;
    public RichEditMathMode MathMode { get; set; } = RichEditMathMode.NoMath;
    public ITextSelection Selection => _selection;

    internal StringBuilder Buffer => _buffer;

    public bool CanCopy() => _selection.Length > 0;
    public bool CanPaste() => true;
    public bool CanRedo() => false;
    public bool CanUndo() => false;

    public void ApplyDisplayUpdates() { if (_batchCount > 0) _batchCount--; }
    public int BatchDisplayUpdates() => ++_batchCount;
    public void BeginUndoGroup() { /* stub */ }
    public void EndUndoGroup() { /* stub */ }

    public void GetText(TextGetOptions options, out string value)
    {
        value = _buffer.ToString();
    }

    public ITextRange GetRange(int startPosition, int endPosition)
        => new TextRange(this, startPosition, endPosition);

    public ITextRange GetRangeFromPoint(global::Windows.Foundation.Point point, PointOptions options)
        => new TextRange(this, 0, 0);

    public void LoadFromStream(TextSetOptions options, IRandomAccessStream value)
    {
        // TODO: RTF / plain-text reader. For now: clear the buffer.
        _buffer.Clear();
    }

    public void Redo() { /* stub — wired when undo stack lands */ }
    public void Undo() { /* stub — wired when undo stack lands */ }

    public void SaveToStream(TextGetOptions options, IRandomAccessStream value)
    {
        // TODO: write RTF / plain-text serialization through value.AsStreamForWrite().
    }

    public void SetDefaultCharacterFormat(ITextCharacterFormat value)
    {
        if (value is TextCharacterFormat tcf)
            _defaultCharacterFormat = (TextCharacterFormat)tcf.GetClone();
    }

    public void SetDefaultParagraphFormat(ITextParagraphFormat value)
    {
        if (value is TextParagraphFormat tpf)
            _defaultParagraphFormat = (TextParagraphFormat)tpf.GetClone();
    }

    public ITextCharacterFormat GetDefaultCharacterFormat()
        => (ITextCharacterFormat)_defaultCharacterFormat.GetClone();

    public ITextParagraphFormat GetDefaultParagraphFormat()
        => (ITextParagraphFormat)_defaultParagraphFormat.GetClone();

    public void SetText(TextSetOptions options, string value)
    {
        _buffer.Clear();
        if (!string.IsNullOrEmpty(value))
            _buffer.Append(value);
        _selection.SetRange(0, 0);
    }
}

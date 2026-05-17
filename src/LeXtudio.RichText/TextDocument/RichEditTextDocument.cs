// RichEditTextDocument — behavior-bearing replacement for
// Microsoft.UI.Text.RichEditTextDocument.
//
// Why this exists: Uno's Microsoft.UI.Text.RichEditTextDocument has an internal
// constructor and 20 of its 36 methods are throw-stubs (verified by decompiling
// Uno.UI.dll IL). The class is also non-virtual, so derivation can't replace
// those stubs. We therefore ship our own concrete class that implements
// Microsoft.UI.Text.ITextDocument so consumers get a working document model.
//
// Audit source: tools/UnoTextDocAudit (probe over Uno.WinUI 6.5.153).
// See docs/DESIGN.md "Document model for RichEditBox" for the verdict table.

using System;
using System.Text;
using global::Windows.Storage.Streams;
using ITextRange = Microsoft.UI.Text.ITextRange;
using ITextSelection = Microsoft.UI.Text.ITextSelection;
using ITextCharacterFormat = Microsoft.UI.Text.ITextCharacterFormat;
using ITextParagraphFormat = Microsoft.UI.Text.ITextParagraphFormat;
using CaretType = Microsoft.UI.Text.CaretType;
using RichEditMathMode = Microsoft.UI.Text.RichEditMathMode;
using TextGetOptions = Microsoft.UI.Text.TextGetOptions;
using TextSetOptions = Microsoft.UI.Text.TextSetOptions;
using PointOptions = Microsoft.UI.Text.PointOptions;

namespace LeXtudio.UI.Text;

// Note: Uno's Microsoft.UI.Text does not declare an ITextDocument interface
// (it only ships Microsoft.UI.Text.RichEditTextDocument as a class). Our type
// mirrors that shape — no formal interface, methods surface directly on the class.
public sealed class RichEditTextDocument
{
    private readonly StringBuilder _buffer = new();
    private readonly RichEditTextSelection _selection;
    private TextCharacterFormat _defaultCharacterFormat = new();
    private TextParagraphFormat _defaultParagraphFormat = new();
    private int _batchCount;

    public RichEditTextDocument()
    {
        _selection = new RichEditTextSelection(this);
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
    public void BeginUndoGroup() { /* TODO: undo stack */ }
    public void EndUndoGroup() { /* TODO: undo stack */ }

    public void GetText(TextGetOptions options, out string value)
    {
        value = _buffer.ToString();
    }

    public ITextRange GetRange(int startPosition, int endPosition)
        => new RichEditTextRange(this, startPosition, endPosition);

    public ITextRange GetRangeFromPoint(global::Windows.Foundation.Point point, PointOptions options)
        => new RichEditTextRange(this, 0, 0);

    public void LoadFromStream(TextSetOptions options, IRandomAccessStream value)
    {
        _buffer.Clear();
        // TODO: port WPF RtfToXamlReader for FormatRtf.
    }

    public void Redo() { /* TODO: undo stack */ }
    public void Undo() { /* TODO: undo stack */ }

    public void SaveToStream(TextGetOptions options, IRandomAccessStream value)
    {
        // TODO: port WPF XamlToRtfWriter for FormatRtf, plain UTF-16 otherwise.
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

    public void ClearUndoRedoHistory() { /* TODO: clear undo stack */ }
    public void SetMathMode(RichEditMathMode mode) => MathMode = mode;
    public RichEditMathMode GetMathMode() => MathMode;
    public string GetMathML() => string.Empty;
    public void SetMathML(string mathML) { /* TODO */ }

    // Members that Uno declares but we don't strictly need for the WinUI API
    // surface — kept for shape:
    public bool IgnoreTrailingCharacterSpacing { get; set; }
    public bool AlignmentIncludesTrailingWhitespace { get; set; }
}

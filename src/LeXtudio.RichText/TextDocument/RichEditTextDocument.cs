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
using System.Collections.Generic;
using System.Linq;
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
using FormatEffect = Microsoft.UI.Text.FormatEffect;

namespace LeXtudio.UI.Text;

public sealed record RichEditCharacterFormatRun(int Start, int End, TextCharacterFormat Format);

// Note: Uno's Microsoft.UI.Text does not declare an ITextDocument interface
// (it only ships Microsoft.UI.Text.RichEditTextDocument as a class). Our type
// mirrors that shape — no formal interface, methods surface directly on the class.
public sealed class RichEditTextDocument
{
    /// <summary>
    /// Raised after the document buffer is mutated (via SetText, LoadFromStream, …).
    /// Internal-only: lets the hosting RichEditBox sync its editor view.
    /// </summary>
    internal event System.EventHandler? TextChanged;

    internal event System.EventHandler? FormattingChanged;

    private readonly StringBuilder _buffer = new();
    private readonly List<CharacterFormatRun> _characterFormatRuns = new();
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
        _characterFormatRuns.Clear();
        if (!string.IsNullOrEmpty(value))
            _buffer.Append(value);
        _selection.SetRange(0, 0);
        TextChanged?.Invoke(this, System.EventArgs.Empty);
    }

    internal void InsertText(int offset, string text, TextCharacterFormat? format = null)
    {
        text ??= string.Empty;
        if (text.Length == 0)
            return;

        offset = Math.Clamp(offset, 0, _buffer.Length);
        int delta = text.Length;

        TextCharacterFormat? explicitFormat = format is null ? null : (TextCharacterFormat)format.GetClone();
        bool inheritsFromRun = _characterFormatRuns.Any(run => run.Start < offset && offset <= run.End);
        if (explicitFormat is null && !inheritsFromRun)
        {
            var pristineDefault = new TextCharacterFormat();
            if (!_defaultCharacterFormat.IsEqual(pristineDefault))
                explicitFormat = (TextCharacterFormat)_defaultCharacterFormat.GetClone();
        }

        _buffer.Insert(offset, text);

        var shiftedRuns = new List<CharacterFormatRun>(_characterFormatRuns.Count + 1);
        foreach (var run in _characterFormatRuns)
        {
            if (run.End < offset)
            {
                shiftedRuns.Add(run);
            }
            else if (run.Start < offset && offset <= run.End)
            {
                shiftedRuns.Add(new CharacterFormatRun(run.Start, run.End + delta, (TextCharacterFormat)run.Format.GetClone()));
            }
            else
            {
                shiftedRuns.Add(new CharacterFormatRun(run.Start + delta, run.End + delta, (TextCharacterFormat)run.Format.GetClone()));
            }
        }

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(MergeAdjacentRuns(shiftedRuns.OrderBy(run => run.Start)));

        if (explicitFormat is not null)
            ApplyCharacterFormat(offset, offset + delta, explicitFormat);

        TextChanged?.Invoke(this, System.EventArgs.Empty);
    }

    internal void DeleteRange(int start, int end)
    {
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);
        if (end <= start)
            return;

        int deletedLength = end - start;
        _buffer.Remove(start, deletedLength);

        var nextRuns = new List<CharacterFormatRun>(_characterFormatRuns.Count);
        foreach (var run in _characterFormatRuns)
        {
            if (run.End <= start)
            {
                nextRuns.Add(run);
                continue;
            }

            if (run.Start >= end)
            {
                nextRuns.Add(new CharacterFormatRun(run.Start - deletedLength, run.End - deletedLength, (TextCharacterFormat)run.Format.GetClone()));
                continue;
            }

            int leftKeep = Math.Max(0, start - run.Start);
            int rightKeep = Math.Max(0, run.End - end);
            int keptLength = leftKeep + rightKeep;
            if (keptLength == 0)
                continue;

            int newStart = Math.Min(run.Start, start);
            nextRuns.Add(new CharacterFormatRun(newStart, newStart + keptLength, (TextCharacterFormat)run.Format.GetClone()));
        }

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(MergeAdjacentRuns(nextRuns.OrderBy(run => run.Start)));
        _selection.SetRange(Math.Min(_selection.StartPosition, _buffer.Length), Math.Min(_selection.EndPosition, _buffer.Length));
        TextChanged?.Invoke(this, System.EventArgs.Empty);
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

    public IReadOnlyList<RichEditCharacterFormatRun> GetCharacterFormatRuns()
        => _characterFormatRuns
            .Select(run => new RichEditCharacterFormatRun(run.Start, run.End, (TextCharacterFormat)run.Format.GetClone()))
            .ToList();

    internal TextCharacterFormat GetCharacterFormat(int start, int end)
    {
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);

        var format = (TextCharacterFormat)_defaultCharacterFormat.GetClone();
        var run = _characterFormatRuns.FirstOrDefault(item => item.Start <= start && item.End > start);
        if (run is not null)
            format.SetClone(run.Format);

        return format;
    }

    internal void ApplyCharacterFormat(int start, int end, TextCharacterFormat format)
    {
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);

        if (start == end)
        {
            var nextDefault = ResolveToggleValues(_defaultCharacterFormat, format);
            _defaultCharacterFormat = nextDefault;
            FormattingChanged?.Invoke(this, System.EventArgs.Empty);
            return;
        }

        var nextFormat = ResolveToggleValues(GetCharacterFormat(start, end), format);
        var nextRuns = new List<CharacterFormatRun>();

        foreach (var run in _characterFormatRuns)
        {
            if (run.End <= start || run.Start >= end)
            {
                nextRuns.Add(run);
                continue;
            }

            if (run.Start < start)
                nextRuns.Add(new CharacterFormatRun(run.Start, start, (TextCharacterFormat)run.Format.GetClone()));

            if (run.End > end)
                nextRuns.Add(new CharacterFormatRun(end, run.End, (TextCharacterFormat)run.Format.GetClone()));
        }

        nextRuns.Add(new CharacterFormatRun(start, end, nextFormat));

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(MergeAdjacentRuns(nextRuns.OrderBy(run => run.Start)));
        FormattingChanged?.Invoke(this, System.EventArgs.Empty);
    }

    private static TextCharacterFormat ResolveToggleValues(TextCharacterFormat current, TextCharacterFormat requested)
    {
        var resolved = (TextCharacterFormat)requested.GetClone();
        resolved.Bold = ResolveToggle(current.Bold, requested.Bold);
        resolved.Italic = ResolveToggle(current.Italic, requested.Italic);
        return resolved;
    }

    private static FormatEffect ResolveToggle(FormatEffect current, FormatEffect requested)
    {
        if (requested != FormatEffect.Toggle)
            return requested;

        return current == FormatEffect.On ? FormatEffect.Off : FormatEffect.On;
    }

    private static IEnumerable<CharacterFormatRun> MergeAdjacentRuns(IEnumerable<CharacterFormatRun> runs)
    {
        CharacterFormatRun? previous = null;

        foreach (var run in runs)
        {
            if (previous is null)
            {
                previous = run;
                continue;
            }

            if (previous.End == run.Start && previous.Format.IsEqual(run.Format))
            {
                previous = new CharacterFormatRun(previous.Start, run.End, previous.Format);
                continue;
            }

            yield return previous;
            previous = run;
        }

        if (previous is not null)
            yield return previous;
    }

    private sealed record CharacterFormatRun(int Start, int End, TextCharacterFormat Format);
}

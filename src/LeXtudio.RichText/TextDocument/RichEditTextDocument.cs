// RichEditTextDocument — behavior-bearing replacement for
// Microsoft.UI.Text.RichEditTextDocument.
//
// Why this exists: Uno's Microsoft.UI.Text.RichEditTextDocument has an internal
// constructor and many throw-stub members. We ship a concrete implementation
// that preserves the WinUI-shaped API and behavior needed by our controls.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.UI.Text;
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
public sealed record RichEditParagraphRange(int Start, int End);

// Note: Uno's Microsoft.UI.Text does not declare an ITextDocument interface.
public sealed class RichEditTextDocument
{
    /// <summary>
    /// Raised after document text is mutated.
    /// </summary>
    internal event EventHandler? TextChanged;

    /// <summary>
    /// Raised after document formatting is mutated.
    /// </summary>
    internal event EventHandler? FormattingChanged;

    private readonly StringBuilder _buffer = new();
    private readonly List<CharacterFormatRun> _characterFormatRuns = new();
    private readonly Stack<EditOperation> _undoStack = new();
    private readonly Stack<EditOperation> _redoStack = new();
    private readonly RichEditTextSelection _selection;

    private TextCharacterFormat _defaultCharacterFormat = new();
    private TextCharacterFormat? _caretInputFormat;
    private TextParagraphFormat _defaultParagraphFormat = new();

    private int _batchCount;
    private bool _isReplayingHistory;
    private int _preserveCaretInputFormatDepth;

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

    internal IDisposable PreserveCaretInputFormatScope() => new CaretInputFormatScope(this);

    public bool CanCopy() => _selection.Length > 0;
    public bool CanPaste() => true;
    public bool CanRedo() => _redoStack.Count > 0;
    public bool CanUndo() => _undoStack.Count > 0;

    public void ApplyDisplayUpdates()
    {
        if (_batchCount > 0)
        {
            _batchCount--;
        }
    }

    public int BatchDisplayUpdates() => ++_batchCount;

    public void BeginUndoGroup() { /* TODO: group operations */ }
    public void EndUndoGroup() { /* TODO: group operations */ }

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
        _characterFormatRuns.Clear();
        ClearUndoRedoHistory();
        // TODO: port WPF RtfToXamlReader for FormatRtf.
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }

        var operation = _redoStack.Pop();
        ReplaySnapshot(operation.After);
        _undoStack.Push(operation);
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }

        var operation = _undoStack.Pop();
        ReplaySnapshot(operation.Before);
        _redoStack.Push(operation);
    }

    public void SaveToStream(TextGetOptions options, IRandomAccessStream value)
    {
        // TODO: port WPF XamlToRtfWriter for FormatRtf, plain UTF-16 otherwise.
    }

    public void SetDefaultCharacterFormat(ITextCharacterFormat value)
    {
        if (value is TextCharacterFormat tcf)
        {
            _defaultCharacterFormat = (TextCharacterFormat)tcf.GetClone();
        }
    }

    public void SetDefaultParagraphFormat(ITextParagraphFormat value)
    {
        if (value is TextParagraphFormat tpf)
        {
            _defaultParagraphFormat = (TextParagraphFormat)tpf.GetClone();
        }
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
        {
            _buffer.Append(value);
        }

        _selection.SetRange(0, 0);
        ClearUndoRedoHistory();
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void InsertText(int offset, string text, TextCharacterFormat? format = null)
    {
        text ??= string.Empty;
        if (text.Length == 0)
        {
            return;
        }

        var before = CaptureSnapshot();
        offset = Math.Clamp(offset, 0, _buffer.Length);
        int delta = text.Length;

        TextCharacterFormat? explicitFormat = format is null ? null : (TextCharacterFormat)format.GetClone();
        if (explicitFormat is null && _caretInputFormat is not null)
        {
            explicitFormat = (TextCharacterFormat)_caretInputFormat.GetClone();
        }

        bool inheritsFromRun = _characterFormatRuns.Any(run => run.Start < offset && offset <= run.End);
        if (explicitFormat is null && !inheritsFromRun)
        {
            var pristineDefault = new TextCharacterFormat();
            if (!_defaultCharacterFormat.IsEqual(pristineDefault))
            {
                explicitFormat = (TextCharacterFormat)_defaultCharacterFormat.GetClone();
            }
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
        _characterFormatRuns.AddRange(NormalizeRunsToParagraphBoundaries(shiftedRuns));

        if (explicitFormat is not null)
        {
            ApplyCharacterFormat(offset, offset + delta, explicitFormat);
        }

        RecordEditOperation(new EditOperation(
            offset,
            string.Empty,
            text,
            new List<RichEditCharacterFormatRun>(),
            before,
            CaptureSnapshot()));

        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void DeleteRange(int start, int end)
    {
        var before = CaptureSnapshot();
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);
        if (end <= start)
        {
            return;
        }

        string deletedText = _buffer.ToString(start, end - start);
        var deletedRuns = CaptureRunsInRange(start, end);

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
            {
                continue;
            }

            int newStart = Math.Min(run.Start, start);
            nextRuns.Add(new CharacterFormatRun(newStart, newStart + keptLength, (TextCharacterFormat)run.Format.GetClone()));
        }

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(NormalizeRunsToParagraphBoundaries(nextRuns));
        _selection.SetRange(Math.Min(_selection.StartPosition, _buffer.Length), Math.Min(_selection.EndPosition, _buffer.Length));

        RecordEditOperation(new EditOperation(
            start,
            deletedText,
            string.Empty,
            deletedRuns,
            before,
            CaptureSnapshot()));

        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearUndoRedoHistory()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    public void SetMathMode(RichEditMathMode mode) => MathMode = mode;
    public RichEditMathMode GetMathMode() => MathMode;
    public string GetMathML() => string.Empty;
    public void SetMathML(string mathML) { /* TODO */ }

    // Members Uno declares that we preserve for API shape.
    public bool IgnoreTrailingCharacterSpacing { get; set; }
    public bool AlignmentIncludesTrailingWhitespace { get; set; }

    public IReadOnlyList<RichEditCharacterFormatRun> GetCharacterFormatRuns()
        => _characterFormatRuns
            .Select(run => new RichEditCharacterFormatRun(run.Start, run.End, (TextCharacterFormat)run.Format.GetClone()))
            .ToList();

    internal IReadOnlyList<RichEditParagraphRange> GetParagraphRanges()
        => ComputeParagraphRanges(_buffer.ToString());

    internal TextCharacterFormat GetCharacterFormat(int start, int end)
    {
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);

        if (start == end
            && _caretInputFormat is not null
            && _selection.StartPosition == start
            && _selection.EndPosition == end)
        {
            return (TextCharacterFormat)_caretInputFormat.GetClone();
        }

        return GetCharacterFormatIgnoringCaretInput(start);
    }

    internal void ApplyCharacterFormat(int start, int end, TextCharacterFormat format)
    {
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);

        if (start == end)
        {
            var current = GetCharacterFormatIgnoringCaretInput(start);
            if (_caretInputFormat is not null)
            {
                current = (TextCharacterFormat)_caretInputFormat.GetClone();
            }

            _caretInputFormat = ResolveToggleValues(current, format);
            FormattingChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        _caretInputFormat = null;

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
            {
                nextRuns.Add(new CharacterFormatRun(run.Start, start, (TextCharacterFormat)run.Format.GetClone()));
            }

            if (run.End > end)
            {
                nextRuns.Add(new CharacterFormatRun(end, run.End, (TextCharacterFormat)run.Format.GetClone()));
            }
        }

        nextRuns.Add(new CharacterFormatRun(start, end, nextFormat));

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(NormalizeRunsToParagraphBoundaries(nextRuns));
        FormattingChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void NotifySelectionRangeChanged(int oldStart, int oldEnd, int newStart, int newEnd)
    {
        if (_preserveCaretInputFormatDepth > 0)
        {
            return;
        }

        if (oldStart != newStart || oldEnd != newEnd)
        {
            _caretInputFormat = null;
        }
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
        {
            return requested;
        }

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
        {
            yield return previous;
        }
    }

    private List<CharacterFormatRun> NormalizeRunsToParagraphBoundaries(IEnumerable<CharacterFormatRun> runs)
    {
        int length = _buffer.Length;
        var normalized = new List<CharacterFormatRun>();

        foreach (var run in runs.OrderBy(item => item.Start))
        {
            int start = Math.Clamp(run.Start, 0, length);
            int end = Math.Clamp(run.End, start, length);
            if (end <= start)
            {
                continue;
            }

            int segmentStart = start;
            for (int i = start; i < end; i++)
            {
                if (!IsParagraphSeparator(_buffer[i]))
                {
                    continue;
                }

                if (segmentStart < i)
                {
                    normalized.Add(new CharacterFormatRun(segmentStart, i, (TextCharacterFormat)run.Format.GetClone()));
                }

                // Skip paragraph separator itself so no run spans across paragraph boundaries.
                segmentStart = i + 1;
            }

            if (segmentStart < end)
            {
                normalized.Add(new CharacterFormatRun(segmentStart, end, (TextCharacterFormat)run.Format.GetClone()));
            }
        }

        return MergeAdjacentRuns(normalized.OrderBy(item => item.Start)).ToList();
    }

    private static IReadOnlyList<RichEditParagraphRange> ComputeParagraphRanges(string text)
    {
        var ranges = new List<RichEditParagraphRange>();
        int start = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (!IsParagraphSeparator(text[i]))
            {
                continue;
            }

            ranges.Add(new RichEditParagraphRange(start, i));
            start = i + 1;
        }

        // Always expose at least one paragraph, and preserve trailing empty paragraph after final separator.
        ranges.Add(new RichEditParagraphRange(start, text.Length));
        return ranges;
    }

    private static bool IsParagraphSeparator(char c)
        => c == '\n' || c == '\r';

    private DocumentSnapshot CaptureSnapshot()
        => new(
            _buffer.ToString(),
            CloneRuns(_characterFormatRuns),
            (TextCharacterFormat)_defaultCharacterFormat.GetClone(),
            _caretInputFormat is null ? null : (TextCharacterFormat)_caretInputFormat.GetClone());

    private List<RichEditCharacterFormatRun> CaptureRunsInRange(int start, int end)
    {
        if (end <= start)
        {
            return new List<RichEditCharacterFormatRun>();
        }

        return _characterFormatRuns
            .Where(run => run.End > start && run.Start < end)
            .Select(run => new RichEditCharacterFormatRun(run.Start, run.End, (TextCharacterFormat)run.Format.GetClone()))
            .ToList();
    }

    private static List<CharacterFormatRun> CloneRuns(IEnumerable<CharacterFormatRun> runs)
        => runs.Select(run => new CharacterFormatRun(run.Start, run.End, (TextCharacterFormat)run.Format.GetClone())).ToList();

    private void RecordEditOperation(EditOperation operation)
    {
        if (_isReplayingHistory)
        {
            return;
        }

        _undoStack.Push(operation);
        _redoStack.Clear();
    }

    private void ReplaySnapshot(DocumentSnapshot snapshot)
    {
        _isReplayingHistory = true;
        try
        {
            _buffer.Clear();
            _buffer.Append(snapshot.Text);

            _characterFormatRuns.Clear();
            _characterFormatRuns.AddRange(CloneRuns(snapshot.Runs));

            _defaultCharacterFormat = (TextCharacterFormat)snapshot.DefaultCharacterFormat.GetClone();
            _caretInputFormat = snapshot.CaretInputFormat is null ? null : (TextCharacterFormat)snapshot.CaretInputFormat.GetClone();
            _selection.SetRange(Math.Min(_selection.StartPosition, _buffer.Length), Math.Min(_selection.EndPosition, _buffer.Length));
        }
        finally
        {
            _isReplayingHistory = false;
        }

        TextChanged?.Invoke(this, EventArgs.Empty);
        FormattingChanged?.Invoke(this, EventArgs.Empty);
    }

    private TextCharacterFormat GetCharacterFormatIgnoringCaretInput(int position)
    {
        position = Math.Clamp(position, 0, _buffer.Length);

        var format = (TextCharacterFormat)_defaultCharacterFormat.GetClone();
        var run = _characterFormatRuns.FirstOrDefault(item => item.Start <= position && item.End > position);
        if (run is not null)
        {
            format.SetClone(run.Format);
        }

        return format;
    }

    private sealed class CaretInputFormatScope : IDisposable
    {
        private readonly RichEditTextDocument _owner;
        private bool _disposed;

        public CaretInputFormatScope(RichEditTextDocument owner)
        {
            _owner = owner;
            _owner._preserveCaretInputFormatDepth++;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _owner._preserveCaretInputFormatDepth = Math.Max(0, _owner._preserveCaretInputFormatDepth - 1);
        }
    }

    private sealed record CharacterFormatRun(int Start, int End, TextCharacterFormat Format);
    private sealed record DocumentSnapshot(
        string Text,
        List<CharacterFormatRun> Runs,
        TextCharacterFormat DefaultCharacterFormat,
        TextCharacterFormat? CaretInputFormat);

    private sealed record EditOperation(
        int Start,
        string DeletedText,
        string InsertedText,
        List<RichEditCharacterFormatRun> DeletedRuns,
        DocumentSnapshot Before,
        DocumentSnapshot After);
}

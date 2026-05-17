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

    // Tracks the range of the most recent text insertion. While the selection
    // is still collapsed at the trailing end of this range (i.e. the user just
    // finished typing and hasn't moved the caret) AND the host is dispatching
    // its TextChanged notification, a delta format applied to the caret
    // retro-applies to the inserted span. This mirrors WinUI's RichEditBox
    // behavior where TextChanged-time format assignments color the just-typed
    // characters.
    private (int Start, int End)? _pendingInsertRange;
    private int _retroApplyDepth;

    internal IDisposable EnterRetroApplyScope() => new RetroApplyScope(this);

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

        // Extend the pending input range if this insertion is contiguous with
        // the previous one (caret was at its trailing edge); otherwise start a
        // fresh range. Both cases let a follow-up TextChanged-time format set
        // through the selection retro-apply to all of the just-typed text.
        if (_pendingInsertRange is { } prev && prev.End == offset)
            _pendingInsertRange = (prev.Start, offset + delta);
        else
            _pendingInsertRange = (offset, offset + delta);

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

    internal void ApplyCharacterFormat(int start, int end, TextCharacterFormat format, TextCharacterFormat? baseline = null)
    {
        start = Math.Clamp(start, 0, _buffer.Length);
        end = Math.Clamp(end, start, _buffer.Length);

        var before = CaptureSnapshot();

        if (start == end)
        {
            var current = GetCharacterFormatIgnoringCaretInput(start);
            if (_caretInputFormat is not null)
            {
                current = (TextCharacterFormat)_caretInputFormat.GetClone();
            }

            var newCaretFormat = baseline is null
                ? ResolveToggleValues(current, format)
                : ApplyFormatDelta(current, format, baseline);

            // WinUI parity: when the caret sits at the trailing edge of the
            // most recent insertion, a delta format applied via the selection
            // also colors the just-typed text. Without this, an external
            // TextChanged handler that runs after each keystroke only affects
            // future input and the leading characters stay uncolored.
            bool retroApply = baseline is not null
                && _retroApplyDepth > 0
                && _pendingInsertRange is { } pendingProbe
                && start == pendingProbe.End
                && pendingProbe.End > pendingProbe.Start;

            if (retroApply)
            {
                var pending = _pendingInsertRange!.Value;
                ApplyDeltaToRunsInRange(pending.Start, pending.End, format, baseline!);
                // Restore caret state after the range mutation so future input
                // continues to pick up the delta-merged format.
                _caretInputFormat = newCaretFormat;
                // Keep the pending range so subsequent typing stays attached.
                _pendingInsertRange = pending;

                RecordEditOperation(new EditOperation(
                    start,
                    string.Empty,
                    string.Empty,
                    new List<RichEditCharacterFormatRun>(),
                    before,
                    CaptureSnapshot()));
            }
            else
            {
                _caretInputFormat = newCaretFormat;
            }

            FormattingChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        _caretInputFormat = null;

        var nextRuns = new List<CharacterFormatRun>();

        if (baseline is null)
        {
            // No baseline: caller is asserting the full format. Preserve existing
            // behavior of replacing the range with a single resolved run.
            var nextFormat = ResolveToggleValues(GetCharacterFormat(start, end), format);

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
        }
        else
        {
            ApplyDeltaToRunsInRange(start, end, format, baseline);
            RecordEditOperation(new EditOperation(
                start,
                string.Empty,
                string.Empty,
                new List<RichEditCharacterFormatRun>(),
                before,
                CaptureSnapshot()));
            FormattingChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(NormalizeRunsToParagraphBoundaries(nextRuns));

        RecordEditOperation(new EditOperation(
            start,
            string.Empty,
            string.Empty,
            new List<RichEditCharacterFormatRun>(),
            before,
            CaptureSnapshot()));

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

        // Once the caret leaves the trailing edge of the most recent insertion,
        // retro-application is no longer applicable: a later format set on the
        // selection should not reach back to that older typing run.
        if (_pendingInsertRange is { } pending
            && (newStart != newEnd || newStart != pending.End))
        {
            _pendingInsertRange = null;
        }
    }

    private static TextCharacterFormat ResolveToggleValues(TextCharacterFormat current, TextCharacterFormat requested)
    {
        var resolved = (TextCharacterFormat)requested.GetClone();
        resolved.Bold = ResolveToggle(current.Bold, requested.Bold);
        resolved.Italic = ResolveToggle(current.Italic, requested.Italic);
        return resolved;
    }

    // Applies the (requested - baseline) field delta to every run intersecting
    // [start, end], splitting and synthesizing runs as needed. Caller is
    // responsible for snapshot/record/event bookkeeping.
    private void ApplyDeltaToRunsInRange(int start, int end, TextCharacterFormat requested, TextCharacterFormat baseline)
    {
        if (end <= start)
            return;

        var nextRuns = new List<CharacterFormatRun>();
        int cursor = start;

        foreach (var run in _characterFormatRuns.OrderBy(r => r.Start))
        {
            if (run.End <= start || run.Start >= end)
            {
                nextRuns.Add(run);
                continue;
            }

            if (run.Start < start)
                nextRuns.Add(new CharacterFormatRun(run.Start, start, (TextCharacterFormat)run.Format.GetClone()));

            int segStart = Math.Max(run.Start, start);
            int segEnd = Math.Min(run.End, end);

            if (segStart > cursor)
            {
                var gapFormat = ApplyFormatDelta(
                    (TextCharacterFormat)_defaultCharacterFormat.GetClone(),
                    requested,
                    baseline);
                nextRuns.Add(new CharacterFormatRun(cursor, segStart, gapFormat));
            }

            var mergedFormat = ApplyFormatDelta(
                (TextCharacterFormat)run.Format.GetClone(),
                requested,
                baseline);
            nextRuns.Add(new CharacterFormatRun(segStart, segEnd, mergedFormat));
            cursor = segEnd;

            if (run.End > end)
                nextRuns.Add(new CharacterFormatRun(end, run.End, (TextCharacterFormat)run.Format.GetClone()));
        }

        if (cursor < end)
        {
            var gapFormat = ApplyFormatDelta(
                (TextCharacterFormat)_defaultCharacterFormat.GetClone(),
                requested,
                baseline);
            nextRuns.Add(new CharacterFormatRun(cursor, end, gapFormat));
        }

        _characterFormatRuns.Clear();
        _characterFormatRuns.AddRange(NormalizeRunsToParagraphBoundaries(nextRuns));
    }

    // Returns a new format = target with only the fields where requested differs
    // from baseline overlaid. Bold/Italic toggles resolve against target's
    // current value so toggling against a per-run state flips that run.
    private static TextCharacterFormat ApplyFormatDelta(TextCharacterFormat target, TextCharacterFormat requested, TextCharacterFormat baseline)
    {
        if (requested.Bold != baseline.Bold)
            target.Bold = requested.Bold == FormatEffect.Toggle ? ResolveToggle(target.Bold, FormatEffect.Toggle) : requested.Bold;
        if (requested.Italic != baseline.Italic)
            target.Italic = requested.Italic == FormatEffect.Toggle ? ResolveToggle(target.Italic, FormatEffect.Toggle) : requested.Italic;
        if (requested.Underline != baseline.Underline)
            target.Underline = requested.Underline;
        if (requested.Strikethrough != baseline.Strikethrough)
            target.Strikethrough = requested.Strikethrough;
        if (!requested.ForegroundColor.Equals(baseline.ForegroundColor))
            target.ForegroundColor = requested.ForegroundColor;
        if (!requested.BackgroundColor.Equals(baseline.BackgroundColor))
            target.BackgroundColor = requested.BackgroundColor;
        if (requested.Size != baseline.Size)
            target.Size = requested.Size;
        if (requested.Weight != baseline.Weight)
            target.Weight = requested.Weight;
        if (requested.Name != baseline.Name)
            target.Name = requested.Name;
        if (requested.FontStyle != baseline.FontStyle)
            target.FontStyle = requested.FontStyle;
        if (requested.FontStretch != baseline.FontStretch)
            target.FontStretch = requested.FontStretch;
        if (requested.AllCaps != baseline.AllCaps)
            target.AllCaps = requested.AllCaps;
        if (requested.SmallCaps != baseline.SmallCaps)
            target.SmallCaps = requested.SmallCaps;
        if (requested.Hidden != baseline.Hidden)
            target.Hidden = requested.Hidden;
        if (requested.Outline != baseline.Outline)
            target.Outline = requested.Outline;
        if (requested.ProtectedText != baseline.ProtectedText)
            target.ProtectedText = requested.ProtectedText;
        if (requested.Subscript != baseline.Subscript)
            target.Subscript = requested.Subscript;
        if (requested.Superscript != baseline.Superscript)
            target.Superscript = requested.Superscript;
        if (requested.Kerning != baseline.Kerning)
            target.Kerning = requested.Kerning;
        if (requested.Position != baseline.Position)
            target.Position = requested.Position;
        if (requested.Spacing != baseline.Spacing)
            target.Spacing = requested.Spacing;
        if (requested.LanguageTag != baseline.LanguageTag)
            target.LanguageTag = requested.LanguageTag;
        if (requested.TextScript != baseline.TextScript)
            target.TextScript = requested.TextScript;
        return target;
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
            _caretInputFormat is null ? null : (TextCharacterFormat)_caretInputFormat.GetClone(),
            _selection.StartPosition,
            _selection.EndPosition);

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
            var restoredStart = Math.Clamp(snapshot.SelectionStart, 0, _buffer.Length);
            var restoredEnd = Math.Clamp(snapshot.SelectionEnd, restoredStart, _buffer.Length);
            _selection.SetRange(restoredStart, restoredEnd);
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
        var run = _characterFormatRuns.FirstOrDefault(item => item.Start <= position && item.End > position)
            ?? _characterFormatRuns.FirstOrDefault(item => item.Start < position && item.End == position);
        if (run is not null)
        {
            format.SetClone(run.Format);
        }

        return format;
    }

    private sealed class RetroApplyScope : IDisposable
    {
        private readonly RichEditTextDocument _owner;
        private bool _disposed;

        public RetroApplyScope(RichEditTextDocument owner)
        {
            _owner = owner;
            _owner._retroApplyDepth++;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _owner._retroApplyDepth = Math.Max(0, _owner._retroApplyDepth - 1);
        }
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
        TextCharacterFormat? CaretInputFormat,
        int SelectionStart,
        int SelectionEnd);

    private sealed record EditOperation(
        int Start,
        string DeletedText,
        string InsertedText,
        List<RichEditCharacterFormatRun> DeletedRuns,
        DocumentSnapshot Before,
        DocumentSnapshot After);
}

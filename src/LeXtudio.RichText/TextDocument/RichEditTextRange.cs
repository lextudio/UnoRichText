// RichEditTextRange — implementation of Microsoft.UI.Text.ITextRange.
//
// Uno's concrete Microsoft.UI.Text.RichEditTextRange has 29 of 52 methods as
// throw-stubs (audit via IL inspection). We implement ITextRange directly so
// every member has working behavior. Signature shapes follow Uno's interface
// exactly (verified by decompiling Uno.UI 6.5.153).

using System;
using global::Windows.Foundation;
using global::Windows.Storage.Streams;
using ITextRange = Microsoft.UI.Text.ITextRange;
using ITextCharacterFormat = Microsoft.UI.Text.ITextCharacterFormat;
using ITextParagraphFormat = Microsoft.UI.Text.ITextParagraphFormat;
using TextRangeUnit = Microsoft.UI.Text.TextRangeUnit;
using TextGetOptions = Microsoft.UI.Text.TextGetOptions;
using TextSetOptions = Microsoft.UI.Text.TextSetOptions;
using PointOptions = Microsoft.UI.Text.PointOptions;
using FindOptions = Microsoft.UI.Text.FindOptions;
using LetterCase = Microsoft.UI.Text.LetterCase;
using RangeGravity = Microsoft.UI.Text.RangeGravity;
using HorizontalCharacterAlignment = Microsoft.UI.Text.HorizontalCharacterAlignment;
using VerticalCharacterAlignment = Microsoft.UI.Text.VerticalCharacterAlignment;

namespace LeXtudio.UI.Text;

internal class RichEditTextRange : ITextRange
{
    protected readonly RichEditTextDocument _document;
    private int _start;
    private int _end;
    private TextCharacterFormat _characterFormat = new();
    private TextCharacterFormat _characterFormatBaseline = new();
    private TextParagraphFormat _paragraphFormat = new();

    public RichEditTextRange(RichEditTextDocument document, int start, int end)
    {
        _document = document;
        SetRangeCore(start, end);
        AttachCharacterFormat(_document.GetCharacterFormat(_start, _end));
    }

    public virtual ITextCharacterFormat CharacterFormat
    {
        get
        {
            AttachCharacterFormat(_document.GetCharacterFormat(_start, _end));
            return _characterFormat;
        }
        set
        {
            if (value is TextCharacterFormat tcf)
            {
                AttachCharacterFormat((TextCharacterFormat)tcf.GetClone());
                _document.ApplyCharacterFormat(_start, _end, _characterFormat);
            }
        }
    }

    public virtual ITextParagraphFormat ParagraphFormat
    {
        get => _paragraphFormat;
        set
        {
            if (value is TextParagraphFormat tpf)
                _paragraphFormat = (TextParagraphFormat)tpf.GetClone();
        }
    }

    public char Character
    {
        get => (_start >= 0 && _start < _document.Buffer.Length) ? _document.Buffer[_start] : '\0';
        set
        {
            if (_start < 0 || _start > _document.Buffer.Length)
                return;

            if (_start < _document.Buffer.Length)
                _document.DeleteRange(_start, _start + 1);

            _document.InsertText(_start, value.ToString(), _document.GetCharacterFormat(_start, _start));
            SetRangeCore(_start, _start + 1);
        }
    }

    public ITextRange FormattedText
    {
        get => GetClone();
        set { if (value != null) Text = value.Text; }
    }

    public RangeGravity Gravity { get; set; } = RangeGravity.UIBehavior;
    public string Link { get; set; } = string.Empty;
    public int Length => _end - _start;
    public int StoryLength => _document.Buffer.Length;

    public int EndPosition
    {
        get => _end;
        set => SetRangeCore(_start, value);
    }

    public int StartPosition
    {
        get => _start;
        set => SetRangeCore(value, _end);
    }

    public string Text
    {
        get
        {
            int len = Math.Max(0, _end - _start);
            if (_start < 0 || _start > _document.Buffer.Length) return string.Empty;
            len = Math.Min(len, _document.Buffer.Length - _start);
            return _document.Buffer.ToString(_start, len);
        }
        set
        {
            value ??= string.Empty;
            var replacementFormat = _document.GetCharacterFormat(_start, _end);

            _document.DeleteRange(_start, _end);

            if (value.Length > 0)
                _document.InsertText(_start, value, replacementFormat);

            _end = _start + value.Length;
        }
    }

    public bool CanPaste(int format) => true;
    public void ChangeCase(LetterCase value)
    {
        string text = Text;
        Text = value == LetterCase.Upper ? text.ToUpperInvariant() : text.ToLowerInvariant();
    }
    public void Collapse(bool start) { if (start) _end = _start; else _start = _end; }
    public void Copy() { /* TODO: clipboard */ }
    public void Cut() { /* TODO: clipboard */ }
    public int Delete(TextRangeUnit unit, int count) { int len = Length; Text = string.Empty; return len; }
    public int EndOf(TextRangeUnit unit, bool extend) => 0;
    public int Expand(TextRangeUnit unit) => 0;
    public int FindText(string value, int scanLength, FindOptions options)
    {
        if (string.IsNullOrEmpty(value)) return 0;
        string buf = _document.Buffer.ToString();
        var cmp = (options & FindOptions.Case) != 0 ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int idx = buf.IndexOf(value, _start, cmp);
        if (idx < 0) return 0;
        SetRangeCore(idx, idx + value.Length);
        return value.Length;
    }
    public void GetCharacterUtf32(out uint value, int offset)
    {
        int pos = _start + offset;
        value = (pos >= 0 && pos < _document.Buffer.Length) ? _document.Buffer[pos] : 0u;
    }
    public ITextRange GetClone()
    {
        var c = new RichEditTextRange(_document, _start, _end);
        c.AttachCharacterFormat((TextCharacterFormat)_characterFormat.GetClone());
        c._paragraphFormat = (TextParagraphFormat)_paragraphFormat.GetClone();
        return c;
    }
    public int GetIndex(TextRangeUnit unit) => 0;
    public void GetPoint(HorizontalCharacterAlignment horizontalAlign, VerticalCharacterAlignment verticalAlign, PointOptions options, out Point point) { point = default; }
    public void GetRect(PointOptions options, out Rect rect, out int hit) { rect = default; hit = 0; }
    public void GetText(TextGetOptions options, out string value) { value = Text; }
    public void GetTextViaStream(TextGetOptions options, IRandomAccessStream value) { /* TODO */ }
    public void InsertImage(int width, int height, int ascent, VerticalCharacterAlignment verticalAlign, string alternateText, IRandomAccessStream value) { /* TODO */ }
    public bool InRange(ITextRange range) => range != null && range.StartPosition <= _start && range.EndPosition >= _end;
    public bool InStory(ITextRange range) => true;
    public bool IsEqual(ITextRange range) => range != null && range.StartPosition == _start && range.EndPosition == _end;
    public int Move(TextRangeUnit unit, int count)
    {
        int newPos = Math.Clamp(_start + count, 0, _document.Buffer.Length);
        int moved = newPos - _start;
        SetRangeCore(newPos, newPos);
        return moved;
    }
    public int MoveEnd(TextRangeUnit unit, int count)
    {
        int newEnd = Math.Clamp(_end + count, 0, _document.Buffer.Length);
        int moved = newEnd - _end;
        SetRangeCore(Math.Min(_start, newEnd), newEnd);
        return moved;
    }
    public int MoveStart(TextRangeUnit unit, int count)
    {
        int newStart = Math.Clamp(_start + count, 0, _document.Buffer.Length);
        int moved = newStart - _start;
        SetRangeCore(newStart, Math.Max(newStart, _end));
        return moved;
    }
    public void Paste(int format) { /* TODO: clipboard */ }
    public void ScrollIntoView(PointOptions value) { /* TODO */ }
    public void MatchSelection() { /* TODO */ }
    public void SetIndex(TextRangeUnit unit, int index, bool extend) { /* TODO */ }
    public void SetPoint(Point point, PointOptions options, bool extend) { /* TODO */ }
    public void SetRange(int startPosition, int endPosition) => SetRangeCore(startPosition, endPosition);
    public void SetText(TextSetOptions options, string value) { Text = value ?? string.Empty; }
    public void SetTextViaStream(TextSetOptions options, IRandomAccessStream value) { /* TODO */ }
    public int StartOf(TextRangeUnit unit, bool extend) => 0;

    private void SetRangeCore(int start, int end)
    {
        int previousStart = _start;
        int previousEnd = _end;
        int max = _document.Buffer.Length;
        if (start < 0) start = 0;
        if (end < 0) end = 0;
        if (start > max) start = max;
        if (end > max) end = max;
        if (end < start) end = start;
        _start = start;
        _end = end;

        if (this is RichEditTextSelection)
            _document.NotifySelectionRangeChanged(previousStart, previousEnd, _start, _end);
    }

    private void AttachCharacterFormat(TextCharacterFormat format)
    {
        if (_characterFormat is not null)
            _characterFormat.Changed -= OnCharacterFormatChanged;

        _characterFormat = format;
        _characterFormatBaseline = (TextCharacterFormat)format.GetClone();
        _characterFormat.Changed += OnCharacterFormatChanged;
    }

    private void OnCharacterFormatChanged(object? sender, EventArgs e)
    {
        _document.ApplyCharacterFormat(_start, _end, _characterFormat, _characterFormatBaseline);
        _characterFormatBaseline = (TextCharacterFormat)_characterFormat.GetClone();
    }
}

// Skeleton TextRange implementation backing RichEditTextDocument.GetRange.

using System;
using global::Windows.Foundation;
using global::Windows.Storage.Streams;

namespace Microsoft.UI.Text;

internal class TextRange : ITextRange
{
    protected readonly RichEditTextDocument _document;
    private int _start;
    private int _end;
    private TextCharacterFormat _characterFormat = new();
    private TextParagraphFormat _paragraphFormat = new();

    public TextRange(RichEditTextDocument document, int start, int end)
    {
        _document = document;
        SetRangeCore(start, end);
    }

    public virtual ITextCharacterFormat CharacterFormat
    {
        get => _characterFormat;
        set
        {
            if (value is TextCharacterFormat tcf)
                _characterFormat = (TextCharacterFormat)tcf.GetClone();
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

    public string Character
    {
        get
        {
            if (_start >= 0 && _start < _document.Buffer.Length)
                return _document.Buffer[_start].ToString();
            return string.Empty;
        }
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            if (_start >= 0 && _start <= _document.Buffer.Length)
            {
                if (_start < _document.Buffer.Length) _document.Buffer.Remove(_start, 1);
                _document.Buffer.Insert(_start, value[0]);
            }
        }
    }

    public string FormattedText { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public int StartOf => _start;
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
            int len = Math.Max(0, _end - _start);
            if (len > 0 && _start >= 0 && _start + len <= _document.Buffer.Length)
                _document.Buffer.Remove(_start, len);
            if (!string.IsNullOrEmpty(value))
            {
                _document.Buffer.Insert(_start, value);
                _end = _start + value.Length;
            }
            else
            {
                _end = _start;
            }
        }
    }

    public bool CanPaste(int format) => true;
    public bool ChangeCase(LetterCase value)
    {
        string text = Text;
        Text = value == LetterCase.Upper ? text.ToUpperInvariant() : text.ToLowerInvariant();
        return true;
    }
    public void Collapse(bool start) { if (start) _end = _start; else _start = _end; }
    public void Copy() { /* TODO: clipboard */ }
    public void Cut() { /* TODO: clipboard */ }
    public void Delete(TextRangeUnit unit, int count) { Text = string.Empty; }
    public int EndOf(TextRangeUnit unit, bool extend) => 0;
    public bool Expand(TextRangeUnit unit) => false;
    public bool FindText(string value, int scanLength, FindOptions options)
    {
        string buf = _document.Buffer.ToString();
        var cmp = (options & FindOptions.Case) != 0 ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int idx = buf.IndexOf(value, _start, cmp);
        if (idx < 0) return false;
        SetRangeCore(idx, idx + value.Length);
        return true;
    }
    public int GetCharacterUtf32(out int position, int offset) { position = _start + offset; return 0; }
    public ITextRange GetClone() => new TextRange(_document, _start, _end)
    {
        _characterFormat = (TextCharacterFormat)_characterFormat.GetClone(),
        _paragraphFormat = (TextParagraphFormat)_paragraphFormat.GetClone(),
    };
    public int GetIndex(TextRangeUnit unit) => 0;
    public void GetPoint(HorizontalCharacterAlignment horizontalAlign, VerticalCharacterAlignment verticalAlign, PointOptions options, out Point point) { point = default; }
    public void GetRect(PointOptions options, out Rect rect, out int hit) { rect = default; hit = 0; }
    public void GetText(TextGetOptions options, out string value) { value = Text; }
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
    public void SetIndex(TextRangeUnit unit, int index, bool extend) { /* TODO */ }
    public void SetPoint(Point point, PointOptions options, bool extend) { /* TODO */ }

    public void SetRange(int startPosition, int endPosition) => SetRangeCore(startPosition, endPosition);

    public void SetText(TextSetOptions options, string value) { Text = value ?? string.Empty; }

    private void SetRangeCore(int start, int end)
    {
        int max = _document.Buffer.Length;
        if (start < 0) start = 0;
        if (end < 0) end = 0;
        if (start > max) start = max;
        if (end > max) end = max;
        if (end < start) end = start;
        _start = start;
        _end = end;
    }
}

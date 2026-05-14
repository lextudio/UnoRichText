using System.Collections.Specialized;
using System.Text;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using IoPath = System.IO.Path;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Pretext;
using System.Windows.Documents;

namespace LeXtudio.UI.Xaml.Controls;

public class RichTextBlock : Panel
{
    private static SolidColorBrush? _selectionBrush;
    private static SolidColorBrush SelectionBrush =>
        _selectionBrush ??= new(Color.FromArgb(80, 0, 120, 215));
    private static InputCursor? _textSelectionCursor;
    private static InputCursor TextSelectionCursor =>
        _textSelectionCursor ??= InputSystemCursor.Create(InputSystemCursorShape.IBeam);
    private static InputCursor? _hyperlinkCursor;
    private static InputCursor HyperlinkCursor =>
        _hyperlinkCursor ??= InputSystemCursor.Create(InputSystemCursorShape.Hand);

    // All live RichTextBlock instances — used to clear other blocks' selections on pointer press.
    private static readonly List<WeakReference<RichTextBlock>> AllInstances = new();
    private static readonly System.Windows.DependencyObject WpfCollectionOwner = new();

    private readonly Canvas _canvas = new();
    private readonly InlineCollection _inlines;
    private readonly BlockCollection _blocks;
    private readonly FocusSink _focusSink = new();
    private readonly RichTextBlockTextLayoutHost _textLayoutHost;

    private FlatItem[]? _flatItems;
    private int[] _flatItemCharOffsets = Array.Empty<int>();
    private PreparedSegment[] _preparedSegments = Array.Empty<PreparedSegment>();
    private double _totalHeight;

    private readonly List<PlacedFragment> _placedFragments = new();
    private int _selectionAnchor = -1;
    private int _selectionFocus = -1;
    private bool _isPointerDown;
    private bool _isTextSelectionEnabled = true;

    public RichTextBlock()
    {
        VerticalAlignment = VerticalAlignment.Top;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        _textLayoutHost = new RichTextBlockTextLayoutHost(this);
        _inlines = new InlineCollection(WpfCollectionOwner, true);
        _blocks = new BlockCollection(WpfCollectionOwner, true);

        lock (AllInstances)
        {
            AllInstances.RemoveAll(wr => !wr.TryGetTarget(out _));
            AllInstances.Add(new WeakReference<RichTextBlock>(this));
        }

        Children.Add(_canvas);
        Children.Add(_focusSink);
        _inlines.CollectionChanged += OnContentChanged;
        _blocks.CollectionChanged += OnBlocksChanged;
        AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
        AddHandler(PointerMovedEvent, new PointerEventHandler(OnPointerMoved), true);
        AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
        _focusSink.KeyDown += OnKeyDown;
        UpdateSelectionCursor();
    }

    // ── Diagnostics ───────────────────────────────────────────────────

    public static bool DiagnosticsEnabled { get; set; }

    private static string DiagPath =>
        IoPath.Combine(IoPath.GetTempPath(), "LeXtudio.RichText.Diag.txt");

    private void DiagLog(string message)
    {
        if (!DiagnosticsEnabled) return;
        var line = $"[{DateTime.Now:HH:mm:ss.fff}] [{GetHashCode():X8}] {message}";
        System.Diagnostics.Debug.WriteLine(line);
        File.AppendAllText(DiagPath, line + Environment.NewLine);
    }

    public string GetDiagnosticSnapshot()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== RichTextBlock {GetHashCode():X8} ===");
        sb.AppendLine($"  Inlines.Count: {_inlines.Count}");
        sb.AppendLine($"  Blocks.Count: {_blocks.Count}");
        sb.AppendLine($"  FlatItems: {(_flatItems?.Length.ToString() ?? "null")}");
        if (_flatItems != null)
        {
            for (var i = 0; i < _flatItems.Length; i++)
            {
                var fi = _flatItems[i];
                if (fi is TextRunItem t)
                    sb.AppendLine($"    [{i}] TextRunItem: \"{t.Text.Replace("\n", "\\n")}\"");
                else if (fi is UiContainerItem u)
                    sb.AppendLine($"    [{i}] UiContainerItem: MeasuredSize={u.MeasuredWidth:F1}x{u.MeasuredHeight:F1} Type={u.Child.GetType().Name}");
            }
        }
        sb.AppendLine($"  PreparedSegments: {_preparedSegments.Length}");
        sb.AppendLine($"  PlacedFragments: {_placedFragments.Count}");
        foreach (var f in _placedFragments)
            sb.AppendLine($"    Frag [{f.CharStart}..{f.CharEnd}] @ ({f.X:F1},{f.Y:F1}) w={f.Width:F1} \"{f.Text.Replace("\n", "\\n")}\"");
        return sb.ToString();
    }

    // ── Public API ────────────────────────────────────────────────────

    public InlineCollection Inlines => _inlines;
    public BlockCollection Blocks => _blocks;
    public System.Windows.Documents.ITextLayoutHost TextLayoutHost => _textLayoutHost;
    internal bool IsTextLayoutValid => _preparedSegments.Length == 0 || _flatItems is not null;
    internal double ExtentHeight => _totalHeight;

    public FontFamily FontFamily { get; set; } = new FontFamily("Segoe UI");
    public double FontSize { get; set; } = 14;
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public Brush Foreground { get; set; } = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
    public TextWrapping TextWrapping { get; set; } = TextWrapping.WrapWholeWords;
    public double LineHeight { get; set; }

    public bool IsTextSelectionEnabled
    {
        get => _isTextSelectionEnabled;
        set
        {
            if (_isTextSelectionEnabled == value)
                return;

            _isTextSelectionEnabled = value;
            if (!value)
                ClearSelectionSilent();
            UpdateSelectionCursor();
        }
    }

    public string SelectedText => BuildSelectedText();

    public event EventHandler<HyperlinkClickEventArgs>? LinkClicked;

    public void SelectAll()
    {
        if (_flatItemCharOffsets.Length == 0) return;
        _selectionAnchor = 0;
        _selectionFocus = _flatItemCharOffsets[^1];
        InvalidateArrange();
    }

    public void ClearSelection()
    {
        _selectionAnchor = -1;
        _selectionFocus = -1;
        InvalidateArrange();
    }

    // ── Layout ────────────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        var root = RootProperties();
        var flatItems = new List<FlatItem>();
        CollectFlatItems(flatItems, root);

        _focusSink.Measure(new Size(0, 0));

        if (flatItems.Count == 0)
        {
            _flatItems = Array.Empty<FlatItem>();
            _flatItemCharOffsets = Array.Empty<int>();
            _preparedSegments = Array.Empty<PreparedSegment>();
            _totalHeight = 0;
            _canvas.Measure(new Size(0, 0));
            return Size.Empty;
        }

        // Add UI container children to the canvas so they have a visual tree parent
        // during measurement — required for accurate DesiredSize on Uno Skia.
        foreach (var item in flatItems.OfType<UiContainerItem>())
        {
            _canvas.Children.Add(item.Child);
            item.Child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            // Cache before removal — removing from canvas may reset DesiredSize on Uno Skia.
            item.MeasuredWidth = item.Child.DesiredSize.Width;
            item.MeasuredHeight = item.Child.DesiredSize.Height;
            _canvas.Children.Remove(item.Child);
            DiagLog($"UiContainer measured: {item.MeasuredWidth:F1}x{item.MeasuredHeight:F1} Type={item.Child.GetType().Name}");
        }

        _flatItems = flatItems.ToArray();
        _flatItemCharOffsets = BuildCharOffsets(flatItems);

        // Split flat items at \n boundaries; each segment gets its own PreparedRichInline.
        double maxWidth = double.IsPositiveInfinity(availableSize.Width) ? 9999 : availableSize.Width;
        var segments = new List<PreparedSegment>();
        var segStart = 0;
        for (var i = 0; i <= flatItems.Count; i++)
        {
            var isNewline = i < flatItems.Count && flatItems[i] is TextRunItem { Text: "\n" };
            var isEnd = i == flatItems.Count;
            if (isNewline || isEnd)
            {
                if (i > segStart)
                {
                    var segItems = flatItems.GetRange(segStart, i - segStart);
                    // Convert leading space-only items to NBSP so PretextSharp's
                    // TrimCollapsibleBoundaryText doesn't strip code indentation.
                    for (var j = 0; j < segItems.Count; j++)
                    {
                        if (segItems[j] is TextRunItem { Text: var t } ti && t.Length > 0 && t.All(c => c == ' '))
                            segItems[j] = ti with { Text = new string(' ', t.Length) };
                        else
                            break;
                    }
                    var richItems = BuildRichInlineItems(segItems);
                    DiagLog($"Segment [{segStart}..{i}) richItems={richItems.Length}");
                    var prepared = PretextLayout.PrepareRichInline(richItems);
                    if (prepared != null)
                        segments.Add(new PreparedSegment(prepared, segStart, richItems));
                }
                segStart = i + 1; // skip the \n item
            }
        }
        _preparedSegments = segments.ToArray();
        DiagLog($"PreparedSegments: {_preparedSegments.Length}");

        if (_preparedSegments.Length == 0)
        {
            _totalHeight = 0;
            _canvas.Measure(new Size(0, 0));
            return Size.Empty;
        }

        double totalHeight = 0;
        double maxLineWidth = 0;
        foreach (var seg in _preparedSegments)
        {
            var stats = PretextLayout.MeasureRichInlineStats(seg.Prepared, maxWidth);
            var segLineHeight = ResolvedLineHeight;
            for (var j = seg.FlatItemOffset; j < seg.FlatItemOffset + seg.Items.Length && j < _flatItems.Length; j++)
            {
                if (_flatItems[j] is TextRunItem t)
                    segLineHeight = Math.Max(segLineHeight, t.Props.FontSize * 1.4);
                else if (_flatItems[j] is UiContainerItem ui)
                    segLineHeight = Math.Max(segLineHeight, ui.MeasuredHeight);
            }
            totalHeight += stats.LineCount * segLineHeight;
            maxLineWidth = Math.Max(maxLineWidth, stats.MaxLineWidth);
        }
        _totalHeight = totalHeight;

        var desired = new Size(Math.Min(maxLineWidth, availableSize.Width), _totalHeight);
        _canvas.Measure(desired);
        return desired;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _canvas.Children.Clear();
        _placedFragments.Clear();

        if (_preparedSegments.Length == 0 || _flatItems == null)
        {
            _canvas.Arrange(new Rect(0, 0, 0, 0));
            return finalSize;
        }

        double maxWidth = finalSize.Width > 0 ? finalSize.Width : 9999;
        double currentY = 0;
        var selMin = Math.Min(_selectionAnchor, _selectionFocus);
        var selMax = Math.Max(_selectionAnchor, _selectionFocus);
        var hasSelection = IsTextSelectionEnabled && _selectionAnchor >= 0 && _selectionFocus >= 0
                           && _selectionAnchor != _selectionFocus;

        var consumedPerItem = new Dictionary<int, int>();

        foreach (var seg in _preparedSegments)
        {
            PretextLayout.WalkRichInlineLineRanges(seg.Prepared, maxWidth, range =>
            {
                var line = PretextLayout.MaterializeRichInlineLineRange(seg.Prepared, range);
                double x = 0;
                double y = currentY;
                double lineHeight = ResolvedLineHeight;

                var lineFragments = new List<PendingPlacedFragment>();

                foreach (var fragment in line.Fragments)
                {
                    x += fragment.GapBefore;

                    // fragment.ItemIndex is relative to the segment; offset to global flat item index.
                    var flatIndex = seg.FlatItemOffset + fragment.ItemIndex;
                    var flatItem = _flatItems[flatIndex];
                    int fragCharStart, fragCharEnd;
                    string fragText;

                    if (flatItem is UiContainerItem)
                    {
                        var itemBase = flatIndex < _flatItemCharOffsets.Length
                            ? _flatItemCharOffsets[flatIndex] : 0;
                        fragCharStart = itemBase;
                        fragCharEnd = itemBase;
                        fragText = "￼";
                    }
                    else if (flatItem is TextRunItem)
                    {
                        var itemBase = flatIndex < _flatItemCharOffsets.Length
                            ? _flatItemCharOffsets[flatIndex] : 0;
                        var consumed = consumedPerItem.GetValueOrDefault(flatIndex, 0);
                        fragCharStart = itemBase + consumed;
                        fragCharEnd = fragCharStart + fragment.Text.Length;
                        consumedPerItem[flatIndex] = consumed + fragment.Text.Length;
                        fragText = fragment.Text;
                    }
                    else
                    {
                        x += fragment.OccupiedWidth;
                        return;
                    }

                    // Determine the actual rendered width using Uno's own font metrics.
                    // PretextSharp's OccupiedWidth may over/under-estimate for some fonts;
                    // measuring the TextBlock gives the ground-truth width for x-advance and
                    // selection rects, eliminating gaps and clipping.
                    double fragmentWidth;
                    UIElement el;
                    if (flatItem is UiContainerItem uiItem)
                    {
                        fragmentWidth = uiItem.MeasuredWidth > 0 ? uiItem.MeasuredWidth : fragment.OccupiedWidth;
                        lineHeight = Math.Max(lineHeight, uiItem.MeasuredHeight);
                        DiagLog($"Placing UiContainer @ ({x:F1},{y:F1}) w={fragmentWidth:F1} h={uiItem.MeasuredHeight:F1}");
                        el = uiItem.Child;
                    }
                    else
                    {
                        var textItem = (TextRunItem)flatItem;
                        var tb = new Microsoft.UI.Xaml.Controls.TextBlock
                        {
                            Text = fragment.Text,
                            FontFamily = textItem.Props.FontFamily,
                            FontSize = textItem.Props.FontSize,
                            FontWeight = textItem.Props.FontWeight,
                            FontStyle = textItem.Props.FontStyle,
                            Foreground = textItem.Props.Foreground,
                            TextWrapping = TextWrapping.NoWrap,
                        };
                        tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        fragmentWidth = tb.DesiredSize.Width > 0 ? tb.DesiredSize.Width : fragment.OccupiedWidth;
                        if (tb.DesiredSize.Height > 0)
                            lineHeight = Math.Max(lineHeight, tb.DesiredSize.Height);
                        if (textItem.Hyperlink is { } link)
                        {
                            tb.PointerEntered += (_, _) => ProtectedCursor = HyperlinkCursor;
                            tb.PointerExited += (_, _) => UpdateSelectionCursor();
                            tb.Tapped += (_, _) => OnHyperlinkTapped(link);
                        }
                        el = tb;
                    }

                    lineFragments.Add(new PendingPlacedFragment(
                        el, flatItem, x, y, fragmentWidth,
                        fragCharStart, fragCharEnd, fragText));
                    x += fragmentWidth;
                }

                foreach (var placed in lineFragments)
                {
                    _placedFragments.Add(new PlacedFragment(
                        placed.X, placed.Y, placed.Width, lineHeight,
                        placed.CharStart, placed.CharEnd, placed.Text));

                    // Selection highlight behind the fragment.
                    if (hasSelection
                        && placed.CharEnd > placed.CharStart
                        && placed.CharEnd > selMin
                        && placed.CharStart < selMax)
                    {
                        var len = placed.CharEnd - placed.CharStart;
                        var t0 = (double)(Math.Max(selMin, placed.CharStart) - placed.CharStart) / len;
                        var t1 = (double)(Math.Min(selMax, placed.CharEnd) - placed.CharStart) / len;
                        var highlight = new Rectangle
                        {
                            Width = (t1 - t0) * placed.Width,
                            Height = lineHeight,
                            Fill = SelectionBrush
                        };
                        Canvas.SetLeft(highlight, placed.X + t0 * placed.Width);
                        Canvas.SetTop(highlight, placed.Y);
                        _canvas.Children.Add(highlight);
                    }

                    if (placed.FlatItem is TextRunItem textRunItem)
                        DrawDecorations(textRunItem.Props.TextDecorations, textRunItem.Props.Foreground,
                            placed.X, placed.Y, placed.Width, lineHeight);

                    Canvas.SetLeft(placed.Element, placed.X);
                    Canvas.SetTop(placed.Element, placed.Y);
                    _canvas.Children.Add(placed.Element);
                }

                currentY += lineHeight;
            });
        }

        _totalHeight = currentY;
        _focusSink.Arrange(new Rect(0, 0, 0, 0));
        _canvas.Width = finalSize.Width;
        _canvas.Height = _totalHeight;
        _canvas.Arrange(new Rect(0, 0, finalSize.Width, _totalHeight));
        return new Size(finalSize.Width, _totalHeight);
    }

    private void DrawDecorations(TextDecorations decorations, Brush foreground, double x, double y, double width, double lineHeight)
    {
        if (decorations == TextDecorations.None) return;

        if ((decorations & TextDecorations.Strikethrough) != 0)
        {
            var r = new Rectangle { Width = width, Height = 1, Fill = foreground };
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y + lineHeight * 0.55);
            _canvas.Children.Add(r);
        }
        if ((decorations & TextDecorations.Underline) != 0)
        {
            var r = new Rectangle { Width = width, Height = 1, Fill = foreground };
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y + lineHeight * 0.88);
            _canvas.Children.Add(r);
        }
    }

    // ── Text selection ────────────────────────────────────────────────

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!IsTextSelectionEnabled) return;
        if (IsNestedRichTextPointerEvent(e)) return;

        _focusSink.Focus(FocusState.Pointer);
        var pt = e.GetCurrentPoint(this).Position;
        var idx = HitTest(pt);
        if (idx < 0) return;
        NotifyGroupSelectionStarting();
        _selectionAnchor = idx;
        _selectionFocus = idx;
        _isPointerDown = true;
        CapturePointer(e.Pointer);
        InvalidateArrange();
        e.Handled = true;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!IsTextSelectionEnabled || !_isPointerDown) return;
        var pt = e.GetCurrentPoint(this).Position;
        var idx = HitTest(pt, clampToContent: true);
        if (idx >= 0)
        {
            _selectionFocus = idx;
            InvalidateArrange();
        }
        e.Handled = true;
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!IsTextSelectionEnabled || !_isPointerDown) return;
        _isPointerDown = false;
        ReleasePointerCapture(e.Pointer);
        e.Handled = true;
    }

    private bool IsNestedRichTextPointerEvent(PointerRoutedEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source)
            return false;

        var current = source;
        while (current is not null && !ReferenceEquals(current, this))
        {
            if (current is RichTextBlock)
                return true;

            current = VisualTreeHelper.GetParent(current);
        }

        return false;
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!IsTextSelectionEnabled) return;
        var ctrl = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
                    & CoreVirtualKeyStates.Down) != 0;
        if (!ctrl) return;

        if (e.Key == VirtualKey.A)
        {
            SelectAll();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.C)
        {
            var text = SelectedText;
            if (!string.IsNullOrEmpty(text))
            {
                var dp = new DataPackage();
                dp.SetText(text);
                Clipboard.SetContent(dp);
            }
            e.Handled = true;
        }
    }

    private int HitTest(Point pt)
        => HitTest(pt, clampToContent: false);

    private int HitTest(Point pt, bool clampToContent)
    {
        if (_flatItemCharOffsets.Length == 0)
            return -1;

        PlacedFragment? best = null;
        double bestDist = double.MaxValue;

        foreach (var frag in _placedFragments)
        {
            var dx = pt.X < frag.X ? frag.X - pt.X :
                pt.X > frag.X + frag.Width ? pt.X - (frag.X + frag.Width) : 0;
            var dy = pt.Y < frag.Y ? frag.Y - pt.Y :
                pt.Y > frag.Y + frag.Height ? pt.Y - (frag.Y + frag.Height) : 0;
            var dist = (dy * dy * 4) + (dx * dx);

            if (dist < bestDist) { bestDist = dist; best = frag; }
        }

        if (best is null)
            return clampToContent
                ? pt.Y < 0 ? 0 : TextLength
                : -1;

        if (pt.X >= best.X && pt.X <= best.X + best.Width
            && pt.Y >= best.Y && pt.Y <= best.Y + best.Height)
        {
            var len = best.CharEnd - best.CharStart;
            if (len == 0) return clampToContent ? best.CharStart : -1;
            var t = (pt.X - best.X) / best.Width;
            return best.CharStart + (int)Math.Round(t * len);
        }

        return pt.X <= best.X ? best.CharStart : best.CharEnd;
    }

    private int TextLength => _flatItemCharOffsets.Length == 0 ? 0 : _flatItemCharOffsets[^1];

    private string BuildSelectedText()
    {
        if (_selectionAnchor < 0 || _selectionFocus < 0 || _selectionAnchor == _selectionFocus
            || _flatItems is null || _flatItemCharOffsets.Length == 0)
            return string.Empty;

        var min = Math.Min(_selectionAnchor, _selectionFocus);
        var max = Math.Max(_selectionAnchor, _selectionFocus);
        var sb = new StringBuilder();

        for (var i = 0; i < _flatItems.Length; i++)
        {
            var charStart = _flatItemCharOffsets[i];
            var charEnd = _flatItemCharOffsets[i + 1];
            if (charEnd <= min || charStart >= max) continue;

            var localStart = Math.Max(min, charStart) - charStart;
            var localEnd = Math.Min(max, charEnd) - charStart;

            if (_flatItems[i] is TextRunItem textItem)
            {
                if (localEnd > localStart)
                    sb.Append(textItem.Text[localStart..Math.Min(localEnd, textItem.Text.Length)]);
            }
        }

        return sb.ToString();
    }

    // ── Selection coordination ────────────────────────────────────────

    private void NotifyGroupSelectionStarting()
    {
        lock (AllInstances)
        {
            foreach (var wr in AllInstances)
            {
                if (wr.TryGetTarget(out var other) && !ReferenceEquals(other, this))
                    other.ClearSelectionSilent();
            }
        }
    }

    private void ClearSelectionSilent()
    {
        _selectionAnchor = -1;
        _selectionFocus = -1;
        InvalidateArrange();
    }

    private static List<RichTextBlock> GetLiveBlocks()
    {
        lock (AllInstances)
        {
            var blocks = new List<RichTextBlock>();
            AllInstances.RemoveAll(wr => !wr.TryGetTarget(out _));
            foreach (var wr in AllInstances)
            {
                if (wr.TryGetTarget(out var block))
                    blocks.Add(block);
            }

            return blocks;
        }
    }

    private void UpdateSelectionCursor()
    {
        ProtectedCursor = IsTextSelectionEnabled ? TextSelectionCursor : null;
    }

    // ── Content collection ────────────────────────────────────────────

    private void CollectFlatItems(List<FlatItem> result, InheritedProperties root)
    {
        if (_inlines.Count > 0)
        {
            FlattenInlines(_inlines, result, root);
            return;
        }

        for (var i = 0; i < _blocks.Count; i++)
        {
            if (i > 0)
                result.Add(new TextRunItem("\n", root));
            if (_blocks[i] is Paragraph bp)
            {
                var blockProps = root;
                if (!double.IsNaN(bp.FontSize)) blockProps = blockProps with { FontSize = bp.FontSize };
                if (bp.FontWeight.Weight != FontWeights.Normal.Weight) blockProps = blockProps with { FontWeight = bp.FontWeight };
                if (bp.FontFamily is not null) blockProps = blockProps with { FontFamily = bp.FontFamily };
                if (bp.Foreground is not null) blockProps = blockProps with { Foreground = bp.Foreground };
                FlattenInlines(bp.Inlines, result, blockProps);
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────

    private double ResolvedLineHeight => LineHeight > 0 ? LineHeight : FontSize * 1.4;

    private InheritedProperties RootProperties() => new(
        FontFamily, FontSize, FontWeight, FontStyle, Foreground, TextDecorations.None);

    private void OnContentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => InvalidateMeasure();

    private void OnBlocksChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
                if (item is Paragraph p) p.Inlines.CollectionChanged += OnContentChanged;
        }
        InvalidateMeasure();
    }

    private void OnHyperlinkTapped(Hyperlink link)
    {
        link.RaiseClick();
        LinkClicked?.Invoke(this, new HyperlinkClickEventArgs(link));
    }

    // ── Flatten ───────────────────────────────────────────────────────

    private static void FlattenInlines(
        InlineCollection inlines,
        List<FlatItem> result,
        InheritedProperties inherited,
        Hyperlink? currentLink = null)
    {
        foreach (var inline in inlines)
        {
            var props = inherited.Merge(inline);
            switch (inline)
            {
                case Run run when run.Text.Length > 0:
                    // Normalize all line-ending styles (\r\n, \r, \n) before splitting.
                    // \r\n may arrive as two separate Run items (\r then \n) from syntax highlighters;
                    // deduplication prevents a double newline in that case.
                    var normalized = run.Text.Replace("\r\n", "\n").Replace("\r", "\n");
                    var segments = normalized.Split('\n');
                    for (var si = 0; si < segments.Length; si++)
                    {
                        if (segments[si].Length > 0)
                            result.Add(new TextRunItem(segments[si], props, currentLink));
                        if (si < segments.Length - 1)
                        {
                            if (result.Count == 0 || result[^1] is not TextRunItem { Text: "\n" })
                                result.Add(new TextRunItem("\n", props, currentLink));
                        }
                    }
                    break;
                case LineBreak:
                    result.Add(new TextRunItem("\n", props, currentLink));
                    break;
                case InlineUIContainer { Child: { } child }:
                    result.Add(new UiContainerItem(child, props));
                    break;
                case Hyperlink link:
                    FlattenInlines(link.Inlines, result, props, link);
                    break;
                case Span span:
                    FlattenInlines(span.Inlines, result, props, currentLink);
                    break;
            }
        }
    }

    // ── PretextSharp items ────────────────────────────────────────────

    private static RichInlineItem[] BuildRichInlineItems(List<FlatItem> flatItems)
    {
        var result = new RichInlineItem[flatItems.Count];
        for (var i = 0; i < flatItems.Count; i++)
        {
            result[i] = flatItems[i] switch
            {
                UiContainerItem ui => new RichInlineItem(
                    " ",  // non-breaking space: passes PretextSharp's LayoutNextLineRange filter (U+200B does not)
                    ui.Props.ToPretextFontString(),
                    RichInlineBreakMode.Never,
                    ui.MeasuredWidth),
                TextRunItem text => new RichInlineItem(
                    text.Text,
                    text.Props.ToPretextFontString()),
                _ => new RichInlineItem(string.Empty, "14px sans-serif")
            };
        }
        return result;
    }

    private static int[] BuildCharOffsets(List<FlatItem> items)
    {
        var offsets = new int[items.Count + 1];
        for (var i = 0; i < items.Count; i++)
            offsets[i + 1] = offsets[i] + (items[i] is TextRunItem t ? t.Text.Length : 1);
        return offsets;
    }

    // ── Flat item types ───────────────────────────────────────────────

    private abstract record FlatItem(InheritedProperties Props);

    private record TextRunItem(string Text, InheritedProperties Props, Hyperlink? Hyperlink = null)
        : FlatItem(Props);

    private record UiContainerItem(UIElement Child, InheritedProperties Props) : FlatItem(Props)
    {
        // Cached measured size — set during MeasureOverride after adding to visual tree.
        public double MeasuredWidth { get; set; }
        public double MeasuredHeight { get; set; }
    }

    private record PreparedSegment(PreparedRichInline Prepared, int FlatItemOffset, RichInlineItem[] Items);

    private record PendingPlacedFragment(
        UIElement Element,
        FlatItem FlatItem,
        double X,
        double Y,
        double Width,
        int CharStart,
        int CharEnd,
        string Text);

    private record PlacedFragment(double X, double Y, double Width, double Height,
        int CharStart, int CharEnd, string Text);

    // ── Focus sink for keyboard events ────────────────────────────────

    private sealed class FocusSink : Control
    {
        public FocusSink()
        {
            IsTabStop = true;
            Width = 0;
            Height = 0;
            Opacity = 0;
        }
    }
}

public sealed class HyperlinkClickEventArgs(Hyperlink hyperlink) : EventArgs
{
    public Hyperlink Hyperlink { get; } = hyperlink;
}

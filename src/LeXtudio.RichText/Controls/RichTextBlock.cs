using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
#if WINDOWS_APP_SDK
public partial class RichTextBlock : Microsoft.UI.Xaml.Controls.RichTextBlock
{
    // No stubs here: the real WinUI control is used when targeting Windows.
}
#else
public class RichTextBlock : Panel
{
    public static DependencyProperty CharacterSpacingProperty { get; } =
        DependencyProperty.Register(nameof(CharacterSpacing), typeof(int), typeof(RichTextBlock), new PropertyMetadata(0, OnLayoutPropertyChanged));
    public static DependencyProperty FontFamilyProperty { get; } =
        DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(RichTextBlock), new PropertyMetadata(new FontFamily("Segoe UI"), OnLayoutPropertyChanged));
    public static DependencyProperty FontSizeProperty { get; } =
        DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(RichTextBlock), new PropertyMetadata(14d, OnLayoutPropertyChanged));
    public static DependencyProperty FontStretchProperty { get; } =
        DependencyProperty.Register(nameof(FontStretch), typeof(FontStretch), typeof(RichTextBlock), new PropertyMetadata(System.Windows.FontStretches.Normal, OnLayoutPropertyChanged));
    public static DependencyProperty FontStyleProperty { get; } =
        DependencyProperty.Register(nameof(FontStyle), typeof(FontStyle), typeof(RichTextBlock), new PropertyMetadata(FontStyle.Normal, OnLayoutPropertyChanged));
    public static DependencyProperty FontWeightProperty { get; } =
        DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(RichTextBlock), new PropertyMetadata(FontWeights.Normal, OnLayoutPropertyChanged));
    public static DependencyProperty ForegroundProperty { get; } =
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(RichTextBlock), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), OnLayoutPropertyChanged));
    public static DependencyProperty HasOverflowContentProperty { get; } =
        DependencyProperty.Register(nameof(HasOverflowContent), typeof(bool), typeof(RichTextBlock), new PropertyMetadata(false));
    public static DependencyProperty HorizontalTextAlignmentProperty { get; } =
        DependencyProperty.Register(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(RichTextBlock), new PropertyMetadata(TextAlignment.Left, OnLayoutPropertyChanged));
    public static DependencyProperty IsColorFontEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsColorFontEnabled), typeof(bool), typeof(RichTextBlock), new PropertyMetadata(true, OnLayoutPropertyChanged));
    public static DependencyProperty IsTextScaleFactorEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsTextScaleFactorEnabled), typeof(bool), typeof(RichTextBlock), new PropertyMetadata(true, OnLayoutPropertyChanged));
    public static DependencyProperty IsTextSelectionEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsTextSelectionEnabled), typeof(bool), typeof(RichTextBlock), new PropertyMetadata(true, OnIsTextSelectionEnabledChanged));
    public static DependencyProperty IsTextTrimmedProperty { get; } =
        DependencyProperty.Register(nameof(IsTextTrimmed), typeof(bool), typeof(RichTextBlock), new PropertyMetadata(false));
    public static DependencyProperty LineHeightProperty { get; } =
        DependencyProperty.Register(nameof(LineHeight), typeof(double), typeof(RichTextBlock), new PropertyMetadata(0d, OnLayoutPropertyChanged));
    public static DependencyProperty LineStackingStrategyProperty { get; } =
        DependencyProperty.Register(nameof(LineStackingStrategy), typeof(LineStackingStrategy), typeof(RichTextBlock), new PropertyMetadata(LineStackingStrategy.MaxHeight, OnLayoutPropertyChanged));
    public static DependencyProperty MaxLinesProperty { get; } =
        DependencyProperty.Register(nameof(MaxLines), typeof(int), typeof(RichTextBlock), new PropertyMetadata(0, OnLayoutPropertyChanged));
    public static DependencyProperty OpticalMarginAlignmentProperty { get; } =
        DependencyProperty.Register(nameof(OpticalMarginAlignment), typeof(OpticalMarginAlignment), typeof(RichTextBlock), new PropertyMetadata(OpticalMarginAlignment.None, OnLayoutPropertyChanged));
    public static DependencyProperty OverflowContentTargetProperty { get; } =
        DependencyProperty.Register(nameof(OverflowContentTarget), typeof(RichTextBlockOverflow), typeof(RichTextBlock), new PropertyMetadata(null, OnOverflowContentTargetChanged));
    public static DependencyProperty PaddingProperty { get; } =
        DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(RichTextBlock), new PropertyMetadata(default(Thickness), OnLayoutPropertyChanged));
    public static DependencyProperty SelectedTextProperty { get; } =
        DependencyProperty.Register(nameof(SelectedText), typeof(string), typeof(RichTextBlock), new PropertyMetadata(string.Empty));
    public static DependencyProperty SelectionFlyoutProperty { get; } =
        DependencyProperty.Register(nameof(SelectionFlyout), typeof(FlyoutBase), typeof(RichTextBlock), new PropertyMetadata(null));
    public static DependencyProperty SelectionHighlightColorProperty { get; } =
        DependencyProperty.Register(nameof(SelectionHighlightColor), typeof(SolidColorBrush), typeof(RichTextBlock), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(80, 0, 120, 215)), OnSelectionVisualPropertyChanged));
    public static DependencyProperty TextAlignmentProperty { get; } =
        DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(RichTextBlock), new PropertyMetadata(TextAlignment.Left, OnLayoutPropertyChanged));
    public static DependencyProperty TextDecorationsProperty { get; } =
        DependencyProperty.Register(nameof(TextDecorations), typeof(TextDecorations), typeof(RichTextBlock), new PropertyMetadata(TextDecorations.None, OnLayoutPropertyChanged));
    public static DependencyProperty TextIndentProperty { get; } =
        DependencyProperty.Register(nameof(TextIndent), typeof(double), typeof(RichTextBlock), new PropertyMetadata(0d, OnLayoutPropertyChanged));
    public static DependencyProperty TextLineBoundsProperty { get; } =
        DependencyProperty.Register(nameof(TextLineBounds), typeof(TextLineBounds), typeof(RichTextBlock), new PropertyMetadata(TextLineBounds.Full, OnLayoutPropertyChanged));
    public static DependencyProperty TextReadingOrderProperty { get; } =
        DependencyProperty.Register(nameof(TextReadingOrder), typeof(TextReadingOrder), typeof(RichTextBlock), new PropertyMetadata(TextReadingOrder.DetectFromContent, OnLayoutPropertyChanged));
    public static DependencyProperty TextTrimmingProperty { get; } =
        DependencyProperty.Register(nameof(TextTrimming), typeof(TextTrimming), typeof(RichTextBlock), new PropertyMetadata(TextTrimming.None, OnLayoutPropertyChanged));
    public static DependencyProperty TextWrappingProperty { get; } =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(RichTextBlock), new PropertyMetadata(TextWrapping.WrapWholeWords, OnLayoutPropertyChanged));

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
    private static readonly System.Windows.FrameworkContentElement WpfCollectionOwner = new();

    private readonly Canvas _canvas = new();
    // Use the WPF-shaped collections from System.Windows.Documents (our shim) explicitly —
    // unqualified BlockCollection / InlineCollection would resolve to Uno's not-implemented
    // Microsoft.UI.Xaml.Documents projections.
    private readonly System.Windows.Documents.InlineCollection _inlines;
    private readonly System.Windows.Documents.BlockCollection _blocks;
    private readonly FocusSink _focusSink = new();
    private readonly RichTextBlockTextLayoutHost _textLayoutHost;
    private FlowDocument? _documentSource;

    private FlatItem[]? _flatItems;
    private int[] _flatItemCharOffsets = Array.Empty<int>();
    private PreparedSegment[] _preparedSegments = Array.Empty<PreparedSegment>();
    private double _totalHeight;

    private readonly List<PlacedFragment> _placedFragments = new();
    private readonly List<PlacedLine> _placedLines = new();
    private int _selectionAnchor = -1;
    private int _selectionFocus = -1;
    private bool _isPointerDown;
    private bool _hasKeyboardFocus;
    private double? _preferredCaretX;
    private double _lastConsumedContentHeight;
    private readonly ObservableCollection<TextHighlighter> _textHighlighters = new();

    public RichTextBlock()
    {
        VerticalAlignment = VerticalAlignment.Top;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        _textLayoutHost = new RichTextBlockTextLayoutHost(this);
        _inlines = new System.Windows.Documents.InlineCollection(WpfCollectionOwner, true);
        _blocks = new System.Windows.Documents.BlockCollection(WpfCollectionOwner, true);

        lock (AllInstances)
        {
            AllInstances.RemoveAll(wr => !wr.TryGetTarget(out _));
            AllInstances.Add(new WeakReference<RichTextBlock>(this));
        }

        Children.Add(_canvas);
        Children.Add(_focusSink);
        _inlines.CollectionChanged += OnContentChanged;
        _blocks.CollectionChanged += OnBlocksChanged;
        _textHighlighters.CollectionChanged += OnTextHighlightersChanged;
        AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
        AddHandler(PointerMovedEvent, new PointerEventHandler(OnPointerMoved), true);
        AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
        _focusSink.KeyDown += OnKeyDown;
        _focusSink.GotFocus += OnFocusSinkGotFocus;
        _focusSink.LostFocus += OnFocusSinkLostFocus;
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
        sb.AppendLine($"  DocumentSource.Blocks.Count: {_documentSource?.Blocks.Count.ToString() ?? "null"}");
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

    public System.Windows.Documents.InlineCollection Inlines => _inlines;
    public System.Windows.Documents.BlockCollection Blocks => _blocks;
    public System.Windows.Documents.ITextLayoutHost TextLayoutHost => _textLayoutHost;
    internal bool IsTextLayoutValid => _preparedSegments.Length == 0 || _flatItems is not null;
    internal double ExtentHeight => _totalHeight;

    internal void SetDocumentSource(FlowDocument? document)
    {
        if (ReferenceEquals(_documentSource, document))
        {
            return;
        }

        if (_documentSource is not null)
        {
            DetachBlockHandlers(_documentSource.Blocks);
            _documentSource.Blocks.CollectionChanged -= OnDocumentBlocksChanged;
        }

        _documentSource = document;

        if (_documentSource is not null)
        {
            _documentSource.Blocks.CollectionChanged += OnDocumentBlocksChanged;
            AttachBlockHandlers(_documentSource.Blocks);
        }

        InvalidateMeasure();
    }

    public double BaselineOffset => 0d;
    public int CharacterSpacing
    {
        get => (int)GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }

    public TextPointer ContentEnd => CreateTextPointer(TextLength);
    public TextPointer ContentStart => CreateTextPointer(0);

    public FontFamily FontFamily
    {
        get => (FontFamily)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public FontStretch FontStretch
    {
        get => (FontStretch)GetValue(FontStretchProperty);
        set => SetValue(FontStretchProperty, value);
    }

    public FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    public FontStyle FontStyle
    {
        get => (FontStyle)GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    public Brush Foreground
    {
        get => (Brush)GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public bool HasOverflowContent => (bool)GetValue(HasOverflowContentProperty);

    public TextAlignment HorizontalTextAlignment
    {
        get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    public bool IsColorFontEnabled
    {
        get => (bool)GetValue(IsColorFontEnabledProperty);
        set => SetValue(IsColorFontEnabledProperty, value);
    }

    public bool IsTextScaleFactorEnabled
    {
        get => (bool)GetValue(IsTextScaleFactorEnabledProperty);
        set => SetValue(IsTextScaleFactorEnabledProperty, value);
    }

    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    public bool IsTextTrimmed => (bool)GetValue(IsTextTrimmedProperty);

    public double LineHeight
    {
        get => (double)GetValue(LineHeightProperty);
        set => SetValue(LineHeightProperty, value);
    }

    public LineStackingStrategy LineStackingStrategy
    {
        get => (LineStackingStrategy)GetValue(LineStackingStrategyProperty);
        set => SetValue(LineStackingStrategyProperty, value);
    }

    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    public OpticalMarginAlignment OpticalMarginAlignment
    {
        get => (OpticalMarginAlignment)GetValue(OpticalMarginAlignmentProperty);
        set => SetValue(OpticalMarginAlignmentProperty, value);
    }

    public RichTextBlockOverflow? OverflowContentTarget
    {
        get => (RichTextBlockOverflow?)GetValue(OverflowContentTargetProperty);
        set => SetValue(OverflowContentTargetProperty, value);
    }

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public string SelectedText => BuildSelectedText();

    public TextPointer SelectionEnd => CreateTextPointer(Math.Max(Math.Max(_selectionAnchor, _selectionFocus), 0));

    public FlyoutBase? SelectionFlyout
    {
        get => (FlyoutBase?)GetValue(SelectionFlyoutProperty);
        set => SetValue(SelectionFlyoutProperty, value);
    }

    public SolidColorBrush SelectionHighlightColor
    {
        get => (SolidColorBrush)GetValue(SelectionHighlightColorProperty);
        set => SetValue(SelectionHighlightColorProperty, value);
    }

    public TextPointer SelectionStart => CreateTextPointer(Math.Min(
        _selectionAnchor >= 0 ? _selectionAnchor : TextLength,
        _selectionFocus >= 0 ? _selectionFocus : TextLength));

    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    public TextDecorations TextDecorations
    {
        get => (TextDecorations)GetValue(TextDecorationsProperty);
        set => SetValue(TextDecorationsProperty, value);
    }

    public ObservableCollection<TextHighlighter> TextHighlighters => _textHighlighters;

    public double TextIndent
    {
        get => (double)GetValue(TextIndentProperty);
        set => SetValue(TextIndentProperty, value);
    }

    public TextLineBounds TextLineBounds
    {
        get => (TextLineBounds)GetValue(TextLineBoundsProperty);
        set => SetValue(TextLineBoundsProperty, value);
    }

    public TextReadingOrder TextReadingOrder
    {
        get => (TextReadingOrder)GetValue(TextReadingOrderProperty);
        set => SetValue(TextReadingOrderProperty, value);
    }

    public TextTrimming TextTrimming
    {
        get => (TextTrimming)GetValue(TextTrimmingProperty);
        set => SetValue(TextTrimmingProperty, value);
    }

    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public event EventHandler<HyperlinkClickEventArgs>? LinkClicked;
    public event ContextMenuOpeningEventHandler? ContextMenuOpening;
    public event global::Windows.Foundation.TypedEventHandler<RichTextBlock, IsTextTrimmedChangedEventArgs>? IsTextTrimmedChanged;
    public event RoutedEventHandler? SelectionChanged;

    public void SelectAll()
    {
        EnsureFlatItemsSnapshot();
        if (_flatItemCharOffsets.Length == 0) return;
        SetSelection(0, _flatItemCharOffsets[^1]);
    }

    public void ClearSelection()
    {
        SetSelection(-1, -1);
    }

    public void CopySelectionToClipboard()
    {
        var text = SelectedText;
        if (string.IsNullOrEmpty(text))
            return;

        var dp = new DataPackage();
        dp.SetText(text);
        Clipboard.SetContent(dp);
    }

    public TextPointer GetPositionFromPoint(Point point)
        => CreateTextPointer(HitTest(point, clampToContent: true));

    public void Select(TextPointer start, TextPointer end)
    {
        SetSelection(GetOffset(start), GetOffset(end));
    }

    /// <summary>
    /// Drives caret/selection display from an external editor host (e.g. RichEditBox's _editorHost).
    /// Pass anchor == focus for a caret-only position, different values for a selection range.
    /// Pass hasFocus=false to hide the caret when the host loses keyboard focus.
    /// </summary>
    internal void SetExternalCaret(int anchor, int focus, bool hasFocus)
    {
        _hasKeyboardFocus = hasFocus;
        _selectionAnchor = anchor;
        _selectionFocus = focus;
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
        double maxWidth = TextWrapping == TextWrapping.NoWrap
            ? 9999
            : double.IsPositiveInfinity(availableSize.Width) ? 9999 : availableSize.Width;
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

        var desiredHeight = OverflowContentTarget is not null && !double.IsPositiveInfinity(availableSize.Height)
            ? Math.Min(_totalHeight, availableSize.Height)
            : _totalHeight;
        var desired = new Size(Math.Min(maxLineWidth, availableSize.Width), desiredHeight);
        _canvas.Measure(desired);
        return desired;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        DiagLog($"ArrangeOverride enter finalSize={finalSize.Width:F1}x{finalSize.Height:F1} segs={_preparedSegments.Length} foreground={Foreground}");
        Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };
        var renderResult = RenderContent(
            _canvas,
            finalSize,
            verticalOffset: 0,
            recordHitTest: true,
            requireWholeLineVisibility: OverflowContentTarget is not null);
        _totalHeight = renderResult.TotalHeight;
        _lastConsumedContentHeight = renderResult.ConsumedHeight;
        _focusSink.Arrange(new Rect(0, 0, 0, 0));
        _canvas.Width = finalSize.Width;
        _canvas.Height = finalSize.Height;
        _canvas.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
        SetValue(HasOverflowContentProperty, _totalHeight > _lastConsumedContentHeight);
        OverflowContentTarget?.InvalidateMeasure();
        OverflowContentTarget?.InvalidateArrange();
        var firstTb = _canvas.Children.OfType<Microsoft.UI.Xaml.Controls.TextBlock>().FirstOrDefault();
        DiagLog($"ArrangeOverride exit canvasChildren={_canvas.Children.Count} firstTbForeground={firstTb?.Foreground} firstTbText=\"{firstTb?.Text}\"");
        return finalSize;
    }

    internal double ArrangeOverflowContent(RichTextBlockOverflow overflow, Canvas canvas, Size finalSize)
    {
        if (_preparedSegments.Length == 0 || _flatItems is null)
            Measure(new Size(finalSize.Width, double.PositiveInfinity));

        var offset = _lastConsumedContentHeight;
        var current = OverflowContentTarget;
        while (current is not null && !ReferenceEquals(current, overflow))
        {
            offset = current.LastConsumedContentHeight > offset
                ? current.LastConsumedContentHeight
                : offset + current.LastViewportHeight;
            current = current.OverflowContentTarget;
        }

        var renderResult = RenderContent(
            canvas,
            finalSize,
            offset,
            recordHitTest: false,
            requireWholeLineVisibility: true);
        overflow.SetOverflowState(renderResult.ConsumedHeight, renderResult.TotalHeight > renderResult.ConsumedHeight, false);
        return renderResult.TotalHeight;
    }

    private RenderResult RenderContent(Canvas target, Size finalSize, double verticalOffset, bool recordHitTest, bool requireWholeLineVisibility)
    {
        target.Children.Clear();
        if (recordHitTest)
        {
            _placedFragments.Clear();
            _placedLines.Clear();
        }

        if (_preparedSegments.Length == 0 || _flatItems == null)
        {
            target.Measure(new Size(0, 0));
            return new RenderResult(0, verticalOffset);
        }

        double maxWidth = finalSize.Width > 0 ? finalSize.Width : 9999;
        double viewportHeight = finalSize.Height > 0 ? finalSize.Height : double.PositiveInfinity;
        double viewportBottom = double.IsPositiveInfinity(viewportHeight)
            ? double.PositiveInfinity
            : verticalOffset + viewportHeight;
        double consumedHeight = verticalOffset;
        double currentY = 0;
        var selMin = Math.Min(_selectionAnchor, _selectionFocus);
        var selMax = Math.Max(_selectionAnchor, _selectionFocus);
        var hasSelection = IsTextSelectionEnabled && _selectionAnchor >= 0 && _selectionFocus >= 0
                           && _selectionAnchor != _selectionFocus;
        var showCaret = IsTextSelectionEnabled && _hasKeyboardFocus && _selectionAnchor >= 0 && _selectionAnchor == _selectionFocus;
        var caretOffset = _selectionFocus >= 0 ? _selectionFocus : 0;
        var caretDrawn = false;
        var consumedPerItem = new Dictionary<int, int>();

        foreach (var seg in _preparedSegments)
        {
            PretextLayout.WalkRichInlineLineRanges(seg.Prepared, maxWidth, range =>
            {
                var line = PretextLayout.MaterializeRichInlineLineRange(seg.Prepared, range);
                double x = 0;
                double lineHeight = ResolvedLineHeight;
                var lineFragments = new List<PendingPlacedFragment>();

                foreach (var fragment in line.Fragments)
                {
                    x += fragment.GapBefore;
                    var flatIndex = seg.FlatItemOffset + fragment.ItemIndex;
                    var flatItem = _flatItems[flatIndex];
                    int fragCharStart, fragCharEnd;
                    string fragText;

                    if (flatItem is UiContainerItem)
                    {
                        var itemBase = flatIndex < _flatItemCharOffsets.Length ? _flatItemCharOffsets[flatIndex] : 0;
                        fragCharStart = itemBase;
                        fragCharEnd = itemBase;
                        fragText = "￼";
                    }
                    else if (flatItem is TextRunItem)
                    {
                        var itemBase = flatIndex < _flatItemCharOffsets.Length ? _flatItemCharOffsets[flatIndex] : 0;
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

                    double fragmentWidth;
                    UIElement? el;
                    if (flatItem is UiContainerItem uiItem)
                    {
                        fragmentWidth = uiItem.MeasuredWidth > 0 ? uiItem.MeasuredWidth : fragment.OccupiedWidth;
                        lineHeight = Math.Max(lineHeight, uiItem.MeasuredHeight);
                        el = recordHitTest ? uiItem.Child : null;
                    }
                    else
                    {
                        var textItem = (TextRunItem)flatItem;
                        var stretchScale = InheritedProperties.FontStretchScale(textItem.Props.FontStretch);
                        var tb = new Microsoft.UI.Xaml.Controls.TextBlock
                        {
                            Text = fragment.Text,
                            FontFamily = textItem.Props.FontFamily,
                            FontSize = textItem.Props.FontSize,
                            FontWeight = textItem.Props.FontWeight,
                            FontStyle = textItem.Props.FontStyle,
                            FontStretch = FontStretch.Normal,
                            CharacterSpacing = textItem.Props.CharacterSpacing,
                            TextWrapping = TextWrapping.NoWrap,
                            Foreground = CloneBrush(textItem.Props.Foreground),
                        };
                        if (stretchScale != 1.0)
                        {
                            tb.RenderTransformOrigin = new Point(0, 0);
                            tb.RenderTransform = new ScaleTransform { ScaleX = stretchScale };
                        }
                        tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        fragmentWidth = (tb.DesiredSize.Width > 0 ? tb.DesiredSize.Width : fragment.OccupiedWidth) * stretchScale;
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

                    lineFragments.Add(new PendingPlacedFragment(el!, flatItem, x, currentY - verticalOffset, fragmentWidth, fragCharStart, fragCharEnd, fragText));
                    x += fragmentWidth;
                }

                var visualY = currentY - verticalOffset;
                var lineTop = currentY;
                var lineBottom = currentY + lineHeight;
                var lineVisible = IsLineVisible(lineTop, lineBottom, lineHeight, verticalOffset, viewportBottom, viewportHeight, consumedHeight, requireWholeLineVisibility);
                if (recordHitTest)
                {
                    _placedLines.Add(BuildPlacedLine(lineFragments, visualY, lineHeight));
                }

                if (lineVisible)
                {
                    consumedHeight = Math.Max(consumedHeight, lineBottom);
                    foreach (var placed in lineFragments)
                    {
                        if (recordHitTest)
                            _placedFragments.Add(new PlacedFragment(placed.X, placed.Y, placed.Width, lineHeight, placed.CharStart, placed.CharEnd, placed.Text));

                        foreach (var textHighlighter in _textHighlighters)
                        {
                            if (textHighlighter.Background is null)
                                continue;
                            foreach (var textRange in textHighlighter.Ranges)
                            {
                                int rangeStart = textRange.StartIndex;
                                int rangeEnd = rangeStart + textRange.Length;
                                if (rangeEnd > placed.CharStart && rangeStart < placed.CharEnd)
                                {
                                    var len = placed.CharEnd - placed.CharStart;
                                    var t0 = (double)(Math.Max(rangeStart, placed.CharStart) - placed.CharStart) / len;
                                    var t1 = (double)(Math.Min(rangeEnd, placed.CharEnd) - placed.CharStart) / len;
                                    var highlightRect = new Rectangle { Width = (t1 - t0) * placed.Width, Height = lineHeight, Fill = CloneBrush(textHighlighter.Background) };
                                    Canvas.SetLeft(highlightRect, placed.X + t0 * placed.Width);
                                    Canvas.SetTop(highlightRect, placed.Y);
                                    target.Children.Add(highlightRect);
                                }
                            }
                        }

                        if (hasSelection && placed.CharEnd > placed.CharStart && placed.CharEnd > selMin && placed.CharStart < selMax)
                        {
                            var len = placed.CharEnd - placed.CharStart;
                            var t0 = (double)(Math.Max(selMin, placed.CharStart) - placed.CharStart) / len;
                            var t1 = (double)(Math.Min(selMax, placed.CharEnd) - placed.CharStart) / len;
                            var highlight = new Rectangle { Width = (t1 - t0) * placed.Width, Height = lineHeight, Fill = SelectionHighlightColor };
                            Canvas.SetLeft(highlight, placed.X + t0 * placed.Width);
                            Canvas.SetTop(highlight, placed.Y);
                            target.Children.Add(highlight);
                        }

                        if (placed.FlatItem is TextRunItem textRunItem)
                            DrawDecorations(target, textRunItem.Props.TextDecorations, textRunItem.Props.Foreground, placed.X, placed.Y, placed.Width, lineHeight);

                        if (placed.Element is not null)
                        {
                            Canvas.SetLeft(placed.Element, placed.X);
                            Canvas.SetTop(placed.Element, placed.Y);
                            target.Children.Add(placed.Element);
                        }
                    }

                    if (showCaret && !caretDrawn && TryGetCaretX(lineFragments, caretOffset, out var caretX))
                    {
                        var caretRect = new Rectangle
                        {
                            Width = 1,
                            Height = Math.Max(1, lineHeight),
                            Fill = CloneBrush(Foreground) ?? SelectionBrush,
                        };
                        Canvas.SetLeft(caretRect, caretX);
                        Canvas.SetTop(caretRect, visualY);
                        target.Children.Add(caretRect);
                        caretDrawn = true;
                    }
                }

                currentY += lineHeight;
            });
        }

        if (!requireWholeLineVisibility)
        {
            consumedHeight = Math.Min(currentY, viewportBottom);
        }

        return new RenderResult(currentY, consumedHeight);
    }

    private static bool IsLineVisible(
        double lineTop,
        double lineBottom,
        double lineHeight,
        double viewportTop,
        double viewportBottom,
        double viewportHeight,
        double consumedHeight,
        bool requireWholeLineVisibility)
    {
        if (!requireWholeLineVisibility)
        {
            var visualTop = lineTop - viewportTop;
            return visualTop + lineHeight >= 0 && visualTop <= viewportHeight;
        }

        var startsInViewport = lineTop >= viewportTop - 0.5;
        if (!startsInViewport)
            return false;

        if (lineBottom <= viewportBottom + 0.5)
            return true;

        return consumedHeight <= viewportTop + 0.5
            && lineTop <= viewportBottom + 0.5
            && lineHeight > viewportHeight + 0.5;
    }

    private void DrawDecorations(Canvas target, TextDecorations decorations, Brush foreground, double x, double y, double width, double lineHeight)
    {
        if (decorations == TextDecorations.None) return;

        if ((decorations & TextDecorations.Strikethrough) != 0)
        {
            var r = new Rectangle { Width = width, Height = 1, Fill = CloneBrush(foreground) };
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y + lineHeight * 0.55);
            target.Children.Add(r);
        }
        if ((decorations & TextDecorations.Underline) != 0)
        {
            var r = new Rectangle { Width = width, Height = 1, Fill = CloneBrush(foreground) };
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y + lineHeight * 0.88);
            target.Children.Add(r);
        }
    }

    // A Brush DependencyObject in WinUI/Uno has a single inheritance context. A brush bound to
    // RichTextBlock.Foreground is already "owned" by this control; assigning that same instance
    // to a child element's Foreground/Fill is silently ignored on subsequent layout passes, so
    // the child keeps its default color. Cloning produces a fresh, unparented brush per child.
    private static Brush? CloneBrush(Brush? brush) => brush switch
    {
        null => null,
        SolidColorBrush scb => new SolidColorBrush(scb.Color) { Opacity = scb.Opacity },
        _ => brush
    };

    private static bool TryGetCaretX(List<PendingPlacedFragment> lineFragments, int caretOffset, out double caretX)
    {
        caretX = 0;
        if (lineFragments.Count == 0)
        {
            return false;
        }

        foreach (var fragment in lineFragments)
        {
            if (caretOffset < fragment.CharStart || caretOffset > fragment.CharEnd)
            {
                continue;
            }

            var fragmentLength = fragment.CharEnd - fragment.CharStart;
            if (fragmentLength <= 0)
            {
                caretX = fragment.X;
                return true;
            }

            var t = (double)(caretOffset - fragment.CharStart) / fragmentLength;
            caretX = fragment.X + (t * fragment.Width);
            return true;
        }

        var last = lineFragments[^1];
        if (caretOffset >= last.CharEnd)
        {
            caretX = last.X + last.Width;
            return true;
        }

        var first = lineFragments[0];
        if (caretOffset <= first.CharStart)
        {
            caretX = first.X;
            return true;
        }

        return false;
    }

    private static PlacedLine BuildPlacedLine(List<PendingPlacedFragment> lineFragments, double y, double height)
    {
        if (lineFragments.Count == 0)
        {
            return new PlacedLine(0, 0, y, height, 0, 0);
        }

        var first = lineFragments[0];
        var last = lineFragments[^1];
        return new PlacedLine(
            first.X,
            last.X + last.Width,
            y,
            height,
            first.CharStart,
            last.CharEnd);
    }

    // ── Text selection ────────────────────────────────────────────────

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!IsTextSelectionEnabled) return;
        if (IsNestedRichTextPointerEvent(e)) return;

        _focusSink.Focus(FocusState.Pointer);
        var pt = e.GetCurrentPoint(this).Position;
        var idx = HitTest(pt, clampToContent: true);
        if (idx < 0) return;
        NotifyGroupSelectionStarting();
        _preferredCaretX = null;
        SetSelection(idx, idx);
        _isPointerDown = true;
        CapturePointer(e.Pointer);
        e.Handled = true;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!IsTextSelectionEnabled || !_isPointerDown) return;
        var pt = e.GetCurrentPoint(this).Position;
        var idx = HitTest(pt, clampToContent: true);
        if (idx >= 0)
        {
            _preferredCaretX = null;
            SetSelection(_selectionAnchor >= 0 ? _selectionAnchor : idx, idx);
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

        var shift = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift)
                     & CoreVirtualKeyStates.Down) != 0;
        var ctrl = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
                    & CoreVirtualKeyStates.Down) != 0;

        if (ctrl && e.Key == VirtualKey.A)
        {
            SelectAll();
            e.Handled = true;
        }
        else if (ctrl && e.Key == VirtualKey.C)
        {
            CopySelectionToClipboard();
            e.Handled = true;
        }
        else if (!ctrl)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    MoveCaret(-1, shift);
                    e.Handled = true;
                    break;
                case VirtualKey.Right:
                    MoveCaret(1, shift);
                    e.Handled = true;
                    break;
                case VirtualKey.Home:
                    MoveCaretTo(0, shift);
                    e.Handled = true;
                    break;
                case VirtualKey.End:
                    MoveCaretTo(TextLength, shift);
                    e.Handled = true;
                    break;
                case VirtualKey.Up:
                    MoveCaretVertical(-1, shift);
                    e.Handled = true;
                    break;
                case VirtualKey.Down:
                    MoveCaretVertical(1, shift);
                    e.Handled = true;
                    break;
            }
        }
    }

    private int HitTest(Point pt)
        => HitTest(pt, clampToContent: false);

    private int HitTest(Point pt, bool clampToContent)
    {
        if (_flatItemCharOffsets.Length == 0 || _placedLines.Count == 0)
            return -1;

        var line = FindNearestLine(pt.Y);
        if (line is null)
            return clampToContent
                ? pt.Y < 0 ? 0 : TextLength
                : -1;

        var fragments = _placedFragments
            .Where(f => Math.Abs(f.Y - line.Y) < 0.5)
            .OrderBy(f => f.X)
            .ToList();

        if (fragments.Count == 0)
        {
            return clampToContent
                ? pt.X <= line.StartX ? line.StartOffset : line.EndOffset
                : -1;
        }

        foreach (var fragment in fragments)
        {
            if (pt.X < fragment.X)
                return fragment.CharStart;

            if (pt.X <= fragment.X + fragment.Width)
                return HitTestWithinFragment(fragment, pt.X, clampToContent);
        }

        var last = fragments[^1];
        return pt.X > last.X + last.Width
            ? last.CharEnd
            : HitTestWithinFragment(last, pt.X, clampToContent);
    }

    private int HitTestWithinFragment(PlacedFragment fragment, double x, bool clampToContent)
    {
        if (x >= fragment.X && x <= fragment.X + fragment.Width)
        {
            var len = fragment.CharEnd - fragment.CharStart;
            if (len == 0) return clampToContent ? fragment.CharStart : -1;
            var t = (x - fragment.X) / fragment.Width;
            return fragment.CharStart + (int)Math.Round(t * len);
        }

        return x <= fragment.X ? fragment.CharStart : fragment.CharEnd;
    }

    private int TextLength => _flatItemCharOffsets.Length == 0 ? 0 : _flatItemCharOffsets[^1];

    private string BuildSelectedText()
    {
        EnsureFlatItemsSnapshot();
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

    // Selection APIs can be invoked before the first layout pass (or between
    // document mutations and the next measure), so keep a lightweight text
    // snapshot available for offset/selection text calculations.
    private void EnsureFlatItemsSnapshot()
    {
        if (_flatItems is { Length: > 0 } && _flatItemCharOffsets.Length > 0)
            return;

        var flatItems = new List<FlatItem>();
        CollectFlatItems(flatItems, RootProperties());
        _flatItems = flatItems.ToArray();
        _flatItemCharOffsets = BuildCharOffsets(flatItems);
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
        SetSelection(-1, -1, raiseSelectionChanged: false);
    }

    private void MoveCaret(int delta, bool extendSelection)
    {
        var current = _selectionFocus >= 0
            ? _selectionFocus
            : Math.Clamp(_selectionAnchor, 0, TextLength);

        if (!extendSelection && HasExpandedSelection)
        {
            current = delta < 0
                ? Math.Min(_selectionAnchor, _selectionFocus)
                : Math.Max(_selectionAnchor, _selectionFocus);
        }

        _preferredCaretX = null;
        MoveCaretTo(current + delta, extendSelection);
    }

    private void MoveCaretTo(int offset, bool extendSelection)
    {
        var clamped = Math.Clamp(offset, 0, TextLength);
        if (extendSelection)
        {
            var anchor = _selectionAnchor >= 0
                ? _selectionAnchor
                : (_selectionFocus >= 0 ? _selectionFocus : clamped);
            SetSelection(anchor, clamped);
            return;
        }

        SetSelection(clamped, clamped);
    }

    private void MoveCaretVertical(int lineDelta, bool extendSelection)
    {
        if (_placedLines.Count == 0)
        {
            return;
        }

        var currentOffset = _selectionFocus >= 0
            ? _selectionFocus
            : Math.Clamp(_selectionAnchor, 0, TextLength);
        var currentLine = FindLineForOffset(currentOffset) ?? _placedLines[0];
        var currentIndex = _placedLines.IndexOf(currentLine);
        var targetIndex = Math.Clamp(currentIndex + lineDelta, 0, _placedLines.Count - 1);
        if (targetIndex == currentIndex)
        {
            return;
        }

        _preferredCaretX ??= GetCaretXForOffset(currentOffset);
        var targetLine = _placedLines[targetIndex];
        var targetOffset = GetOffsetForLineX(targetLine, _preferredCaretX.Value);
        MoveCaretTo(targetOffset, extendSelection);
    }

    private PlacedLine? FindNearestLine(double y)
    {
        if (_placedLines.Count == 0)
        {
            return null;
        }

        PlacedLine? best = null;
        var bestDistance = double.MaxValue;
        foreach (var line in _placedLines)
        {
            var distance = y < line.Y ? line.Y - y :
                y > line.Y + line.Height ? y - (line.Y + line.Height) : 0;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = line;
            }
        }

        return best;
    }

    private PlacedLine? FindLineForOffset(int offset)
    {
        foreach (var line in _placedLines)
        {
            if (offset >= line.StartOffset && offset <= line.EndOffset)
            {
                return line;
            }
        }

        if (_placedLines.Count == 0)
        {
            return null;
        }

        return offset <= _placedLines[0].StartOffset ? _placedLines[0] : _placedLines[^1];
    }

    private double GetCaretXForOffset(int offset)
    {
        var line = FindLineForOffset(offset);
        if (line is null)
        {
            return 0;
        }

        var fragments = _placedFragments
            .Where(f => Math.Abs(f.Y - line.Y) < 0.5)
            .OrderBy(f => f.X)
            .ToList();

        return TryGetCaretX(
            fragments.Select(f => new PendingPlacedFragment(null!, null!, f.X, f.Y, f.Width, f.CharStart, f.CharEnd, f.Text)).ToList(),
            offset,
            out var caretX)
            ? caretX
            : line.StartX;
    }

    private int GetOffsetForLineX(PlacedLine line, double x)
    {
        var fragments = _placedFragments
            .Where(f => Math.Abs(f.Y - line.Y) < 0.5)
            .OrderBy(f => f.X)
            .ToList();

        if (fragments.Count == 0)
        {
            return line.StartOffset;
        }

        foreach (var fragment in fragments)
        {
            if (x < fragment.X)
                return fragment.CharStart;

            if (x <= fragment.X + fragment.Width)
                return HitTestWithinFragment(fragment, x, clampToContent: true);
        }

        return fragments[^1].CharEnd;
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

    // Associates character offsets with TextPointer instances for RichTextBlock hit-testing.
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TextPointer, PointerOffsetRecord> _pointerOffsets = new();
    private TextContainer? _pointerContainer;

    private sealed class PointerOffsetRecord { public int Offset; }

    private TextPointer CreateTextPointer(int offset)
    {
        return TextPointerShim.Create(this, Math.Clamp(offset, 0, TextLength));
    }

    private int GetOffset(TextPointer pointer)
    {
        if (TextPointerShim.GetParent(pointer) is RichTextBlock block && ReferenceEquals(block, this))
        {
            return Math.Clamp(TextPointerShim.GetOffset(pointer), 0, TextLength);
        }

        return 0;
    }

    private void SetSelection(int anchor, int focus, bool raiseSelectionChanged = true)
    {
        if (_selectionAnchor == anchor && _selectionFocus == focus)
            return;

        _selectionAnchor = anchor;
        _selectionFocus = focus;
        SetValue(SelectedTextProperty, BuildSelectedText());
        InvalidateArrange();
        if (raiseSelectionChanged)
            SelectionChanged?.Invoke(this, new RoutedEventArgs());
    }

    private bool HasExpandedSelection =>
        _selectionAnchor >= 0 && _selectionFocus >= 0 && _selectionAnchor != _selectionFocus;

    // ── Content collection ────────────────────────────────────────────

    private void CollectFlatItems(List<FlatItem> result, InheritedProperties root)
    {
        var blocks = ActiveBlocks;
        DiagLog($"CollectFlatItems: root.Foreground={root.Foreground} blocks={blocks.Count} inlines={_inlines.Count}");
        if (_inlines.Count > 0)
        {
            FlattenInlines(_inlines, result, root);
            return;
        }

        for (var i = 0; i < blocks.Count; i++)
        {
            if (i > 0)
                result.Add(new TextRunItem("\n", root));
            if (blocks[i] is Paragraph bp)
            {
                var blockProps = root;
                if (InheritedProperties.IsExplicitFontSize(bp.FontSize))
                    blockProps = blockProps with { FontSize = bp.FontSize };
                if (InheritedProperties.IsExplicitFontWeight(bp.FontWeight))
                {
                    blockProps = blockProps with { FontWeight = InheritedProperties.ConvertFontWeight(bp.FontWeight) };
                }
                if (InheritedProperties.IsExplicitFontFamily(bp.FontFamily))
                    blockProps = blockProps with { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily(bp.FontFamily!.ToString()) };
                if (InheritedProperties.IsExplicitFontStretch(bp.FontStretch))
                    blockProps = blockProps with { FontStretch = bp.FontStretch };
                if (InheritedProperties.IsExplicitCharacterSpacing(bp.CharacterSpacing))
                    blockProps = blockProps with { CharacterSpacing = bp.CharacterSpacing };
                if (InheritedProperties.IsExplicitForeground(bp.Foreground))
                    blockProps = blockProps with { Foreground = bp.Foreground! };
                FlattenInlines(bp.Inlines, result, blockProps);
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────

    private double ResolvedLineHeight => LineHeight > 0 ? LineHeight : FontSize * 1.4;

    private InheritedProperties RootProperties() => new(
        FontFamily, FontSize, FontWeight, FontStyle, FontStretch, CharacterSpacing, Foreground, TextDecorations.None);

    private System.Windows.Documents.BlockCollection ActiveBlocks => _documentSource?.Blocks ?? _blocks;

    private void OnContentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => InvalidateMeasure();

    private void OnFocusSinkGotFocus(object sender, RoutedEventArgs e)
    {
        _hasKeyboardFocus = true;
        if (_selectionAnchor < 0 || _selectionFocus < 0)
        {
            SetSelection(0, 0, raiseSelectionChanged: false);
        }
        else
        {
            InvalidateArrange();
        }
    }

    private void OnFocusSinkLostFocus(object sender, RoutedEventArgs e)
    {
        _hasKeyboardFocus = false;
        InvalidateArrange();
    }

    private void OnBlocksChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AttachBlockHandlers(e.NewItems);
        DetachBlockHandlers(e.OldItems);
        InvalidateMeasure();
    }

    private void OnDocumentBlocksChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AttachBlockHandlers(e.NewItems);
        DetachBlockHandlers(e.OldItems);
        InvalidateMeasure();
    }

    private void AttachBlockHandlers(System.Collections.IEnumerable? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is Paragraph paragraph)
            {
                paragraph.Inlines.CollectionChanged += OnContentChanged;
            }
        }
    }

    private void DetachBlockHandlers(System.Collections.IEnumerable? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (item is Paragraph paragraph)
            {
                paragraph.Inlines.CollectionChanged -= OnContentChanged;
            }
        }
    }

    private void OnTextHighlightersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // When TextHighlighters collection changes, invalidate layout to re-render highlights
        InvalidateArrange();
    }

    private void OnHyperlinkTapped(Hyperlink link)
    {
        link.RaiseClick();

        if (link.NavigateUri is { } uri)
        {
            try
            {
                _ = Launcher.LaunchUriAsync(uri);
            }
            catch
            {
                // Keep behavior non-fatal; host app may handle LinkClicked itself.
            }
        }

        LinkClicked?.Invoke(this, new HyperlinkClickEventArgs(link));
    }

    // ── Flatten ───────────────────────────────────────────────────────

    private static void FlattenInlines(
        System.Windows.Documents.InlineCollection inlines,
        List<FlatItem> result,
        InheritedProperties inherited,
        Hyperlink? currentLink = null)
    {
        foreach (var inline in inlines)
        {
            if (DiagnosticsEnabled)
                File.AppendAllText(IoPath.Combine(IoPath.GetTempPath(), "LeXtudio.RichText.Diag.txt"),
                    $"[{DateTime.Now:HH:mm:ss.fff}] [flatten owner={(currentLink?.GetHashCode().ToString("X8") ?? "n/a")}] inline={inline.GetType().Name} inline.Foreground={inline.Foreground?.ToString() ?? "<null>"} inherited.Foreground={inherited.Foreground}\n");
            var props = inherited.Merge(inline);
            if (DiagnosticsEnabled)
                File.AppendAllText(IoPath.Combine(IoPath.GetTempPath(), "LeXtudio.RichText.Diag.txt"),
                    $"[flatten]   -> merged.Foreground={props.Foreground}\n");
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

    private record PlacedLine(double StartX, double EndX, double Y, double Height, int StartOffset, int EndOffset);

    private readonly record struct RenderResult(double TotalHeight, double ConsumedHeight);

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

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBlock block)
        {
            block.DiagLog($"OnLayoutPropertyChanged: prop={e.Property} old={e.OldValue} new={e.NewValue}");
            block.InvalidateMeasure();
        }
    }

    private static void OnOverflowContentTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RichTextBlock block)
            return;

        if (e.NewValue is RichTextBlockOverflow overflow)
            overflow.ContentSource = block;

        block.InvalidateMeasure();
        block.InvalidateArrange();
    }

    private static void OnSelectionVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBlock block)
            block.InvalidateArrange();
    }

    private static void OnIsTextSelectionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RichTextBlock block)
            return;

        if (!(bool)e.NewValue)
            block.ClearSelectionSilent();

        block.UpdateSelectionCursor();
    }
}

public sealed class HyperlinkClickEventArgs(Hyperlink hyperlink) : EventArgs
{
    public Hyperlink Hyperlink { get; } = hyperlink;
}
#endif

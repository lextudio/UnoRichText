// Skeleton implementation of RichTextBlockOverflow targeting WinUI 3 API parity.
// Real layout chains into the source RichTextBlock; this skeleton wires up the
// dependency properties, events, and template parts so the compat tool sees the
// surface area. Layout-side implementation (overflow chain painting + selection
// forwarding) will be filled in alongside RichTextBlock's HasOverflowContent /
// OverflowContentTarget plumbing.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

namespace LeXtudio.UI.Xaml.Controls;

public partial class RichTextBlockOverflow : Panel
{
    public static DependencyProperty ContentSourceProperty { get; } =
        DependencyProperty.Register(nameof(ContentSource), typeof(DependencyObject), typeof(RichTextBlockOverflow), new PropertyMetadata(null, OnContentSourceChanged));

    public static DependencyProperty OverflowContentTargetProperty { get; } =
        DependencyProperty.Register(nameof(OverflowContentTarget), typeof(RichTextBlockOverflow), typeof(RichTextBlockOverflow), new PropertyMetadata(null, OnOverflowContentTargetChanged));

    public static DependencyProperty HasOverflowContentProperty { get; } =
        DependencyProperty.Register(nameof(HasOverflowContent), typeof(bool), typeof(RichTextBlockOverflow), new PropertyMetadata(false));

    public static DependencyProperty IsTextTrimmedProperty { get; } =
        DependencyProperty.Register(nameof(IsTextTrimmed), typeof(bool), typeof(RichTextBlockOverflow), new PropertyMetadata(false));

    public static DependencyProperty MaxLinesProperty { get; } =
        DependencyProperty.Register(nameof(MaxLines), typeof(int), typeof(RichTextBlockOverflow), new PropertyMetadata(0));

    public static DependencyProperty PaddingProperty { get; } =
        DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(RichTextBlockOverflow), new PropertyMetadata(default(Thickness)));

    public DependencyObject? ContentSource
    {
        get => (DependencyObject?)GetValue(ContentSourceProperty);
        set => SetValue(ContentSourceProperty, value);
    }

    public RichTextBlockOverflow? OverflowContentTarget
    {
        get => (RichTextBlockOverflow?)GetValue(OverflowContentTargetProperty);
        set => SetValue(OverflowContentTargetProperty, value);
    }

    public bool HasOverflowContent
    {
        get => (bool)GetValue(HasOverflowContentProperty);
        private set => SetValue(HasOverflowContentProperty, value);
    }

    public bool IsTextTrimmed
    {
        get => (bool)GetValue(IsTextTrimmedProperty);
        private set => SetValue(IsTextTrimmedProperty, value);
    }

    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    /// <summary>
    /// Raised when <see cref="IsTextTrimmed"/> changes. Matches WinUI 3 shape.
    /// </summary>
    public event TypedEventHandler<RichTextBlockOverflow, IsTextTrimmedChangedEventArgs>? IsTextTrimmedChanged;

    /// <summary>
    /// Forwarded selection-changed from the source RichTextBlock when selection enters
    /// or leaves this overflow region.
    /// </summary>
    public event RoutedEventHandler? SelectionChanged;

    private readonly Canvas _canvas = new();
    private RichTextBlock? _source;

    internal double LastViewportHeight { get; private set; }
    internal double LastConsumedContentHeight { get; private set; }

    public RichTextBlockOverflow()
    {
        Children.Add(_canvas);
    }

    /// <summary>
    /// Returns the positions describing the bounds of the content that fits in this overflow region.
    /// Skeleton implementation: returns an empty rect until the chained layout pass is wired.
    /// </summary>
    public Rect ContentStart() => default;
    public Rect ContentEnd() => default;

    private static void OnContentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var overflow = (RichTextBlockOverflow)d;
        overflow._source = e.NewValue as RichTextBlock;
        if (overflow.OverflowContentTarget is not null)
            overflow.OverflowContentTarget.ContentSource = overflow._source;
        overflow.InvalidateMeasure();
        overflow.InvalidateArrange();
    }

    private static void OnOverflowContentTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var overflow = (RichTextBlockOverflow)d;
        if (e.NewValue is RichTextBlockOverflow target)
            target.ContentSource = overflow._source;

        overflow.InvalidateArrange();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsPositiveInfinity(availableSize.Width) ? 0 : availableSize.Width;
        var height = double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height;
        _canvas.Measure(new Size(width, height));
        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        LastViewportHeight = finalSize.Height;
        LastConsumedContentHeight = 0;
        _canvas.Children.Clear();
        Clip = new RectangleGeometry { Rect = new Rect(0, 0, finalSize.Width, finalSize.Height) };

        if (_source is not null && finalSize.Width > 0 && finalSize.Height > 0)
        {
            _source.ArrangeOverflowContent(this, _canvas, finalSize);
        }

        _canvas.Width = finalSize.Width;
        _canvas.Height = finalSize.Height;
        _canvas.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
        OverflowContentTarget?.InvalidateMeasure();
        OverflowContentTarget?.InvalidateArrange();
        return finalSize;
    }

    internal void SetOverflowState(double consumedContentHeight, bool hasOverflowContent, bool isTextTrimmed)
    {
        LastConsumedContentHeight = consumedContentHeight;
        SetValue(HasOverflowContentProperty, hasOverflowContent);
        if (IsTextTrimmed != isTextTrimmed)
        {
            SetValue(IsTextTrimmedProperty, isTextTrimmed);
        }
    }

    internal void RaiseIsTextTrimmedChanged(IsTextTrimmedChangedEventArgs e)
        => IsTextTrimmedChanged?.Invoke(this, e);

    internal void RaiseSelectionChanged(RoutedEventArgs e)
        => SelectionChanged?.Invoke(this, e);
}

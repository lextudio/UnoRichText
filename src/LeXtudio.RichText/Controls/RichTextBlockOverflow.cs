// Skeleton implementation of RichTextBlockOverflow targeting WinUI 3 API parity.
// Real layout chains into the source RichTextBlock; this skeleton wires up the
// dependency properties, events, and template parts so the compat tool sees the
// surface area. Layout-side implementation (overflow chain painting + selection
// forwarding) will be filled in alongside RichTextBlock's HasOverflowContent /
// OverflowContentTarget plumbing.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Windows.Foundation;

namespace LeXtudio.UI.Xaml.Controls;

public partial class RichTextBlockOverflow : FrameworkElement
{
    public static DependencyProperty ContentSourceProperty { get; } =
        DependencyProperty.Register(nameof(ContentSource), typeof(DependencyObject), typeof(RichTextBlockOverflow), new PropertyMetadata(null, OnContentSourceChanged));

    public static DependencyProperty OverflowContentTargetProperty { get; } =
        DependencyProperty.Register(nameof(OverflowContentTarget), typeof(RichTextBlockOverflow), typeof(RichTextBlockOverflow), new PropertyMetadata(null));

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

    public RichTextBlockOverflow()
    {
    }

    /// <summary>
    /// Returns the positions describing the bounds of the content that fits in this overflow region.
    /// Skeleton implementation: returns an empty rect until the chained layout pass is wired.
    /// </summary>
    public Rect ContentStart() => default;
    public Rect ContentEnd() => default;

    private static void OnContentSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // TODO: detach from previous source, attach to new source, request re-layout.
    }

    internal void RaiseIsTextTrimmedChanged(IsTextTrimmedChangedEventArgs e)
        => IsTextTrimmedChanged?.Invoke(this, e);

    internal void RaiseSelectionChanged(RoutedEventArgs e)
        => SelectionChanged?.Invoke(this, e);
}

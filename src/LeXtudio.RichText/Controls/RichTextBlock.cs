using System.Collections.Specialized;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Text;
using Pretext;
using LeXtudio.UI.Xaml.Documents;

namespace LeXtudio.UI.Xaml.Controls;

public class RichTextBlock : Panel
{
    private readonly Canvas _canvas = new();
    private readonly InlineCollection _inlines = new();
    private readonly BlockCollection _blocks = new();

    private FlatItem[]? _flatItems;
    private PreparedRichInline? _prepared;
    private double _totalHeight;

    public RichTextBlock()
    {
        Children.Add(_canvas);
        _inlines.CollectionChanged += OnContentChanged;
        _blocks.CollectionChanged += OnBlocksChanged;
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>Inline content for single-paragraph use.</summary>
    public InlineCollection Inlines => _inlines;

    /// <summary>Block content for multi-paragraph use (mirrors WinUI RichTextBlock.Blocks).</summary>
    public BlockCollection Blocks => _blocks;

    public FontFamily FontFamily { get; set; } = new FontFamily("Segoe UI");
    public double FontSize { get; set; } = 14;
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public Brush Foreground { get; set; } = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
    public TextWrapping TextWrapping { get; set; } = TextWrapping.WrapWholeWords;
    public double LineHeight { get; set; }

    public event EventHandler<HyperlinkClickEventArgs>? LinkClicked;

    // ── Layout ────────────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        var root = RootProperties();
        var flatItems = new List<FlatItem>();
        CollectFlatItems(flatItems, root);

        if (flatItems.Count == 0)
        {
            _flatItems = Array.Empty<FlatItem>();
            _prepared = null;
            _totalHeight = 0;
            _canvas.Measure(new Size(0, 0));
            return Size.Empty;
        }

        foreach (var item in flatItems.OfType<UiContainerItem>())
            item.Child.Measure(new Size(double.PositiveInfinity, ResolvedLineHeight));

        var richItems = BuildRichInlineItems(flatItems);
        _prepared = PretextLayout.PrepareRichInline(richItems);
        _flatItems = flatItems.ToArray();

        double maxWidth = double.IsPositiveInfinity(availableSize.Width) ? 9999 : availableSize.Width;
        var stats = PretextLayout.MeasureRichInlineStats(_prepared, maxWidth);
        _totalHeight = stats.LineCount * ResolvedLineHeight;

        var desired = new Size(Math.Min(stats.MaxLineWidth, availableSize.Width), _totalHeight);
        _canvas.Measure(desired);
        return desired;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _canvas.Children.Clear();

        if (_prepared == null || _flatItems == null)
        {
            _canvas.Arrange(new Rect(0, 0, 0, 0));
            return finalSize;
        }

        double maxWidth = finalSize.Width > 0 ? finalSize.Width : 9999;
        int lineIndex = 0;

        PretextLayout.WalkRichInlineLineRanges(_prepared, maxWidth, range =>
        {
            var line = PretextLayout.MaterializeRichInlineLineRange(_prepared, range);
            double x = 0;
            double y = lineIndex * ResolvedLineHeight;

            foreach (var fragment in line.Fragments)
            {
                x += fragment.GapBefore;

                UIElement el;
                var flatItem = _flatItems[fragment.ItemIndex];

                if (flatItem is UiContainerItem uiItem)
                {
                    el = uiItem.Child;
                }
                else if (flatItem is TextRunItem textItem)
                {
                    var tb = new TextBlock
                    {
                        Text = fragment.Text,
                        FontFamily = textItem.Props.FontFamily,
                        FontSize = textItem.Props.FontSize,
                        FontWeight = textItem.Props.FontWeight,
                        FontStyle = textItem.Props.FontStyle,
                        Foreground = textItem.Props.Foreground,
                        TextDecorations = textItem.Props.TextDecorations,
                        Width = fragment.OccupiedWidth,
                        TextWrapping = TextWrapping.NoWrap,
                    };
                    if (textItem.Hyperlink is { } link)
                        tb.Tapped += (_, _) => OnHyperlinkTapped(link);
                    el = tb;
                }
                else
                {
                    x += fragment.OccupiedWidth;
                    return;
                }

                Canvas.SetLeft(el, x);
                Canvas.SetTop(el, y);
                _canvas.Children.Add(el);
                x += fragment.OccupiedWidth;
            }

            lineIndex++;
        });

        _totalHeight = lineIndex * ResolvedLineHeight;
        _canvas.Width = finalSize.Width;
        _canvas.Height = _totalHeight;
        _canvas.Arrange(new Rect(0, 0, finalSize.Width, _totalHeight));
        return new Size(finalSize.Width, _totalHeight);
    }

    // ── Content collection ────────────────────────────────────────────

    /// <summary>
    /// Collects flat items from both Inlines (direct) and Blocks (paragraphs).
    /// Blocks are separated by a hard line break in the flow.
    /// </summary>
    private void CollectFlatItems(List<FlatItem> result, InheritedProperties root)
    {
        // Direct inlines take priority; Blocks is used when Inlines is empty.
        if (_inlines.Count > 0)
        {
            FlattenInlines(_inlines, result, root);
            return;
        }

        for (var i = 0; i < _blocks.Count; i++)
        {
            if (i > 0)
                result.Add(new TextRunItem("\n", root));
            FlattenInlines(_blocks[i].Inlines, result, root);
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
        // Subscribe to each new paragraph's Inlines changes.
        if (e.NewItems is not null)
        {
            foreach (Paragraph p in e.NewItems)
                p.Inlines.CollectionChanged += OnContentChanged;
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
                    result.Add(new TextRunItem(run.Text, props, currentLink));
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
                    string.Empty,
                    ui.Props.ToPretextFontString(),
                    RichInlineBreakMode.Never,
                    ui.Child.DesiredSize.Width),
                TextRunItem text => new RichInlineItem(
                    text.Text,
                    text.Props.ToPretextFontString()),
                _ => new RichInlineItem(string.Empty, "14px sans-serif")
            };
        }
        return result;
    }

    // ── Flat item types ───────────────────────────────────────────────

    private abstract record FlatItem(InheritedProperties Props);

    private record TextRunItem(string Text, InheritedProperties Props, Hyperlink? Hyperlink = null)
        : FlatItem(Props);

    private record UiContainerItem(UIElement Child, InheritedProperties Props)
        : FlatItem(Props);
}

public sealed class HyperlinkClickEventArgs(Hyperlink hyperlink) : EventArgs
{
    public Hyperlink Hyperlink { get; } = hyperlink;
}

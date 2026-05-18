using System.Windows.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;
using LeXtudioOverflow = LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;

namespace LeXtudio.RichText.Sample;

/// <summary>
/// Diagnostic page: renders an inline-code scenario and a multi-line code block,
/// dumps the RichTextBlock pipeline state to stdout after layout, then exits.
/// Exit code 0 = all checks pass.  Exit code 1 = one or more checks failed.
/// </summary>
public sealed class DiagPage : Page
{
    private readonly LeXtudioRTB _rtbInline;
    private readonly LeXtudioRTB _rtbMultiLine;
    private readonly LeXtudioRTB _rtbSelection;

    public DiagPage()
    {
        _rtbInline = BuildInlineCodeBlock();
        _rtbMultiLine = BuildMultiLineCodeBlock();
        _rtbSelection = BuildSelectionGeometryBlock();

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(_rtbInline);
        panel.Children.Add(_rtbMultiLine);
        panel.Children.Add(_rtbSelection);
        Content = panel;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // ── Inline code test ──────────────────────────────────────────────
        _rtbInline.Measure(new Size(800, double.PositiveInfinity));
        _rtbInline.Arrange(new Rect(0, 0, 800,
            _rtbInline.DesiredSize.Height > 0 ? _rtbInline.DesiredSize.Height : 200));

        var snapInline = _rtbInline.GetDiagnosticSnapshot();
        Console.WriteLine("=== Inline Code ===");
        Console.WriteLine(snapInline);
        Console.Out.Flush();

        var hasContainer = snapInline.Contains("UiContainerItem");
        var hasMeasuredSize = System.Text.RegularExpressions.Regex.IsMatch(snapInline,
            @"UiContainerItem: MeasuredSize=\d+\.\d+x");
        var hasInlineFragment = hasContainer && hasMeasuredSize && snapInline.Contains("\"￼\"");
        Console.WriteLine(hasInlineFragment
            ? "INLINE: PASS — inline code fragment placed"
            : $"INLINE: FAIL — inline code fragment missing (hasContainer={hasContainer}, hasMeasuredSize={hasMeasuredSize})");
        Console.Out.Flush();

        // ── Multi-line code block test ────────────────────────────────────
        _rtbMultiLine.Measure(new Size(800, double.PositiveInfinity));
        _rtbMultiLine.Arrange(new Rect(0, 0, 800,
            _rtbMultiLine.DesiredSize.Height > 0 ? _rtbMultiLine.DesiredSize.Height : 400));

        var snapMulti = _rtbMultiLine.GetDiagnosticSnapshot();
        Console.WriteLine("=== Multi-Line Code Block ===");
        Console.WriteLine(snapMulti);
        Console.Out.Flush();

        // Count distinct Y values in placed fragments — should be >= 4 for a 4-line block.
        var yValues = new System.Collections.Generic.HashSet<string>();
        foreach (System.Text.RegularExpressions.Match m in
            System.Text.RegularExpressions.Regex.Matches(snapMulti, @"@ \([\d.]+,([\d.]+)\)"))
            yValues.Add(m.Groups[1].Value);

        var multiLinePass = yValues.Count >= 4;
        Console.WriteLine(multiLinePass
            ? $"MULTILINE: PASS — {yValues.Count} distinct Y positions found (≥4)"
            : $"MULTILINE: FAIL — only {yValues.Count} distinct Y positions found (need ≥4)");
        Console.Out.Flush();

        // ── Selection geometry test ──────────────────────────────────────
        _rtbSelection.Measure(new Size(800, double.PositiveInfinity));
        _rtbSelection.Arrange(new Rect(0, 0, 800,
            _rtbSelection.DesiredSize.Height > 0 ? _rtbSelection.DesiredSize.Height : 200));
        _rtbSelection.SelectAll();
        _rtbSelection.Measure(new Size(800, double.PositiveInfinity));
        _rtbSelection.Arrange(new Rect(0, 0, 800,
            _rtbSelection.DesiredSize.Height > 0 ? _rtbSelection.DesiredSize.Height : 200));

        var selectionPass = CheckSelectionGeometry(_rtbSelection, out var selectionMessage);
        Console.WriteLine(selectionPass
            ? $"SELECTION: PASS — {selectionMessage}"
            : $"SELECTION: FAIL — {selectionMessage}");
        Console.Out.Flush();

        // ── Overflow line-boundary test ──────────────────────────────────
        var overflowPass = CheckOverflowLineBoundaries(out var overflowMessage);
        Console.WriteLine(overflowPass
            ? $"OVERFLOW: PASS — {overflowMessage}"
            : $"OVERFLOW: FAIL — {overflowMessage}");
        Console.Out.Flush();

        var galleryOverflowPass = CheckGalleryOverflowLayout(out var galleryOverflowMessage);
        Console.WriteLine(galleryOverflowPass
            ? $"GALLERY_OVERFLOW: PASS — {galleryOverflowMessage}"
            : $"GALLERY_OVERFLOW: FAIL — {galleryOverflowMessage}");
        Console.Out.Flush();

        // ── Document model tests ────────────────────────────────────────
        var documentsPass = CheckDocumentModel(out var documentsMessage);
        Console.WriteLine(documentsPass
            ? $"DOCUMENTS: PASS — {documentsMessage}"
            : $"DOCUMENTS: FAIL — {documentsMessage}");
        Console.Out.Flush();

        var allPass = hasInlineFragment && multiLinePass && selectionPass && overflowPass && galleryOverflowPass && documentsPass;
        Console.WriteLine(allPass ? "RESULT: PASS" : "RESULT: FAIL");
        Console.Out.Flush();

        Environment.Exit(allPass ? 0 : 1);
    }

    private static LeXtudioRTB BuildInlineCodeBlock()
    {
        var rtb = new LeXtudioRTB { FontSize = 14 };
        var p = new Paragraph();
        p.Inlines.Add(new Run { Text = "Call " });
        p.Inlines.Add(new InlineUIContainer
        {
            Child = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(255, 40, 40, 40)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(4, 0, 4, 1),
                Child = new TextBlock
                {
                    Text = "myFunction()",
                    FontFamily = new FontFamily("Courier New"),
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 170))
                }
            }
        });
        p.Inlines.Add(new Run { Text = " to proceed." });
        rtb.Blocks.Add(p);
        return rtb;
    }

    /// <summary>
    /// Mirrors what WinUI.Markdown's NativeSyntaxHighlighter produces for a C# code block:
    /// a single Paragraph with multiple Run elements, newlines as separate Run("\n") tokens.
    /// </summary>
    private static LeXtudioRTB BuildMultiLineCodeBlock()
    {
        var rtb = new LeXtudioRTB { FontSize = 13 };
        var p = new Paragraph();

        // Simulate a 4-line C# snippet:
        //   public class Foo {
        //       public void Bar() {
        //           return;
        //       }
        // Each line is a sequence of colored Run tokens; newlines are separate Run("\n").
        var keywordBrush = new SolidColorBrush(Color.FromArgb(255, 86, 156, 214));
        var defaultBrush = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220));
        var classBrush = new SolidColorBrush(Color.FromArgb(255, 78, 201, 176));

        void AddRun(string text, Brush brush) =>
            p.Inlines.Add(new Run { Text = text, Foreground = brush });
        void AddNewline() =>
            p.Inlines.Add(new Run { Text = "\n", Foreground = defaultBrush });

        AddRun("public", keywordBrush);
        AddRun(" ", defaultBrush);
        AddRun("class", keywordBrush);
        AddRun(" ", defaultBrush);
        AddRun("Foo", classBrush);
        AddRun(" {", defaultBrush);
        AddNewline();

        AddRun("    ", defaultBrush);
        AddRun("public", keywordBrush);
        AddRun(" ", defaultBrush);
        AddRun("void", keywordBrush);
        AddRun(" Bar() {", defaultBrush);
        AddNewline();

        AddRun("        ", defaultBrush);
        AddRun("return", keywordBrush);
        AddRun(";", defaultBrush);
        AddNewline();

        AddRun("    }", defaultBrush);

        rtb.Blocks.Add(p);
        return rtb;
    }

    private static LeXtudioRTB BuildSelectionGeometryBlock()
    {
        var rtb = new LeXtudioRTB
        {
            FontSize = 14,
            LineHeight = 14 * 1.4
        };

        rtb.Blocks.Add(new Paragraph
        {
            FontSize = 32,
            FontWeight = System.Windows.FontWeights.SemiBold,
            Inlines =
            {
                new Run { Text = "Large heading" }
            }
        });
        rtb.Blocks.Add(new Paragraph
        {
            FontSize = 14,
            Inlines =
            {
                new Run { Text = "Body text" }
            }
        });

        return rtb;
    }

    private static bool CheckSelectionGeometry(LeXtudioRTB rtb, out string message)
    {
        var canvas = (Canvas)typeof(LeXtudioRTB)
            .GetField("_canvas", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .GetValue(rtb)!;

        var selectionRects = canvas.Children.OfType<Rectangle>().ToArray();
        var renderedText = canvas.Children.OfType<TextBlock>().ToArray();

        if (selectionRects.Length != 2 || renderedText.Length != 2)
        {
            message = $"expected 2 selection rectangles and 2 text fragments, got {selectionRects.Length} rectangles and {renderedText.Length} text fragments";
            return false;
        }

        var headingText = renderedText.Single(text => text.Text == "Large heading");
        var bodyText = renderedText.Single(text => text.Text == "Body text");
        var headingSelection = selectionRects.OrderBy(Canvas.GetTop).First();
        var bodySelection = selectionRects.OrderBy(Canvas.GetTop).Last();

        var headingPass = headingSelection.Height >= headingText.DesiredSize.Height - 0.5
            && headingSelection.Height > rtb.LineHeight
            && headingSelection.Height > bodySelection.Height;
        var bodyPass = bodySelection.Height >= bodyText.DesiredSize.Height - 0.5;

        message =
            $"headingSelection={headingSelection.Height:F1}, headingText={headingText.DesiredSize.Height:F1}, bodySelection={bodySelection.Height:F1}, bodyText={bodyText.DesiredSize.Height:F1}, baseLineHeight={rtb.LineHeight:F1}";
        return headingPass && bodyPass;
    }

    private static bool CheckOverflowLineBoundaries(out string message)
    {
        var overflow = new LeXtudioOverflow();
        var rtb = new LeXtudioRTB
        {
            FontSize = 10,
            LineHeight = 20,
            TextWrapping = TextWrapping.NoWrap,
            OverflowContentTarget = overflow
        };

        rtb.Blocks.Add(new Paragraph(new Run("Line 1\nLine 2\nLine 3\nLine 4\nLine 5")));

        var region = new Size(200, 55);
        rtb.Measure(new Size(region.Width, double.PositiveInfinity));
        ArrangeOverrideForDiagnostics(rtb, region);
        overflow.Measure(region);
        ArrangeOverrideForDiagnostics(overflow, region);

        var sourceText = GetRenderedText(rtb);
        var overflowText = GetRenderedText(overflow);
        var overflowTops = overflowText.Select(Canvas.GetTop).OrderBy(v => v).ToArray();

        var sourceHasOnlyFullLines = sourceText.Select(t => t.Text).SequenceEqual(new[] { "Line 1", "Line 2" });
        var overflowStartsOnWholeLine = overflowText.Select(t => t.Text).SequenceEqual(new[] { "Line 3", "Line 4" });
        var noNegativeTop = overflowTops.Length > 0 && overflowTops.All(top => top >= -0.5);
        var noBottomCut = overflowTops.SequenceEqual(new[] { 0d, 20d });

        message =
            $"source=[{string.Join(", ", sourceText.Select(t => t.Text))}], overflow=[{string.Join(", ", overflowText.Select(t => $"{t.Text}@{Canvas.GetTop(t):F1}"))}]";
        return sourceHasOnlyFullLines && overflowStartsOnWholeLine && noNegativeTop && noBottomCut;
    }

    private static bool CheckGalleryOverflowLayout(out string message)
    {
        var firstOverflow = new LeXtudioOverflow { Margin = new Thickness(12, 0, 12, 0) };
        var secondOverflow = new LeXtudioOverflow { Margin = new Thickness(12, 0, 12, 0) };
        var source = new LeXtudioRTB
        {
            Margin = new Thickness(12, 0, 12, 0),
            TextAlignment = TextAlignment.Justify,
            OverflowContentTarget = firstOverflow
        };
        firstOverflow.OverflowContentTarget = secondOverflow;

        source.Blocks.Add(new Paragraph(new Run("Linked text containers allow text which does not fit in one element to overflow into a different element on the page. Creative use of linked text containers enables basic multicolumn support and other advanced page layouts.")));
        source.Blocks.Add(new Paragraph(new Run("Duis sed nulla metus, id hendrerit velit. Curabitur dolor purus, bibendum eu cursus lacinia, interdum vel augue. Aenean euismod eros et sapien vehicula dictum. Duis ullamcorper, turpis nec feugiat tincidunt, dui erat luctus risus, aliquam accumsan lacus est vel quam. Nunc lacus massa, varius eget accumsan id, congue sed orci. Duis dignissim hendrerit egestas. Proin ut turpis magna, sit amet porta erat. Nunc semper metus nec magna imperdiet nec vestibulum dui fringilla. Sed sed ante libero, nec porttitor mi. Ut luctus, neque vitae placerat egestas, urna leo auctor magna, sit amet ultricies ipsum felis quis sapien. Proin eleifend varius dui, at vestibulum nunc consectetur nec. Mauris nulla elit, ultrices a sodales non, aliquam ac est. Quisque sit amet risus nulla. Quisque vestibulum posuere velit, vitae vestibulum eros scelerisque sit amet. In in risus est, at laoreet dolor. Nullam aliquet pellentesque convallis. Ut vel tincidunt nulla. Mauris auctor tincidunt auctor.")));

        var grid = new Grid { Height = 300 };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        Grid.SetColumn(source, 0);
        Grid.SetColumn(firstOverflow, 1);
        Grid.SetColumn(secondOverflow, 2);
        grid.Children.Add(source);
        grid.Children.Add(firstOverflow);
        grid.Children.Add(secondOverflow);

        grid.Measure(new Size(900, 300));
        grid.Arrange(new Rect(0, 0, 900, 300));

        var sourceCount = GetRenderedText(source).Length;
        var firstCount = GetRenderedText(firstOverflow).Length;
        var secondCount = GetRenderedText(secondOverflow).Length;
        message = $"source={sourceCount}, firstOverflow={firstCount}, secondOverflow={secondCount}, desiredHeight={source.DesiredSize.Height:F1}, hasOverflow={source.HasOverflowContent}";
        return source.DesiredSize.Height <= 300.5
            && source.HasOverflowContent
            && sourceCount > 0
            && firstCount > 0;
    }

    private static void ArrangeOverrideForDiagnostics(object control, Size finalSize)
    {
        control.GetType()
            .GetMethod("ArrangeOverride", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(control, new object[] { finalSize });
    }

    private static TextBlock[] GetRenderedText(object control)
    {
        var canvas = (Canvas)control.GetType()
            .GetField("_canvas", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .GetValue(control)!;

        return canvas.Children.OfType<TextBlock>().ToArray();
    }

    private static bool CheckDocumentModel(out string message)
    {
        try
        {
            var span = new Span();
            var run = new Run { Text = "hello" };
            span.Inlines.Add(run);
            if (span.Inlines.Count != 1 || !ReferenceEquals(span.Inlines[0], run))
            {
                message = "Span did not own its inline collection";
                return false;
            }

            var bold = new Bold(new Run("bold"));
            if (bold.Inlines.Count != 1)
            {
                message = "Bold did not own its child inline";
                return false;
            }

            var italic = new Italic(new Run("italic"));
            if (italic.Inlines.Count != 1)
            {
                message = "Italic did not own its child inline";
                return false;
            }

            var firstInline = new Run("one");
            var secondInline = new Run("two");
            span.Inlines.Clear();
            span.Inlines.Add(firstInline);
            span.Inlines.Add(secondInline);
            span.Inlines.Remove(firstInline);
            if (span.Inlines.Count != 1 || !ReferenceEquals(span.Inlines[0], secondInline))
            {
                message = "Span did not track inline removal";
                return false;
            }

            var constructedRun = new Run("hello");
            if (constructedRun.Text != "hello")
            {
                message = $"Run text was '{constructedRun.Text}'";
                return false;
            }

            var section = new Section();
            var paragraph = new Paragraph(new Run("hello"));
            section.Blocks.Add(paragraph);
            if (section.Blocks.Count != 1 || !ReferenceEquals(section.Blocks[0], paragraph))
            {
                message = "Section did not own its block collection";
                return false;
            }

            var firstBlock = new Paragraph(new Run("one"));
            var secondBlock = new Paragraph(new Run("two"));
            section.Blocks.Clear();
            section.Blocks.Add(firstBlock);
            section.Blocks.Add(secondBlock);
            section.Blocks.Remove(firstBlock);
            if (section.Blocks.Count != 1 || !ReferenceEquals(section.Blocks[0], secondBlock))
            {
                message = "Section did not track block removal";
                return false;
            }

            var list = new System.Windows.Documents.List();
            var item = new ListItem(new Paragraph(new Run("hello")));
            list.ListItems.Add(item);
            if (list.ListItems.Count != 1 || !ReferenceEquals(list.ListItems[0], item) || item.Blocks.Count != 1)
            {
                message = "List did not own its list item collection";
                return false;
            }

            var document = new FlowDocument();
            var documentParagraph = new Paragraph(new Run("hello"));
            document.Blocks.Add(documentParagraph);
            if (document.Blocks.Count != 1 || !ReferenceEquals(document.Blocks[0], documentParagraph))
            {
                message = "FlowDocument did not own its block collection";
                return false;
            }

            var host = new FakeTextLayoutHost();
            document.TextLayoutHost = host;
            if (!ReferenceEquals(document.TextLayoutHost, host))
            {
                message = "FlowDocument did not retain its text layout host";
                return false;
            }

            message = "inline, block, list, and flow document checks passed";
            return true;
        }
        catch (Exception ex)
        {
            message = $"{ex.GetType().Name}: {ex.Message}";
            return false;
        }
    }

    private sealed class FakeTextLayoutHost : ITextLayoutHost
    {
        public object RenderScope => new();
        public bool IsLayoutValid => true;
        public double ViewportWidth => 0;
        public double ViewportHeight => 0;
        public double ExtentHeight => 0;
        public void InvalidateLayout()
        {
        }
    }
}

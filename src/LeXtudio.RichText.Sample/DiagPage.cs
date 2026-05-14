using System.Windows.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;
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

        var allPass = hasInlineFragment && multiLinePass && selectionPass;
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
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
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
}

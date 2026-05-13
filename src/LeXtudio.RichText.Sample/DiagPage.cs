using LeXtudio.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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

    public DiagPage()
    {
        _rtbInline = BuildInlineCodeBlock();
        _rtbMultiLine = BuildMultiLineCodeBlock();

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(_rtbInline);
        panel.Children.Add(_rtbMultiLine);
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

        var allPass = hasInlineFragment && multiLinePass;
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
}

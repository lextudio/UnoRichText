using System.Windows.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;

namespace LeXtudio.RichText.Sample;

/// <summary>
/// Interactive sample page — shows inline-code and multi-line code-block scenarios.
/// Launch with no arguments to see this page.  Pass --diag for automated exit-code testing.
/// </summary>
public sealed class MainPage : Page
{
    public MainPage()
    {
        var scroll = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };

        var root = new StackPanel { Spacing = 24, Padding = new Thickness(24) };
        scroll.Content = root;
        Content = scroll;

        root.Children.Add(SectionLabel("Inline Code (InlineUIContainer)"));
        root.Children.Add(BuildInlineCodeBlock());

        root.Children.Add(SectionLabel("Multi-Line Code Block (syntax-highlighted, 4 lines)"));
        root.Children.Add(new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
            Padding = new Thickness(12, 8, 12, 8),
            Child = BuildMultiLineCodeBlock()
        });

        root.Children.Add(SectionLabel("Plain Paragraph"));
        root.Children.Add(BuildPlainParagraph());
    }

    private static TextBlock SectionLabel(string text) => new()
    {
        Text = text,
        FontSize = 11,
        Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130)),
        Margin = new Thickness(0, 8, 0, 0)
    };

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

    private static LeXtudioRTB BuildMultiLineCodeBlock()
    {
        var rtb = new LeXtudioRTB { FontSize = 13 };
        var p = new Paragraph();

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

    private static LeXtudioRTB BuildPlainParagraph()
    {
        var rtb = new LeXtudioRTB { FontSize = 14 };
        var p = new Paragraph();
        p.Inlines.Add(new Run { Text = "This is a " });
        p.Inlines.Add(new Run { Text = "bold", FontWeight = System.Windows.FontWeights.Bold });
        p.Inlines.Add(new Run { Text = " and " });
        p.Inlines.Add(new Run { Text = "italic", FontStyle = System.Windows.FontStyles.Italic });
        p.Inlines.Add(new Run { Text = " paragraph with normal text wrapping." });
        rtb.Blocks.Add(p);
        return rtb;
    }
}

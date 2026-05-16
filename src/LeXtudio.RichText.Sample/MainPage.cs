using System.Windows.Documents;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.WinUI.Controls;
using UnoPropertyGrid;
using Windows.UI;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;

namespace LeXtudio.RichText.Sample;

/// <summary>
/// Interactive sample page — shows inline-code and multi-line code-block scenarios and a live RichTextBlock property inspector.
/// Launch with no arguments to see this page.  Pass --diag for automated exit-code testing.
/// </summary>
public sealed class MainPage : Page
{
    private readonly LeXtudioRTB _liveRichTextBlock;

    public MainPage()
    {
        _liveRichTextBlock = BuildLiveRichTextPreview();

        var propertyGrid = new PropertyGridControl
        {
            SelectedObject = _liveRichTextBlock,
            PropertyGridTheme = ElementTheme.Light,
            ShowReadOnlyProperties = false,
            Margin = new Thickness(0, 8, 0, 0)
        };

        var rightPanelContent = new Grid { RowSpacing = 12 };
        rightPanelContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        rightPanelContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var headerPanel = new StackPanel { Spacing = 12 };
        headerPanel.Children.Add(new TextBlock
        {
            Text = "RichTextBlock Properties",
            FontSize = 18,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 4)
        });

        propertyGrid.VerticalAlignment = VerticalAlignment.Stretch;
        propertyGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

        rightPanelContent.Children.Add(headerPanel);
        rightPanelContent.Children.Add(propertyGrid);
        Grid.SetRow(headerPanel, 0);
        Grid.SetRow(propertyGrid, 1);

        var propertyPanelWrapper = new Border
        {
            Child = rightPanelContent,
            Padding = new Thickness(16),
            Background = new SolidColorBrush(Color.FromArgb(255, 250, 250, 250)),
            CornerRadius = new CornerRadius(12),
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var root = new Grid { Padding = new Thickness(24), ColumnSpacing = 16 };
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 200 });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var propertyPanelColumn = new ColumnDefinition { Width = new GridLength(320), MinWidth = 200 };
        root.ColumnDefinitions.Add(propertyPanelColumn);

        var leftPanel = new StackPanel { Spacing = 24 };
        leftPanel.Children.Add(SectionLabel("Live RichTextBlock Preview"));
        leftPanel.Children.Add(new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            Child = _liveRichTextBlock
        });

        leftPanel.Children.Add(SectionLabel("Inline Code (InlineUIContainer)"));
        leftPanel.Children.Add(BuildInlineCodeBlock());

        leftPanel.Children.Add(SectionLabel("Multi-Line Code Block (syntax-highlighted, 4 lines)"));
        leftPanel.Children.Add(new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
            Padding = new Thickness(12, 8, 12, 8),
            Child = BuildMultiLineCodeBlock()
        });

        leftPanel.Children.Add(SectionLabel("Plain Paragraph"));
        leftPanel.Children.Add(BuildPlainParagraph());

        var leftScroller = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = leftPanel
        };

        Grid.SetColumn(leftScroller, 0);

        var splitter = new GridSplitter
        {
            Width = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Stretch,
            ResizeDirection = GridSplitter.GridResizeDirection.Columns,
            Background = new SolidColorBrush(Color.FromArgb(64, 128, 128, 128))
        };
        Grid.SetColumn(splitter, 1);
        Grid.SetColumn(propertyPanelWrapper, 2);

        root.Children.Add(leftScroller);
        root.Children.Add(splitter);
        root.Children.Add(propertyPanelWrapper);

        Content = root;
    }

    private static TextBlock SectionLabel(string text) => new()
    {
        Text = text,
        FontSize = 11,
        Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130)),
        Margin = new Thickness(0, 8, 0, 0)
    };

    private static LeXtudioRTB BuildLiveRichTextPreview()
    {
        var rtb = new LeXtudioRTB
        {
            FontSize = 15,
            LineHeight = 24,
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
        };

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(new Run { Text = "This sample shows a live " });
        paragraph.Inlines.Add(new Run { Text = "RichTextBlock", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });
        paragraph.Inlines.Add(new Run { Text = " preview. Use the property grid on the right to adjust properties such as " });
        paragraph.Inlines.Add(new Run { Text = "FontSize", FontStyle = global::Windows.UI.Text.FontStyle.Italic });
        paragraph.Inlines.Add(new Run { Text = ", " });
        paragraph.Inlines.Add(new Run { Text = "Foreground", FontStyle = global::Windows.UI.Text.FontStyle.Italic });
        paragraph.Inlines.Add(new Run { Text = ", or " });
        paragraph.Inlines.Add(new Run { Text = "LineHeight", FontStyle = global::Windows.UI.Text.FontStyle.Italic });
        paragraph.Inlines.Add(new Run { Text = "." });

        var secondParagraph = new Paragraph();
        secondParagraph.Inlines.Add(new Run { Text = "The property grid is bound directly to this RichTextBlock instance, so changes are applied immediately." });
        secondParagraph.Inlines.Add(new Run { Text = "\n\n" });
        secondParagraph.Inlines.Add(new Run { Text = "Use FontWeight, FontStyle, TextWrapping, and Foreground to see instant live updates." });

        rtb.Blocks.Add(paragraph);
        rtb.Blocks.Add(secondParagraph);
        return rtb;
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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
// Use System.Windows.Documents (our WPF-shaped, fully-implemented model) for all
// rich-text document elements. Our RichTextBlock and RichEditBox expose
// System.Windows.Documents.* types — using Microsoft.UI.Xaml.Documents.* (Uno's
// not-implemented projections) here would cause type mismatches with .Blocks /
// .Inlines collections.
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;
using Span = System.Windows.Documents.Span;
using Bold = System.Windows.Documents.Bold;
using Italic = System.Windows.Documents.Italic;
using Underline = System.Windows.Documents.Underline;
using Hyperlink = System.Windows.Documents.Hyperlink;
using LineBreak = System.Windows.Documents.LineBreak;
using InlineUIContainer = System.Windows.Documents.InlineUIContainer;
using CommunityToolkit.WinUI.Controls;
using UnoPropertyGrid;
using Windows.UI;
using FontWeights = Microsoft.UI.Text.FontWeights;
using FontStyle = global::Windows.UI.Text.FontStyle;
using TextDecorations = global::Windows.UI.Text.TextDecorations;
using TextRange = LeXtudio.UI.Xaml.Controls.TextRange;
using TextHighlighter = LeXtudio.UI.Xaml.Controls.TextHighlighter;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;
using LeXtudioRTBO = LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow;
using LeXtudioReb = LeXtudio.UI.Xaml.Controls.RichEditBox;

namespace LeXtudio.RichText.Sample;

/// <summary>
/// Interactive sample page — hosts two tabs:
///   1. RichTextBlock: live preview + property grid for the read-only rich-text control.
///   2. RichEditBox:   live editable preview + property grid for the editable rich-text control.
/// Launch with no arguments to see this page. Pass --diag for automated exit-code testing.
/// </summary>
public sealed class MainPage : Page
{
    public MainPage()
    {
        var tabView = new TabView
        {
            IsAddTabButtonVisible = false,
            CanReorderTabs = false,
            CanDragTabs = false,
        };

        tabView.TabItems.Add(new TabViewItem
        {
            Header = "RichTextBlock",
            IsClosable = false,
            Content = BuildRichTextBlockTab(),
        });

        tabView.TabItems.Add(new TabViewItem
        {
            Header = "RichTextBlock Gallery",
            IsClosable = false,
            Content = BuildRichTextBlockGalleryTab(),
        });

        tabView.TabItems.Add(new TabViewItem
        {
            Header = "RichEditBox",
            IsClosable = false,
            Content = BuildRichEditBoxTab(),
        });

        tabView.TabItems.Add(new TabViewItem
        {
            Header = "RichEditBox Gallery",
            IsClosable = false,
            Content = BuildRichEditBoxGalleryTab(),
        });

        Content = tabView;
    }

    // ── RichTextBlock tab ────────────────────────────────────────────────────────

    private static UIElement BuildRichTextBlockTab()
    {
        var liveRichTextBlock = BuildLiveRichTextPreview();

        var leftPanel = new StackPanel { Spacing = 24 };
        leftPanel.Children.Add(SectionLabel("Live RichTextBlock Preview"));
        leftPanel.Children.Add(new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            Child = liveRichTextBlock,
        });

        return BuildPreviewWithPropertyGrid(
            leftContent: leftPanel,
            propertyTarget: liveRichTextBlock,
            propertyPanelHeader: "RichTextBlock Properties");
    }

    // ── RichEditBox tab ──────────────────────────────────────────────────────────

    private static UIElement BuildRichEditBoxTab()
    {
        var liveRichEditBox = BuildLiveRichEditBoxPreview();
        var toolbar = BuildRichEditBoxToolbar(liveRichEditBox);

        // Editor card: toolbar on top, RichEditBox below filling the remaining space.
        var editorCard = new Grid
        {
            RowSpacing = 8,
            Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
            MinHeight = 240,
        };
        editorCard.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        editorCard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(toolbar, 0);
        Grid.SetRow(liveRichEditBox, 1);
        editorCard.Children.Add(toolbar);
        editorCard.Children.Add(liveRichEditBox);

        var leftPanel = new Grid { RowSpacing = 16 };
        leftPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        leftPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        leftPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var header = SectionLabel("Live RichEditBox Preview");
        Grid.SetRow(header, 0);
        leftPanel.Children.Add(header);

        Grid.SetRow(editorCard, 1);
        leftPanel.Children.Add(editorCard);

        var notes = new TextBlock
        {
            Text = "Select text inside the editor and use the toolbar above (Bold / Italic / Underline / "
                + "Code) to wrap the selection with markdown-style markers. Properties on the right "
                + "bind directly to this RichEditBox instance.",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90)),
        };
        Grid.SetRow(notes, 2);
        leftPanel.Children.Add(notes);

        return BuildPreviewWithPropertyGrid(
            leftContent: leftPanel,
            propertyTarget: liveRichEditBox,
            propertyPanelHeader: "RichEditBox Properties");
    }

    private static UIElement BuildRichTextBlockGalleryTab()
    {
        var panel = new StackPanel { Spacing = 24, Padding = new Thickness(24) };
        panel.Children.Add(BuildGalleryCard("A simple RichTextBlock.", BuildSimpleRichTextBlockExample()));
        panel.Children.Add(BuildGalleryCard("A RichTextBlock with a custom selection highlight color.", BuildHighlightedRichTextBlockExample()));
        panel.Children.Add(BuildGalleryCard("A RichTextBlock with overflow.", BuildOverflowRichTextBlockExample()));
        panel.Children.Add(BuildGalleryCard("RichTextBlock with custom TextHighlighting", BuildTextHighlightingRichTextBlockExample()));
        return new ScrollViewer
        {
            Content = panel,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
        };
    }

    private static UIElement BuildRichEditBoxGalleryTab()
    {
        var panel = new StackPanel { Spacing = 24, Padding = new Thickness(24) };
        panel.Children.Add(BuildGalleryCard("A simple text editor using RichEditBox.", BuildSimpleRichEditBoxExample()));
        panel.Children.Add(BuildGalleryCard("Customizing RichEditBox's CommandBarFlyout - Adding 'Share'", BuildCommandBarCustomRichEditBoxExample()));
        panel.Children.Add(BuildGalleryCard("A custom editor with RichEditBox.", BuildCustomRichEditBoxExample()));
        panel.Children.Add(BuildGalleryCard("Rich edit box in math mode.", BuildMathModeRichEditBoxExample()));
        panel.Children.Add(BuildGalleryCard("Working with MathML in RichEditBox", BuildMathMlRichEditBoxExample()));
        return new ScrollViewer
        {
            Content = panel,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
        };
    }

    private static UIElement BuildGalleryCard(string header, UIElement example)
    {
        var card = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 250, 250, 250)),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(16),
        };

        var stack = new StackPanel { Spacing = 12 };
        stack.Children.Add(new TextBlock
        {
            Text = header,
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
        });
        stack.Children.Add(example);
        card.Child = stack;
        return card;
    }

    private static UIElement BuildSimpleRichTextBlockExample()
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Child = new LeXtudioRTB
            {
                Blocks =
                {
                    new Paragraph { Inlines = { new Run { Text = "I am a RichTextBlock." } } }
                }
            }
        };
    }

    private static UIElement BuildHighlightedRichTextBlockExample()
    {
        var rtb = new LeXtudioRTB
        {
            SelectionHighlightColor = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)),
            Blocks =
            {
                new Paragraph
                {
                    Inlines =
                    {
                        new Run { Text = "RichTextBlock provides a rich text display container that supports" },
                        new Run { Text = " formatted text", FontStyle = FontStyle.Italic, FontWeight = FontWeights.Bold },
                        new Run { Text = ", " },
                        new Hyperlink { Inlines = { new Run { Text = "hyperlinks" } }, NavigateUri = new Uri("https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Documents.Hyperlink") },
                        new Run { Text = ", inline images, and other rich content." }
                    }
                },
                new Paragraph { Inlines = { new Run { Text = "RichTextBlock also supports a built-in overflow model." } } }
            }
        };

        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Child = rtb,
        };
    }

    private static UIElement BuildOverflowRichTextBlockExample()
    {
        var grid = new Grid { Height = 300 };
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        var p1 = "Linked text containers allow text which does not fit in one element to overflow into a different element on the page. Creative use of linked text containers enables basic multicolumn support and other advanced page layouts.";
        var p2 = "Duis sed nulla metus, id hendrerit velit. Curabitur dolor purus, bibendum eu cursus lacinia, interdum vel augue. Aenean euismod eros et sapien vehicula dictum. Duis ullamcorper, turpis nec feugiat tincidunt, dui erat luctus risus, aliquam accumsan lacus est vel quam. Nunc lacus massa, varius eget accumsan id, congue sed orci. Duis dignissim hendrerit egestas. Proin ut turpis magna, sit amet porta erat.";
        var p3 = "Nunc semper metus nec magna imperdiet nec vestibulum dui fringilla. Sed sed ante libero, nec porttitor mi. Ut luctus, neque vitae placerat egestas, urna leo auctor magna, sit amet ultricies ipsum felis quis sapien. Proin eleifend varius dui, at vestibulum nunc consectetur nec. Mauris nulla elit, ultrices a sodales non, aliquam ac est.";
        var p4 = "Quisque sit amet risus nulla. Quisque vestibulum posuere velit, vitae vestibulum eros scelerisque sit amet. In in risus est, at laoreet dolor. Nullam aliquet pellentesque convallis. Ut vel tincidunt nulla. Mauris auctor tincidunt auctor. Aenean orci ante, vulputate ac sagittis sit amet, consequat at mi.";

        LeXtudioRTB MakeColumn(string t1, string t2)
        {
            var col = new LeXtudioRTB
            {
                Margin = new Thickness(12, 0, 12, 0),
                TextAlignment = TextAlignment.Justify,
            };
            col.Blocks.Add(new Paragraph { Inlines = { new Run { Text = t1 } } });
            col.Blocks.Add(new Paragraph { Inlines = { new Run { Text = t2 } } });
            return col;
        }

        var col1 = MakeColumn(p1, p2);
        var col2 = MakeColumn(p3, p4);
        var col3 = MakeColumn(p2, p3);

        Grid.SetColumn(col1, 0);
        Grid.SetColumn(col2, 1);
        Grid.SetColumn(col3, 2);
        grid.Children.Add(col1);
        grid.Children.Add(col2);
        grid.Children.Add(col3);

        return grid;
    }

    private static UIElement BuildTextHighlightingRichTextBlockExample()
    {
        // Exact test case from WinUI Gallery
        var rtb = new LeXtudioRTB();
        var p = new Paragraph();
        p.Inlines.Add(new Run { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua" });
        rtb.Blocks.Add(p);

        var combo = new ComboBox { Header = "Text highlighting color" };
        combo.Items.Add(new ComboBoxItem { Content = "Yellow", IsSelected = true });
        combo.Items.Add(new ComboBoxItem { Content = "Red" });
        combo.Items.Add(new ComboBoxItem { Content = "Blue" });
        combo.SelectionChanged += (sender, _) =>
        {
            // Get color to use (matching WinUI Gallery behavior)
            var selectedItem = (sender as ComboBox)?.SelectedItem as ComboBoxItem;
            var color = Windows.UI.Colors.Yellow;
            switch (selectedItem?.Content as string)
            {
                case "Yellow":
                    color = Windows.UI.Colors.Yellow;
                    break;
                case "Red":
                    color = Windows.UI.Colors.Red;
                    break;
                case "Blue":
                    color = Windows.UI.Colors.Blue;
                    break;
            }

            // Get text range and highlighter (WinUI test: StartIndex=28, Length=11 targets "do eiusmod")
            var textRange = new TextRange()
            {
                StartIndex = 28,
                Length = 11
            };
            var highlighter = new TextHighlighter()
            {
                Background = new SolidColorBrush(color),
            };
            highlighter.Ranges.Add(textRange);

            // Switch texthighlighter (clear and re-add, like WinUI Gallery)
            rtb.TextHighlighters.Clear();
            rtb.TextHighlighters.Add(highlighter);
        };

        // Trigger initial highlight
        combo.SelectedIndex = 0;

        var stack = new StackPanel { Spacing = 12 };
        stack.Children.Add(rtb);
        stack.Children.Add(combo);
        return stack;
    }

    private static UIElement BuildSimpleRichEditBoxExample()
    {
        var editor = new LeXtudioReb { Height = 200 };
        AutomationProperties.SetName(editor, "simple text editor");

        return new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Child = editor,
        };
    }

    private static UIElement BuildCommandBarCustomRichEditBoxExample()
    {
        var editor = new LeXtudioReb
        {
            Width = 800,
            Height = 200,
        };
        AutomationProperties.SetName(editor, "editor with custom menu");

        void MenuOpening(object? sender, object e)
        {
            if (sender is CommandBarFlyout flyout && flyout.Target == editor)
            {
                foreach (var cmd in flyout.PrimaryCommands)
                {
                    if (cmd is AppBarButton existing && existing.Label == "Share")
                    {
                        return;
                    }
                }

                flyout.PrimaryCommands.Add(new AppBarButton
                {
                    Label = "Share",
                    Icon = new SymbolIcon(Symbol.Share),
                });
            }
        }

        if (editor.SelectionFlyout is CommandBarFlyout selectionFlyout)
        {
            selectionFlyout.Opening += MenuOpening;
        }
        if (editor.ContextFlyout is CommandBarFlyout contextFlyout)
        {
            contextFlyout.Opening += MenuOpening;
        }

        return editor;
    }

    private static UIElement BuildCustomRichEditBoxExample()
    {
        var editor = new LeXtudioReb
        {
            Height = 200,
            MinWidth = 300,
        };
        AutomationProperties.SetName(editor, "custom editor");

        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        var open = MakeSimpleEditorButton("Open", "Open file");
        var save = MakeSimpleEditorButton("Save", "Save file");
        var bold = MakeSimpleEditorButton("B", "Bold", FontWeights.Bold);
        var italic = MakeSimpleEditorButton("I", "Italic", FontWeights.Normal, FontStyle.Italic);
        var fontColor = new DropDownButton
        {
            Content = new SymbolIcon(Symbol.FontColor),
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
            BorderThickness = new Thickness(0),
        };
        ToolTipService.SetToolTip(fontColor, "Font color");

        var colorPalette = new StackPanel { Spacing = 8 };
        var paletteRows = new[]
        {
            new[]
            {
                Color.FromArgb(255, 255, 0, 0),
                Color.FromArgb(255, 255, 165, 0),
                Color.FromArgb(255, 255, 255, 0),
                Color.FromArgb(255, 0, 128, 0),
            },
            new[]
            {
                Color.FromArgb(255, 0, 0, 255),
                Color.FromArgb(255, 75, 0, 130),
                Color.FromArgb(255, 238, 130, 238),
                Color.FromArgb(255, 128, 128, 128),
            },
        };
        foreach (var row in paletteRows)
        {
            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            foreach (var c in row)
            {
                var swatchColor = c;
                var swatchButton = new Button
                {
                    Width = 32,
                    Height = 32,
                    Padding = new Thickness(0),
                    Content = new Border
                    {
                        Background = new SolidColorBrush(swatchColor),
                        Width = 24,
                        Height = 24,
                    },
                };
                swatchButton.Click += (_, _) =>
                {
                    editor.Foreground = new SolidColorBrush(swatchColor);
                    fontColor.Flyout?.Hide();
                };
                rowPanel.Children.Add(swatchButton);
            }
            colorPalette.Children.Add(rowPanel);
        }
        fontColor.Flyout = new Flyout { Content = colorPalette };

        open.Click += (_, _) => editor.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "Open file action is sample-only in this demo.");
        save.Click += (_, _) => editor.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "Save file action is sample-only in this demo.");
        bold.Click += (_, _) => editor.WrapSelection("**", "**");
        italic.Click += (_, _) => editor.WrapSelection("*", "*");

        toolbar.Children.Add(open);
        toolbar.Children.Add(save);
        toolbar.Children.Add(bold);
        toolbar.Children.Add(italic);
        toolbar.Children.Add(fontColor);

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(toolbar);
        panel.Children.Add(editor);

        var findStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        findStack.Children.Add(new TextBlock { Text = "Find:", VerticalAlignment = VerticalAlignment.Center });
        var findBox = new TextBox { Width = 224, PlaceholderText = "Enter search text" };
        var findStatus = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90)),
        };
        findBox.TextChanged += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(findBox.Text))
            {
                findStatus.Text = string.Empty;
                return;
            }

            editor.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var content);
            var index = content.IndexOf(findBox.Text, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                findStatus.Text = $"Found at index {index}";
            }
            else
            {
                findStatus.Text = "Not found";
            }
        };
        findStack.Children.Add(findBox);
        findStack.Children.Add(findStatus);
        panel.Children.Add(findStack);

        return panel;
    }

    private static Button MakeSimpleEditorButton(string text, string tooltip, global::Windows.UI.Text.FontWeight? weight = null, global::Windows.UI.Text.FontStyle style = default)
    {
        var content = new TextBlock
        {
            Text = text,
            FontWeight = weight ?? FontWeights.Normal,
            FontStyle = style,
            FontSize = 14,
        };
        var button = new Button
        {
            Content = content,
            Width = 44,
            Height = 32,
            Padding = new Thickness(0),
        };
        ToolTipService.SetToolTip(button, tooltip);
        return button;
    }

    private static UIElement BuildMathModeRichEditBoxExample()
    {
        var editor = new LeXtudioReb
        {
            FontSize = 16,
            Height = 80,
            Width = 724,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        var stack = new StackPanel { Spacing = 12 };
        var description = new LeXtudioRTB();
        var paragraph = new Paragraph();
        paragraph.Inlines.Add(new Run { Text = "Math mode enables users to have input automatically recognized and converted to math expressions while being received." });
        description.Blocks.Add(paragraph);
        stack.Children.Add(description);
        editor.Document.SetMathMode(Microsoft.UI.Text.RichEditMathMode.MathOnly);
        stack.Children.Add(editor);
        var sampleFormulaButton = new Button
        {
            Content = "Set sample formula",
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        sampleFormulaButton.Click += (_, _) => editor.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "x^2 = 4");
        stack.Children.Add(sampleFormulaButton);
        return stack;
    }

    private static UIElement BuildMathMlRichEditBoxExample()
    {
        var description = new LeXtudioRTB();
        var p1 = new Paragraph();
        p1.Inlines.Add(new Bold { Inlines = { new Run { Text = "SetMathML" } } });
        p1.Inlines.Add(new Run { Text = " takes a MathML string and displays the equation in the " });
        p1.Inlines.Add(new Bold { Inlines = { new Run { Text = "RichEditBox" } } });
        p1.Inlines.Add(new Run { Text = ". It replaces any existing equation with the new one." });
        description.Blocks.Add(p1);

        var p2 = new Paragraph();
        p2.Inlines.Add(new Bold { Inlines = { new Run { Text = "GetMathML" } } });
        p2.Inlines.Add(new Run { Text = " retrieves MathML if the equation is in a single line." });
        description.Blocks.Add(p2);

        var mathEditor = new LeXtudioReb
        {
            FontSize = 16,
            Height = 80,
            Width = 724,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        mathEditor.Document.SetMathMode(Microsoft.UI.Text.RichEditMathMode.MathOnly);

        var mathMlCode = new TextBox
        {
            IsReadOnly = true,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            MinHeight = 140,
            Text = "<!-- No MathML content -->",
        };

        var setFormulaButton = new Button
        {
            Content = "Set sample formula",
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        setFormulaButton.Click += (_, _) =>
        {
            var formulaMathML =
                "<mml:math xmlns:mml=\"http://www.w3.org/1998/Math/MathML\" display=\"block\">\r\n" +
                "  <mml:mi mathcolor=\"#000000\">x</mml:mi>\r\n" +
                "  <mml:mo mathcolor=\"#000000\">∈</mml:mo>\r\n" +
                "  <mml:mi mathcolor=\"#000000\">P</mml:mi>\r\n" +
                "  <mml:mfenced><mml:mrow><mml:mi mathcolor=\"#000000\">A</mml:mi></mml:mrow></mml:mfenced>\r\n" +
                "  <mml:mo mathcolor=\"#000000\">↔</mml:mo>\r\n" +
                "  <mml:mi mathcolor=\"#000000\">x</mml:mi>\r\n" +
                "  <mml:mo mathcolor=\"#000000\">⊆</mml:mo>\r\n" +
                "  <mml:mi mathcolor=\"#000000\">A</mml:mi>\r\n" +
                "</mml:math>";
            mathEditor.Document.SetMathML(formulaMathML);
            mathMlCode.Text = FormatMathML(formulaMathML);
        };

        var stack = new StackPanel { Spacing = 12 };
        stack.Children.Add(description);
        stack.Children.Add(mathEditor);
        stack.Children.Add(new TextBlock
        {
            Text = "MathML Code",
            FontWeight = FontWeights.SemiBold,
        });
        stack.Children.Add(mathMlCode);
        stack.Children.Add(setFormulaButton);
        return stack;
    }

    private static string FormatMathML(string mathML)
    {
        try
        {
            var doc = System.Xml.Linq.XDocument.Parse(mathML);
            return doc.ToString();
        }
        catch
        {
            return mathML;
        }
    }

    private static UIElement BuildRichEditBoxToolbar(LeXtudioReb editor)
    {
        // Markdown-style toolbar: these wrap the selection with text markers. When the engine
        // gains real character-format support, the same buttons will toggle
        // editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle, etc.
        var bold = MakeToolbarButton("B", "Bold (Ctrl+B)", FontWeights.Bold, FontStyle.Normal,
            click: () => editor.WrapSelection("**", "**"));
        var italic = MakeToolbarButton("I", "Italic (Ctrl+I)", FontWeights.Normal, FontStyle.Italic,
            click: () => editor.WrapSelection("*", "*"));
        var underline = MakeToolbarButton("U", "Underline (Ctrl+U)", FontWeights.Normal, FontStyle.Normal,
            click: () => editor.WrapSelection("__", "__"),
            decorations: TextDecorations.Underline);
        var strike = MakeToolbarButton("S", "Strikethrough", FontWeights.Normal, FontStyle.Normal,
            click: () => editor.WrapSelection("~~", "~~"),
            decorations: TextDecorations.Strikethrough);
        var code = MakeToolbarButton("</>", "Code", FontWeights.Normal, FontStyle.Normal,
            click: () => editor.WrapSelection("`", "`"),
            font: new FontFamily("Consolas"));

        var upper = MakeToolbarButton("Aa", "Toggle case",
            FontWeights.SemiBold, FontStyle.Normal,
            click: () => editor.TransformSelectedText(s => s.ToUpperInvariant()));

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 4,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        panel.Children.Add(bold);
        panel.Children.Add(italic);
        panel.Children.Add(underline);
        panel.Children.Add(strike);
        panel.Children.Add(code);
        panel.Children.Add(new AppBarSeparator { Margin = new Thickness(4, 0, 4, 0) });
        panel.Children.Add(upper);

        return panel;
    }

    private static Button MakeToolbarButton(
        string label,
        string tooltip,
        global::Windows.UI.Text.FontWeight weight,
        global::Windows.UI.Text.FontStyle style,
        System.Action click,
        TextDecorations decorations = TextDecorations.None,
        FontFamily? font = null)
    {
        var content = new TextBlock
        {
            Text = label,
            FontWeight = weight,
            FontStyle = style,
            TextDecorations = decorations,
            FontSize = 14,
        };
        if (font is not null) content.FontFamily = font;

        var b = new Button
        {
            Content = content,
            Width = 32,
            Height = 32,
            Padding = new Thickness(0),
        };
        ToolTipService.SetToolTip(b, tooltip);
        b.Click += (_, _) => click();
        return b;
    }

    // ── Shared preview-with-property-grid layout ─────────────────────────────────

    private static UIElement BuildPreviewWithPropertyGrid(UIElement leftContent, object propertyTarget, string propertyPanelHeader)
    {
        var propertyGrid = new PropertyGridControl
        {
            SelectedObject = propertyTarget,
            PropertyGridTheme = ElementTheme.Light,
            ShowReadOnlyProperties = false,
            Margin = new Thickness(0, 8, 0, 0),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var rightPanelContent = new Grid { RowSpacing = 12 };
        rightPanelContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        rightPanelContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var headerPanel = new StackPanel { Spacing = 12 };
        headerPanel.Children.Add(new TextBlock
        {
            Text = propertyPanelHeader,
            FontSize = 18,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 4),
        });

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
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        var root = new Grid { Padding = new Thickness(24), ColumnSpacing = 16 };
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 200 });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(320), MinWidth = 200 });

        var leftScroller = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = leftContent,
        };
        Grid.SetColumn(leftScroller, 0);

        var splitter = new GridSplitter
        {
            Width = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Stretch,
            ResizeDirection = GridSplitter.GridResizeDirection.Columns,
            Background = new SolidColorBrush(Color.FromArgb(64, 128, 128, 128)),
        };
        Grid.SetColumn(splitter, 1);
        Grid.SetColumn(propertyPanelWrapper, 2);

        root.Children.Add(leftScroller);
        root.Children.Add(splitter);
        root.Children.Add(propertyPanelWrapper);

        return root;
    }

    private static TextBlock SectionLabel(string text) => new()
    {
        Text = text,
        FontSize = 11,
        Foreground = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130)),
        Margin = new Thickness(0, 8, 0, 0),
    };

    // ── RichTextBlock builders ───────────────────────────────────────────────────

    private static LeXtudioRTB BuildLiveRichTextPreview()
    {
        var rtb = new LeXtudioRTB
        {
            FontSize = 15,
            LineHeight = 24,
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
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
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 220, 170)),
                },
            },
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

    // ── RichEditBox builders ─────────────────────────────────────────────────────

    private static LeXtudioReb BuildLiveRichEditBoxPreview()
    {
        var reb = new LeXtudioReb
        {
            FontSize = 15,
            MinHeight = 200,
            PlaceholderText = "Replace me here",
            Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
        };

        reb.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "Replace me here");
        return reb;
    }
}

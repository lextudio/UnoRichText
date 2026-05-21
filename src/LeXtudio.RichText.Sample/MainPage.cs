using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using LeXtudio.UI.Xaml.Controls;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using FlowDocument = System.Windows.Documents.FlowDocument;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;
using Hyperlink = System.Windows.Documents.Hyperlink;
using Inline = System.Windows.Documents.Inline;
using Span = System.Windows.Documents.Span;
using WpfRichTextBox = System.Windows.Controls.RichTextBox;

namespace LeXtudio.RichText.Sample;

public sealed partial class MainPage : Page
{
    private global::Windows.UI.Color currentColor = Colors.Green;
    private WpfRichTextBox? _wpfRichTextBox;
    private static readonly string _wpfSelectionLogPath =
        System.IO.Path.Combine(System.IO.Path.GetTempPath(), "rtb-template.log");

    public MainPage()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            MainTabs.SelectedIndex = 2;
        };

        LiveRichEditBox.Document.SetText(TextSetOptions.None, "Replace me here");
        MathEditor.Document.SetMathMode(RichEditMathMode.MathOnly);
        mathEditor2.Document.SetMathMode(RichEditMathMode.MathOnly);
        InitializeWpfRichTextBoxSample();
        EnsureGalleryRichTextBlocks();
        UpdateEditorToolbarState();
        UpdateLiveRichEditBoxSnapshot();
    }

    private void InitializeWpfRichTextBoxSample()
    {
        _wpfRichTextBox = new WpfRichTextBox();
        _wpfRichTextBox.SelectionChanged += LiveWpfRichTextBox_SelectionChanged;
        WpfRichTextBoxHost.Content = _wpfRichTextBox;
        WpfRichTextBoxPropertyGrid.SelectedObject = _wpfRichTextBox;

        var document = new FlowDocument();

        var intro = new Paragraph();
        intro.Inlines.Add(new Run { Text = "This sample hosts a " });
        intro.Inlines.Add(new Run { Text = "FlowDocument", FontWeight = FontWeights.SemiBold });
        intro.Inlines.Add(new Run { Text = " inside the ported " });
        intro.Inlines.Add(new Run { Text = "System.Windows.Controls.RichTextBox", FontStyle = global::Windows.UI.Text.FontStyle.Italic });
        intro.Inlines.Add(new Run { Text = "." });
        document.Blocks.Add(intro);

        var details = new Paragraph();
        details.Inlines.Add(new Run { Text = "Formatting, inline runs, and " });
        var hyperlink = new Hyperlink { NavigateUri = new Uri("https://learn.microsoft.com/dotnet/desktop/wpf/controls/richtextbox-overview") };
        hyperlink.Inlines.Add(new Run { Text = "hyperlinks" });
        details.Inlines.Add(hyperlink);
        details.Inlines.Add(new Run { Text = " are constructed with WPF-shaped document types." });
        document.Blocks.Add(details);

        var note = new Paragraph();
        note.Inlines.Add(new Run { Text = "Editing driven by the ported TextEditor/TextBoxBase stack.", FontWeight = FontWeights.SemiBold });
        document.Blocks.Add(note);

        _wpfRichTextBox.Document = document;
        UpdateWpfRichTextBoxSelectionStatus();
    }

    private void EnsureGalleryRichTextBlocks()
    {
        if (HasText(SimpleRichTextBlockExample)
            && HasText(SelectionHighlightRichTextBlockExample)
            && HasText(OverflowRichTextBlockExample)
            && HasText(TextHighlightingRichTextBlock))
        {
            return;
        }

        SimpleRichTextBlockExample.Blocks.Clear();
        SimpleRichTextBlockExample.Blocks.Add(ParagraphWithRuns("I am a RichTextBlock."));

        SelectionHighlightRichTextBlockExample.Blocks.Clear();
        var selectionParagraph = new Paragraph();
        selectionParagraph.Inlines.Add(new Run { Text = "RichTextBlock provides a rich text display container that supports " });
        selectionParagraph.Inlines.Add(new Run { Text = "formatted text", FontStyle = global::Windows.UI.Text.FontStyle.Italic, FontWeight = FontWeights.Bold });
        selectionParagraph.Inlines.Add(new Run { Text = ", " });
        var hyperlink = new Hyperlink { NavigateUri = new Uri("https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.Documents.Hyperlink") };
        hyperlink.Inlines.Add(new Run { Text = "hyperlinks" });
        selectionParagraph.Inlines.Add(hyperlink);
        selectionParagraph.Inlines.Add(new Run { Text = ", inline images, and other rich content." });
        SelectionHighlightRichTextBlockExample.Blocks.Add(selectionParagraph);
        SelectionHighlightRichTextBlockExample.Blocks.Add(ParagraphWithRuns("RichTextBlock also supports a built-in overflow model."));

        OverflowRichTextBlockExample.Blocks.Clear();
        OverflowRichTextBlockExample.Blocks.Add(ParagraphWithRuns("Linked text containers allow text which does not fit in one element to overflow into a different element on the page. Creative use of linked text containers enables basic multicolumn support and other advanced page layouts."));
        OverflowRichTextBlockExample.Blocks.Add(ParagraphWithRuns("Duis sed nulla metus, id hendrerit velit. Curabitur dolor purus, bibendum eu cursus lacinia, interdum vel augue. Aenean euismod eros et sapien vehicula dictum. Duis ullamcorper, turpis nec feugiat tincidunt, dui erat luctus risus, aliquam accumsan lacus est vel quam. Nunc lacus massa, varius eget accumsan id, congue sed orci. Duis dignissim hendrerit egestas. Proin ut turpis magna, sit amet porta erat. Nunc semper metus nec magna imperdiet nec vestibulum dui fringilla. Sed sed ante libero, nec porttitor mi. Ut luctus, neque vitae placerat egestas, urna leo auctor magna, sit amet ultricies ipsum felis quis sapien. Proin eleifend varius dui, at vestibulum nunc consectetur nec. Mauris nulla elit, ultrices a sodales non, aliquam ac est. Quisque sit amet risus nulla. Quisque vestibulum posuere velit, vitae vestibulum eros scelerisque sit amet. In in risus est, at laoreet dolor. Nullam aliquet pellentesque convallis. Ut vel tincidunt nulla. Mauris auctor tincidunt auctor."));

        TextHighlightingRichTextBlock.Blocks.Clear();
        TextHighlightingRichTextBlock.Blocks.Add(ParagraphWithRuns("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"));

        MathModeDescriptionRichTextBlock.Blocks.Clear();
        MathModeDescriptionRichTextBlock.Blocks.Add(ParagraphWithRuns("Math mode enables users to have input automatically recognized and converted to math expressions while being received."));

        MathMlDescriptionRichTextBlock.Blocks.Clear();
        var setMathMl = new Paragraph();
        setMathMl.Inlines.Add(new Run { Text = "SetMathML", FontWeight = FontWeights.SemiBold });
        setMathMl.Inlines.Add(new Run { Text = " takes a MathML string and displays the equation in the " });
        setMathMl.Inlines.Add(new Run { Text = "RichEditBox", FontWeight = FontWeights.SemiBold });
        setMathMl.Inlines.Add(new Run { Text = ". It replaces any existing equation with the new one." });
        MathMlDescriptionRichTextBlock.Blocks.Add(setMathMl);

        var getMathMl = new Paragraph();
        getMathMl.Inlines.Add(new Run { Text = "GetMathML", FontWeight = FontWeights.SemiBold });
        getMathMl.Inlines.Add(new Run { Text = " retrieves the MathML string of the equation from the " });
        getMathMl.Inlines.Add(new Run { Text = "RichEditBox", FontWeight = FontWeights.SemiBold });
        getMathMl.Inlines.Add(new Run { Text = "." });
        MathMlDescriptionRichTextBlock.Blocks.Add(getMathMl);
    }

    private static Paragraph ParagraphWithRuns(string text)
    {
        var paragraph = new Paragraph();
        paragraph.Inlines.Add(new Run { Text = text });
        return paragraph;
    }

    private static bool HasText(LeXtudio.UI.Xaml.Controls.RichTextBlock richTextBlock)
    {
        foreach (var block in richTextBlock.Blocks)
        {
            if (block is Paragraph paragraph && HasText(paragraph))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasText(Paragraph paragraph)
    {
        foreach (var inline in paragraph.Inlines)
        {
            if (HasText(inline))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasText(Inline inline)
    {
        if (inline is Run run)
        {
            return !string.IsNullOrWhiteSpace(run.Text);
        }

        if (inline is Span span)
        {
            foreach (var child in span.Inlines)
            {
                if (HasText(child))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void HighlightColorCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Get color to use
        var selectedItem = (sender as ComboBox)?.SelectedItem as ComboBoxItem;
        var color = Colors.Yellow;
        switch (selectedItem?.Content as string)
        {
            case "Yellow":
                color = Colors.Yellow;
                break;
            case "Red":
                color = Colors.Red;
                break;
            case "Blue":
                color = Colors.Blue;
                break;
        }

        // Get text range and highlighter
        TextRange textRange = new TextRange()
        {
            StartIndex = 28,
            Length = 11
        };
        TextHighlighter highlighter = new TextHighlighter()
        {
            Background = new SolidColorBrush(color),
            Ranges = { textRange }
        };

        // Switch texthighlighter
        TextHighlightingRichTextBlock.TextHighlighters.Clear();
        TextHighlightingRichTextBlock.TextHighlighters.Add(highlighter);
    }

    private void Menu_Opening(object? sender, object e)
    {
        CommandBarFlyout? myFlyout = sender as CommandBarFlyout;
        if (myFlyout != null && myFlyout.Target == REBCustom)
        {
            AppBarButton myButton = new AppBarButton
            {
                Command = new StandardUICommand(StandardUICommandKind.Share)
            };
            myFlyout.PrimaryCommands.Add(myButton);
        }
    }

    private void REBCustom_Loaded(object sender, RoutedEventArgs e)
    {
        if (REBCustom.SelectionFlyout is CommandBarFlyout selectionFlyout)
        {
            selectionFlyout.Opening += Menu_Opening;
        }

        if (REBCustom.ContextFlyout is CommandBarFlyout contextFlyout)
        {
            contextFlyout.Opening += Menu_Opening;
        }
    }

    private void REBCustom_Unloaded(object sender, RoutedEventArgs e)
    {
        if (REBCustom.SelectionFlyout is CommandBarFlyout selectionFlyout)
        {
            selectionFlyout.Opening -= Menu_Opening;
        }

        if (REBCustom.ContextFlyout is CommandBarFlyout contextFlyout)
        {
            contextFlyout.Opening -= Menu_Opening;
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.SetText(TextSetOptions.None, "Open file is wired here to mirror WinUI Gallery; file picker plumbing is platform-specific in this Uno sample.");
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.GetText(TextGetOptions.None, out _);
    }

    private void BoldButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        UpdateEditorToolbarState();
    }

    private void ItalicButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        UpdateEditorToolbarState();
    }

    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        // Extract the color of the button that was clicked.
        Button clickedColor = (Button)sender;
        var rectangle = (Rectangle)clickedColor.Content;
        var color = ((SolidColorBrush)rectangle.Fill).Color;

        editor.Document.Selection.CharacterFormat.ForegroundColor = color;

        fontColorButton.Flyout.Hide();
        editor.Focus(FocusState.Keyboard);
        currentColor = color;
    }

    private void FindBoxHighlightMatches(object sender, RoutedEventArgs e)
    {
        FindBoxHighlightMatches();
    }

    private void FindBoxRemoveHighlights(object sender, RoutedEventArgs e)
    {
        FindBoxRemoveHighlights();
    }

    private void FindBoxHighlightMatches()
    {
        FindBoxRemoveHighlights();

        var highlightBackgroundColor = Colors.DodgerBlue;
        var highlightForegroundColor = Colors.White;

        string textToFind = findBox.Text;
        if (textToFind != null)
        {
            var searchRange = editor.Document.GetRange(0, 0);
            while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
            }
        }
    }

    private void FindBoxRemoveHighlights()
    {
        if (editor.Background is not SolidColorBrush defaultBackground ||
            editor.Foreground is not SolidColorBrush defaultForeground)
        {
            return;
        }

        var documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
        documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    }

    private void Editor_GotFocus(object sender, RoutedEventArgs e)
    {
        editor.Document.GetText(TextGetOptions.UseCrlf, out _);

        var documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        if (editor.Background is SolidColorBrush background)
        {
            documentRange.CharacterFormat.BackgroundColor = background.Color;
        }

        UpdateEditorToolbarState();
    }

    private void Editor_TextChanged(object sender, RoutedEventArgs e)
    {
        if (editor.Document.Selection.CharacterFormat.ForegroundColor != currentColor)
        {
            editor.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
        }

        UpdateEditorToolbarState();
    }

    private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
    {
        UpdateEditorToolbarState();
    }

    private void UpdateEditorToolbarState()
    {
        var format = editor.Document.Selection.CharacterFormat;
        boldButton.IsChecked = ToNullableBool(format.Bold);
        italicButton.IsChecked = ToNullableBool(format.Italic);
    }

    private static bool? ToNullableBool(FormatEffect effect)
        => effect switch
        {
            FormatEffect.On => true,
            FormatEffect.Off => false,
            _ => null,
        };

    private void mathEditor2_TextChanged(object sender, RoutedEventArgs e)
    {
        var extractedMathML = mathEditor2.Document.GetMathML();
        MathmlPresenter.Text = !string.IsNullOrEmpty(extractedMathML)
            ? FormatMathML(extractedMathML)
            : "<!-- No MathML content -->";
    }

    private void SetMathmlFormulaBtn_Click(object sender, RoutedEventArgs e)
    {
        string formulaMathML =
            "<mml:math xmlns:mml=\"http://www.w3.org/1998/Math/MathML\" display=\"block\">\r\n" +
            "  <mml:mi mathcolor=\"#000000\">x</mml:mi>\r\n" +
            "  <mml:mo mathcolor=\"#000000\">\u2208</mml:mo>\r\n" +
            "  <mml:mi mathcolor=\"#000000\">P</mml:mi>\r\n" +
            "  <mml:mfenced>\r\n" +
            "    <mml:mrow>\r\n" +
            "      <mml:mi mathcolor=\"#000000\">A</mml:mi>\r\n" +
            "    </mml:mrow>\r\n" +
            "  </mml:mfenced>\r\n" +
            "  <mml:mo mathcolor=\"#000000\">\u2194</mml:mo>\r\n" +
            "  <mml:mi mathcolor=\"#000000\">x</mml:mi>\r\n" +
            "  <mml:mo mathcolor=\"#000000\">\u2286</mml:mo>\r\n" +
            "  <mml:mi mathcolor=\"#000000\">A</mml:mi>\r\n" +
            "</mml:math>";

        mathEditor2.Document.SetMathML(formulaMathML);
    }

    private static string FormatMathML(string mathML)
    {
        try
        {
            XDocument doc = XDocument.Parse(mathML);
            return doc.ToString();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error formatting MathML: {ex.Message}");
            return mathML;
        }
    }

    private void LiveBoldButton_Click(object sender, RoutedEventArgs e)
    {
        LiveRichEditBox.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        UpdateLiveRichEditBoxSnapshot();
        LiveRichEditBox.Focus(FocusState.Keyboard);
    }

    private void LiveItalicButton_Click(object sender, RoutedEventArgs e)
    {
        LiveRichEditBox.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        UpdateLiveRichEditBoxSnapshot();
        LiveRichEditBox.Focus(FocusState.Keyboard);
    }

    private void LiveUnderlineButton_Click(object sender, RoutedEventArgs e)
    {
        LiveRichEditBox.ToggleSelectionUnderline();
        UpdateLiveRichEditBoxSnapshot();
        LiveRichEditBox.Focus(FocusState.Keyboard);
    }

    private void LiveRichEditBox_StateChanged(object sender, RoutedEventArgs e)
    {
        UpdateLiveRichEditBoxSnapshot();
    }

    private void UpdateLiveRichEditBoxSnapshot()
    {
        LiveRichEditBox.Document.GetText(TextGetOptions.None, out var text);
        text ??= string.Empty;

        var selection = LiveRichEditBox.Document.Selection;
        var format = selection.CharacterFormat;
        var runs = LiveRichEditBox.Document.GetCharacterFormatRuns()
            .Select(run => $"{run.Start}..{run.End}:B={ShortFormat(run.Format.Bold)},I={ShortFormat(run.Format.Italic)},U={run.Format.Underline}")
            .ToArray();

        LiveRichEditBoxSnapshot.Text =
            $"selection: {selection.StartPosition}..{selection.EndPosition}\n" +
            $"format: B={ShortFormat(format.Bold)}, I={ShortFormat(format.Italic)}, U={format.Underline}\n" +
            $"text: {EscapeSnapshotText(text)}\n" +
            $"runs: {(runs.Length == 0 ? "(none)" : string.Join("; ", runs))}";
    }

    private static string ShortFormat(FormatEffect effect)
        => effect switch
        {
            FormatEffect.On => "on",
            FormatEffect.Off => "off",
            FormatEffect.Toggle => "toggle",
            _ => effect.ToString(),
        };

    private static string EscapeSnapshotText(string text)
        => "\"" + text.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"") + "\"";

    private void SelectAllWpfRichTextBox_Click(object sender, RoutedEventArgs e)
    {
        _wpfRichTextBox?.SelectAll();
        UpdateWpfRichTextBoxSelectionStatus();
    }

    private void CopyWpfRichTextBoxSelection_Click(object sender, RoutedEventArgs e)
    {
        _wpfRichTextBox?.Copy();
    }

    private void LiveWpfRichTextBox_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        LogWpfSelection("LiveWpfRichTextBox_SelectionChanged");
        UpdateWpfRichTextBoxSelectionStatus();
    }

    private void UpdateWpfRichTextBoxSelectionStatus()
    {
        var selectedText = _wpfRichTextBox?.Selection?.Text ?? string.Empty;
        WpfRichTextBoxSelectionStatus.Text = string.IsNullOrEmpty(selectedText)
            ? "Selection: none"
            : $"Selection: {selectedText}";
        LogWpfSelection("UpdateWpfRichTextBoxSelectionStatus");
    }

    private void LogWpfSelection(string prefix)
    {
        try
        {
            var selection = _wpfRichTextBox?.Selection;
            var text = selection?.Text ?? string.Empty;
            var escaped = text
                .Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
            var label = WpfRichTextBoxSelectionStatus?.Text ?? string.Empty;
            var document = _wpfRichTextBox?.Document;
            int? startOffset = null;
            int? endOffset = null;

            if (selection is not null && document is not null)
            {
                startOffset = document.ContentStart.GetOffsetToPosition(selection.Start);
                endOffset = document.ContentStart.GetOffsetToPosition(selection.End);
            }

            System.IO.File.AppendAllText(
                _wpfSelectionLogPath,
                $"{DateTime.Now:HH:mm:ss.fff}  [Sample] {prefix}: start={startOffset} end={endOffset} empty={selection?.IsEmpty} text=\"{escaped}\" label=\"{label}\"\n");
        }
        catch
        {
        }
    }
}

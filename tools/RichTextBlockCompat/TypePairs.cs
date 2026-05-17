using System.Collections.Generic;

namespace RichTextBlockCompat;

/// <summary>
/// Pairs of (WinUI 3 reference type full name, LeXtudio subject type full name) that the
/// parity check covers. Types are resolved at run-time via the metadata load context.
/// </summary>
public static class TypePairs
{
    public static IReadOnlyList<TypePair> All { get; } = new[]
    {
        // The controls
        new TypePair(
            "Microsoft.UI.Xaml.Controls.RichTextBlock",
            "LeXtudio.UI.Xaml.Controls.RichTextBlock"),
        new TypePair(
            "Microsoft.UI.Xaml.Controls.RichTextBlockOverflow",
            "LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow"),
        new TypePair(
            "Microsoft.UI.Xaml.Controls.RichEditBox",
            "LeXtudio.UI.Xaml.Controls.RichEditBox"),

        // Document model — block types
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Block",
            "System.Windows.Documents.Block"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Paragraph",
            "System.Windows.Documents.Paragraph"),

        // Document model — inline types
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Inline",
            "System.Windows.Documents.Inline"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Run",
            "System.Windows.Documents.Run"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Span",
            "System.Windows.Documents.Span"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Bold",
            "System.Windows.Documents.Bold"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Italic",
            "System.Windows.Documents.Italic"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Underline",
            "System.Windows.Documents.Underline"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.Hyperlink",
            "System.Windows.Documents.Hyperlink"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.LineBreak",
            "System.Windows.Documents.LineBreak"),
        new TypePair(
            "Microsoft.UI.Xaml.Documents.InlineUIContainer",
            "System.Windows.Documents.InlineUIContainer"),

        // RichEditBox text-document model (Microsoft.UI.Text.*)
        new TypePair(
            "Microsoft.UI.Text.ITextDocument",
            "Microsoft.UI.Text.ITextDocument"),
        new TypePair(
            "Microsoft.UI.Text.RichEditTextDocument",
            "Microsoft.UI.Text.RichEditTextDocument"),
        new TypePair(
            "Microsoft.UI.Text.ITextRange",
            "Microsoft.UI.Text.ITextRange"),
        new TypePair(
            "Microsoft.UI.Text.ITextSelection",
            "Microsoft.UI.Text.ITextSelection"),
        new TypePair(
            "Microsoft.UI.Text.ITextCharacterFormat",
            "Microsoft.UI.Text.ITextCharacterFormat"),
        new TypePair(
            "Microsoft.UI.Text.ITextParagraphFormat",
            "Microsoft.UI.Text.ITextParagraphFormat"),
    };
}

public sealed record TypePair(string WinUITypeName, string LocalTypeName);

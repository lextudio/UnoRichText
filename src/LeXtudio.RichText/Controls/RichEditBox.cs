// Skeleton implementation of RichEditBox targeting WinUI 3 API parity.
//
// This file establishes the dependency-property, event, and method surface so the
// compat tool can measure parity. Real behavior is layered in across the editing,
// IME, clipboard, and rendering pipelines — most of which reuse code from sibling
// repos (TextCore.Uno, UnoEdit, WindowsShims). See docs/DESIGN.md and
// docs/COMPAT-PLAN.md for the source-reuse map.

using System;
using Microsoft.UI.Text;
using LeXtudioTextDoc = LeXtudio.UI.Text.RichEditTextDocument;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;

namespace LeXtudio.UI.Xaml.Controls;

public partial class RichEditBox : ContentControl
{
    // ---- Dependency properties --------------------------------------------------

    public static DependencyProperty AcceptsReturnProperty { get; } =
        DependencyProperty.Register(nameof(AcceptsReturn), typeof(bool), typeof(RichEditBox), new PropertyMetadata(true));

    public static DependencyProperty CharacterCasingProperty { get; } =
        DependencyProperty.Register(nameof(CharacterCasing), typeof(CharacterCasing), typeof(RichEditBox), new PropertyMetadata(CharacterCasing.Normal));

    public static DependencyProperty ClipboardCopyFormatProperty { get; } =
        DependencyProperty.Register(nameof(ClipboardCopyFormat), typeof(RichEditClipboardFormat), typeof(RichEditBox), new PropertyMetadata(RichEditClipboardFormat.AllFormats));

    public static DependencyProperty ContentLinkBackgroundColorProperty { get; } =
        DependencyProperty.Register(nameof(ContentLinkBackgroundColor), typeof(SolidColorBrush), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty ContentLinkForegroundColorProperty { get; } =
        DependencyProperty.Register(nameof(ContentLinkForegroundColor), typeof(SolidColorBrush), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty CornerRadiusProperty { get; } =
        DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(RichEditBox), new PropertyMetadata(default(CornerRadius)));

    public static DependencyProperty DescriptionProperty { get; } =
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty DesiredCandidateWindowAlignmentProperty { get; } =
        DependencyProperty.Register(nameof(DesiredCandidateWindowAlignment), typeof(CandidateWindowAlignment), typeof(RichEditBox), new PropertyMetadata(CandidateWindowAlignment.Default));

    public static DependencyProperty DisabledFormattingAcceleratorsProperty { get; } =
        DependencyProperty.Register(nameof(DisabledFormattingAccelerators), typeof(DisabledFormattingAccelerators), typeof(RichEditBox), new PropertyMetadata(DisabledFormattingAccelerators.None));

    public static DependencyProperty HeaderProperty { get; } =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty HeaderTemplateProperty { get; } =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty HorizontalTextAlignmentProperty { get; } =
        DependencyProperty.Register(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(RichEditBox), new PropertyMetadata(TextAlignment.Left));

    public static DependencyProperty InputScopeProperty { get; } =
        DependencyProperty.Register(nameof(InputScope), typeof(InputScope), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty IsColorFontEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsColorFontEnabled), typeof(bool), typeof(RichEditBox), new PropertyMetadata(true));

    public static DependencyProperty IsHandwritingViewEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsHandwritingViewEnabled), typeof(bool), typeof(RichEditBox), new PropertyMetadata(true));

    public static DependencyProperty IsReadOnlyProperty { get; } =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RichEditBox), new PropertyMetadata(false, OnIsReadOnlyChanged));

    public static DependencyProperty IsSpellCheckEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsSpellCheckEnabled), typeof(bool), typeof(RichEditBox), new PropertyMetadata(true));

    public static DependencyProperty IsTextPredictionEnabledProperty { get; } =
        DependencyProperty.Register(nameof(IsTextPredictionEnabled), typeof(bool), typeof(RichEditBox), new PropertyMetadata(true));

    public static DependencyProperty MaxLengthProperty { get; } =
        DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(RichEditBox), new PropertyMetadata(0));

    public static DependencyProperty PlaceholderTextProperty { get; } =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(RichEditBox), new PropertyMetadata(string.Empty));

    public static DependencyProperty PlaceholderForegroundProperty { get; } =
        DependencyProperty.Register(nameof(PlaceholderForeground), typeof(Brush), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty PreventKeyboardDisplayOnProgrammaticFocusProperty { get; } =
        DependencyProperty.Register(nameof(PreventKeyboardDisplayOnProgrammaticFocus), typeof(bool), typeof(RichEditBox), new PropertyMetadata(false));

    public static DependencyProperty ProofingMenuFlyoutProperty { get; } =
        DependencyProperty.Register(nameof(ProofingMenuFlyout), typeof(FlyoutBase), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty SelectionFlyoutProperty { get; } =
        DependencyProperty.Register(nameof(SelectionFlyout), typeof(FlyoutBase), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty SelectionHighlightColorProperty { get; } =
        DependencyProperty.Register(nameof(SelectionHighlightColor), typeof(SolidColorBrush), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty SelectionHighlightColorWhenNotFocusedProperty { get; } =
        DependencyProperty.Register(nameof(SelectionHighlightColorWhenNotFocused), typeof(SolidColorBrush), typeof(RichEditBox), new PropertyMetadata(null));

    public static DependencyProperty TextAlignmentProperty { get; } =
        DependencyProperty.Register(nameof(TextAlignment), typeof(TextAlignment), typeof(RichEditBox), new PropertyMetadata(TextAlignment.Left));

    public static DependencyProperty TextReadingOrderProperty { get; } =
        DependencyProperty.Register(nameof(TextReadingOrder), typeof(TextReadingOrder), typeof(RichEditBox), new PropertyMetadata(TextReadingOrder.DetectFromContent));

    public static DependencyProperty TextWrappingProperty { get; } =
        DependencyProperty.Register(nameof(TextWrapping), typeof(TextWrapping), typeof(RichEditBox), new PropertyMetadata(TextWrapping.Wrap));

    // ---- Property accessors -----------------------------------------------------

    public bool AcceptsReturn { get => (bool)GetValue(AcceptsReturnProperty); set => SetValue(AcceptsReturnProperty, value); }
    public CharacterCasing CharacterCasing { get => (CharacterCasing)GetValue(CharacterCasingProperty); set => SetValue(CharacterCasingProperty, value); }
    public RichEditClipboardFormat ClipboardCopyFormat { get => (RichEditClipboardFormat)GetValue(ClipboardCopyFormatProperty); set => SetValue(ClipboardCopyFormatProperty, value); }
    public SolidColorBrush? ContentLinkBackgroundColor { get => (SolidColorBrush?)GetValue(ContentLinkBackgroundColorProperty); set => SetValue(ContentLinkBackgroundColorProperty, value); }
    public SolidColorBrush? ContentLinkForegroundColor { get => (SolidColorBrush?)GetValue(ContentLinkForegroundColorProperty); set => SetValue(ContentLinkForegroundColorProperty, value); }
    public new CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public object? Description { get => GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }
    public CandidateWindowAlignment DesiredCandidateWindowAlignment { get => (CandidateWindowAlignment)GetValue(DesiredCandidateWindowAlignmentProperty); set => SetValue(DesiredCandidateWindowAlignmentProperty, value); }
    public DisabledFormattingAccelerators DisabledFormattingAccelerators { get => (DisabledFormattingAccelerators)GetValue(DisabledFormattingAcceleratorsProperty); set => SetValue(DisabledFormattingAcceleratorsProperty, value); }
    /// <summary>
    /// The text document model. WinUI 3 types this as <c>Microsoft.UI.Text.RichEditTextDocument</c>;
    /// we expose <c>LeXtudio.UI.Text.RichEditTextDocument</c> because Uno's concrete class is
    /// majority-stubbed and non-virtual. Cast to <c>Microsoft.UI.Text.ITextDocument</c> for the
    /// WinUI-shape API. See docs/DESIGN.md "Document model for RichEditBox".
    /// </summary>
    public LeXtudioTextDoc Document { get; }
    public object? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public DataTemplate? HeaderTemplate { get => (DataTemplate?)GetValue(HeaderTemplateProperty); set => SetValue(HeaderTemplateProperty, value); }
    public TextAlignment HorizontalTextAlignment { get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty); set => SetValue(HorizontalTextAlignmentProperty, value); }
    public InputScope? InputScope { get => (InputScope?)GetValue(InputScopeProperty); set => SetValue(InputScopeProperty, value); }
    public bool IsColorFontEnabled { get => (bool)GetValue(IsColorFontEnabledProperty); set => SetValue(IsColorFontEnabledProperty, value); }
    public bool IsHandwritingViewEnabled { get => (bool)GetValue(IsHandwritingViewEnabledProperty); set => SetValue(IsHandwritingViewEnabledProperty, value); }
    public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public bool IsSpellCheckEnabled { get => (bool)GetValue(IsSpellCheckEnabledProperty); set => SetValue(IsSpellCheckEnabledProperty, value); }
    public bool IsTextPredictionEnabled { get => (bool)GetValue(IsTextPredictionEnabledProperty); set => SetValue(IsTextPredictionEnabledProperty, value); }
    public int MaxLength { get => (int)GetValue(MaxLengthProperty); set => SetValue(MaxLengthProperty, value); }
    public string PlaceholderText { get => (string)GetValue(PlaceholderTextProperty); set => SetValue(PlaceholderTextProperty, value); }
    public Brush? PlaceholderForeground { get => (Brush?)GetValue(PlaceholderForegroundProperty); set => SetValue(PlaceholderForegroundProperty, value); }
    public bool PreventKeyboardDisplayOnProgrammaticFocus { get => (bool)GetValue(PreventKeyboardDisplayOnProgrammaticFocusProperty); set => SetValue(PreventKeyboardDisplayOnProgrammaticFocusProperty, value); }
    public FlyoutBase? ProofingMenuFlyout { get => (FlyoutBase?)GetValue(ProofingMenuFlyoutProperty); set => SetValue(ProofingMenuFlyoutProperty, value); }
    public FlyoutBase? SelectionFlyout { get => (FlyoutBase?)GetValue(SelectionFlyoutProperty); set => SetValue(SelectionFlyoutProperty, value); }
    public SolidColorBrush? SelectionHighlightColor { get => (SolidColorBrush?)GetValue(SelectionHighlightColorProperty); set => SetValue(SelectionHighlightColorProperty, value); }
    public SolidColorBrush? SelectionHighlightColorWhenNotFocused { get => (SolidColorBrush?)GetValue(SelectionHighlightColorWhenNotFocusedProperty); set => SetValue(SelectionHighlightColorWhenNotFocusedProperty, value); }
    public TextAlignment TextAlignment { get => (TextAlignment)GetValue(TextAlignmentProperty); set => SetValue(TextAlignmentProperty, value); }
    public TextReadingOrder TextReadingOrder { get => (TextReadingOrder)GetValue(TextReadingOrderProperty); set => SetValue(TextReadingOrderProperty, value); }
    public TextWrapping TextWrapping { get => (TextWrapping)GetValue(TextWrappingProperty); set => SetValue(TextWrappingProperty, value); }

    // ---- Events -----------------------------------------------------------------

    public event TypedEventHandler<RichEditBox, RichEditBoxTextCompositionStartedEventArgs>? TextCompositionStarted;
    public event TypedEventHandler<RichEditBox, RichEditBoxTextCompositionChangedEventArgs>? TextCompositionChanged;
    public event TypedEventHandler<RichEditBox, RichEditBoxTextCompositionEndedEventArgs>? TextCompositionEnded;
    public event TypedEventHandler<RichEditBox, CandidateWindowBoundsChangedEventArgs>? CandidateWindowBoundsChanged;
    public event RoutedEventHandler? SelectionChanged;
    public event TypedEventHandler<RichEditBox, RichEditBoxSelectionChangingEventArgs>? SelectionChanging;
    public event TextControlPasteEventHandler? Paste;
    public event TypedEventHandler<RichEditBox, TextControlCopyingToClipboardEventArgs>? CopyingToClipboard;
    public event TypedEventHandler<RichEditBox, TextControlCuttingToClipboardEventArgs>? CuttingToClipboard;
    // (Note: ContextMenuOpening is exposed at the FrameworkElement level via ContextFlyout)
    public event RoutedEventHandler? TextChanged;
    public event TypedEventHandler<RichEditBox, RichEditBoxTextChangingEventArgs>? TextChanging;

    // ---- Constructor ------------------------------------------------------------

    // Internal editing host — LeXtudio.UI.Controls.TextBox provides a real caret, selection,
    // keyboard, and a CoreTextEditContext-backed IME bridge that works on all Uno platforms.
    private readonly LeXtudio.UI.Controls.TextBox _editorHost;
    private bool _syncingFromDocument;

    public RichEditBox()
    {
        Document = new LeXtudioTextDoc();

        _editorHost = new LeXtudio.UI.Controls.TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        // editor → Document
        _editorHost.TextChanged += OnEditorHostTextChanged;
        _editorHost.SelectionChanged += OnEditorHostSelectionChanged;

        // Document → editor (when consumer calls Document.SetText programmatically)
        Document.TextChanged += OnDocumentTextChanged;

        Content = _editorHost;

        // RichEditBox itself should fill whatever its parent gives it.
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        // Propagate the WinUI-shape RichEditBox properties to the underlying host.
        RegisterPropertyChangedCallback(PlaceholderTextProperty, (_, _) => _editorHost.PlaceholderText = PlaceholderText ?? string.Empty);
        RegisterPropertyChangedCallback(AcceptsReturnProperty, (_, _) => _editorHost.AcceptsReturn = AcceptsReturn);
        RegisterPropertyChangedCallback(TextWrappingProperty, (_, _) => _editorHost.TextWrapping = TextWrapping);
        RegisterPropertyChangedCallback(HeaderProperty, (_, _) => _editorHost.Header = Header);

        // Seed the host with the current property values (so they apply before any change).
        _editorHost.PlaceholderText = PlaceholderText ?? string.Empty;
    }

    /// <summary>Exposes the underlying editor host so RichEditBox.Selection-style helpers can poke into it.</summary>
    internal LeXtudio.UI.Controls.TextBox EditorHost => _editorHost;

    /// <summary>
    /// Wraps the current selection (or inserts at the caret) with <paramref name="prefix"/> /
    /// <paramref name="suffix"/>. Used by the editor toolbar for bold / italic / underline /
    /// code-span markdown-style formatting on a plain-text backing.
    /// </summary>
    public void WrapSelection(string prefix, string suffix) => _editorHost.WrapSelection(prefix, suffix);

    /// <summary>Inserts <paramref name="text"/> at the caret (or replaces the selection).</summary>
    public void InsertText(string text) => _editorHost.ReplaceSelection(text);

    /// <summary>Replaces the current selection with text after applying a transform.</summary>
    public void TransformSelectedText(System.Func<string, string> transform)
    {
        if (transform is null) return;
        _editorHost.ReplaceSelection(transform(_editorHost.SelectedText ?? string.Empty));
    }

    private void OnDocumentTextChanged(object? sender, System.EventArgs e)
    {
        if (_syncingFromDocument) return;
        _syncingFromDocument = true;
        try
        {
            Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var s);
            if (_editorHost.Text != s) _editorHost.Text = s ?? string.Empty;
        }
        finally { _syncingFromDocument = false; }
    }

    private void OnEditorHostTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_syncingFromDocument) return;
        _syncingFromDocument = true;
        try
        {
            Document.SetText(Microsoft.UI.Text.TextSetOptions.None, _editorHost.Text);
            RaiseTextChanged(new RoutedEventArgs());
        }
        finally { _syncingFromDocument = false; }
    }

    private void OnEditorHostSelectionChanged(object sender, RoutedEventArgs e)
    {
        int start = _editorHost.SelectionStart;
        int length = _editorHost.SelectionLength;
        Document.Selection.SetRange(start, start + length);
        RaiseSelectionChanged(e);
    }

    // ---- Methods (parity-shaped) ------------------------------------------------

    /// <summary>Selects all the content in the RichEditBox.</summary>
    public void SelectAll()
    {
        Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var current);
        Document.Selection.SetRange(0, current?.Length ?? 0);
    }

    /// <summary>Returns the linguistic alternatives for the word at the current selection.</summary>
    public RichEditTextRangeAlternatives GetLinguisticAlternativesAsync() => new();

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // TODO: update visual state, gate input.
    }

    internal void RaiseSelectionChanged(RoutedEventArgs e) => SelectionChanged?.Invoke(this, e);
    internal void RaiseSelectionChanging(RichEditBoxSelectionChangingEventArgs e) => SelectionChanging?.Invoke(this, e);
    internal void RaiseTextChanged(RoutedEventArgs e) => TextChanged?.Invoke(this, e);
    internal void RaiseTextChanging(RichEditBoxTextChangingEventArgs e) => TextChanging?.Invoke(this, e);
    internal void RaisePaste(TextControlPasteEventArgs e) => Paste?.Invoke(this, e);
    internal void RaiseCopyingToClipboard(TextControlCopyingToClipboardEventArgs e) => CopyingToClipboard?.Invoke(this, e);
    internal void RaiseCuttingToClipboard(TextControlCuttingToClipboardEventArgs e) => CuttingToClipboard?.Invoke(this, e);
    // ContextMenuOpening intentionally omitted — handled via FrameworkElement.ContextFlyout in WinUI 3.
    internal void RaiseTextCompositionStarted(RichEditBoxTextCompositionStartedEventArgs e) => TextCompositionStarted?.Invoke(this, e);
    internal void RaiseTextCompositionChanged(RichEditBoxTextCompositionChangedEventArgs e) => TextCompositionChanged?.Invoke(this, e);
    internal void RaiseTextCompositionEnded(RichEditBoxTextCompositionEndedEventArgs e) => TextCompositionEnded?.Invoke(this, e);
    internal void RaiseCandidateWindowBoundsChanged(CandidateWindowBoundsChangedEventArgs e) => CandidateWindowBoundsChanged?.Invoke(this, e);
}

// ---- Support types (event args + enums + small structs) -----------------------

public enum RichEditClipboardFormat
{
    AllFormats = 0,
    PlainText = 1,
}

public enum CandidateWindowAlignment
{
    Default = 0,
    BottomEdge = 1,
}

[System.Flags]
public enum DisabledFormattingAccelerators : uint
{
    None = 0,
    Bold = 1,
    Italic = 2,
    Underline = 4,
    All = 0xFFFFFFFF,
}

public sealed class RichEditBoxTextChangingEventArgs
{
    public bool IsContentChanging { get; internal set; }
}

public sealed class RichEditBoxSelectionChangingEventArgs
{
    public int SelectionStart { get; internal set; }
    public int SelectionLength { get; internal set; }
    public bool Cancel { get; set; }
}

public sealed class RichEditBoxTextCompositionStartedEventArgs
{
    public int StartIndex { get; internal set; }
    public int Length { get; internal set; }
}

public sealed class RichEditBoxTextCompositionChangedEventArgs
{
    public int StartIndex { get; internal set; }
    public int Length { get; internal set; }
}

public sealed class RichEditBoxTextCompositionEndedEventArgs
{
    public int StartIndex { get; internal set; }
    public int Length { get; internal set; }
}

public sealed class CandidateWindowBoundsChangedEventArgs
{
    public Rect Bounds { get; internal set; }
}

public sealed class RichEditTextRangeAlternatives
{
    public System.Collections.Generic.IList<string> Alternatives { get; } = new System.Collections.Generic.List<string>();
}

// Skeleton implementation of RichEditBox targeting WinUI 3 API parity.
//
// This file establishes the dependency-property, event, and method surface so the
// compat tool can measure parity. Real behavior is layered in across the editing,
// IME, clipboard, and rendering pipelines — most of which reuse code from sibling
// repos (TextCore.Uno, UnoEdit, WindowsShims). See docs/DESIGN.md and
// docs/COMPAT-PLAN.md for the source-reuse map.

using System;
using System.IO;
using System.Linq;
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
using LeXtudio.UI.Controls;
using FlowDocuments = System.Windows.Documents;
using InputScope = Microsoft.UI.Xaml.Input.InputScope;

namespace LeXtudio.UI.Xaml.Controls;

#if !WINDOWS_APP_SDK
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
    private readonly RichTextBlock _renderOverlay;
    private bool _syncingFromDocument;
    private bool _editorHostFocused;
    private static readonly string s_diagnosticLogPath = Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.RichEditBox.log");

    public static bool DiagnosticsEnabled { get; set; }

    public RichEditBox()
    {
        Document = new LeXtudioTextDoc();

        _editorHost = new LeXtudio.UI.Controls.TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            // Stretch during arrange, but keep Auto-sized measurement content
            // driven. That lets plain RichEditBox start as a single line while
            // explicit Height/star-row gallery cases fill their allotted slot.
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            IsReadOnly = IsReadOnly,
        };

        // Disable the inner ScrollViewer so the host's measured size always
        // tracks content (single line at first, growing as more lines are
        // added). Without this Uno's TextBox would show internal scrollbars
        // after the first overflow and stop reporting a taller measured size.
        //
        // Leave VerticalAlignment alone (Stretch by default): in an Auto-
        // sized parent slot, Stretch measures to content; in a bounded slot
        // (explicit Height, star row), Stretch fills. That dual behavior is
        // exactly what WinUI's RichEditBox does, so `<RichEditBox/>` in a
        // StackPanel grows line-by-line while `<RichEditBox Height="200"/>`
        // stays 200 tall.
        ScrollViewer.SetVerticalScrollMode(_editorHost.InnerTextBox, ScrollMode.Disabled);
        ScrollViewer.SetVerticalScrollBarVisibility(_editorHost.InnerTextBox, ScrollBarVisibility.Disabled);
        ScrollViewer.SetHorizontalScrollMode(_editorHost.InnerTextBox, ScrollMode.Disabled);
        ScrollViewer.SetHorizontalScrollBarVisibility(_editorHost.InnerTextBox, ScrollBarVisibility.Disabled);

        _renderOverlay = new RichTextBlock
        {
            IsHitTestVisible = false,
            Padding = new Thickness(8, 6, 8, 6),
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Visibility = Visibility.Collapsed,
        };

        // editor → Document
        _editorHost.TextChanged += OnEditorHostTextChanged;
        _editorHost.SelectionChanged += OnEditorHostSelectionChanged;
        _editorHost.FormattingAcceleratorRequested += OnEditorHostFormattingAcceleratorRequested;
        _editorHost.EditingCommandRequested += OnEditorHostEditingCommandRequested;

        // Track host focus so the overlay caret shows/hides correctly.
        _editorHost.GotFocus += (_, _) => { _editorHostFocused = true; SyncOverlayCaret(); };
        _editorHost.LostFocus += (_, _) => { _editorHostFocused = false; SyncOverlayCaret(); };

        // Document → editor (when consumer calls Document.SetText programmatically)
        Document.TextChanged += OnDocumentTextChanged;
        Document.FormattingChanged += OnDocumentFormattingChanged;

        Content = new Grid
        {
            Children =
            {
                _editorHost,
                _renderOverlay,
            }
        };

        // Default flyouts mirror WinUI's RichEditBox: a CommandBarFlyout with
        // Bold/Italic/Underline on the primary strip and Cut/Copy/Paste/Undo/
        // Redo/Select All in the secondary list. Consumers can replace either
        // DP if they want a custom menu. The same flyout is shown via right-
        // click regardless of selection state — Bold/Italic/Underline and
        // Paste stay available on an empty box, matching WinUI Gallery.
        ContextFlyout = RichEditBoxCommandBarFlyoutFactory.Create(this);
        SelectionFlyout = RichEditBoxCommandBarFlyoutFactory.Create(this);

        // The platform TextBox installs its own default Paste/Select All
        // ContextFlyout that the right-click gesture targets directly. We have
        // to replace it on the inner control, not the shim, otherwise Uno's
        // default wins and our richer flyout is never reached.
        _editorHost.InnerTextBox.ContextFlyout = RichEditBoxCommandBarFlyoutFactory.Create(this);
        _editorHost.InnerTextBox.SelectionFlyout = RichEditBoxCommandBarFlyoutFactory.Create(this);

        // Preempt Uno's TextBox right-click logic by handling PointerPressed
        // at the root pointer level. handledEventsToo=true catches the event
        // even if a child already marked it handled. We mark Handled, run our
        // own context-menu logic from a posted continuation (so the inner
        // TextBox can still update caret position and selection first), and
        // then sweep away any popups Uno may have opened in parallel.
        _editorHost.InnerTextBox.AddHandler(
            RightTappedEvent,
            new RightTappedEventHandler(OnInnerTextBoxRightTapped),
            handledEventsToo: true);

        _editorHost.InnerTextBox.ContextRequested += OnInnerTextBoxContextRequested;

        // RichEditBox itself should fill whatever its parent gives it. This
        // does not change Auto-sized measurement, but it makes explicit
        // Height/star-row gallery cases arrange the editor to the full slot.
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

    private void OnEditorHostFormattingAcceleratorRequested(object? sender, TextFormattingAcceleratorRequestedEventArgs e)
    {
        LogDiagnostic($"FormattingAccelerator requested={e.Accelerator} selection={e.SelectionStart}+{e.SelectionLength} text={DescribeText(_editorHost.Text)} readOnly={IsReadOnly}");
        if (IsReadOnly)
        {
            // Read-only: swallow the accelerator so the host does not flip
            // formatting state on a non-editable surface. Stay consistent with
            // WinUI which simply ignores Ctrl+B/I/U on a read-only RichEditBox.
            e.Handled = true;
            return;
        }
        Document.Selection.SetRange(e.SelectionStart, e.SelectionStart + e.SelectionLength);
        LogDiagnostic($"FormattingAccelerator before selection={Document.Selection.StartPosition}..{Document.Selection.EndPosition} format={DescribeSelectionFormat()} runs={DescribeRuns()}");

        switch (e.Accelerator)
        {
            case TextFormattingAccelerator.Bold:
                Document.Selection.CharacterFormat.Bold = Microsoft.UI.Text.FormatEffect.Toggle;
                e.Handled = true;
                break;
            case TextFormattingAccelerator.Italic:
                Document.Selection.CharacterFormat.Italic = Microsoft.UI.Text.FormatEffect.Toggle;
                e.Handled = true;
                break;
            case TextFormattingAccelerator.Underline:
                ToggleSelectionUnderline();
                e.Handled = true;
                break;
        }
        LogDiagnostic($"FormattingAccelerator handled={e.Handled} selection={Document.Selection.StartPosition}..{Document.Selection.EndPosition} format={DescribeSelectionFormat()} runs={DescribeRuns()}");
    }

    private void OnEditorHostEditingCommandRequested(object? sender, TextEditingCommandRequestedEventArgs e)
    {
        LogDiagnostic($"EditingCommand requested={e.Command} canUndo={Document.CanUndo()} canRedo={Document.CanRedo()} text={DescribeText(_editorHost.Text)} runs={DescribeRuns()} readOnly={IsReadOnly}");
        if (IsReadOnly)
        {
            e.Handled = true;
            return;
        }
        switch (e.Command)
        {
            case TextEditingCommand.Undo when Document.CanUndo():
                Document.Undo();
                e.Handled = true;
                break;
            case TextEditingCommand.Redo when Document.CanRedo():
                Document.Redo();
                e.Handled = true;
                break;
        }

        LogDiagnostic($"EditingCommand handled={e.Handled} runs={DescribeRuns()}");
    }

    public void ToggleSelectionUnderline()
    {
        var format = Document.Selection.CharacterFormat;
        format.Underline = format.Underline == Microsoft.UI.Text.UnderlineType.None
            ? Microsoft.UI.Text.UnderlineType.Single
            : Microsoft.UI.Text.UnderlineType.None;
    }

    public void ToggleSelectionBold()
    {
        if (IsReadOnly) return;
        Document.Selection.CharacterFormat.Bold = Microsoft.UI.Text.FormatEffect.Toggle;
    }

    public void ToggleSelectionItalic()
    {
        if (IsReadOnly) return;
        Document.Selection.CharacterFormat.Italic = Microsoft.UI.Text.FormatEffect.Toggle;
    }

    public void CopySelection()
    {
        if (Document.Selection.Length <= 0) return;
        var text = _editorHost.SelectedText ?? string.Empty;
        if (text.Length == 0) return;
        var package = new global::Windows.ApplicationModel.DataTransfer.DataPackage();
        package.SetText(text);
        global::Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
    }

    public void CutSelection()
    {
        if (IsReadOnly || Document.Selection.Length <= 0) return;
        CopySelection();
        var start = Document.Selection.StartPosition;
        var end = Document.Selection.EndPosition;
        // Mutate through the host so the diff path produces one undo entry and
        // selection collapses naturally where the cut happened.
        _editorHost.ReplaceSelection(string.Empty);
        Document.Selection.SetRange(start, start);
        _ = end;
    }

    public void PasteFromClipboard()
    {
        if (IsReadOnly) return;
        var view = global::Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
        if (view is null || !view.Contains(global::Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
        {
            return;
        }

        string clipboardText;
        try
        {
            clipboardText = view.GetTextAsync().AsTask().GetAwaiter().GetResult() ?? string.Empty;
        }
        catch
        {
            return;
        }

        if (clipboardText.Length == 0) return;
        _editorHost.ReplaceSelection(clipboardText);
    }

    private void OnDocumentTextChanged(object? sender, System.EventArgs e)
    {
        LogDiagnostic($"DocumentTextChanged syncing={_syncingFromDocument} runs={DescribeRuns()}");
        if (_syncingFromDocument) return;
        _syncingFromDocument = true;
        try
        {
            Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var s);
            if (_editorHost.Text != s) _editorHost.Text = s ?? string.Empty;
            var textLength = _editorHost.Text?.Length ?? 0;
            var selectionStart = Math.Clamp(Document.Selection.StartPosition, 0, textLength);
            var selectionLength = Math.Clamp(Document.Selection.EndPosition - Document.Selection.StartPosition, 0, textLength - selectionStart);
            _editorHost.Select(selectionStart, selectionLength);
            RefreshRichRenderOverlay();
        }
        finally { _syncingFromDocument = false; }

        // Surface document-driven state changes (e.g. undo/redo, programmatic
        // edits) so external listeners — toolbars, snapshots — can refresh.
        RaiseTextChanged(new RoutedEventArgs());
    }

    private void OnDocumentFormattingChanged(object? sender, System.EventArgs e)
    {
        LogDiagnostic($"DocumentFormattingChanged runs={DescribeRuns()}");
        if (!_syncingFromDocument)
        {
            _syncingFromDocument = true;
            try
            {
                var textLength = _editorHost.Text?.Length ?? 0;
                var selectionStart = Math.Clamp(Document.Selection.StartPosition, 0, textLength);
                var selectionLength = Math.Clamp(Document.Selection.EndPosition - Document.Selection.StartPosition, 0, textLength - selectionStart);
                if (_editorHost.SelectionStart != selectionStart || _editorHost.SelectionLength != selectionLength)
                {
                    _editorHost.Select(selectionStart, selectionLength);
                }
            }
            finally { _syncingFromDocument = false; }
        }
        RefreshRichRenderOverlay();
    }

    private void OnEditorHostTextChanged(object sender, TextChangedEventArgs e)
    {
        LogDiagnostic($"EditorHostTextChanged syncing={_syncingFromDocument} hostText={DescribeText(_editorHost.Text)} runsBefore={DescribeRuns()}");
        if (_syncingFromDocument) return;
        _syncingFromDocument = true;
        try
        {
            Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var documentText);
            var oldText = documentText ?? string.Empty;
            var newText = _editorHost.Text ?? string.Empty;

            if (oldText != newText)
            {
                int prefixLength = 0;
                int maxPrefix = Math.Min(oldText.Length, newText.Length);
                while (prefixLength < maxPrefix && oldText[prefixLength] == newText[prefixLength])
                    prefixLength++;

                int oldRemainder = oldText.Length - prefixLength;
                int newRemainder = newText.Length - prefixLength;
                int suffixLength = 0;
                while (suffixLength < oldRemainder
                    && suffixLength < newRemainder
                    && oldText[oldText.Length - 1 - suffixLength] == newText[newText.Length - 1 - suffixLength])
                {
                    suffixLength++;
                }

                int deleteStart = prefixLength;
                int deleteEnd = oldText.Length - suffixLength;
                string insertedText = newText.Substring(prefixLength, newText.Length - prefixLength - suffixLength);

                LeXtudio.UI.Text.TextCharacterFormat? replacementFormat = null;
                if (deleteEnd > deleteStart)
                    replacementFormat = Document.GetCharacterFormat(deleteStart, deleteEnd);

                LogDiagnostic($"EditorHostTextChanged diff delete={deleteStart}..{deleteEnd} insert={DescribeText(insertedText)} replacementFormat={DescribeFormat(replacementFormat)} selectionBefore={Document.Selection.StartPosition}..{Document.Selection.EndPosition} selectionFormat={DescribeSelectionFormat()}");

                if (deleteEnd > deleteStart)
                    Document.DeleteRange(deleteStart, deleteEnd);

                if (insertedText.Length > 0)
                    Document.InsertText(deleteStart, insertedText, replacementFormat);
            }

            using (Document.PreserveCaretInputFormatScope())
                Document.Selection.SetRange(_editorHost.SelectionStart, _editorHost.SelectionStart + _editorHost.SelectionLength);
            RefreshRichRenderOverlay();
            LogDiagnostic($"EditorHostTextChanged after SetText runsAfter={DescribeRuns()} overlay={_renderOverlay.Visibility} opacity={_editorHost.Opacity}");
            // Arm retro-apply only for the duration of TextChanged dispatch so
            // a handler that runs `Selection.CharacterFormat.X = ...` colors the
            // just-typed characters (WinUI parity). Programmatic format edits
            // outside this window keep their normal "future input only" effect.
            using (Document.EnterRetroApplyScope())
                RaiseTextChanged(new RoutedEventArgs());
        }
        finally { _syncingFromDocument = false; }
    }

    private void OnInnerTextBoxContextRequested(UIElement sender, ContextRequestedEventArgs args)
    {
        try
        {
            File.AppendAllText(
                Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.Flyout.log"),
                $"{System.DateTimeOffset.Now:O} ContextRequested sender={sender?.GetType().Name} alreadyHandled={args.Handled}{System.Environment.NewLine}");
        }
        catch { }

        // Suppress Uno's built-in Paste/Select All menu and open ours instead,
        // so the gesture (mouse, touch hold, Shift+F10, Apps key) lands on one
        // and only one flyout.
        var flyout = ContextFlyout;
        if (flyout is null) return;

        global::Windows.Foundation.Point position;
        bool gotPosition = args.TryGetPosition(_editorHost.InnerTextBox, out position);

        args.Handled = true;

        try
        {
            var options = new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions
            {
                Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowMode.Standard,
            };
            if (gotPosition)
                options.Position = position;
            flyout.ShowAt(_editorHost.InnerTextBox, options);
        }
        catch (System.Exception ex)
        {
            try
            {
                File.AppendAllText(
                    Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.Flyout.log"),
                    $"{System.DateTimeOffset.Now:O} ContextRequested-ShowAt-failed ex={ex.Message}{System.Environment.NewLine}");
            }
            catch { }
        }
    }

    private void OnInnerTextBoxRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        // Always log so we can confirm this gesture actually fires regardless
        // of DiagnosticsEnabled — the flyout-render bug we are chasing here
        // depends on knowing whether the event even reaches us.
        try
        {
            File.AppendAllText(
                Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.Flyout.log"),
                $"{System.DateTimeOffset.Now:O} RightTapped sender={sender?.GetType().Name} hasContextFlyout={ContextFlyout is not null} hostContextFlyout={_editorHost.InnerTextBox.ContextFlyout is not null}{System.Environment.NewLine}");
        }
        catch { }

        // Mark handled so the platform TextBox doesn't show its own default
        // Paste/Select All flyout on top of ours.
        e.Handled = true;

        var flyout = ContextFlyout ?? _editorHost.InnerTextBox.ContextFlyout;
        if (flyout is null) return;

        // Snapshot the popup set BEFORE we ShowAt. Anything already open is
        // not ours and is presumed to be Uno's parallel default; we hide it
        // shortly after our flyout opens. Anything that appears between the
        // pre-snapshot and the post-snapshot is presumed to belong to our
        // CommandBarFlyout (it may create multiple Popups for primary +
        // overflow bands) and is left alone.
        var xamlRoot = XamlRoot;
        var preOpenPopups = new System.Collections.Generic.HashSet<Microsoft.UI.Xaml.Controls.Primitives.Popup>();
        if (xamlRoot is not null)
        {
            foreach (var p in Microsoft.UI.Xaml.Media.VisualTreeHelper.GetOpenPopupsForXamlRoot(xamlRoot))
                if (p is not null) preOpenPopups.Add(p);
        }

        try
        {
            // Target the RichEditBox itself (not the inner TextBox) so that
            // CommandBarFlyout.Target == this RichEditBox. WinUI samples gate
            // their "Add Share button" logic on `flyout.Target == myRichEditBox`,
            // and pointing the flyout at the inner host would break that.
            var position = e.GetPosition(this);
            flyout.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions
            {
                Position = position,
                Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.BottomEdgeAlignedLeft,
                ShowMode = Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowMode.Standard,
            });
        }
        catch (System.Exception ex)
        {
            try
            {
                File.AppendAllText(
                    Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.Flyout.log"),
                    $"{System.DateTimeOffset.Now:O} ShowAt-failed ex={ex.Message}{System.Environment.NewLine}");
            }
            catch { }
        }

        DispatcherQueue?.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            try
            {
                if (xamlRoot is null) return;
                int closed = 0;
                int total = 0;
                int ownCount = 0;
                foreach (var popup in Microsoft.UI.Xaml.Media.VisualTreeHelper.GetOpenPopupsForXamlRoot(xamlRoot))
                {
                    if (popup is null) continue;
                    total++;
                    if (!preOpenPopups.Contains(popup)) { ownCount++; continue; }
                    if (popup.Child is UIElement child)
                    {
                        child.Visibility = Visibility.Collapsed;
                        child.IsHitTestVisible = false;
                        if (child is FrameworkElement fe)
                        {
                            fe.Opacity = 0;
                            fe.Height = 0;
                            fe.Width = 0;
                        }
                    }
                    closed++;
                }
                File.AppendAllText(
                    Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.Flyout.log"),
                    $"{System.DateTimeOffset.Now:O} SweepOtherPopups total={total} own(new)={ownCount} hidden(pre-existing)={closed}{System.Environment.NewLine}");
            }
            catch (System.Exception ex)
            {
                try
                {
                    File.AppendAllText(
                        Path.Combine(Path.GetTempPath(), "LeXtudio.RichText.Flyout.log"),
                        $"{System.DateTimeOffset.Now:O} SweepOtherPopups-failed ex={ex.Message}{System.Environment.NewLine}");
                }
                catch { }
            }
        });
    }

    private static bool IsOwnPopup(Microsoft.UI.Xaml.Controls.Primitives.Popup popup)
    {
        var child = popup.Child;
        if (child is null) return false;
        // Our root StackPanel sits inside a FlyoutPresenter inside the Popup;
        // walk a bounded depth of children/content looking for our sentinel
        // Tag rather than reflecting on internal Popup.AssociatedFlyout.
        return ContainsOwnSentinel(child, depth: 0);
    }

    private static bool ContainsOwnSentinel(DependencyObject element, int depth)
    {
        if (depth > 6) return false;
        if (element is FrameworkElement fe
            && fe.Tag is string tag
            && tag == RichEditBoxCommandBarFlyoutFactory.OwnPopupSentinel)
        {
            return true;
        }

        if (element is ContentControl cc && cc.Content is DependencyObject inner)
        {
            if (ContainsOwnSentinel(inner, depth + 1)) return true;
        }
        if (element is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is DependencyObject d && ContainsOwnSentinel(d, depth + 1)) return true;
            }
        }
        int count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(element);
        for (int i = 0; i < count; i++)
        {
            var vc = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(element, i);
            if (ContainsOwnSentinel(vc, depth + 1)) return true;
        }
        return false;
    }

    private void OnEditorHostSelectionChanged(object sender, RoutedEventArgs e)
    {
        int start = _editorHost.SelectionStart;
        int length = _editorHost.SelectionLength;
        LogDiagnostic($"EditorHostSelectionChanged selection={start}+{length} runs={DescribeRuns()}");
        Document.Selection.SetRange(start, start + length);
        SyncOverlayCaret();
        RaiseSelectionChanged(e);
    }

    private void SyncOverlayCaret()
    {
        if (_renderOverlay.Visibility != Visibility.Visible)
            return;
        int start = _editorHost.SelectionStart;
        int length = _editorHost.SelectionLength;
        _renderOverlay.SetExternalCaret(start, start + length, _editorHostFocused);
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
        if (d is RichEditBox box && box._editorHost is not null)
        {
            box._editorHost.IsReadOnly = (bool)e.NewValue;
        }
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

    private void RefreshRichRenderOverlay()
    {
        Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out var text);
        text ??= string.Empty;

        var runs = Document.GetCharacterFormatRuns()
            .Where(run => run.Start < run.End && run.Start < text.Length && IsVisibleFormat(run.Format))
            .OrderBy(run => run.Start)
            .ToList();

        LogDiagnostic($"RefreshOverlay text={DescribeText(text)} visibleRuns={runs.Count} allRuns={DescribeRuns()} overlayVisible={_renderOverlay.Visibility == Visibility.Visible}");
        _renderOverlay.Blocks.Clear();

        if (string.IsNullOrEmpty(text) || runs.Count == 0)
        {
            _renderOverlay.Visibility = Visibility.Collapsed;
            _renderOverlay.SetExternalCaret(-1, -1, false);
            _editorHost.Opacity = 1;
            LogDiagnostic("RefreshOverlay collapsed");
            return;
        }

        var paragraph = new FlowDocuments.Paragraph();
        int position = 0;

        foreach (var run in runs)
        {
            int start = Math.Clamp(run.Start, 0, text.Length);
            int end = Math.Clamp(run.End, start, text.Length);
            if (start > position)
                paragraph.Inlines.Add(new FlowDocuments.Run(text[position..start]));

            paragraph.Inlines.Add(CreateFormattedInline(text[start..end], run.Format));
            position = end;
        }

        if (position < text.Length)
            paragraph.Inlines.Add(new FlowDocuments.Run(text[position..]));

        _renderOverlay.Blocks.Add(paragraph);

        if (_renderOverlay.Visibility != Visibility.Visible)
            LogOverlayAlignment();

        _renderOverlay.Visibility = Visibility.Visible;

        // The overlay paints formatted text while the real text host keeps caret,
        // selection, IME, and keyboard behavior alive underneath. The host is made
        // fully invisible so its unformatted text cannot bleed through the overlay —
        // on macOS the Skia font metrics cause a pixel offset between the two text
        // layers which produces a visible shadow at any non-zero opacity. Instead
        // the overlay draws its own caret and selection highlight via SetExternalCaret,
        // driven by the host's SelectionChanged and GotFocus/LostFocus events.
        _editorHost.Opacity = 0;
        SyncOverlayCaret();
        LogDiagnostic($"RefreshOverlay visible blocks={_renderOverlay.Blocks.Count}");
    }

    private static bool IsVisibleFormat(LeXtudio.UI.Text.TextCharacterFormat format)
        => format.Bold == Microsoft.UI.Text.FormatEffect.On
            || format.Italic == Microsoft.UI.Text.FormatEffect.On
            || format.Underline != Microsoft.UI.Text.UnderlineType.None
            || format.ForegroundColor != Color.FromArgb(255, 0, 0, 0);

    private static FlowDocuments.Inline CreateFormattedInline(string text, LeXtudio.UI.Text.TextCharacterFormat format)
    {
        FlowDocuments.Inline inline = new FlowDocuments.Run(text);

        if (format.Underline != Microsoft.UI.Text.UnderlineType.None)
            inline = new FlowDocuments.Underline(inline);

        if (format.Italic == Microsoft.UI.Text.FormatEffect.On)
            inline = new FlowDocuments.Italic(inline);

        if (format.Bold == Microsoft.UI.Text.FormatEffect.On)
            inline = new FlowDocuments.Bold(inline);

        if (inline is FlowDocuments.Span span && format.ForegroundColor != Color.FromArgb(255, 0, 0, 0))
            span.Foreground = new SolidColorBrush(format.ForegroundColor);
        else if (inline is FlowDocuments.Run run && format.ForegroundColor != Color.FromArgb(255, 0, 0, 0))
            run.Foreground = new SolidColorBrush(format.ForegroundColor);

        return inline;
    }

    private void LogOverlayAlignment()
    {
        if (!DiagnosticsEnabled)
            return;

        try
        {
            var innerPad = _editorHost.InnerTextBox.Padding;
            var overlayPad = _renderOverlay.Padding;
            LogDiagnostic(
                $"OverlayAlignment " +
                $"innerPad=({innerPad.Left},{innerPad.Top},{innerPad.Right},{innerPad.Bottom}) " +
                $"overlayPad=({overlayPad.Left},{overlayPad.Top},{overlayPad.Right},{overlayPad.Bottom}) " +
                $"innerSize=({_editorHost.InnerTextBox.ActualWidth:F1}x{_editorHost.InnerTextBox.ActualHeight:F1}) " +
                $"overlaySize=({_renderOverlay.ActualWidth:F1}x{_renderOverlay.ActualHeight:F1})");
        }
        catch (Exception ex)
        {
            LogDiagnostic($"OverlayAlignment failed: {ex.Message}");
        }
    }

    private static void LogDiagnostic(string message)
    {
        if (!DiagnosticsEnabled)
            return;

        try
        {
            File.AppendAllText(s_diagnosticLogPath, $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
            // Diagnostics must not affect editing behavior.
        }
    }

    private string DescribeRuns()
        => string.Join("; ", Document.GetCharacterFormatRuns().Select(run =>
            $"{run.Start}..{run.End} B={run.Format.Bold} I={run.Format.Italic} U={run.Format.Underline} FG={run.Format.ForegroundColor}"));

    private string DescribeSelectionFormat()
        => DescribeFormat(Document.Selection.CharacterFormat as LeXtudio.UI.Text.TextCharacterFormat);

    private static string DescribeFormat(LeXtudio.UI.Text.TextCharacterFormat? format)
        => format is null
            ? "<null>"
            : $"B={format.Bold} I={format.Italic} U={format.Underline} FG={format.ForegroundColor}";

    private static string DescribeText(string? text)
    {
        if (text is null)
            return "<null>";

        return "\"" + text
            .Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\u0002", "\\u0002")
            .Replace("\"", "\\\"") + "\"";
    }
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
#endif

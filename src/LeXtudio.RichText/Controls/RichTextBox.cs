using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FlowDocument = System.Windows.Documents.FlowDocument;
using TextPointer = System.Windows.Documents.TextPointer;
using TextSelection = System.Windows.Documents.TextSelection;

namespace LeXtudio.UI.Xaml.Controls;

/// <summary>
/// Uno host for a WPF-shaped <see cref="FlowDocument"/>.
/// </summary>
[Obsolete("RichEditBox or RichTextBlock should be used for production scenarios. This is just an experimental control for testing the FlowDocument implementation.")]
public sealed class RichTextBox : ContentControl
{
    public static DependencyProperty IsSpellCheckEnabledProperty { get; } =
        DependencyProperty.Register(
            nameof(IsSpellCheckEnabled),
            typeof(bool),
            typeof(RichTextBox),
            new PropertyMetadata(false, OnIsSpellCheckEnabledChanged));

    public static DependencyProperty DocumentProperty { get; } =
        DependencyProperty.Register(
            nameof(Document),
            typeof(FlowDocument),
            typeof(RichTextBox),
            new PropertyMetadata(null, OnDocumentChanged));
    public static DependencyProperty IsReadOnlyProperty { get; } =
        DependencyProperty.Register(
            nameof(IsReadOnly),
            typeof(bool),
            typeof(RichTextBox),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

    private readonly RichTextBlock _renderer = new();
    private FlowDocument? _attachedDocument;
    private System.Windows.Controls.SpellCheck? _spellCheck;

    public RichTextBox()
    {
        Document = new FlowDocument();
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        _renderer.SelectionChanged += OnRendererSelectionChanged;
        Content = _renderer;
    }

    public FlowDocument Document
    {
        get => (FlowDocument)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value ?? new FlowDocument());
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool IsSpellCheckEnabled
    {
        get => (bool)GetValue(IsSpellCheckEnabledProperty);
        set => SetValue(IsSpellCheckEnabledProperty, value);
    }

    public System.Windows.Controls.SpellCheck SpellCheck =>
        _spellCheck ??= new System.Windows.Controls.SpellCheck(v => IsSpellCheckEnabled = v);

    public TextPointer SelectionStart => _renderer.SelectionStart;

    public TextPointer SelectionEnd => _renderer.SelectionEnd;

    public string SelectedText => _renderer.SelectedText;

    public TextSelection Selection => throw new NotSupportedException("RichTextBox.Selection depends on the WPF TextEditor/TextSelection bridge, which is not enabled in the Phase 0 render host.");

    public event RoutedEventHandler? SelectionChanged;

    private static void OnIsSpellCheckEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RichTextBox)d)._renderer.IsSpellCheckEnabled = (bool)e.NewValue;
    }

    private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RichTextBox)d;
        control.AttachDocument(e.NewValue as FlowDocument);
    }

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RichTextBox)d;
        control._renderer.IsTextSelectionEnabled = true;
    }

    private void AttachDocument(FlowDocument? document)
    {
        if (_attachedDocument is not null)
        {
            _attachedDocument.TextLayoutHost = null;
        }

        var resolvedDocument = document ?? new FlowDocument();
        _attachedDocument = resolvedDocument;
        resolvedDocument.TextLayoutHost = _renderer.TextLayoutHost;
        _renderer.SetDocumentSource(resolvedDocument);
    }

    public void SelectAll()
        => _renderer.SelectAll();

    public void Select(TextPointer start, TextPointer end)
    {
        _renderer.Select(start, end);
    }

    public void CopySelectionToClipboard()
        => _renderer.CopySelectionToClipboard();

    private void OnRendererSelectionChanged(object sender, RoutedEventArgs e)
    {
        SelectionChanged?.Invoke(this, e);
    }
}

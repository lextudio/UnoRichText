using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FlowDocument = System.Windows.Documents.FlowDocument;
using TextPointer = System.Windows.Documents.TextPointer;
using TextSelection = System.Windows.Documents.TextSelection;

namespace LeXtudio.UI.Xaml.Controls;
#if RICHTEXTBOX
/// <summary>
/// Uno host for a WPF-shaped <see cref="FlowDocument"/>.
/// </summary>
public sealed class RichTextBox : ContentControl
{
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
    private readonly TextSelection _selection;
    private FlowDocument? _attachedDocument;

    public RichTextBox()
    {
        _selection = new TextSelection(
            () => _renderer.SelectionStart,
            () => _renderer.SelectionEnd,
            () => _renderer.SelectedText,
            (start, end) => _renderer.Select(start, end));

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

    public TextPointer SelectionStart => _renderer.SelectionStart;

    public TextPointer SelectionEnd => _renderer.SelectionEnd;

    public string SelectedText => _renderer.SelectedText;

    public TextSelection Selection => _selection;

    public event RoutedEventHandler? SelectionChanged;

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
        => _renderer.Select(start, end);

    public void CopySelectionToClipboard()
        => _renderer.CopySelectionToClipboard();

    private void OnRendererSelectionChanged(object sender, RoutedEventArgs e)
        => SelectionChanged?.Invoke(this, e);
}
#endif

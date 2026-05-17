using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using FlowDocument = System.Windows.Documents.FlowDocument;

namespace LeXtudio.UI.Xaml.Controls;

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

    private readonly RichTextBlock _renderer = new();
    private FlowDocument? _subscribedDocument;

    public RichTextBox()
    {
        Document = new FlowDocument();
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        Content = _renderer;
    }

    public FlowDocument Document
    {
        get => (FlowDocument)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value ?? new FlowDocument());
    }

    private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RichTextBox)d;
        control.AttachDocument(e.NewValue as FlowDocument);
    }

    private void AttachDocument(FlowDocument? document)
    {
        if (_subscribedDocument is not null)
        {
            _subscribedDocument.Blocks.CollectionChanged -= OnDocumentBlocksChanged;
            _subscribedDocument.TextLayoutHost = null;
        }

        _subscribedDocument = document ?? new FlowDocument();
        _subscribedDocument.TextLayoutHost = _renderer.TextLayoutHost;
        _subscribedDocument.Blocks.CollectionChanged += OnDocumentBlocksChanged;
        RefreshRenderer();
    }

    private void OnDocumentBlocksChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => RefreshRenderer();

    private void RefreshRenderer()
    {
        _renderer.Blocks.Clear();
        if (_subscribedDocument is null)
        {
            return;
        }

        foreach (var block in _subscribedDocument.Blocks)
        {
            _renderer.Blocks.Add(block);
        }
    }
}

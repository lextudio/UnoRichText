using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;

namespace LeXtudio.UI.Xaml.Controls;

/// <summary>
/// Represents a text highlighter that applies background color to text ranges.
/// Equivalent to WinUI's TextHighlighter.
/// </summary>
public class TextHighlighter
{
    private Brush? _background;
    private readonly ObservableCollection<TextRange> _ranges;

    public TextHighlighter()
    {
        _ranges = new ObservableCollection<TextRange>();
    }

    /// <summary>
    /// Gets or sets the brush to use as the background for highlighted text.
    /// </summary>
    public Brush? Background
    {
        get => _background;
        set => _background = value;
    }

    /// <summary>
    /// Gets the collection of text ranges to highlight.
    /// </summary>
    public ObservableCollection<TextRange> Ranges => _ranges;
}

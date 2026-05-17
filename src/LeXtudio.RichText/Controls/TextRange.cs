namespace LeXtudio.UI.Xaml.Controls;

/// <summary>
/// Represents a text range for highlighting or styling.
/// Equivalent to WinUI's TextRange.
/// </summary>
public class TextRange
{
    /// <summary>
    /// Gets or sets the zero-based start index of the text range.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// Gets or sets the length of the text range.
    /// </summary>
    public int Length { get; set; }

    public TextRange()
    {
        StartIndex = 0;
        Length = 0;
    }

    public TextRange(int startIndex, int length)
    {
        StartIndex = startIndex;
        Length = length;
    }
}

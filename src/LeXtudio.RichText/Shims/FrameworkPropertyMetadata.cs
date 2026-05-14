namespace System.Windows;

public sealed class FrameworkPropertyMetadata
{
    public FrameworkPropertyMetadata(object? defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public FrameworkPropertyMetadata(object? defaultValue, FrameworkPropertyMetadataOptions options)
    {
        DefaultValue = defaultValue;
        Options = options;
    }

    public FrameworkPropertyMetadata(object? defaultValue, FrameworkPropertyMetadataOptions options, PropertyChangedCallback? changed, CoerceValueCallback? coerce)
    {
        DefaultValue = defaultValue;
        Options = options;
        PropertyChangedCallback = changed;
        CoerceValueCallback = coerce;
    }

    public object? DefaultValue { get; }
    public FrameworkPropertyMetadataOptions Options { get; }
    public PropertyChangedCallback? PropertyChangedCallback { get; }
    public CoerceValueCallback? CoerceValueCallback { get; }
}

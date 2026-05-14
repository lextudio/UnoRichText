namespace System.Windows;

public sealed class FreezableDefaultValueFactory
{
    public FreezableDefaultValueFactory(object? defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public object? DefaultValue { get; }
}

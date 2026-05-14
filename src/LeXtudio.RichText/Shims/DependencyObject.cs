namespace System.Windows;

public class DependencyObject
{
    private readonly System.Collections.Generic.Dictionary<DependencyProperty, object?> _values = new();

    public object? GetValue(DependencyProperty property)
    {
        return _values.TryGetValue(property, out var value) ? value : property.DefaultValue;
    }

    public void SetValue(DependencyProperty property, object? value)
    {
        if (property.Metadata.CoerceValueCallback is not null)
        {
            value = property.Metadata.CoerceValueCallback(this, value!);
        }

        _values[property] = value;
        property.Metadata.PropertyChangedCallback?.Invoke(
            this,
            new DependencyPropertyChangedEventArgs
            {
                NewValue = value,
                NewEntry = new Entry { IsDeferredReference = false }
            });
    }

    protected void SetCurrentDeferredValue(DependencyProperty property, object? value)
    {
        _values[property] = value;
    }

    protected object LookupEntry(int globalIndex) => new();

    protected bool HasExpression(object entry, DependencyProperty property) => false;
}

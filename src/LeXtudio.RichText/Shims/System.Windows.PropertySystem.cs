namespace System.Windows;

public sealed class DependencyProperty
{
    public static readonly object UnsetValue = new();
    private static int _nextGlobalIndex;

    private DependencyProperty(string name, System.Type propertyType, System.Type ownerType, FrameworkPropertyMetadata metadata)
    {
        Name = name;
        PropertyType = propertyType;
        OwnerType = ownerType;
        Metadata = metadata;
    }

    public int GlobalIndex { get; } = System.Threading.Interlocked.Increment(ref _nextGlobalIndex);
    public string Name { get; }
    public System.Type PropertyType { get; }
    public System.Type OwnerType { get; }
    public FrameworkPropertyMetadata Metadata { get; private set; }
    public object? DefaultValue => Metadata.DefaultValue is FreezableDefaultValueFactory factory ? factory.DefaultValue : Metadata.DefaultValue;

    public static DependencyProperty Register(string name, System.Type propertyType, System.Type ownerType, FrameworkPropertyMetadata metadata)
    {
        return new DependencyProperty(name, propertyType, ownerType, metadata);
    }

    public static DependencyProperty Register(string name, System.Type propertyType, System.Type ownerType, FrameworkPropertyMetadata metadata, ValidateValueCallback validateValueCallback)
    {
        return new DependencyProperty(name, propertyType, ownerType, metadata);
    }

    public static DependencyProperty RegisterAttached(string name, System.Type propertyType, System.Type ownerType, FrameworkPropertyMetadata metadata)
    {
        return new DependencyProperty(name, propertyType, ownerType, metadata);
    }

    public static DependencyProperty RegisterAttached(string name, System.Type propertyType, System.Type ownerType, FrameworkPropertyMetadata metadata, ValidateValueCallback validateValueCallback)
    {
        return new DependencyProperty(name, propertyType, ownerType, metadata);
    }

    public DependencyProperty AddOwner(System.Type ownerType, FrameworkPropertyMetadata? typeMetadata = null)
    {
        if (typeMetadata is not null)
        {
            Metadata = typeMetadata;
        }

        return this;
    }
}

public delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e);
public delegate object CoerceValueCallback(DependencyObject d, object baseValue);
public delegate bool ValidateValueCallback(object value);

public readonly struct DependencyPropertyChangedEventArgs
{
    public object? NewValue { get; init; }
    public Entry NewEntry { get; init; }
}

public readonly struct Entry
{
    public bool IsDeferredReference { get; init; }
}

[System.Flags]
public enum FrameworkPropertyMetadataOptions
{
    None = 0,
    AffectsRender = 1,
    AffectsMeasure = 2,
    AffectsParentMeasure = 4,
    BindsTwoWayByDefault = 8,
    Inherits = 16
}

public readonly struct ValueSource
{
    public BaseValueSource BaseValueSource { get; init; }
    public bool IsExpression { get; init; }
}

public enum BaseValueSource
{
    Local = 0
}

public static class DependencyPropertyHelper
{
    public static ValueSource GetValueSource(DependencyObject obj, DependencyProperty dp) => new() { BaseValueSource = BaseValueSource.Local, IsExpression = false };
}

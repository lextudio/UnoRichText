using System.Collections.ObjectModel;

namespace System.Windows.Documents;

public class TextElementCollection<T> : ObservableCollection<T>
{
    private readonly System.Windows.DependencyObject _owner;
    private readonly bool _isOwnerParent;

    internal TextElementCollection(System.Windows.DependencyObject owner, bool isOwnerParent)
    {
        _owner = owner;
        _isOwnerParent = isOwnerParent;
    }

    protected System.Windows.DependencyObject Owner => _owner;

    protected bool IsOwnerParent => _isOwnerParent;

    internal object Parent => _owner;

    internal TextContainer TextContainer { get; } = new();

    protected T? FirstChild => Count > 0 ? this[0] : default;

    protected T? LastChild => Count > 0 ? this[Count - 1] : default;

    internal virtual int OnAdd(object value)
    {
        if (value is T typed)
        {
            Add(typed);
            return Count - 1;
        }

        throw new InvalidOperationException($"Unsupported child type {value?.GetType().FullName}");
    }

    internal virtual void ValidateChild(T child)
    {
    }
}

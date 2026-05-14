using System.Collections.ObjectModel;

namespace System.Windows.Documents;

public class TextElementCollection<T> : ObservableCollection<T>
{
    private readonly System.Windows.DependencyObject _owner;
    private readonly bool _isOwnerParent;
    private bool _syncingFromOwner;

    internal TextElementCollection(System.Windows.DependencyObject owner, bool isOwnerParent)
    {
        _owner = owner;
        _isOwnerParent = isOwnerParent;
        TextContainer = owner is TextElement textElementOwner
            ? textElementOwner.TextContainer
            : new TextContainer();

        if (_owner is TextElement textElement)
        {
            _syncingFromOwner = true;
            try
            {
                foreach (var child in textElement.LogicalChildren.OfType<T>())
                {
                    Add(child);
                }
            }
            finally
            {
                _syncingFromOwner = false;
            }
        }
    }

    protected System.Windows.DependencyObject Owner => _owner;

    protected bool IsOwnerParent => _isOwnerParent;

    internal object Parent => _owner;

    internal TextContainer TextContainer { get; }

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

    protected override void InsertItem(int index, T item)
    {
        ValidateChild(item);
        base.InsertItem(index, item);

        if (!_syncingFromOwner && _isOwnerParent && _owner is TextElement textElement && item is not null)
        {
            textElement.InsertLogicalChild(index, item);
        }
    }

    protected override void RemoveItem(int index)
    {
        var item = this[index];
        base.RemoveItem(index);

        if (_isOwnerParent && _owner is TextElement textElement && item is not null)
        {
            textElement.RemoveLogicalChild(item);
        }
    }

    protected override void ClearItems()
    {
        var items = this.ToArray();
        base.ClearItems();

        if (_isOwnerParent && _owner is TextElement textElement)
        {
            foreach (var item in items)
            {
                if (item is not null)
                {
                    textElement.RemoveLogicalChild(item);
                }
            }
        }
    }
}

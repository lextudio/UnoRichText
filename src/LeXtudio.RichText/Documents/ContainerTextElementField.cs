using System.Runtime.CompilerServices;

namespace System.Windows.Documents;

internal static class ContainerTextElementField
{
    private static readonly ConditionalWeakTable<object, Holder> Owners = new();

    internal static void ClearValue(object element)
    {
        Owners.Remove(element);
    }

    internal static void SetValue(object element, TextElement owner)
    {
        Owners.Remove(element);
        Owners.Add(element, new Holder(owner));
    }

    internal static TextElement? GetValue(object element)
    {
        return Owners.TryGetValue(element, out var holder) ? holder.Owner : null;
    }

    private sealed class Holder(TextElement owner)
    {
        internal TextElement Owner { get; } = owner;
    }
}

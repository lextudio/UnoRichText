namespace System.Windows.Documents;

internal static class TextSchema
{
    internal static void ValidateChild(TextElement parent, Inline child, bool throwIfIllegalChild, bool throwIfIllegalHyperlinkDescendent)
    {
    }

    internal static bool IsValidChildOfContainer(Type parentType, Type childType)
    {
        return true;
    }

    internal static bool IsValidChild(TextPointer position, Type childType)
    {
        return true;
    }
}

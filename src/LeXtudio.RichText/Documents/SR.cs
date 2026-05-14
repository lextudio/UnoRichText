namespace System.Windows.Documents;

internal static class SR
{
    internal const string TextSchema_ChildTypeIsInvalid = "TextSchema_ChildTypeIsInvalid";
    internal const string InDifferentTextContainers = "InDifferentTextContainers";
    internal const string BadTextPositionOrder = "BadTextPositionOrder";
    internal const string InDifferentParagraphs = "InDifferentParagraphs";
    internal const string TextSchema_CannotSplitElement = "TextSchema_CannotSplitElement";

    internal static string Format(string format, params object[] args)
    {
        return string.Format(format, args);
    }
}

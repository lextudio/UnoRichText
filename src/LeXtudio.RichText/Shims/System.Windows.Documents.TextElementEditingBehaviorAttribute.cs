namespace System.Windows.Documents;

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = true)]
public sealed class TextElementEditingBehaviorAttribute : System.Attribute
{
    public bool IsMergeable { get; set; } = true;
    public bool IsTypographicOnly { get; set; } = true;
}

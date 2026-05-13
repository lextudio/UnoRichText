namespace LeXtudio.UI.Xaml.Documents;

public class Paragraph : Block
{
    public InlineCollection Inlines { get; } = new();
}

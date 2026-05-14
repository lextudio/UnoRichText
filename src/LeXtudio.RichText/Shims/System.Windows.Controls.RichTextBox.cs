namespace System.Windows.Controls;

public sealed class RichTextBox
{
    public TextEditorShim TextEditor { get; } = new();
}

public sealed class TextEditorShim
{
    public UndoManager? _GetUndoManager() => null;
}

public sealed class UndoManager
{
    public bool IsEnabled => false;
    public void Clear()
    {
    }
}

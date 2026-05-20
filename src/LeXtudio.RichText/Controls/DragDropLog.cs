using System.IO;

namespace LeXtudio.UI.Xaml.Controls;

public static class DragDropLog
{
    private static readonly string _path = Path.Combine(Path.GetTempPath(), "uno-dragdrop.log");
    private static readonly object _lock = new();
    private static bool _enabled;

    /// <summary>
    /// Call at app startup to enable logging and reset the log file.
    /// Logging is disabled by default so production builds are unaffected.
    /// </summary>
    public static void Enable()
    {
        _enabled = true;
        System.Windows.Documents.TextEditorDragDropUno.Logger = Write;
        System.Windows.Controls.RichTextBox.Logger = Write;
        try { File.WriteAllText(_path, $"=== Log started {DateTime.Now:HH:mm:ss.fff} ===\n"); } catch { }
    }

    public static string LogPath => _path;

    internal static void Write(string message)
    {
        if (!_enabled) return;
        try
        {
            lock (_lock)
                File.AppendAllText(_path, $"{DateTime.Now:HH:mm:ss.fff}  {message}\n");
        }
        catch { }
    }
}

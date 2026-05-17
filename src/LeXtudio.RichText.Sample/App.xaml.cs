using Microsoft.UI.Xaml;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;
using LeXtudioREB = LeXtudio.UI.Xaml.Controls.RichEditBox;
using LeXtudioTextBox = LeXtudio.UI.Controls.TextBox;

namespace LeXtudio.RichText.Sample;

public partial class App : Application
{
    private MainWindow? _window;

    // Set by Program.Main when --diag is passed.
    public static bool DiagMode { get; set; }

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        LeXtudioRTB.DiagnosticsEnabled = true;
        LeXtudioREB.DiagnosticsEnabled = DiagMode;
        LeXtudioTextBox.DiagnosticsEnabled = DiagMode;
        if (DiagMode)
        {
            ResetRichTextDiagnosticLog("LeXtudio.RichText.RichEditBox.log");
            ResetRichTextDiagnosticLog("LeXtudio.RichText.TextBox.log");
        }
        global::UnoPropertyGrid.PropertyGridLogger.Enabled = true;
        global::UnoPropertyGrid.PropertyGridLogger.Reset();
        _window = new MainWindow();
        _window.Content = DiagMode ? new DiagPage() : new MainPage();
        _window.Activate();
    }

    private static void ResetRichTextDiagnosticLog(string fileName)
    {
        try
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        catch
        {
            // Diagnostics must never block the sample app from launching.
        }
    }
}

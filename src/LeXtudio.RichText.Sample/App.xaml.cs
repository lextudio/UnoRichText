using LeXtudio.DevFlow.Agent.Uno;
using Microsoft.Maui.DevFlow.Agent.Core;
using Microsoft.UI.Xaml;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;
using LeXtudioREB = LeXtudio.UI.Xaml.Controls.RichEditBox;
using LeXtudioTextBox = LeXtudio.UI.Controls.TextBox;

namespace LeXtudio.RichText.Sample;

public partial class App : Application
{
    private MainWindow? _window;
    private UnoAgentService? _devFlowAgent;

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
        LeXtudio.UI.Xaml.Controls.DragDropLog.Enable();
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

        _devFlowAgent = new UnoAgentService(new AgentOptions { Port = AgentOptions.DefaultPort });
        _devFlowAgent.Start();
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

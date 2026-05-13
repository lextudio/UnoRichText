using Microsoft.UI.Xaml;
using LeXtudioRTB = LeXtudio.UI.Xaml.Controls.RichTextBlock;

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
        LeXtudioRTB.DiagnosticsEnabled = DiagMode;
        _window = new MainWindow();
        _window.Content = DiagMode ? new DiagPage() : new MainPage();
        _window.Activate();
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NUnitLite;

namespace LeXtudio.RichText.Tests.Runtime;

public sealed partial class RuntimeTestApp : Application
{
    private Window? _window;

    public static string[]? NUnitArguments { get; set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new Window
        {
            Content = new Grid()
        };
        _window.Activate();
        UnoRuntimeTestHost.NotifyLaunched(_window);

        if (NUnitArguments is { } nunitArguments)
        {
            _ = Task.Run(() =>
            {
                var exitCode = new AutoRun(typeof(Program).Assembly).Execute(nunitArguments);
                Environment.Exit(exitCode);
            });
        }
    }
}

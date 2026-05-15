using Uno.UI.Hosting;

namespace LeXtudio.RichText.Sample;

public static class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        App.DiagMode = args.Contains("--diag");

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseMacOS()
            .UseWin32()
            .Build();
        await host.RunAsync();
    }
}

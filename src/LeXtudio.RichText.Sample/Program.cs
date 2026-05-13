using Uno.UI.Hosting;

namespace LeXtudio.RichText.Sample;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.DiagMode = args.Contains("--diag");

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseMacOS()
            .UseWin32()
            .Build();
        host.Run();
    }
}

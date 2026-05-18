using NUnitLite;
using Uno.UI.Hosting;
using LeXtudio.RichText.Tests.Runtime;

namespace LeXtudio.RichText.Tests;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Contains("--uno-runtime-tests"))
        {
            RuntimeTestApp.NUnitArguments = args.Where(arg => arg != "--uno-runtime-tests").ToArray();

            var host = UnoPlatformHostBuilder.Create()
                .App(() => new RuntimeTestApp())
                .UseX11()
                .UseMacOS()
                .UseWin32()
                .Build();

            await host.RunAsync();
            return Environment.ExitCode;
        }

        return new AutoRun(typeof(Program).Assembly).Execute(args);
    }
}

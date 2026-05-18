using NUnit.Framework;

namespace LeXtudio.RichText.Tests;

internal static class SampleDiagnostics
{
    private static readonly Lazy<(string Output, string Error, int ExitCode)> Result = new(Run);

    public static string Output => Result.Value.Output;
    public static string Error => Result.Value.Error;
    public static int ExitCode => Result.Value.ExitCode;

    public static void AssertPassMarker(string marker)
    {
        Assert.That(Output, Does.Contain(marker), Output + Error);
        Assert.That(ExitCode, Is.EqualTo(0), Output + Error);
    }

    private static (string Output, string Error, int ExitCode) Run()
    {
        var sampleExe = FindSampleExecutable();
        if (sampleExe is null)
            Assert.Ignore("Sample app executable not found. Build LeXtudio.RichText.Sample first.");

        using var process = new System.Diagnostics.Process();
        process.StartInfo = new System.Diagnostics.ProcessStartInfo(sampleExe, "--diag")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetDirectoryName(sampleExe)!
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        var exited = process.WaitForExit(15000);

        Console.WriteLine("=== Sample stdout ===");
        Console.WriteLine(output);
        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine("=== Sample stderr ===");
            Console.WriteLine(error);
        }

        Assert.That(exited, Is.True, "Sample app did not exit within 15 seconds.");
        return (output, error, process.ExitCode);
    }

    private static string? FindSampleExecutable()
    {
        var testDir = Path.GetDirectoryName(typeof(SampleDiagnostics).Assembly.Location)!;
        var sampleNames = new[] { "LeXtudio.RichText.Sample.exe", "LeXtudio.RichText.Sample" };

        var dir = new DirectoryInfo(testDir);
        while (dir != null)
        {
            foreach (var sampleName in sampleNames)
            {
                var candidate = Path.Combine(dir.FullName, "LeXtudio.RichText.Sample", "bin", "Debug", "net10.0-desktop", sampleName);
                if (File.Exists(candidate)) return candidate;

                candidate = Path.Combine(dir.FullName, "LeXtudio.RichText.Sample", "bin", "Release", "net10.0-desktop", sampleName);
                if (File.Exists(candidate)) return candidate;
            }

            dir = dir.Parent;
        }

        return null;
    }
}

using NUnit.Framework;

namespace LeXtudio.RichText.Tests.Controls;

/// <summary>
/// Tests that InlineUIContainer (inline code boxes) produce placed fragments in
/// the RichTextBlock layout pipeline, enabling inline code rendering.
/// Runs the sample app as a subprocess (which has a full Skia host) and checks
/// the exit code that DiagPage reports.
/// </summary>
[TestFixture]
public sealed class InlineCodeRenderingTests : TestBase
{
    [Test]
    public void InlineCode_ProducesPlacedFragment()
    {
        var sampleExe = FindSampleExecutable();
        if (sampleExe is null)
        {
            Assert.Ignore("Sample app executable not found — build LeXtudio.RichText.Sample first.");
            return;
        }

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
        Assert.That(process.ExitCode, Is.EqualTo(0),
            $"Sample app exited with code {process.ExitCode} — inline code did not render.\n{output}\n{error}");
    }

    private static string? FindSampleExecutable()
    {
        var testDir = Path.GetDirectoryName(typeof(InlineCodeRenderingTests).Assembly.Location)!;
        var sampleName = "LeXtudio.RichText.Sample.exe";

        // Walk up from test binary directory to find the sample in sibling project
        var dir = new DirectoryInfo(testDir);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "LeXtudio.RichText.Sample", "bin", "Debug", "net10.0-desktop", sampleName);
            if (File.Exists(candidate)) return candidate;

            // Also try Release
            candidate = Path.Combine(dir.FullName, "LeXtudio.RichText.Sample", "bin", "Release", "net10.0-desktop", sampleName);
            if (File.Exists(candidate)) return candidate;

            dir = dir.Parent;
        }

        return null;
    }
}

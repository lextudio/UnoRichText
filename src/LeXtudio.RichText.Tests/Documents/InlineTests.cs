using System.Diagnostics;
using LeXtudio.UI.Xaml.Documents;
using Microsoft.UI.Text;
using NUnit.Framework;
using Windows.UI.Text;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class InlineTests
{
    [Test]
    public void Bold_DefaultsToBoldWeight()
    {
        if (RunInStandaloneWinUIProcessIfNeeded())
            return;

        var bold = new Bold();

        Assert.That(bold.FontWeight, Is.Not.Null);
        Assert.That(bold.FontWeight!.Value.Weight, Is.EqualTo(FontWeights.Bold.Weight));
    }

    [Test]
    public void Italic_DefaultsToItalicStyle()
    {
        if (RunInStandaloneWinUIProcessIfNeeded())
            return;

        var italic = new Italic();

        Assert.That(italic.FontStyle, Is.EqualTo(FontStyle.Italic));
    }

    [Test]
    public void Span_OwnsInlineCollection()
    {
        if (RunInStandaloneWinUIProcessIfNeeded())
            return;

        var span = new Span();
        var run = new Run { Text = "hello" };

        span.Inlines.Add(run);

        Assert.That(span.Inlines, Has.Count.EqualTo(1));
        Assert.That(span.Inlines[0], Is.SameAs(run));
    }

    static bool RunInStandaloneWinUIProcessIfNeeded()
    {
#if WINDOWS_APP_SDK
        if (Environment.GetEnvironmentVariable(StandaloneWinUITestEnvironmentVariable) == "1")
            return false;

        var assemblyPath = typeof(InlineTests).Assembly.Location;
        var executablePath = Path.ChangeExtension(assemblyPath, ".exe");
        Assert.That(File.Exists(executablePath), Is.True, $"Test executable was not found at '{executablePath}'.");

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo(executablePath)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetDirectoryName(executablePath)!
        };
        process.StartInfo.ArgumentList.Add("--where");
        process.StartInfo.ArgumentList.Add($"test == '{TestContext.CurrentContext.Test.FullName}'");
        process.StartInfo.Environment[StandaloneWinUITestEnvironmentVariable] = "1";

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Assert.That(process.WaitForExit(60000), Is.True, "Standalone WinUI test process timed out.");
        Assert.That(process.ExitCode, Is.EqualTo(0), output + error);
        return true;
#else
        return false;
#endif
    }

#if WINDOWS_APP_SDK
    const string StandaloneWinUITestEnvironmentVariable = "LEXTUDIO_RICHTEXT_STANDALONE_WINUI_TEST";
#endif
}

using System;
using System.IO;
using System.Linq;

namespace RichTextBlockCompat;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            return Run(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("ERROR: " + ex.Message);
            if (Environment.GetEnvironmentVariable("VERBOSE") == "1")
                Console.Error.WriteLine(ex);
            return 2;
        }
    }

    private static int Run(string[] args)
    {
        var reportPath = ResolveReportPath(args);
        double? threshold = ResolveThreshold(args);

        using var loader = MetadataLoader.Create();

        // DependencyProperty type, loaded from the reference (WinUI 3) and subject assemblies
        // respectively. The extractor uses the FullName to filter static DP fields, so both
        // sides count their DP fields consistently.
        var winuiDp = loader.WinUIReference.GetType("Microsoft.UI.Xaml.DependencyProperty", throwOnError: false);
        var subjectDp = loader.SubjectAssemblies
            .Select(a => a.GetType("Microsoft.UI.Xaml.DependencyProperty", throwOnError: false))
            .FirstOrDefault(t => t is not null);

        var winUIExtractor = new ApiSurfaceExtractor(winuiDp);
        var subjectExtractor = new ApiSurfaceExtractor(subjectDp);
        var checker = new CompatChecker(loader, winUIExtractor, subjectExtractor);

        var results = TypePairs.All.Select(checker.Check).ToList();

        ReportWriter.WriteMarkdown(results, loader, reportPath);
        ReportWriter.WriteConsoleSummary(results, loader, reportPath);

        var grandTotal = results.Sum(r => r.Total);
        var grandMatched = results.Sum(r => r.Matched);
        var pct = grandTotal == 0 ? 100.0 : grandMatched * 100.0 / grandTotal;

        if (threshold is double min && pct < min)
        {
            Console.Error.WriteLine($"FAIL: coverage {pct:F1}% < threshold {min:F1}%");
            return 1;
        }
        return 0;
    }

    private static string ResolveReportPath(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--out") return args[i + 1];
        }
        // Default: UnoRichText/docs/COMPAT-REPORT.md — walk up looking for the docs/DESIGN.md marker.
        var dir = AppContext.BaseDirectory;
        while (dir is not null && !File.Exists(Path.Combine(dir, "docs", "DESIGN.md")))
        {
            dir = Path.GetDirectoryName(dir);
        }
        if (dir is null) return "compat-report.md";
        return Path.Combine(dir, "docs", "COMPAT-REPORT.md");
    }

    private static double? ResolveThreshold(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--min" && double.TryParse(args[i + 1], out var v)) return v;
        }
        return null;
    }
}

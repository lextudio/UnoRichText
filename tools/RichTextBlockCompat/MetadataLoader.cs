using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace RichTextBlockCompat;

/// <summary>
/// Builds a <see cref="MetadataLoadContext"/> populated with everything we need to extract
/// API surface for both sides of the parity check:
/// <list type="bullet">
///   <item>The reference side: <c>Microsoft.WinUI.dll</c> from the Microsoft.WindowsAppSDK NuGet package.</item>
///   <item>The subject side: <c>LeXtudio.RichText.dll</c> and <c>LeXtudio.Windows.dll</c> from the
///       LeXtudio.RichText build output (net10.0-desktop target).</item>
/// </list>
/// MetadataLoadContext is used (not Assembly.LoadFrom) because we just need to read declared
/// members — we never execute anything from these assemblies, and the subject DLLs reference
/// runtimes we do not want to drag in.
/// </summary>
public sealed class MetadataLoader : IDisposable
{
    public MetadataLoadContext Context { get; }
    public Assembly WinUIReference { get; }
    public IReadOnlyList<Assembly> SubjectAssemblies { get; }

    private MetadataLoader(MetadataLoadContext context, Assembly winUI, IReadOnlyList<Assembly> subject)
    {
        Context = context;
        WinUIReference = winUI;
        SubjectAssemblies = subject;
    }

    public static MetadataLoader Create()
    {
        var winUIDll = ResolveWinUIDll();
        var subjectDlls = ResolveSubjectDlls().ToList();

        // The resolver needs everything the load context might encounter while walking metadata:
        // the two target DLLs, the .NET runtime ref assemblies, the rest of the WinAppSDK package
        // (for transitive types like XamlRoot, FontFamily), and a few common assemblies.
        var resolverPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            winUIDll,
        };

        foreach (var d in subjectDlls) resolverPaths.Add(d);

        // Include the entire WinAppSDK package's lib folder so transitive types resolve.
        var winUiDir = Path.GetDirectoryName(winUIDll)!;
        foreach (var dll in Directory.EnumerateFiles(winUiDir, "*.dll", SearchOption.TopDirectoryOnly))
            resolverPaths.Add(dll);

        // Include Microsoft.Windows.SDK.NET.dll (WinRT C# projections) — Microsoft.WinUI.dll
        // references types like Windows.Foundation.Point from this assembly.
        foreach (var sdkRefDll in ResolveSdkNetRefDlls())
            resolverPaths.Add(sdkRefDll);

        // Include all DLLs sitting next to LeXtudio's built outputs (the bin folder).
        foreach (var subjectDll in subjectDlls)
        {
            var dir = Path.GetDirectoryName(subjectDll)!;
            foreach (var dll in Directory.EnumerateFiles(dir, "*.dll", SearchOption.TopDirectoryOnly))
                resolverPaths.Add(dll);
        }

        // Parse the subject's deps.json to find every runtime DLL it depends on (Uno.UI, etc.)
        // — Uno.Sdk doesn't copy these to bin, they live in the NuGet cache.
        foreach (var subjectDll in subjectDlls)
        {
            var depsJson = Path.ChangeExtension(subjectDll, ".deps.json");
            if (!File.Exists(depsJson)) continue;
            foreach (var dll in EnumerateDepsJsonAssemblies(depsJson))
                resolverPaths.Add(dll);
        }

        // Pull in the running runtime's reference assemblies so core types (System.Object, etc.)
        // resolve from the same place — without this, MetadataLoadContext fails on first lookup.
        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        foreach (var dll in Directory.GetFiles(runtimeDir, "*.dll"))
            resolverPaths.Add(dll);

        var resolver = new PathAssemblyResolver(resolverPaths);
        // Use the running runtime's System.Private.CoreLib as the core assembly so primitives match.
        var context = new MetadataLoadContext(resolver, coreAssemblyName: typeof(object).Assembly.GetName().Name);

        var winUIAsm = context.LoadFromAssemblyPath(winUIDll);
        var subjectAsms = subjectDlls.Select(context.LoadFromAssemblyPath).ToList();

        return new MetadataLoader(context, winUIAsm, subjectAsms);
    }

    /// <summary>Find a type in the reference assembly by full name.</summary>
    public Type GetWinUIType(string fullName)
    {
        var t = WinUIReference.GetType(fullName, throwOnError: false);
        if (t is not null) return t;
        // Some WinUI types live in transitively-loaded assemblies (e.g. shared content types).
        foreach (var asm in Context.GetAssemblies())
        {
            t = asm.GetType(fullName, throwOnError: false);
            if (t is not null) return t;
        }
        throw new InvalidOperationException($"WinUI 3 type not found in reference assemblies: {fullName}");
    }

    /// <summary>Find a type across all subject assemblies by full name. Returns null if absent.</summary>
    public Type? GetSubjectType(string fullName)
    {
        foreach (var asm in SubjectAssemblies)
        {
            var t = asm.GetType(fullName, throwOnError: false);
            if (t is not null) return t;
        }
        return null;
    }

    public void Dispose() => Context.Dispose();

    // ── path resolution ───────────────────────────────────────────────

    private static string ResolveWinUIDll()
    {
        var root = ReadAssemblyMetadata("WinAppSdkPackageRoot");
        if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            throw new InvalidOperationException(
                "WinAppSdk package root not embedded in tool assembly. " +
                "Rebuild the tool with the Microsoft.WindowsAppSDK package restored.");

        // Microsoft.WindowsAppSDK NuGet layout: lib/<tfm>/Microsoft.WinUI.dll
        // Pick the highest TFM under lib to be version-resilient.
        var libRoot = Path.Combine(root, "lib");
        if (!Directory.Exists(libRoot))
            throw new InvalidOperationException($"WinAppSdk lib folder missing: {libRoot}");

        foreach (var tfmDir in Directory.EnumerateDirectories(libRoot).OrderByDescending(d => d))
        {
            var dll = Path.Combine(tfmDir, "Microsoft.WinUI.dll");
            if (File.Exists(dll)) return dll;
        }
        throw new InvalidOperationException($"Microsoft.WinUI.dll not found under {libRoot}");
    }

    private static IEnumerable<string> ResolveSdkNetRefDlls()
    {
        var root = ReadAssemblyMetadata("WindowsSdkNetRefRoot");
        if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            yield break; // SDK Ref optional; if absent, MetadataLoadContext may still resolve from runtime dlls.

        // Microsoft.Windows.SDK.NET.Ref layout: lib/<tfm>/Microsoft.Windows.SDK.NET.dll
        var libRoot = Path.Combine(root, "lib");
        if (!Directory.Exists(libRoot)) yield break;

        foreach (var tfmDir in Directory.EnumerateDirectories(libRoot).OrderByDescending(d => d))
        {
            foreach (var dll in Directory.EnumerateFiles(tfmDir, "*.dll", SearchOption.TopDirectoryOnly))
                yield return dll;
            yield break; // first (highest) TFM is enough
        }
    }

    private static IEnumerable<string> ResolveSubjectDlls()
    {
        var outputDir = ReadAssemblyMetadata("LeXtudioRichTextOutputDir");
        if (string.IsNullOrEmpty(outputDir) || !Directory.Exists(outputDir))
            throw new InvalidOperationException(
                $"LeXtudio.RichText output dir not found: {outputDir}. " +
                "Rebuild the tool — the ProjectReference should have triggered LeXtudio.RichText to build.");

        var richText = Path.Combine(outputDir, "LeXtudio.RichText.dll");
        var windowsShim = Path.Combine(outputDir, "LeXtudio.Windows.dll");
        if (!File.Exists(richText))
            throw new InvalidOperationException($"LeXtudio.RichText.dll missing: {richText}");
        if (!File.Exists(windowsShim))
            throw new InvalidOperationException($"LeXtudio.Windows.dll missing: {windowsShim}");

        yield return richText;
        yield return windowsShim;
    }

    /// <summary>
    /// Yields every runtime DLL referenced by the given deps.json, resolved to an absolute path
    /// under one of its declared packageFolders. We use this to find Uno.UI.dll and the other
    /// runtime assemblies the subject DLLs reference — they live in the NuGet cache, not in bin.
    /// </summary>
    private static IEnumerable<string> EnumerateDepsJsonAssemblies(string depsJsonPath)
    {
        using var stream = File.OpenRead(depsJsonPath);
        using var doc = JsonDocument.Parse(stream);

        var packageFolders = new List<string>();
        if (doc.RootElement.TryGetProperty("runtimes", out _) ||
            doc.RootElement.TryGetProperty("packageFolders", out var packageFoldersEl))
        {
            if (doc.RootElement.TryGetProperty("packageFolders", out packageFoldersEl))
            {
                foreach (var prop in packageFoldersEl.EnumerateObject())
                    packageFolders.Add(prop.Name);
            }
        }
        // Fall back to the standard user NuGet cache if packageFolders is empty (some SDKs omit it).
        if (packageFolders.Count == 0)
        {
            var nugetPackages = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                ".nuget", "packages");
            packageFolders.Add(nugetPackages);
        }

        if (!doc.RootElement.TryGetProperty("targets", out var targets))
            yield break;

        foreach (var targetEntry in targets.EnumerateObject())
        {
            foreach (var dep in targetEntry.Value.EnumerateObject())
            {
                // Each dep has the form "PackageId/Version" (or "Project/Version").
                var key = dep.Name;
                if (!dep.Value.TryGetProperty("runtime", out var runtimeFiles)) continue;

                foreach (var file in runtimeFiles.EnumerateObject())
                {
                    var relative = file.Name; // e.g. "lib/net8.0/Uno.UI.dll"
                    if (!relative.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;
                    foreach (var folder in packageFolders)
                    {
                        var path = Path.Combine(folder, key.ToLowerInvariant(), relative);
                        if (File.Exists(path)) { yield return path; break; }
                    }
                }
            }
        }
    }

    private static string? ReadAssemblyMetadata(string key)
    {
        var asm = typeof(MetadataLoader).Assembly;
        foreach (var attr in asm.GetCustomAttributes<AssemblyMetadataAttribute>())
        {
            if (attr.Key == key) return attr.Value;
        }
        return null;
    }
}

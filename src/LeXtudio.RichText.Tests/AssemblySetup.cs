using NUnit.Framework;

namespace LeXtudio.RichText.Tests;

[SetUpFixture]
public class AssemblySetup
{
    [OneTimeSetUp]
    public void InitializeSkiaRuntime()
    {
        var runtimeType = Type.GetType("Uno.UI.Runtime.Skia.SkiaHostBuilder, Uno.UI.Runtime.Skia");
        if (runtimeType != null)
        {
            var createMethod = runtimeType.GetMethod("Create", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (createMethod != null)
            {
                var builderInstance = createMethod.Invoke(null, null);
                var useWpfMethod = builderInstance?.GetType().GetMethod("UseWpf", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (useWpfMethod != null)
                {
                    var wpfBuilder = useWpfMethod.Invoke(builderInstance, null);
                    var buildMethod = wpfBuilder?.GetType().GetMethod("Build", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    if (buildMethod != null)
                    {
                        var host = buildMethod.Invoke(wpfBuilder, null);
                        System.Diagnostics.Debug.WriteLine($"Skia host initialized: {host?.GetType().Name}");
                    }
                }
            }
        }
    }
}

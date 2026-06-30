using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace LeXtudio.RichText.Tests.Runtime;

internal static class UnoRuntimeTestHost
{
    private static readonly TaskCompletionSource<Window> Ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static async Task EnsureAvailableAsync()
    {
        _ = await EnsureStartedAsync().ConfigureAwait(false);
    }

    public static async Task RunOnUIThreadAsync(Action action)
    {
        var window = await EnsureStartedAsync().ConfigureAwait(false);
        var dispatcher = window.DispatcherQueue;

        if (dispatcher.HasThreadAccess)
        {
            action();
            return;
        }

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            try
            {
                action();
                completion.SetResult();
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        }))
        {
            throw new InvalidOperationException("Unable to enqueue work on the Uno runtime dispatcher.");
        }

        await completion.Task.ConfigureAwait(false);
    }

    public static async Task<T> RunOnUIThreadAsync<T>(Func<T> action)
    {
        var result = default(T);
        await RunOnUIThreadAsync(() => result = action()).ConfigureAwait(false);
        return result!;
    }

    internal static void NotifyLaunched(Window window)
    {
        Ready.TrySetResult(window);
    }

    private static Task<Window> EnsureStartedAsync()
    {
        if (RuntimeTestApp.NUnitArguments is null)
        {
            throw new InvalidOperationException(
                "Dispatcher-bound tests must run inside the Uno runtime app. Use: dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackages=false -- --uno-runtime-tests --test=<fixture>");
        }

        return Ready.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }
}

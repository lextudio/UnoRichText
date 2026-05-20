#if WINDOWS_APP_SDK
using System.Runtime.CompilerServices;
using global::Microsoft.UI.Xaml.Controls;

[assembly: TypeForwardedTo(typeof(global::Microsoft.UI.Xaml.Controls.RichEditBox))]

namespace LeXtudio.UI.Xaml.Controls
{
    // Forward the LeXtudio.UI.Xaml.Controls.RichEditBox type to the WinUI implementation.
}
#endif
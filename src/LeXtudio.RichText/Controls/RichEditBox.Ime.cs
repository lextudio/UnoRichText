// RichEditBox — IME and text-services wiring.
//
// This partial is the integration seam between RichEditBox and the cross-platform
// IME / text-services layer in TextCore.Uno (LeXtudio.UI.Text.Core).
//
// Why a separate partial:
//   The Microsoft.UI.Text.Core types (CoreTextEditContext, CoreTextTextRequest,
//   CoreTextSelectionRequest, CoreTextLayoutRequest, the *EventArgs family)
//   are shipped by LeXtudio.UI.Text.Core as platform-aware adapters with Win32,
//   macOS, X11/IBus, and a null-fallback implementation. Keeping the integration
//   in its own file means we can land it as soon as the package reference is
//   added without touching the main RichEditBox.cs surface.
//
// What this file will do once wired:
//   1. Construct a CoreTextEditContext using CoreTextServicesManager.GetForCurrentView().
//   2. Subscribe to TextRequested / SelectionRequested / TextUpdating /
//      SelectionUpdating / LayoutRequested / FocusRemoved / CompositionStarted /
//      CompositionCompleted.
//   3. Translate each request to/from RichEditTextDocument.GetText(),
//      RichEditTextDocument.Selection, and the layout produced by the rendering
//      engine in RichEditBox.Rendering.cs.
//   4. Raise the matching RichEditBox events (TextCompositionStarted/Changed/Ended,
//      TextChanging, TextChanged, SelectionChanging, SelectionChanged,
//      CandidateWindowBoundsChanged).
//
// Reference: TextCore.Uno/src/LeXtudio.UI.Text.Core/CoreTextEditContext.cs
//            TextCore.Uno/src/LeXtudio.UI.Text.Core/Win32TextInputAdapter.cs
//            TextCore.Uno/src/LeXtudio.UI.Text.Core/MacOSTextInputAdapter.cs
//            TextCore.Uno/src/LeXtudio.UI.Text.Core/LinuxIbusTextInputAdapter.cs
//            TextCore.Uno/src/LeXtudio.UI.Text.Core/NullTextInputAdapter.cs

namespace LeXtudio.UI.Xaml.Controls;

public partial class RichEditBox
{
    // Placeholder hooks. These get fleshed out when LeXtudio.UI.Text.Core is
    // added as a dependency. They are intentionally non-virtual and internal —
    // consumers do not see them.
    internal void AttachIme() { /* binds CoreTextEditContext to this control's focus + selection */ }
    internal void DetachIme() { /* releases the context, unsubscribes handlers */ }
    internal void RequestImeCompositionRestart() { /* called from IsReadOnly / Document.SetText changes */ }
}

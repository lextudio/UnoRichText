# Session 18 — TextEditorTyping + TextEditorSelection

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable TextEditorSelection.cs (no patches) and TextEditorTyping.cs (IME-level keyboard input
dispatch) from upstream WPF. Deferred: TextEditorCopyPaste.cs, TextEditorDragDrop.cs.

## What this gets us

Full keyboard navigation and typing command pipeline now compiles from upstream source.
TextEditorSelection (~2500 lines) wires all caret-movement and selection-extend commands;
TextEditorTyping (~1900 lines) handles PreviewKeyDown, KeyDown, KeyUp, and TextInput.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for TextEditorTyping.cs and TextEditorSelection.cs.

### EarlyBatchEditorShims.cs
- Removed stubs for TextEditorTyping and TextEditorSelection.
- Added `SafeNativeMethods.ShowCursor(bool) => 0` no-op.
- Added `TextServicesHost.StartTransitoryExtension` and `StopTransitoryExtension` no-ops.

### KeyEventArgs.cs
- Added `KeyboardDevice` property (returns static `KeyboardDevice.Empty`).
- Added `OriginalSource` and `IsRepeat` properties.

### KeyEventArgs.cs / KeyboardDevice (new class)
- New `KeyboardDevice` class in `System.Windows.Input`: `Modifiers` delegates to `Keyboard.Modifiers`;
  `IsKeyToggled(Key) => false`.

### Input/Key.cs
- Added `LeftShift`, `RightShift`, `LeftCtrl`, `RightCtrl`, `LeftAlt`, `RightAlt`, `ImeProcessed`.

### Input/Keyboard.cs
- Added `PreviewKeyDownEvent`, `KeyDownEvent`, `KeyUpEvent`, `PreviewKeyUpEvent` static RoutedEvents.
- Added `KeyEventHandler` delegate.

### Input/Mouse.cs
- Added `MouseLeaveEvent`, `MouseEnterEvent` static RoutedEvents.

### Input/TextCompositionEventArgs.cs
- Added `TextComposition` class with `Owner: object` and `Text: string`.
- Added `FrameworkTextComposition : TextComposition`.
- Added `TextCompositionManager` with `TextInputEvent`, `TextInputStartEvent`, `TextInputUpdateEvent`.
- Added `TextCompositionEventHandler` delegate.
- Added `TextCompositionEventArgs.TextComposition` and `.Device` properties.

### System.Windows.cs
- Added `InputLanguageEventArgs` with `NewLanguage` and `PreviousLanguage` (CultureInfo).
- Added `InputLanguageEventHandler` delegate.
- Added `InputLanguageManager.InputLanguageChanged` event.
- Added `SourceChangedEventArgs`, `SourceChangedEventHandler`.
- Added `PresentationSource.CriticalFromVisual`, `AddSourceChangedHandler`, `RemoveSourceChangedHandler`.

### EventManager.cs
- Added `RegisterClassHandler(Type, RoutedEvent, Delegate, bool handledEventsToo)` 4-arg overload.

### RoutedEventArgs.cs
- Added `OriginalSource` and `Handled` properties.

### Input/RoutedCommand.cs
- Added `CanExecute(object, object)` and `Execute(object, object)` target-overloads.

### Documents/FlowDocument.cs
- Added `FlowDirectionProperty` (backed by `FrameworkElement.FlowDirectionProperty`).

### Controls/ScrollShims.cs (new file)
- `IScrollInfo` interface with HorizontalOffset/VerticalOffset/ViewportWidth/ViewportHeight/ExtentWidth/ExtentHeight.
- `ScrollBar` static class with `PageDownCommand`, `PageUpCommand`, `LineDownCommand`, `LineUpCommand`.
- `TextBoxView` marker class (used by `IsPaginated` check in TextEditorSelection).

### TextEditorSystemShims.cs
- Added `MS.Internal.Interop` empty namespace.
- Added `SystemParameters.MouseVanish => false`.

### Dispatcher.cs
- Added `ShutdownFinished` and `ShutdownStarted` events.

### Controls/RichTextBox.cs
- Changed `TextEditor` property from `TextEditorShim` to `internal Documents.TextEditor => null`
  (upstream handlers null-check this; returning null makes them no-ops when not wired up).
- Removed `TextEditorShim.implicit operator TextEditor`.

### Upstream patches (ext/wpf)

**TextEditorTyping.cs**
- Gated `IsMouseInputPending` body with `#if HAS_UNO return false; #else ... #endif`.

**TextEditorSelection.cs**
- Gated `DocumentSequenceTextPointer || FixedTextPointer` type checks with `#if HAS_UNO true &&`.

# Session 16 — Promoting Upstream `TextEditor.cs`

Status: done.

## Goal

Enable the upstream WPF `TextEditor.cs` in `LeXtudio.Windows.csproj` by building out the shim
infrastructure it depends on, reducing the active stub surface so subsequent sessions can promote
`TextEditor`'s helper classes (`TextEditorTyping`, `TextEditorSelection`, etc.).

## What this gets us

`TextEditor` is the WPF editing controller wired into `RichTextBox`/`TextBox`. With it compiling
from upstream source, future sessions can activate its sibling classes one at a time and eventually
delete their stubs from `EarlyBatchEditorShims.cs`. The 62-test baseline is unaffected.

Also promoted in this session: `TextRangeEditLists.cs` (zero patches required).

## Changes

### Upstream files promoted

| File | Patches |
|------|---------|
| `TextRangeEditLists.cs` | none — compiled verbatim |
| `TextEditor.cs` | 9 `#if HAS_UNO` / `#if !HAS_UNO` guards (see below) |

### Patches in `TextEditor.cs`

- **`IsEnabledChanged` subscription** (`~line 83, 177`): gated with `#if !HAS_UNO` — C# 14
  extension blocks do not support event declarations; helper methods used instead.
- **SpellCheck / `CustomDictionarySources`** (`~line 91`, `SetCustomDictionaries`, `IsSpellCheckEnabled`):
  gated with `#if !HAS_UNO` / `#if HAS_UNO` — spell-check infrastructure not yet enabled.
- **`DispatcherTimer` 1-arg constructor** (`~line 506`): `new DispatcherTimer(DispatcherPriority.Normal)`
  → `new DispatcherTimer()` under `#if HAS_UNO`.
- **`Timer.Tick` subscription** (`~line 508`): lambda wrapper `(s,e) => HandleMouseSelectionTick(s, EventArgs.Empty)`
  under `#if HAS_UNO` because WinUI's `DispatcherTimer.Tick` is `EventHandler<object>`, not `EventHandler`.
- **`Timer.Tick -= ...`** (`~line 528`): entire unsubscription gated `#if !HAS_UNO` (lambda cannot
  be unsubscribed by reference).
- **`ScrollContentPresenter` ambiguity** (`~line 1320`): qualified to
  `Microsoft.UI.Xaml.Controls.ScrollContentPresenter` under `#if HAS_UNO`.

### New shim files

| File | Purpose |
|------|---------|
| `System.Windows/Input/TextEditorInputShims.cs` | `CanExecuteRoutedEventHandler`, `ExecutedRoutedEventHandler`, `KeyboardFocusChangedEventHandler`, `Cursors`, `KeyboardNavigation`, `PopupControlService` |
| `System.Windows/TextEditorSystemShims.cs` | `SystemParameters`, `SafeSystemMetrics`, `XmlLanguage`, `TextServicesHost`, `TextServicesLoader`, `UnsafeNativeMethods.ITfThreadMgr` |

### Updated shim files

| File | Additions |
|------|-----------|
| `EarlyBatchEditorShims.cs` | `TextEditorTyping._AddInputLanguageChangedEventHandler`, `_RemoveInputLanguageChangedEventHandler`, `_BreakTypingSequence`, `_FlushPendingInputItems`, `_ShowCursor`; `TextEditorSelection.IsPaginated`, `_ClearSuggestedX`, `OnGotKeyboardFocus`, `OnLostKeyboardFocus`; `TextEditorLists._RegisterClassHandlers`; `TextEditorParagraphs._RegisterClassHandlers`; `_DragDropProcess(TextEditor)` constructor; `KeyboardFocusChangedEventArgs` properties; `DragEventArgs`; `DependencyPropertyChangedEventHandler`; `ShutDownListener.StopListening`; `TableColumnResizeInfo.DisposeAdorner`; `UnsafeNativeMethods` made `partial` |
| `MS.Internal/Commands/CommandHelpers.cs` | All `RegisterCommandHandler` overloads called by `TextEditor._RegisterClassHandlers` |
| `System.Windows/Input/Keyboard.cs` | `GotKeyboardFocusEvent`, `LostKeyboardFocusEvent` static fields |
| `System.Windows/Input/KeyGesture.cs` | `CreateFromResourceStrings` factory method |
| `System.Windows/WinUIFrameworkElementExtensions.cs` | `FrameworkElement.GetFrameworkParent` static extension; `UIElement.LostFocusEvent`; `AddIsEnabledChangedHandler`/`RemoveIsEnabledChangedHandler` |
| `System.Windows/WinUIDependencyObjectExtensions.cs` | `GetValueSource` extension |
| `System.Windows/WinUIDependencyPropertyExtensions.cs` | `GetDefaultValue` extension |
| `System.Windows/Controls/Primitives/TextBoxBase.cs` | `AcceptsTabProperty`, `IsReadOnlyCaretVisibleProperty` |
| `System.Windows/Controls/RichTextBox.cs` | `AutoWordSelectionProperty` |
| `System.Windows/Documents/SR.cs` | Three `TextEditor`-specific resource strings |
| `System.Windows/Documents/TextEditorCharacters.cs` | `OneFontPoint`, `MaxFontPoint` constants |
| `System.Windows/Dispatcher.cs` | `BeginInvoke(DispatcherPriority, DispatcherOperationCallback, object?)` overloads |

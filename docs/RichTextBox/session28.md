# Session 28 — Enable Upstream TextBoxBase.cs and RichTextBox.cs

## Goal
Replace the local shim stubs for `TextBoxBase` and `RichTextBox` with the actual upstream WPF source files, compiled via `#if !HAS_UNO` guards for WPF-only code paths.

## Changes

### WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj
- Removed `<Compile Remove>` entries for upstream `RichTextBox.cs` and `TextBoxBase.cs` — both now compile.
- Added `<Compile Include>` for upstream `TextChangedEventArgs.cs`.
- Deleted local shim files: `Controls/RichTextBox.cs` and `Controls/Primitives/TextBoxBase.cs`.

### New shim files

**System.Windows/Controls/Control.cs**
- `System.Windows.Controls.Control` inherits from WinUI `Control` and adds all WPF virtual event-override stubs (`OnKeyDown`, `OnMouseDown`, `OnDragEnter`, etc.) that upstream TextBoxBase overrides.
- Also: `CoerceValue`, `AddLogicalChild/RemoveLogicalChild`, `GetDpi`, `AddHandler/RemoveHandler` (taking `System.Windows.RoutedEvent`), `SetValue(DependencyPropertyKey, object?)`, `OnVisualStatePropertyChanged` (static no-op).

**System.Windows/Controls/TextBoxBaseShims.cs**
- `SpellCheck` with no-arg and `SpellCheck(object owner)` constructors.
- `InputMethod.IsInputMethodEnabledProperty` stub.

**MS.Internal/MSInternalKnownBoxes.cs**
- `MS.Internal.KnownBoxes.BooleanBoxes.{TrueBox,FalseBox}` stubs.

**MS.Internal/Controls/MSInternalControlsShims.cs**
- `EmptyEnumerator` (unchanged).

### Modified shim files

**System.Windows/WinUIDependencyObjectExtensions.cs**
- Added `SetValue(DependencyPropertyKey, object?)` extension on `DependencyObject`.
- Added 3-arg `Register(name, type, ownerType)` extension (no metadata → default null).

**System.Windows/WinUIDependencyPropertyExtensions.cs**
- Added `RegisterReadOnly` / `RegisterAttachedReadOnly` extension methods.

**System.Windows/Controls/ContextMenuShims.cs**
- Added `ScrollViewer` extension methods: `PageLeft/Right/Up/Down`, `ScrollToHome`, `ScrollToEnd`, `CanContentScroll`, `HandlesMouseWheelScrolling`.
- Added `ScrollChangedEventArgs.ViewportHeight/ViewportHeightChange/ViewportWidth/ViewportWidthChange`.
- Removed duplicate `ScrollChangedEventHandler` delegate and the invalid `ScrollChanged` event from the extension block.
- Added `AddScrollChangedHandler`/`RemoveScrollChangedHandler` static helpers (see note below).

**System.Windows/Documents/FlowDocument.cs**
- Added: `StructuralCache` (returns null), `SetDpi(DpiScale)` (no-op), `PageSizeChanged` event (no-op), `TextWrapping` property.

**System.Windows/Documents/SR.cs**
- Added: `TextBoxBase_CantSetIsUndoEnabledInsideChangeBlock`, `TextBoxBase_UnmatchedEndChange`, `TextBoxScrollViewerMarkedAsTextBoxContentMustHaveNoContent`, `TextBoxDecoratorMarkedAsTextBoxContentMustHaveNoContent`, `TextBoxInvalidTextContainer`, `RichTextBox_PointerNotInSameDocument`.

**System.Windows/System.Windows.cs**
- Added `SystemColors.HighlightTextColor`.

**System.Windows/RoutedEventArgs.cs**
- Added `protected virtual void InvokeEventHandler(Delegate genericHandler, object genericTarget)` stub (required by `TextChangedEventArgs`).

**GlobalUsings.cs**
- Added `global using Control = System.Windows.Controls.Control`.
- Added `global using LocalizabilityAttribute = System.Windows.Markup.LocalizabilityAttribute`.
- Added `global using LocalizationCategory = System.Windows.Markup.LocalizationCategory`.

### Upstream patches (ext/wpf/.../TextBoxBase.cs)

| Lines | Change |
|-------|--------|
| 58–62 | Gate `InputMethod.OverrideMetadata` and `IsMouseOverPropertyKey.OverrideMetadata` (WPF-only VSM wiring) with `#if !HAS_UNO` |
| 964–995 | Gate `ChangeVisualState` body (`VisualStates.*`, `IsMouseOver`, `IsKeyboardFocused`) |
| 1552, 1792 | Gate `ScrollChanged +=/-=` subscription (event not available in C# 14 extension block) |
| 1831–1843 | Gate `UninitializeRenderScope` body (`TextBoxView`/`FlowDocumentView` casts) |
| 1860–1864 | Gate `GetDefaultSelectionTextBrush` body; return `Brushes.White` on HAS_UNO |
| 1986–1989 | Gate `OnScrollChanged` body (`ViewportHeightChange`/`ViewportHeight`) |
| 2117–2131 | Gate caret-brush update block (`CaretElement.UpdateCaretBrush`, `InvalidateVisual`, `TextBoxView.InvalidateArrange`) |

### Upstream patches (ext/wpf/.../RichTextBox.cs)

| Lines | Change |
|-------|--------|
| 288 | Gate `OnCreateAutomationPeer` to return null on HAS_UNO |
| 293 | Gate `FlowDocument.SetDpi` call |
| 316–329 | Gate `CreateRenderScope` body; return `Grid` on HAS_UNO |
| 366–371 | Gate `StructuralCache`-based duplicate-document check |
| 554 | Replace `SingleChildEnumerator` with `EmptyEnumerator.Instance` on HAS_UNO |

## ScrollChanged — WinUI Alternative

`ScrollChanged` cannot be declared as an event in a C# 14 extension block. The WPF event fires when scroll offset, viewport size, or extent changes.

**WinUI equivalent**: `ScrollViewer.ViewChanged` (`EventHandler<ScrollViewerViewChangedEventArgs>`).

To wire up Uno-specific scroll notifications for `TextEditor.PageHeightProperty`:
1. In a future HAS_UNO-specific partial class, subscribe to `scrollViewer.ViewChanged`.
2. Construct a `ScrollChangedEventArgs` from the new state (read `ViewportHeight`, compare to previous).
3. Call `OnScrollChanged(sender, adaptedArgs)`.

For now the subscription is gated out; the page-height tracking is inactive on HAS_UNO.

## Result
Both `net10.0-desktop` and `net10.0-windows10.0.19041.0` targets build with 0 errors.

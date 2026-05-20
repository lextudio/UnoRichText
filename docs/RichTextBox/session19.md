# Session 19 — TextEditorCopyPaste + TextEditorDragDrop

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable TextEditorCopyPaste.cs and TextEditorDragDrop.cs from upstream WPF.
Clipboard is structured to eventually delegate to Uno cross-platform clipboard API.
Drag-drop is similarly structured to eventually wrap Uno's drag-drop API.

## What this gets us

Full clipboard command pipeline (Cut/Copy/Paste) and drag-drop plumbing now compile
from upstream source. The actual Clipboard and DragDrop calls are shimmed as no-ops
pending Uno wiring (Phase 3).

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for TextEditorCopyPaste.cs and TextEditorDragDrop.cs.

### EarlyBatchEditorShims.cs
- Removed stubs for TextEditorCopyPaste and TextEditorDragDrop.
- Added `AdornerLayer` stub (used by DragDrop caret management; Adorner files still excluded).
- Added `DragAction` enum with Continue/Drop/Cancel.
- Added `DragEventHandler`, `QueryContinueDragEventHandler`, `GiveFeedbackEventHandler` delegates.
- Added `DragDrop` static class with RoutedEvents (DragEnter, DragLeave, DragOver, Drop, QueryContinueDrag, GiveFeedback, Preview*) and `DoDragDrop` no-op returning None.
- Added `DragDropEffects` and `DragDropKeyStates` [Flags] enums.
- Extended `DragEventArgs`: AllowedEffects, Effects, KeyStates, Data, `GetPosition(object) => default`.
- Extended `QueryContinueDragEventArgs`: Action, KeyStates, EscapePressed.
- Extended `GiveFeedbackEventArgs`: Effects, UseDefaultCursors.
- Added `SafeNativeMethods.IsWindowEnabled(HandleRef) => true`.
- Added `UnsafeNativeMethods.SetForegroundWindow(HandleRef) => false`.

### System.Windows.cs
- Added `IDataObject` interface.
- Extended `DataObject` to implement `IDataObject`; added SetData overloads, ContainsImage, SetImage, GetFormats.
- Added DataFormats constants: Xaml, XamlPackage, Bitmap, FileDrop.
- Added `Clipboard` static class with SetDataObject/GetDataObject/SetText/GetText (no-ops pending Uno wiring).
- Added `DataObjectCopyingEventArgs`, `DataObjectPastingEventArgs`, `DataObjectSettingDataEventArgs`.

### TextEditorSystemShims.cs
- Added `SystemParameters.MinimumHorizontalDragDistance/MinimumVerticalDragDistance => 4.0`.
- Added `IWin32Window` interface and `WindowInteropHelper` stub to `System.Windows.Interop` namespace.

### ScrollShims.cs
- Extended `IScrollInfo` with scroll methods: LineUp/Down/Left/Right, PageUp/Down/Left/Right.

### ContextMenuShims.cs
- Added `ScrollInfo` extension property to `ScrollViewerWpfExtensions` returning null.

### RoutedEvent.cs
- Added 2-arg constructor `RoutedEvent(string name, Type handlerType)` and Name/HandlerType properties.

### SR.cs
- Added CopyPaste key display strings: KeyCutDisplayString, KeyCopyDisplayString, KeyPasteDisplayString,
  KeyCopyFormatDisplayString, KeyPasteFormatDisplayString, KeyCtrlInsertDisplayString,
  KeyShiftDeleteDisplayString, KeyShiftInsertDisplayString.

### XamlRtfShims.cs
- Extended `WpfPayload` stub: added Package, CreateXamlStream, OpenWpfPayload, CreateWpfPayload,
  SaveImage, LoadElement, SaveRange to satisfy CopyPaste references.

### Upstream patches (ext/wpf)

**TextEditorCopyPaste.cs**
- Gated rich-content `_CreateDataObject` block with `#if !HAS_UNO`.
- Gated `ConvertXamlToRtf` body with `#if HAS_UNO return string.Empty; #else`.
- Gated `ConvertRtfToXaml` body with `#if HAS_UNO return null; #else`.
- Gated `FrameworkCompatibilityPreferences` `when` clauses (2x) with `#if !HAS_UNO`.
- Gated `WpfPayload.SaveImage/LoadElement` Bitmap and XamlPackage paste blocks with `#if !HAS_UNO`.
- Gated `XamlReader.Load(..., useRestrictiveXamlReader: true)` with `#if HAS_UNO` (drop named arg).
- Gated `catch (XamlParseException e)` with `#if HAS_UNO catch (Exception e) #else catch (XamlParseException e)`.
- Gated `Cut/Copy(This, args.UserInitiated)` with `#if HAS_UNO ...(This, false) #else ...(This, args.UserInitiated)`.

**TextEditorDragDrop.cs**
- Gated `SourceDoDragDrop` body with `#if HAS_UNO return; #else`.
- Gated `Win32SetForegroundWindow` body with `#if !HAS_UNO`.
- Replaced `AllowDragDrop` body with `#if HAS_UNO return !IsReadOnly && ...` guard.
- Patched `_dragRect.Contains(x, y)` to `Contains(new global::Windows.Foundation.Point(x, y))`.

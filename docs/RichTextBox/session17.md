# Session 17 — TextEditor Helper Classes (Batch 1)

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable 5 upstream TextEditor helper files that were previously stub-compiled:
TextEditorParagraphs.cs, TextEditorLists.cs, TextEditorTables.cs, TextEditorContextMenu.cs, TextEditorMouse.cs.

## What this gets us

These files wire up the full paragraph/list/table/context-menu/mouse editing command pipeline.
Previously these were no-op stubs; after this session the upstream WPF implementations compile in
full, bringing ~1200 lines of real editing logic into the shim build.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for TextEditorContextMenu.cs, TextEditorMouse.cs, TextEditorTables.cs,
  TextEditorLists.cs, TextEditorParagraphs.cs.

### EarlyBatchEditorShims.cs
- Removed stubs for TextEditorMouse, TextEditorLists, TextEditorParagraphs, TextEditorContextMenu,
  TextEditorTables (all now upstream-active).
- Updated `_DragDropProcess`: replaced single `SourceOnMouseLeftButtonDown/Move` overloads with
  both `EventArgs` and `Point` signatures.
- Updated `TableColumnResizeInfo`: added `ResizeColumn(Point)` overload (upstream passes a Point).
- Added `SpellingError.Start/End/Suggestions` stub members needed by TextEditorContextMenu.

### SR.cs
- Added TextEditorContextMenu strings: Cut, Copy, Paste, NoSpellingSuggestions, IgnoreAll,
  Description_SBCSSpace, Description_DBCSSpace, More.
- Added TextEditorTables strings: KeyInsertTable, KeyInsertRows, KeyInsertColumns, KeyDeleteRows,
  KeyDeleteRowsDisplay, KeyDeleteColumns, KeyMergeCells, KeySplitCell.
- Added TextEditorLists strings: KeyRemoveListMarkers, KeyToggleBullets, KeyToggleNumbering,
  KeyIncreaseIndentation, KeyDecreaseIndentation.
- Added TextEditorParagraphs strings: KeyAlignLeft, KeyAlignCenter, KeyAlignRight, KeyAlignJustify,
  KeyApplySingleSpace, KeyApplyOneAndAHalfSpace, KeyApplyDoubleSpace.

### ContextMenuShims.cs (new file)
- PlacementMode enum, ContextMenu, MenuItem, Separator, ItemsControl, ItemCollection,
  ContextMenuService, ContextMenuEventArgs, ScrollChangedEventArgs, ScrollChangedEventHandler.
- C# 14 extension members on ScrollViewer: ScrollChangedEvent, _scrollLineDelta.

### TextEditorSystemShims.cs
- Added empty `namespace System.Windows.Interop { }` (TextEditorContextMenu imports it).
- Added `Visual3D` as `abstract partial class` in `System.Windows.Media.Media3D` with
  WINDOWS_APP_SDK-conditional base type; `IsDescendantOf(object)` returns false.
- Added `UnsafeNativeMethods.ITfCandidateList` COM interface stub.

### GlobalUsings.cs
- Added `global using ContextMenuEventArgs = System.Windows.Controls.ContextMenuEventArgs;`
  to resolve ambiguity with WinUI's `Microsoft.UI.Xaml.Controls.ContextMenuEventArgs`.

### WinUIFrameworkElementExtensions.cs
- Added ContextMenuProperty, ContextMenuOpeningEvent, ContextMenuClosingEvent static extensions.
- Added `ContextMenu` instance property extension on `FrameworkElement`.
- Added `Focusable => true` and `TransformToDescendant` instance extensions.
- Added `IsEnabled => true` to UIElement extension block.

### Input/Mouse.cs
- Added MouseButtonEventHandler, MouseEventHandler delegates.
- Added MouseDownEvent, MouseMoveEvent, MouseUpEvent static RoutedEvent fields.
- Added `GetPosition(UIElement)` and `UpdateCursor()` static methods.

### Input/Keyboard.cs
- Added `FocusedElement => null` static property.

### Input/MouseButtonEventArgs.cs
- Added `MouseButton` enum, `ChangedButton`, `GetPosition(object?)` method.

### Input/MouseEventArgs.cs
- Added `LeftButton/RightButton/MiddleButton` properties, `GetPosition(object?)` method.

### Input/EditingCommands.cs
- Added `CorrectSpellingError` and `IgnoreSpellingError` commands.

### Input/ApplicationCommands.cs
- Added `CorrectionList` command.

### Media/WinUIMediaExtensions.cs
- Added `TryTransform` and `Inverse` to GeneralTransform.

### Controls/Primitives/TextBoxBase.cs
- Added `ViewportHeight => 0.0`.

### TextEditorCharacters.cs
- Added 4-arg `_OnApplyProperty` overload (TextEditorParagraphs uses 4 args).

### Upstream patches (ext/wpf)

**TextEditorMouse.cs**
- `IsPointWithinRenderScope`: gated body with `#if HAS_UNO return true; #else`.

**TextEditorContextMenu.cs**
- Gated PresentationSource/IWin32Window/Win32 block with `#if !HAS_UNO`.
- Gated `VisualTreeHelper.GetClip` with `#if !HAS_UNO` / `#else Geometry clip = null;`.
- Gated `visual.TransformToDescendant` with `#if HAS_UNO element.TransformToDescendant(...) #else`.
- Gated ITfCandidateList.SetResult with `#if !HAS_UNO`.
- Gated ThemeManager.IsFluentThemeEnabled block with `#if !HAS_UNO`.
- Gated `AddReconversionItems` body with `#if HAS_UNO return false; #else`.
- Gated `DelayReleaseCandidateList` body with `#if !HAS_UNO` (Dispatcher.CurrentDispatcher unavailable).

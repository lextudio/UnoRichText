# Session 24 — TextMapOffsetErrorLogger, TextRangeEditTables, TextFindEngine

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Promote three upstream files that were deferred, clean up their dependencies,
and add shims so no net new conditional-compilation guards accumulate in the
non-Fixed, non-Speller paths of `TextRangeEditTables`.

## What this gets us

- `TextMapOffsetErrorLogger.cs` — debug/trace logger for the spell-checker;
  gated `#if !HAS_UNO` since it is a `partial class Speller` extension.
- `TextRangeEditTables.cs` (upstream) — full WPF table editing semantics:
  insert/delete rows, columns, merge/split cells, column-range selection.
  The column-resize adorner path (`TableBorderHitTest` private body) uses a
  one-block `#if !HAS_UNO` guard; all other methods compile unconditionally.
- `TextFindEngine.cs` — text-find engine; Fixed-layout branch gated
  `#if !HAS_UNO`; Win32 NLS/string methods already stubbed.
- `ColumnResizeUndoUnit.cs` — undo unit for column resize; re-enabled after
  the `ColumnResize*.cs` glob was excluding it.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for `TextMapOffsetErrorLogger.cs`,
  `TextRangeEditTables.cs` (upstream), `TextFindEngine.cs`.
- Removed local-stub remove comment for `TextRangeEditTables.cs`.
- Added `<Compile Include>` for `ColumnResizeUndoUnit.cs` after the
  `ColumnResize*.cs` glob remove so the undo unit is compiled.

### EarlyBatchEditorShims.cs
- Deleted the `TextRangeEditTables` stub class (all methods); replaced by
  upstream file.

### TextEditorSystemShims.cs
- Added `MS.Internal.Documents.TextDocumentView` stub — bare class with
  `GetCellInfoFromPoint` returning null; used only for a type-check in
  `TextRangeEditTables.TableBorderHitTest`.
- Added `MS.Internal.PtsHost.CellInfo` stub — properties `TableArea`,
  `CellArea`, `Cell`, `TableAutofitWidth`, `TableColumnWidths`.
- Added `System.Windows.Documents.Internal.ColumnResizeAdorner` stub —
  `Initialize`, `Uninitialize`, `Update` methods; satisfies the type reference
  without implementing WPF adorner infrastructure.

### EarlyBatchEditorShims.cs — UnsafeNativeMethods
- Added 7-param `FindNLSString` overload matching WPF's P/Invoke signature
  used by `TextFindEngine`; returns -1 / foundLength=0 (stub).

### Upstream patches (ext/wpf)

**TextMapOffsetErrorLogger.cs**
- Entire file body wrapped with `#if !HAS_UNO ... #endif` — it is a
  `partial class Speller` extension; Speller is excluded on HAS_UNO.

**TextRangeEditTables.cs**
- `using System.Windows.Documents.Internal;` preserved — `ColumnResizeAdorner`
  is now shimmed.
- Private `TableBorderHitTest` body: the section after the
  `is TextDocumentView` early-return is wrapped with `#if !HAS_UNO ... #endif`
  (one guard). On HAS_UNO the `is` check always returns false before
  reaching the gated cast and PtsHost calls.

**TextFindEngine.cs**
- Fixed-layout path (`DocumentSequenceTextPointer` / `FixedTextPointer` /
  `FixedFindEngine`) gated with `#if !HAS_UNO`.

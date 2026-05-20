# Session 23 — Table Types (Table, TableRow, TableCell, TableRowGroup, collections)

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Promote the 7 upstream WPF Table type files, removing all hand-rolled Table*
stubs that were blocking them.

## What this gets us

- Full upstream `Table`, `TableRow`, `TableCell`, `TableRowGroup` and their
  three collection classes (`TableCellCollection`, `TableRowCollection`,
  `TableRowGroupCollection`) are now compiled from ext/wpf.
- WPF table semantics — column spans, row groups, logical-parent wiring — are
  provided by upstream source rather than a thin stub.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for:
  `Table.cs`, `TableCell.cs`, `TableCellCollection.cs`, `TableRow.cs`,
  `TableRowCollection.cs`, `TableRowGroup.cs`, `TableRowGroupCollection.cs`.

### TableAndHighlightShims.cs
- Removed all Table* stub classes (Table, TableRowGroupCollection, TableRowGroup,
  TableRowCollection, TableRow, TableCell, TableCellCollection).
- Retained only `HighlightsCollection` (which has no upstream equivalent).

### FrameworkElement.cs (FrameworkContentElement)
- Added `internal virtual void OnNewParent(DependencyObject newParent) { }`
  — upstream TableRow, TableCell, and TableRowGroup override this method to
  wire up parent back-references; the base declaration was missing from the shim.

### EarlyBatchEditorShims.cs (InsertTable)
- Removed direct assignments to the read-only `TableRow.RowGroup` and
  `TableCell.Row` properties (which are computed from the logical parent in
  upstream, not settable).
- `Rows.Add(row)` and `Cells.Add(cell)` now establish parent relationships
  automatically, as in real WPF.

### No upstream patches required
- The 7 Table type files compiled without `#if HAS_UNO` guards — no
  platform-specific code paths in these files.

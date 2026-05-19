# Session 12 — Promoting Upstream `TextSchema.cs`

Status: done.

## Goal

Pick a focused, high-value upstream file from the Phase 1 slice (per
[design.md](design.md)) and bring it from `<Compile Remove>` to active
`<Compile Include>` with whatever minimal shims it needs. `TextSchema.cs`
chosen because:

- It is consumed by every existing enabled file (`TextElement`, `Span`,
  `InlineCollection`, `Run`, `Hyperlink`, …) — they currently rely on a
  66-line local stub that returns `true` for almost everything.
- Upstream is self-contained (1,221 lines, no PTS / structural-cache
  dependencies) — unlike `FlowDocument.cs` which transitively requires
  the entire WPF text layout engine.
- It is also pulled in by every Phase 1 file still ahead of us
  (`TextEditor*.cs`, `TextRangeEdit.cs`, `TextEditorSelection.cs`).

## What this gets us

Real WPF schema semantics replace permissive stubs:

- `TextSchema.IsValidChild(parent, child)` now enforces the actual content
  model (`Inline` children only in `Span`/`Paragraph`; nested `Hyperlink`
  rejected; `Block`-vs-`Inline` rules; ListItem/Table/TableCell topology).
- `TextSchema.HasHyperlinkAncestor` / `HasIllegalHyperlinkDescendant` now
  reject nested hyperlinks instead of always returning `false`/`true`.
- `TextSchema.GetInheritableProperties` / `GetNoninheritableProperties`
  now return the real DP lists used by clipboard/serialization paths.
- `IsFormattingType` / `IsMergeableInline` / `IsCharacterProperty` / 
  `IsParagraphProperty` now use the upstream tables instead of two-property
  stubs.

This is the schema foundation `TextRangeEdit` and `TextEditor*` depend on.

## Changes

### Local shim removed

`WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextSchema.cs`
(66-line return-true stub) deleted.

### Upstream file un-excluded

[LeXtudio.Windows.csproj](../../../WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj):
removed the `<Compile Remove>` line targeting upstream `TextSchema.cs`;
added `<Compile Remove="System.Windows\Documents\TextSchema.cs" />` for the
deleted local file path.

### Upstream `TextSchema.cs` patched (guarded with `#if !HAS_UNO`)

Only one new patched-file is touched (Session 10's pattern). The patches
gate references to WPF-only DependencyProperties that don't exist on
WinUI/Uno types:

- **NumberSubstitution.{CultureSource,Substitution,CultureOverride}Property**
  (static ctor, inheritable text-element property list).
- **AccessText** (control type — WPF-only sibling of `TextBlock`).
- **`System.Windows.UIElement`** module check in `IsKnownType` — under
  `HAS_UNO` falls back to `typeof(Microsoft.UI.Xaml.UIElement).Module`.
- **TableCell.{ColumnSpan,RowSpan,Padding,BorderThickness,BorderBrush}Property**
  and **Table.CellSpacingProperty** (non-inheritable property tables — the
  `Table*.cs` shims don't register these DPs).
- **FrameworkElement.{LayoutTransform,Cursor,ForceCursor,ToolTip}Property**,
  **FrameworkContentElement.ToolTipProperty**,
  **UIElement.{OpacityMask,BitmapEffect,BitmapEffectInput,ClipToBounds,
  SnapsToDevicePixels}Property**, **TextBlock.BaselineOffsetProperty**,
  **Image.StretchDirectionProperty** — none of these exist on the WinUI
  equivalents.

### Image: WinUI, not WPF shim

Per design feedback, the patched file aliases `Image` to
`Microsoft.UI.Xaml.Controls.Image` (not a WPF shim type) for the
`_imagePropertyList` table:

```csharp
#if HAS_UNO
using Image = Microsoft.UI.Xaml.Controls.Image;
#endif
```

WinUI `Image` exposes the load-bearing `SourceProperty` and
`StretchProperty`; only `StretchDirectionProperty` is WPF-only and gated.

### SR

Added the missing `SR.TextSchema_IllegalHyperlinkChild` entry referenced
twice from upstream's hyperlink-descendant validation paths.

## Verification

```
Test Count: 60, Passed: 60, Failed: 0, Skipped: 0
```

No regressions in the runtime suite. WindowsShims builds clean against the
real upstream `TextSchema.cs`, and all already-enabled files (`TextElement`,
`Span`, `InlineCollection`, `Run`, `Hyperlink`, …) now resolve `TextSchema`
to the real implementation instead of the stub.

## Notes on patching discipline

All edits to upstream `TextSchema.cs` are `#if !HAS_UNO`-guarded so the
file remains valid against a WPF build of the source tree and the upstream
diff stays minimal — same pattern Session 10 established for
`TextElement.cs`.

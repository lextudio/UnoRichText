# Session 30 — Florence Layout Engine

## Goal
Create the **Florence** cross-platform layout engine to replace WPF's PTS (Page/Text Services)
engine on HAS_UNO, and use it to remove `#if !HAS_UNO` gates from `FlowDocument.cs` that only
existed because PTS types were missing.

## Architecture

```
FlowDocument / TextBoxBase  (upstream WPF source)
        ↓
Florence API surface  (StructuralCache, IFlowDocumentFormatter, …)
        ↓
Florence layout model  (FlorenceDocument → FlorencePage → FlorenceLine)
        ↓
Text arrangement  (future: Preedit / word-wrap integration)
```

## New file

**`src/LeXtudio.Windows/MS.Internal/Florence/FlorenceEngine.cs`**

A single file providing all PTS-equivalent types in their original WPF namespaces so that
`#if !HAS_UNO` guards can be removed without changing calling code.

### `MS.Internal.Florence` — layout model

| Type | Description |
|------|-------------|
| `FlorenceLine` | A single laid-out line: start offset, length, baseline, height |
| `FlorencePage` | A list of `FlorenceLine` objects for one page |
| `FlorenceDocument` | Top-level model: list of `FlorencePage`; `Invalidate()` clears all |

### `MS.Internal.PtsHost` — PTS API surface

| Type | Description |
|------|-------------|
| `StructuralCache` | Holds real `TextContainer` + `FlorenceDocument`; formatting state is always "not yet formatted" (IsFormattedOnce = false) so WPF guard checks exit early |
| `FlorenceTextFormatterHost` | Owns per-DPI context (`PixelsPerDip`); stub for future text-shaping pass |
| `DirtyTextRange` | Marks a changed `[cpFirst, cchAdded, cchDeleted]` span; `StructuralCache.AddDirtyTextRange` calls `FlorenceDocument.Invalidate()` |

### `MS.Internal.Documents` — formatter API surface

| Type | Description |
|------|-------------|
| `IFlowDocumentFormatter` | `Suspend()`, `OnContentInvalidated(bool)`, `OnContentInvalidated(bool, ITextPointer, ITextPointer)`, `IsLayoutDataValid` |
| `FlowDocumentFormatter` | Bottomless (scrollable) formatter; `TODO` marker for line-breaking pass |
| `FlowDocumentPaginator` | Extends `DocumentPaginator`; `TODO` marker for pagination pass |

## FlowDocument.cs gates removed

These `#if !HAS_UNO` gates were eliminated because Florence provides the required types:

| Section | Change |
|---------|--------|
| `_structuralCache`, `_formatter` private fields | Always present; `_pixelsPerDip` still gated |
| `Initialize(TextContainer)` | Creates real `StructuralCache(this, textContainer)` on both platforms; `new TextContainer(…)` still gated (HAS_UNO has null) |
| `BottomlessFormatter` property | Un-gated — creates `FlowDocumentFormatter` |
| `StructuralCache` property | Un-gated |
| `Formatter` property | Un-gated |
| `IsLayoutDataValid` | Un-gated |
| `TextContainer` property | Un-gated — returns `_structuralCache.TextContainer` (null until TextContainer is wired) |
| `PixelsPerDip` | HAS_UNO path returns 1.0 / no-op setter |
| `OnPropertyChanged` | HAS_UNO path calls `InvalidateFormatCache` + `OnContentInvalidated` |
| `SetDpi` | HAS_UNO path calls `_formatter?.OnContentInvalidated(true)` |
| `OnPageMetricsChanged` | HAS_UNO fires `PageSizeChanged` via the `else` branch |
| `IDocumentPaginatorSource.DocumentPaginator` | Un-gated — returns `FlowDocumentPaginator` |

## SR.cs addition

Added `FlowDocumentInvalidContnetChange` (preserving upstream typo) to `SR.cs`.

## Still gated on HAS_UNO

The following remain behind `#if !HAS_UNO` because they require a live `TextContainer`:

- `FlowDocument.ContentStart` / `ContentEnd`
- `FlowDocument.LogicalChildren`
- `FlowDocument.InitializeForFirstFormatting` / `Uninitialize`
- `FlowDocument.OnChildDesiredSizeChanged`
- `FlowDocument.GetObjectPosition` (TextContainerHelper / ContentPosition.Missing)
- `FlowDocument.IAddChild.AddChild` (Block.RepositionWithContent)
- `FlowDocument.IServiceProvider.GetService`
- `FlowDocument.OnHighlightChanged` / `OnTextContainerChanging` / `OnTextContainerChange`
- `RichTextBox.Document` setter (`InitializeTextContainer`)
- `FlowDocument.Initialize` — `new TextContainer(…)` creation block

## Future work (Session 31+)

1. **Wire Preedit** — plug a word-wrap / line-breaking pass into
   `FlowDocumentFormatter.OnContentInvalidated` to populate `FlorenceDocument.Pages`.
2. **FlorenceTextContainer** — a simple `ITextContainer` implementation so
   `ContentStart`/`ContentEnd` return live `ITextPointer` values on HAS_UNO.
3. **Un-gate `InitializeForFirstFormatting`** — once `FlorenceTextContainer` has working
   `Changing` / `Change` events.

## Result
Both `net10.0-desktop` and `net10.0-windows10.0.19041.0` build with 0 errors, 0 warnings.
Florence is in place as the layout engine foundation for HAS_UNO.

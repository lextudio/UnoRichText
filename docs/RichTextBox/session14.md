# Session 14 — Promoting Upstream `TextRangeBase.cs`

Status: done.

## Goal

Replace the hand-rolled stub `TextRangeBase` (lived in
[`HyperlinkSupport.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/HyperlinkSupport.cs),
~250 lines including the Session 11 `GetText` pointer-walker) with the
real upstream 2,086-line `TextRangeBase.cs`. This consolidates the
walker logic into the same code WPF ships, and is a prerequisite for
promoting `TextEditor*` / `TextRangeEdit` next.

## What this gets us

- Real WPF text serialization for plain-text via `TextRangeBase.GetText`
  / `GetTextInternal` (the Session 11 hand-roll was a close approximation;
  upstream covers more edge cases — line breaks, list items, anchored
  blocks, etc.).
- Real change-block semantics: `BeginChange` / `EndChange` /
  `NotifyChanged` / `GetChangeBlockLevel` now back the `ITextRange`
  surface that `TextEditor` and `TextEditor*` files call into.
- `Select` / `SelectWord` / `SelectParagraph` / `ApplyTypingHeuristics` /
  `IsParagraphBoundaryCrossed` / `NormalizeRange` are now real upstream
  code instead of empty stubs.
- `TextRangeBase.GetStart` / `GetEnd` / `GetIsEmpty` / `GetTextSegments`
  read real internal state instead of using my Session 11 workaround
  (`_TextSegments` interface back-door).

## Changes

### Stub removed

The `public static class TextRangeBase { … }` block (lines 28–275) inside
[`HyperlinkSupport.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/HyperlinkSupport.cs)
deleted. File went from 287 → 38 lines (now just `FixedPage`,
`BaseUriHelper`, `RequestSetStatusBarEventArgs`).

### Upstream file un-excluded

[LeXtudio.Windows.csproj](../../../WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj):
the `<Compile Remove>` for upstream `TextRangeBase.cs` removed.

### Upstream `TextRangeBase.cs` patched (`#if !HAS_UNO`)

Clipboard / XAML serialization paths gated:

- `PlainConvertAccessKey` (lines 859–861) — `AccessText` is WPF-only.
- `GetXml` body → returns `string.Empty` under HAS_UNO (no
  `TextRangeSerialization.WriteXaml` in the shim).
- `CanSave` / `CanLoad` → `DataFormats.Text` only under HAS_UNO.
- `Save` body — only the `DataFormats.Text` arm under HAS_UNO; XAML /
  XAML-package / RTF arms gated (`WpfPayload.SaveRange`,
  `TextEditorCopyPaste.ConvertXamlToRtf`).
- `Load` body — symmetric: text arm only; XAML / package / RTF arms
  gated.

These are all clipboard/persistence concerns, not editing semantics.
They land naturally when `WpfPayload.cs` / `TextEditorCopyPaste.cs` /
`TextRangeSerialization.cs` are promoted later.

### Small shim additions

- `MergeFlowDirection(TextPointer)` no-op stub added to local
  [`TextRangeEdit.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextRangeEdit.cs).
  Called from `TextRangeBase.SetText` after content deletion.
- 4-arg `BuildTableRange(TextPointer anchor, TextPointer moving, bool includeCellAtMovingPosition, out bool isTableCellRange)`
  stub added to `TextRangeEditTables` in [`EarlyBatchEditorShims.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/EarlyBatchEditorShims.cs).
  Returns `null` and `false` — preserves WPF semantics where a
  non-table range builds the default single segment.
- Two new SR strings:
  `TextRange_UnsupportedDataFormat`, `TextRange_UnrecognizedStructureInDataFormat`.

## Verification

```
Test Count: 62, Passed: 62, Failed: 0, Skipped: 0
```

All Session 11–13 acceptance tests stay green against the real upstream
implementation, including:

- `Text_ReturnsRunPlainText` — single-run plain text.
- `Text_ReturnsPlainTextAcrossParagraphs` — whole-document range.
- `Text_ReturnsSlicedPlainTextAcrossParagraphs` — partial cross-paragraph
  range returns `"ha\r\nBe"`.

Notably the Session 11 `_TextSegments` interface-back-door workarounds
in `Select`/`GetStart`/`GetEnd`/`GetIsEmpty`/`GetTextSegments` are
retired: those calls now run real upstream code that reads the
collection's segments directly.

## Next slice

`TextRangeEdit.cs` (2,382 lines — range-level Bold/Italic/Underline
application primitives) is the natural follow-on. It's currently a
130-line local stub and is the gate to promoting `TextEditorCharacters.cs`
(character-formatting commands) once `TextEditor.cs` is in.

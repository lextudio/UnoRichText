# Session 15 — Promoting Upstream `TextRangeEdit.cs`

Status: done.

## Goal

Replace the ~50-line local `TextRangeEdit` stub with upstream's 2,382-line
implementation. `TextRangeEdit` owns the range-mutation primitives that
every formatting command relies on — `SetInlineProperty`,
`SetParagraphProperty`, `SplitFormattingElements`, `InsertParagraphBreak`,
`InsertLineBreak`, `CharacterResetFormatting`, etc. It's the immediate
gate for `TextEditor*` formatting commands.

## What this gets us

- Bold / Italic / Underline / Foreground / FontSize / Alignment etc. now
  have real WPF implementations behind their range-application primitives,
  instead of no-op stubs.
- Paragraph merge / split / line-break insertion follow WPF semantics.
- `TextEditorCharacters.cs` (482 lines) becomes reachable in a future
  session — it calls into the methods we just promoted.

## Changes

### Local shim deleted

`WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextRangeEdit.cs`
(~50-line stub) deleted.

### csproj flip

[`LeXtudio.Windows.csproj`](../../../WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj):
upstream `TextRangeEdit.cs` un-excluded; local path now in the remove list.

### New extension methods (Session 10 / 12 pattern)

Upstream calls into two `DependencyObject`/`DependencyProperty` APIs that
don't exist on Uno/WinUI. Both added as C# 14 extension members so the
upstream source compiles unchanged:

- `WinUIDependencyObjectExtensions.GetValueSource(dp, metadata, out hasModifiers)`
  — returns `BaseValueSourceInternal.Default` vs `Local` based on whether
  `ReadLocalValue` reports `UnsetValue`. Inheritance/animations not modeled.
- `WinUIDependencyPropertyExtensions.GetDefaultValue(Type forType)` —
  reads `PropertyMetadata.DefaultValue` via `dp.GetMetadata(forType)`.

### Small new stubs

- [`TextRangeEditLists.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextRangeEditLists.cs):
  `SplitListsForFlowDirectionChange` → `true`, `MergeParagraphs` → no-op.
  Upstream `TextRangeEditLists.cs` isn't promoted yet; only the entry
  points `TextRangeEdit` calls are stubbed.
- [`TextEditorCharacters.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextEditorCharacters.cs):
  `OneFontPoint = 1.0`, `MaxFontPoint = 32767.0` constants only.
  Upstream `TextEditorCharacters.cs` isn't promoted yet; commands are
  out of scope for this slice.

### SR

- `TextRangeEdit_InvalidStructuralPropertyApply` added.

## Verification

```
Test Count: 62, Passed: 62, Failed: 0, Skipped: 0
```

All Session 11–14 acceptance tests still pass. No new tests added —
formatting commands need `TextEditor*` to be observable from a test, which
is the next session's scope.

## Next slice

`TextEditor.cs` (2,054 lines, the editor controller) plus optionally
`TextEditorCharacters.cs` (482 lines, Bold/Italic/Underline commands)
once `TextEditor` is in. After those, `TextSelection.cs` and
`RichTextBox.cs` (the upstream WPF control) become reachable, which is
the Phase 1 acceptance target from [design.md](design.md).

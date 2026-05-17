# Milestone: Delta editing — typed characters preserve and inherit character format

## Context

`RichEditBox` today uses a host `TextBox` as the source of truth and treats `Document` as a shadow that gets rebuilt from scratch on every keystroke. Two concrete defects fall out of that model:

1. **`RichEditTextDocument.SetText` is a full reset.** It clears `_characterFormatRuns` on every call ([`RichEditTextDocument.cs:119-127`](../src/LeXtudio.RichText/TextDocument/RichEditTextDocument.cs#L119-L127)).
2. **Every keystroke calls `SetText` on the whole document.** `OnEditorHostTextChanged` resyncs by doing `Document.SetText(None, _editorHost.Text)` ([`RichEditBox.cs:298-315`](../src/LeXtudio.RichText/Controls/RichEditBox.cs#L298-L315)). All format runs are wiped on every character typed.

A third bug in the same family: `RichEditTextRange.Text` setter mutates `_buffer` directly without touching `_characterFormatRuns` ([`RichEditTextRange.cs:104-128`](../src/LeXtudio.RichText/TextDocument/RichEditTextRange.cs#L104-L128)), so any caller that goes through the `Range.Text = …` path corrupts run offsets.

User-visible symptom: select "Hi", press Ctrl+B (bold applied), type "j" — the bold is gone and the document is plain "Hij" (or "j", depending on whether the host TextBox preserves the selection).

There is also no concept of an **input format** — pressing Ctrl+B with a collapsed selection has no effect on subsequently typed text.

## Goals (in scope)

1. Typed characters keep the format runs that surround them.
2. Selecting a bold range, pressing Ctrl+B, then typing replaces the selection with bold characters.
3. Pressing Ctrl+B with no selection then typing makes the typed characters bold (input format).
4. The Document gets primitive delta operations (`InsertText`, `DeleteRange`). Existing callers that mutate text (`Range.Text` setter, `Selection.TypeText`) route through them.
5. Tests covering the run-splicing edge cases.

## Out of scope

- Undo / redo for the new edits.
- IME composition handling — only direct keystroke-by-keystroke text mutations.
- Paste / clipboard format preservation (`LoadFromStream`, RTF).
- Paragraph splits (Enter key creating new `<Paragraph>` blocks).
- Performance optimization for very large documents (the current scan-all-runs approach is fine for hundreds of runs).

## API additions

All new methods are **internal** — accessed by `RichEditBox`, `RichEditTextRange`, `RichEditTextSelection`.

| Member | Notes |
|---|---|
| `RichEditTextDocument.InsertText(int offset, string text, TextCharacterFormat? format = null)` | Inserts text at `offset` and updates runs. If `format` is null, the inserted text inherits whichever run the offset falls inside (or the document default if none). |
| `RichEditTextDocument.DeleteRange(int start, int end)` | Removes `[start, end)` from the buffer and shifts/trims runs. |

`SetText` (full-document replace) is unchanged — clearing runs on a full replace is correct for that operation.

## Behavior changes

### `RichEditTextRange.Text` setter

Today: directly mutates `_buffer`, leaving `_characterFormatRuns` stale.
New: routes through `DeleteRange + InsertText` so runs stay consistent.

### `RichEditTextSelection.TypeText`

Today: assigns `Text = value` (broken via the above).
New: calls `Document.InsertText` directly so the input-format path is exercised.

### `RichEditBox.OnEditorHostTextChanged`

Today: full-document `Document.SetText(None, _editorHost.Text)` on every change.
New: compute the prefix/suffix common length between the old document text and the new host text to derive a `(start, deletedLen, insertedText)` triple, snapshot the format at the deleted range (so a replace-selection scenario inherits the right format), then `DeleteRange + InsertText(format)`.

This works for single-character typing, paste-as-plain, backspace, and selection-replace — anything the host TextBox emits as a `TextChanged`.

### Input format

`ApplyCharacterFormat` already updates `_defaultCharacterFormat` when called with `start == end` ([`RichEditTextDocument.cs:163-169`](../src/LeXtudio.RichText/TextDocument/RichEditTextDocument.cs#L163-L169)). The new `InsertText` consults this when no explicit format is passed and no surrounding run extends to the insertion point.

WinUI distinguishes a *default character format* (document-wide) from an *input format at caret* (cleared when the caret moves). For this milestone we conflate them — sufficient for toolbar / accelerator toggles. A separate "caret input format" field can layer in later when behavior needs to diverge (see follow-up milestones).

## Algorithm sketch

### Insert: shift surrounding runs

For each run, given an insertion of length `delta` at `offset`:

- `run.End < offset` → no change.
- `run.Start < offset <= run.End` → extend, `re += delta` (the inserted text inherits this run's format by default).
- `run.Start >= offset` → shift, `rs += delta; re += delta`.

The boundary cases are picked so that an insertion at `offset == run.End` inherits the *left* run's format (matches WinUI convention) and an insertion at `offset == run.Start` does **not** inherit the run that starts at `offset`.

### Insert: explicit format

After shifting, if a non-null `format` was provided, splice a new run `[offset, offset+delta)` with that format. Surrounding runs whose interval overlaps the new run are clipped to `[..offset)` and `[offset+delta..)`; matching adjacent runs are merged.

### Delete: shift/trim runs

For each run, given a deletion of `[start, end)`:

- entirely before start (`run.End <= start`): keep as-is.
- entirely at or after end (`run.Start >= end`): shift, `rs -= (end-start); re -= (end-start)`.
- overlapping: keep `leftKeep = max(0, start - rs)` chars before and `rightKeep = max(0, re - end)` chars after, anchored at `newStart = min(rs, start)`, so the resulting run is `[newStart, newStart + leftKeep + rightKeep)`. Discard if length zero.

Run order is preserved; adjacent matching runs are merged.

## Test matrix

| Case | Setup | Action | Expected |
|---|---|---|---|
| Insert in middle of bold run inherits | "Hello" with `[1..4 Bold]` | `InsertText(2, "X")` | "HeXllo", runs=`[1..5 Bold]` |
| Insert with explicit different format splits run | "Hello" with `[0..5 BoldOff]` | `InsertText(2, "X", BoldOn)` | "HeXllo", runs=`[0..2 Off][2..3 On][3..6 Off]` |
| Insert at run start does not inherit | "Hello" with `[2..5 Bold]` | `InsertText(2, "X")` | "HeXllo", runs=`[3..6 Bold]` |
| Insert at run end inherits | "Hello" with `[0..2 Bold]` | `InsertText(2, "X")` | "HeXllo", runs=`[0..3 Bold]` |
| Delete inside a run | "HHHH" with `[0..4 Bold]` | `DeleteRange(1, 3)` | "HH", runs=`[0..2 Bold]` |
| Delete across two runs | "AB" with `[0..1 Bold]+[1..2 Italic]` | `DeleteRange(0, 2)` | "", runs=`[]` |
| Delete trims left and shifts right | "abcdef" with `[2..5 Bold]` | `DeleteRange(0, 3)` | "def", runs=`[0..2 Bold]` |
| Replace-selection inherits deleted format | "BIG" with `[0..3 Bold]` | snapshot fmt, `DeleteRange(0,3)`, `InsertText(0,"j",fmt)` | "j", runs=`[0..1 Bold]` |
| Input format from Ctrl+B-no-selection | empty doc; `Selection.CharacterFormat.Bold = Toggle` (length 0); `InsertText(0,"x")` with default-fmt resolution | | "x", runs=`[0..1 Bold]` |

## Code locations

- `src/LeXtudio.RichText/TextDocument/RichEditTextDocument.cs` — `InsertText`, `DeleteRange`, splice helpers.
- `src/LeXtudio.RichText/TextDocument/RichEditTextRange.cs` — `Text` setter rewritten.
- `src/LeXtudio.RichText/TextDocument/RichEditTextSelection.cs` — `TypeText` rewritten.
- `src/LeXtudio.RichText/Controls/RichEditBox.cs` — `OnEditorHostTextChanged` delta computation.
- `src/LeXtudio.RichText.Tests/Documents/RichEditTextDocumentTests.cs` — new tests for the matrix above.

## Acceptance

1. `dotnet test` passes including the new test matrix.
2. In the sample app's "RichEditBox Gallery" → "A custom editor with RichEditBox" card:
   - Type "Hi", select it, Ctrl+B → "Hi" is bold.
   - Type "j" — "j" is bold and "Hi" remains bold.
   - Press Right (collapse selection at end), Ctrl+B (input format off), type "k" — "k" is not bold.

## Follow-up milestones

- **M2: Undo/redo for delta operations.** Push (start, deletedText, insertedText, deletedRuns) tuples on a stack; replay on Undo.
- **M3: Distinguish default vs. caret input format.** Add `_caretInputFormat` that gets cleared when the caret moves; `InsertText` prefers it over `_defaultCharacterFormat`.
- **M4: IME composition support.** Handle `TextCompositionStarted` / `TextCompositionChanged` / `TextCompositionEnded` instead of relying on `TextChanged` deltas alone.
- **M5: Paragraph splits.** Enter creates a new paragraph element in the document tree, runs respect paragraph boundaries.
- **M6: Paste-with-format.** Recognize HTML / RTF on the clipboard and round-trip runs.

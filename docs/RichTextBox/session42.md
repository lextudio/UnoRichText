## Session 42 - RichTextBox editing evaluation with Uno DevFlow

This session focuses on accelerating runtime validation for the WPF-shaped `RichTextBox` sample by adding direct edit actions and documenting current shim boundaries.

### Runtime evaluation

- Sample build passes on `net10.0-desktop`.
- App launches and remains running from `dotnet run` (expected UI behavior).
- Existing selection telemetry is active (`SelectionChanged` + `%TEMP%/rtb-template.log`).

### Implemented in sample (this repo)

Updated `RichTextBox` tab controls in `LeXtudio.RichText.Sample`:

- Added action buttons to the WPF RichTextBox preview toolbar:
  - `Bold`
  - `Italic`
  - `Underline`
  - `Insert timestamp`
- Implemented `Insert timestamp` as a working edit operation:
  - Inserts at current selection/caret via `Selection.Text`.
  - Refocuses editor and refreshes selection status/log.

### Current shim boundary observed

- Style toggles (`Bold`/`Italic`/`Underline`) are currently wired as explicit placeholders (`not-implemented` logs) because the dependency-property surface exposed by the current shim for this sample target did not match WPF API assumptions during compilation.
- This keeps the sample buildable while preserving visible action points for the next engine/shim wiring pass.

### Files changed

- `src/LeXtudio.RichText.Sample/MainPage.xaml`
  - Added editing action buttons in the RichTextBox preview toolbar.
- `src/LeXtudio.RichText.Sample/MainPage.cs`
  - Added click handlers for the new actions.
  - Implemented timestamp insertion editing path.
  - Added explicit placeholder logging for style toggles.

### Verification

- `dotnet build src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop`
- `dotnet run --project src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop`

### Next session target

1. Wire `Bold`/`Italic`/`Underline` to the exact dependency-property types exposed by `LeXtudio.Windows` on this target.
2. Re-run DevFlow interaction scripts for selection + formatting + insertion and capture regression notes.

### Rendering stale-cache investigation

User-observed symptom: after `Select all` followed by `Insert timestamp`, the selection/status model reported the timestamp text, but the final visual still showed the original document text with a highlight rectangle that matched the new timestamp range.

Root cause confirmed:

- `Selection.Text = ...` mutates the WPF text container correctly.
- The selection/highlight layer can refresh independently from the formatted document visual.
- `FlowDocumentView` cached the Florence `_page` and only rebuilt it for first measure or size changes.
- Text edits therefore left the old `_page` in place, so the stale original text stayed rendered while selection geometry reflected the new document state.

### Runtime/shim changes implemented

- `FlowDocumentView` now acts as the `ITextLayoutHost` for the document and exposes layout invalidation to the text stack.
- `FlowDocumentView.InvalidateDocumentLayout()` clears cached Florence page state, marks selection dirty, invalidates the Uno text view, and requests measure/arrange again.
- `UnoFlowDocumentTextView.OnLayoutInvalidated()` marks the text view invalid so caret/selection geometry is recomputed with the next formatted page.
- `RichTextBox.uno.cs` invalidates the `FlowDocumentView` from:
  - `OnTextContainerChanged`
  - `OnSelectionChanged`
- `OnTrackedSelectionChanged` in `FlowDocumentView` also invalidates document layout if the text-container generation changed; otherwise it keeps the cheaper selection-only arrange path.

### Sample/DevFlow support changes

- `LeXtudio.RichText.Sample` can now force `LeXtudio.RichText` to use local RichText dependencies while keeping DevFlow and PropertyGrid as NuGet packages:
  - `UseLocalRichTextDependencies=true`
  - direct local project references for `LeXtudio.Windows` and `LeXtudio.TextBox`
- The sample has a post-build copy target to overwrite transitive packaged `LeXtudio.Windows.dll`/TextBox DLLs in the app output with local project outputs. This was necessary because packaged sample dependencies can otherwise overwrite the local shim assembly in `bin`.
- Added `UNO_RTB_AUTOTEST=1` sample mode. It switches to the RichTextBox tab, runs Select All, inserts a timestamp, logs the layout, and exits. Normal sample behavior is unchanged unless the environment variable is set.

### Verification results

Commands:

```powershell
dotnet build src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop
$env:UNO_RTB_AUTOTEST='1'
dotnet run --project src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop --no-build
Remove-Item Env:UNO_RTB_AUTOTEST
```

Relevant `%TEMP%/rtb-template.log` evidence:

- Select All invalidated layout and still formatted the original document as 7 lines, as expected before the edit.
- Timestamp insertion triggered both `OnTextContainerChanged` and `OnSelectionChanged` invalidation.
- Florence then reformatted to exactly one line:
  - `IMECharCount=22`
  - `text='[2026-05-25 05:52:35] '`
- Selection/status also reported only `[2026-05-25 05:52:35] `.

Conclusion: the stale visual bug is fixed in the local shim path. The old behavior was caused by stale Florence page caching, not by failed text insertion.

### DevFlow note

After switching the sample output to local `LeXtudio.Windows.dll`, the app still launched and rendered the initial RichTextBox, but the DevFlow HTTP endpoint on `http://localhost:9223` did not become responsive during this run. I used the new `UNO_RTB_AUTOTEST=1` runtime path to verify the exact same edit sequence while preserving the DevFlow-friendly control IDs for manual/agent testing.

Next follow-up: investigate why the DevFlow agent endpoint stops responding when the sample output is overwritten with the local WindowsShims assembly. The core edit-rendering fix itself is verified by runtime layout logs.

### Decoration X-alignment fix

Observed follow-up issue: underline decorations for hyperlinks/underlined text and strikethrough decorations were slightly horizontally misaligned with the rendered glyphs. The Y placement was acceptable; the mismatch was specifically along X.

Root cause:

- `FlowDocumentView.BuildLineVisual` rendered text through a WinUI `TextBlock` with inline `Run`s.
- Underline/strikethrough were drawn separately as `Line` shapes in a `Canvas` using Florence `run.X` and `run.Width`.
- Even when Florence and TextBlock both use TextBlock measurement, native inline shaping/kerning/spacing can differ slightly from the separate geometry pass.
- Those small differences become visible at run boundaries, especially around hyperlinks, underline spans, and strikethrough spans.

Fix implemented:

- Removed the separate Canvas/Line decoration overlay path from `FlowDocumentView.BuildLineVisual`.
- Applied `run.TextDecorations` directly to each WinUI inline `Run` via `inlineRun.TextDecorations`.
- This makes glyphs and decorations come from the same text renderer and shaping pass, eliminating independent X-coordinate accumulation.

Verification:

```powershell
dotnet build src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop
$env:UNO_RTB_AUTOTEST='1'
dotnet run --project src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop --no-build
Remove-Item Env:UNO_RTB_AUTOTEST
```

Runtime smoke results:

- Initial RichTextBox document still formats to 7 lines.
- Select-all and timestamp insertion still trigger layout invalidation and reformat to a single timestamp line.
- Build accepts `Run.TextDecorations` on `net10.0-desktop`, so the renderer no longer needs the custom shape overlay for underline/strikethrough.

Next visual check: reopen the sample and inspect the hyperlink underline, explicit `Underline`, and struck-out text. They should now be horizontally anchored by WinUI's inline text renderer instead of Florence's separate `run.X/run.Width` overlay.

### Hyperlink hit-test / decoration X root cause from logs

Follow-up reproduction showed two symptoms:

- Uno did not render `Run.TextDecorations`, so switching entirely to inline `Run.TextDecorations` removed visible underline/strikethrough.
- Hyperlink cursor changes occurred over a horizontally shifted region, proving the issue was shared geometry/hit-test data rather than just decoration painting.

The decisive log signature was the wrapped style line reporting non-monotonic line offsets before the fix:

```text
line[176..265] ... text='This line should visibly show ... and a b'
line[177..191] ... text='lue hyperlink.'
```

That second line starting at `177` is impossible after a previous line ending at `265`. It meant the Florence logical offsets for nested inline content were wrong after wrapping. The root cause was `CollectSpans` recursively collecting child inline spans (`Hyperlink`, `Bold`, `Italic`, `Span`) with child-local offsets starting at zero, then adding them directly to the parent result without rebasing to the parent's current `localOffset`.

Fix implemented:

- Added `RebaseSpans` in `FlorenceEngine`.
- Rebased recursively collected child spans before adding them to the parent span list.
- Limited `TextMeasurer`'s sentinel workaround to text that actually ends in whitespace. The sentinel was previously appended to every measurement, which could perturb normal glyph advance/kerning and create small cumulative X drift.
- Restored custom Canvas `Line` decoration fallback in `FlowDocumentView`, because Uno currently accepts but does not visibly paint `Run.TextDecorations` on this target.

Post-fix layout trace:

```text
line[176..265] ... text='This line should visibly show ... and a b'
line[265..279] ... text='lue hyperlink.'
```

The logical offsets are now monotonic, so hyperlink hit testing, selection geometry, caret geometry, and custom underline/strikethrough overlays share the corrected run positions.

### Character selection box mismatch

Follow-up issue: selecting individual characters in `This ` showed visibly incorrect selection widths/positions, especially around narrow glyphs (`i`), normal glyphs (`s`), and spaces. This confirmed the problem was not only hyperlink run bounds; per-character caret/selection boundaries were not matching the rendered glyph positions closely enough.

Cause:

- Selection/caret/hit-testing uses `GetPixelXForOffset`, which calculates X from Florence prefix measurements.
- The renderer was still painting each Florence run as one `TextBlock`, letting TextBlock perform its own shaping/kerning/rounding for the whole run.
- Prefix substring measurements are close but not identical to the actual shaped positions inside the full rendered TextBlock, so character boxes drift within a run.

Current pragmatic fix:

- `FlowDocumentView.BuildLineVisual` now renders each character as its own `TextBlock` positioned at the same prefix X used by `GetPixelXForOffset`.
- This makes visible glyph placement, selection rectangles, caret placement, hyperlink hit testing, and decoration overlays share one coordinate model.
- Tradeoff: this is less typographically ideal than full-run shaping, but it prioritizes editor usability and predictable selection geometry until a real text layout backend exposes glyph/character bounds.

Build verification:

```powershell
dotnet build src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop --no-restore
```

Result: build succeeds with existing warnings.

### Paragraph merge crash in `GetLineRange`

Reproduction from manual testing:

- Backspace/Delete paragraph merge attempts from paragraph 2 to 1 and 3 to 2 failed.
- Merging paragraph 4 to 3 crashed with `NullReferenceException` inside WPF's `SplayTreeNode.GetSiblingAtCharOffset`.
- Stack entered through `UnoFlowDocumentTextView.ITextView.GetLineRange`, then `TextPointerBase.IsAtLineWrappingPosition`, then `TextSelection.SetCaretToPosition` during delete handling.

Root cause:

- `GetLineRange` converted Florence visual line offsets directly with `TextContainer.CreatePointerAtCharOffset`.
- Around paragraph boundaries after edit/merge operations, a Florence line boundary can be clamped but still not be a safe text-tree sibling offset for the WPF `TextContainer`.
- Letting that exception escape is fatal because WPF selection/delete code calls `GetLineRange` while normalizing the caret after deleting text.

Fix implemented:

- Added `SafeCreatePointerAtCharOffset` in `UnoFlowDocumentTextView`.
- Replaced all text-view offset-to-pointer conversions with the safe helper.
- Pointer-returning APIs now fall back to the current known-good pointer, or `tc.Start` for hit testing.
- `GetLineRange` now returns `TextSegment.Null` when start/end conversion fails or produces an inverted segment, rather than handing invalid pointers back to `TextSelection`.

Verification:

```powershell
dotnet build UnoRichText/src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop --no-restore
```

Result: build succeeds with existing warnings.

Follow-up to manually verify: repeat paragraph merges 2->1, 3->2, and 4->3 in the sample. Expected result is no crash. If the merge still does not happen, the remaining issue is likely the fast-path boundary detection in `RichTextBox.OnKeyDown`, not the `GetLineRange` splay-tree crash.

### Paragraph boundary merge follow-up

Manual result after the crash guard:

- Paragraph/line 4 merged into 3 correctly.
- Attempts for 3->2 and 2->1 still did not merge, even after repeated Delete/Backspace at the apparent line end.

Log finding:

- Repeated Delete presses were happening at visual line offsets such as `83`, not necessarily at a paragraph `ContentEnd`.
- The first sample paragraph wraps visually, so visual line 2 is still paragraph 1. Deleting there removed text (`TextBox.` became shorter) rather than merging paragraphs.
- The merge fast path also selected the first block whose element range contained the caret. At shared paragraph boundaries this can pick the wrong side for Delete/Backspace.

Fix implemented:

- Added direction-aware paragraph lookup in `RichTextBox.OnKeyDown`.
- Delete now prefers the paragraph whose `ContentEnd.CharOffset` equals the caret offset.
- Backspace now prefers the paragraph whose `ContentStart.CharOffset` equals the caret offset.
- Added compact paragraph boundary logging (`ES/CS/CE/EE`) and `MergeParagraphs` result logging.
- Relaxed Delete paragraph-end detection to accept matching `CharOffset`, not only `TextPointer.CompareTo(ContentEnd) == 0`, because Uno text-view normalization can produce an equivalent symbol offset with a different pointer edge.

Verification:

```powershell
dotnet build UnoRichText/src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj -f net10.0-desktop --no-restore
```

Result: build succeeds with existing warnings.

Next manual check: rerun the sample and try Delete at the real paragraph ends. The log should now show either `paragraph-merge-forward fast-path result=True` or a skipped line with the exact caret and paragraph offsets, which will make any remaining boundary mismatch obvious.

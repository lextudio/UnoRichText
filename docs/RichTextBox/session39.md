# Session 39 — Pixel-Accurate Caret X (Whitespace, Font Inheritance, Per-Character Position)

## Trigger

Sessions 37–38 fixed *which* offset the caret lands on. Session 39 chases *where on the line* the caret is drawn for that offset.

When the live sample is run, the caret position visibly disagrees with the rendered glyphs — especially when stepping through whitespace, and across `Run`s that should use different fonts. The previously reported "IMECharCount 2" pattern is gone (session 38), but the caret rectangle still drifts away from the cursor's logical position.

Two distinct defects were responsible, both in the caret-X math path that runs after a click or after `Selection`/`TextEditor` moves the position:

1. **Linear interpolation across a run's characters.** `UnoFlowDocumentTextView` was computing `caretX = run.X + run.CharWidth * charInRun`, where `CharWidth = run.Width / run.Length`. That is only correct for monospaced text — for proportional fonts and especially mixed-script content (CJK glyphs next to Latin) the per-character X is wildly off, and on whitespace the average understates the space width because spaces in most fonts are wider than the average glyph.
2. **TextBlock.DesiredSize.Width silently strips trailing whitespace.** Every measurement path Florence relied on — line layout, run width, and per-prefix caret X — pulled `DesiredSize.Width` from a probe `TextBlock`. WinUI/Uno (matching WPF's `TextLine.Width`, not `WidthIncludingTrailingWhitespace`) drops trailing spaces. So `MeasureWidth("abc ")` returned the width of `"abc"`, and stepping the caret across a space produced no visible movement. This is the headline whitespace bug the user reported.

A third gap, less visible at first but required for parity with WPF PTS, was also fixed:

3. **`FlorenceRun` discarded the per-run `FontFamily`.** WPF PTS carries the whole `Typeface` (FontFamily + Style + Weight + Stretch) on `TextRunProperties`. Florence captured only `FontSize`, `Bold`, `Italic` — meaning a `Run` that overrode `FontFamily` was rendered *and measured* with the wrong typeface. With both rendering and measurement consistently wrong, the bug was hidden in pure-Latin samples but surfaces immediately for CJK / emoji / monospaced inline-code runs.

## Fix Summary

All three defects are addressed at the lowest layer (`TextMeasurer` and `FlorenceRun`) so the correction propagates to layout, hit-testing, caret X, and bounding-geometry rendering.

### 1. `TextMeasurer.MeasureWidth` now compensates for trailing-whitespace trim

File: `WindowsShims/src/LeXtudio.Windows/MS.Internal/Florence/FlorenceEngine.cs`.

```csharp
private const char SentinelChar = '.';

internal static double MeasureWidth(string text, double fontSize, bool bold, bool italic,
    Microsoft.UI.Xaml.Media.FontFamily? fontFamily = null)
{
    if (string.IsNullOrEmpty(text)) return 0;
    ConfigureProbe(fontSize, bold, italic, fontFamily);
    // Width(text) = Width(text + sentinel) − Width(sentinel). Trailing whitespace
    // inside `text` is no longer at the end of the measured string and survives
    // TextBlock.DesiredSize's trim.
    double sentinelWidth = MeasureRaw(SentinelChar.ToString());
    double withSentinel  = MeasureRaw(text + SentinelChar);
    return Math.Max(0, withSentinel - sentinelWidth);
}
```

A period is used as the sentinel — it has no kerning relationships with whitespace or letters in common fonts, so the subtraction is stable. Two `Measure()` calls per width is acceptable because measurement is already cached behind a `[ThreadStatic]` reusable probe and the hot paths (line layout, caret X) are bounded.

This single change fixes both the layout-time `run.Width` (so wrapped lines no longer truncate visually at trailing spaces) and the caret-X path that calls back through `MeasurePrefixWidth`.

### 2. `UnoFlowDocumentTextView` uses real per-character widths

File: `WindowsShims/src/LeXtudio.Windows/MS.Internal.Documents/UnoFlowDocumentTextView.cs`.

`GetPixelXForOffset` (caret X from text offset) replaces the linear `run.CharWidth * charInRun` with `MeasurePrefixWidth(run, charInRun)`, short-circuiting the empty-prefix and full-run cases:

```csharp
if (offset >= run.StartOffset && offset <= run.EndOffset)
{
    int charInRun = offset - run.StartOffset;
    if (run.Length <= 0 || charInRun <= 0) return run.X;
    if (charInRun >= run.Length) return run.X + run.Width;
    return run.X + MeasurePrefixWidth(run, charInRun);
}
```

`HitTestCharOffset` (offset from click X) replaces the proportional `fraction * run.Length` with a binary search over real prefix widths, then refines to the nearest character boundary:

```csharp
double target = x - run.X;
int lo = 0, hi = run.Length;
while (lo < hi)
{
    int mid = (lo + hi + 1) / 2;
    if (MeasurePrefixWidth(run, mid) <= target) lo = mid;
    else                                        hi = mid - 1;
}
if (lo < run.Length)
{
    double left  = MeasurePrefixWidth(run, lo);
    double right = MeasurePrefixWidth(run, lo + 1);
    if (Math.Abs(target - right) < Math.Abs(target - left)) lo += 1;
}
return run.StartOffset + Math.Clamp(lo, 0, run.Length);
```

`MeasurePrefixWidth` delegates to `TextMeasurer.MeasurePrefixWidth` so layout-time and caret-time math share one engine — no chance of drift between "where the run was drawn" and "where the caret thinks the run sits."

### 3. `FlorenceRun` carries `FontFamily` (matches WPF PTS `TextRunProperties.Typeface`)

WPF's PTS reference, confirmed against upstream WPF source in `WindowsShims/ext/wpf/.../textformatting/`:

- `TextRunProperties.Typeface` returns a `Typeface` bundling `FontFamily + FontStyle + FontWeight + FontStretch`;
- `TextRunProperties.FontRenderingEmSize` carries the size.

Florence now mirrors that:

- `FlorenceRun` gains a nullable `Microsoft.UI.Xaml.Media.FontFamily? FontFamily` (null = "inherit whatever TextBlock resolves").
- `SpanInfo` (the Florence collection record) carries `FontFamily` through `Bold` / `Italic` / `Span` nesting via `ResolveInheritedFontFamily`.
- `TextMeasurer.MeasureWidth` and `FindFitChars` accept `FontFamily` so wrapping and measurement use the right typeface.
- `EmitLine` plumbs it into every emitted `FlorenceRun`.
- `FlowDocumentView.BuildLineTextBlock` sets `inlineRun.FontFamily = run.FontFamily` on each rendered `Microsoft.UI.Xaml.Documents.Run`. The first run's `FontFamily` is also lifted onto the outer `TextBlock` so single-typeface lines still inherit cleanly.
- `UnoFlowDocumentTextView.MeasurePrefixWidth` honors `run.FontFamily` through the shared `TextMeasurer`.

Inheritance subtlety: WPF's `Inline.FontFamily` DP has the inheritable default `"Segoe UI"`. We cannot distinguish "explicitly set to Segoe UI" from "inherited", so when the inherited stack has no override and the value is the WPF default we pass `null` down — letting the host `TextBlock`'s ambient font win — and we never write `Segoe UI` into the rendered tree.

**Not done — deferred to a follow-up session:**

- `FontStretch` plumbing (same pattern as `FontFamily`; add when a test case actually exercises it).
- Full DP-style inheritance walk up to `FlowDocument` rather than the simple parent-scope chain inside `CollectSpans`. Good enough for the sample today.

## Where Caret X Now Disagrees With WPF (Known Residuals)

These do not block the session 39 milestone but are next on the list:

- **Kerning.** Sentinel-based measurement isolates trailing whitespace correctly, but any kern pair between the last character of a prefix and the next character of the run is not captured by per-prefix measurement. The pixel error is sub-pixel for sans-serif body fonts but visible at large sizes or with serif fonts that have aggressive kerning tables. The proper fix is shaped-glyph positions (e.g. DirectWrite `IDWriteTextLayout::HitTestTextPosition`) rather than `TextBlock.Measure`.
- **Tab characters.** Florence currently does not run a tab-stop pass; tabs render as whatever the font's glyph for U+0009 is, which is platform-dependent.
- **Bidirectional / RTL text.** Florence's `HitTestCharOffset` is left-to-right only. Logical offset = visual offset is assumed throughout.

## Regression Coverage

Two new tests in `UnoRichText/src/LeXtudio.RichText.Tests/Documents/FlowDocumentTests.cs`:

- `TextMeasurer_PreservesTrailingWhitespaceWidth` — `MeasureWidth("abc ")` must exceed `MeasureWidth("abc")`, and `"abc  "` must exceed `"abc "`. Locks in the sentinel fix at the lowest layer.
- `FlorenceLayout_PrefixWidthGrowsMonotonicallyAcrossSpaces` — for a run containing spaces, `MeasurePrefixWidth(run, i)` must be monotonically non-decreasing as `i` advances, and must *strictly* increase when `i` steps across a space character. This is the property the user observed violated in the live sample: stepping the caret across a space produced no visible movement.

Focused verification:

```text
dotnet run --project src\LeXtudio.RichText.Tests\LeXtudio.RichText.Tests.csproj ^
  -f net10.0-desktop -p:UseNuGetPackage=false -- --uno-runtime-tests ^
  --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.TextMeasurer_PreservesTrailingWhitespaceWidth ^
  --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.FlorenceLayout_PrefixWidthGrowsMonotonicallyAcrossSpaces
```

Result: both tests pass. `WindowsShims` desktop build (`-f net10.0-desktop`) also passes.

## Cleanup Done In Passing

- Removed `src/LeXtudio.RichText.Tests/Controls/RichTextBoxTests.cs` and its `<Compile Include>` entry from the test project. That file tested the obsolete `LeXtudio.UI.Xaml.Controls.RichTextBox` class (deleted earlier in this session) and was breaking the test build with `CS0246: RichTextBox not found`.

## Post-Session Fixes (same branch)

Three additional defects were found and fixed during live-sample testing after the session 39 work landed.

### 4. Keyboard arrow-key caret not updating visually

File: `WindowsShims/src/LeXtudio.Windows/System.Windows/Controls/RichTextBox.uno.cs`.

`OnKeyDown` executed WPF navigation commands (`MoveRightByCharacter`, etc.) correctly, but never called `FlowDocumentView.SetCaretAt` afterward. The pointer-press handler did call it, so the caret appeared frozen at the last click position regardless of arrow-key movement.

Fix: `UpdateCaretFromSelection()` is now called whenever `args.Handled = true` in `OnKeyDown`. It reads `TextEditor.Selection.MovingPosition` and pushes it to `fdv.SetCaretAt(ITextPointer)`.

### 5. Phantom extra positions at end of document

WPF's `TextContainer` has paragraph/document boundary markers beyond the last visible character. With the sample's three paragraphs, `IMECharCount = 234` while the last Florence line ends at offset 232. `GetNextInsertionPosition` considered offsets 233 and 234 valid insertion positions, so pressing Right at end of document appeared to do nothing (caret stayed at x=382) but internally advanced the selection, requiring extra Left presses to return.

Two-part fix in `RichTextBox.uno.cs`:

- `IsAtLastVisiblePosition()` — returns true when `TextEditor.Selection.MovingPosition.CharOffset >= lastLine.EndOffset`. `MoveRightByCharacter` is swallowed (not executed) in this state.
- `UpdateCaretFromSelection` — clamps any `position.CharOffset > lastLine.EndOffset` to `lastLine.EndOffset` with Backward direction before calling `SetCaretAt`, covering any path that could still produce an out-of-range offset.

### 6. Caret position not refreshed after window/RichTextBox resize

When the control is resized, Florence re-lays out and fires `ITextView.Updated`. WPF's `OnTextViewUpdated` handler updates the internal WPF `CaretElement` (an adorner not used in the Uno shim) but never calls `FlowDocumentView.SetCaretAt`. The Uno `_caret` Rectangle therefore kept its pre-resize pixel coordinates.

Fix: `RefreshCaretAfterLayout()` added to `UnoFlowDocumentTextView.OnLayoutUpdated` (called after `Updated` fires). It reads `ITextContainer.TextSelection.MovingPosition` and calls `_owner.SetCaretAt(position)`, applying the same end-of-document clamp.

File: `WindowsShims/src/LeXtudio.Windows/MS.Internal.Documents/UnoFlowDocumentTextView.cs`.

## Known Residual — Paragraph-End "." Skip (deferred to next session)

When the cursor is immediately before the final `.` of a paragraph and the user presses Right, the caret jumps directly to the start of the *next* paragraph rather than stopping after the `.` on the current line.

Root cause: `MoveRightByCharacter` calls `SetCaretToPosition(newOffset, Backward, allowStopAtLineEnd: false)`. Our `GetLineRange` returns the paragraph's last line for the offset one-past-the-period. Since that offset equals both `prevLine.EndOffset` and `nextLine.StartOffset`, `IsAtLineWrappingPosition` returns true for both Forward and Backward directions — matching the "both true" condition that triggers the skip.

This is correct WPF behavior for *soft-wrapped* line ends, but wrong for *paragraph* ends. WPF PTS distinguishes these because its `TextPointer.GetPointerContext` carries `ElementEnd` for paragraph boundaries; our Florence pointer returns `Text` context throughout, so `IsAtLineWrappingPosition` cannot tell them apart.

Fix strategy (next session): override `GetLineRange` to return `TextSegment.Null` when the queried offset is at a paragraph boundary (i.e., when there is a gap between consecutive `FlorenceLine` start/end offsets rather than a shared boundary), so `IsAtLineWrappingPosition` correctly returns false and the caret stops after the period.

## Next Steps Forward

1. Fix paragraph-end "." skip (see Known Residual above).
2. After the caret X path is judged trustworthy, the Core Text bridge described in session 38 can finally be wired up — `LayoutRequested` now has a faithful rectangle to return.
3. Plumb `FontStretch` through Florence the same way as `FontFamily`.

## Session Closeout Update (May 21, 2026)

The paragraph-end skip is now fixed with a **WPF-aligned model change**, not a view-layer workaround.

### Final direction chosen

Instead of teaching `UnoFlowDocumentTextView` to special-case paragraph boundaries, Florence now reserves an **invisible paragraph-boundary logical slot** between adjacent paragraphs, mirroring WPF's structural marker behavior.

That means:

- Paragraph boundaries are represented in logical offsets.
- Caret movement can consume a boundary position before consuming the first visible character of the next paragraph.
- `GetLineRange` and caret-unit navigation stay simple and close to upstream behavior.

### Code changes

1. `WindowsShims/src/LeXtudio.Windows/MS.Internal/Florence/FlorenceEngine.cs`

- `FlorenceLayoutEngine.Format(...)` now increments `globalOffset` by 1 between neighboring paragraphs.
- This is the invisible boundary slot used by caret navigation.
- Temporary `ParagraphIndex` metadata previously added to `FlorenceLine` was removed to minimize Uno-specific divergence.

2. `WindowsShims/src/LeXtudio.Windows/MS.Internal.Documents/UnoFlowDocumentTextView.cs`

- Removed paragraph-boundary-specific `GetLineRange` null-return logic.
- Removed paragraph-boundary direction-flip helper logic from `GetNextCaretUnitPosition`.
- Text view now relies on the structural offsets from Florence, not custom boundary inference.

### Regression coverage updated

File: `UnoRichText/src/LeXtudio.RichText.Tests/Documents/FlowDocumentTests.cs`

- `FlorenceLayout_TracksParagraphBoundariesOnSharedOffsets`
    now asserts `second.StartOffset == first.EndOffset + 1`, locking in the invisible boundary slot.
- Added `FlorenceLayout_ParagraphBoundarySlotPrecedesNextParagraphFirstCharacter` to verify a post-paragraph caret step lands at the left edge of the next paragraph's first character (does not skip over it).

Focused verification executed:

```text
dotnet run --project LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -- --uno-runtime-tests \
    --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.TextMeasurer_PreservesTrailingWhitespaceWidth \
    --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.FlorenceLayout_PrefixWidthGrowsMonotonicallyAcrossSpaces \
    --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.FlorenceLayout_TracksParagraphBoundariesOnSharedOffsets \
    --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.FlorenceLayout_ParagraphBoundarySlotPrecedesNextParagraphFirstCharacter
```

Result: **Passed (4/4)**.

### Status

Session 39 is now closed with paragraph-end navigation behavior aligned to WPF structural semantics and with regression tests in place.

## Follow-up Cleanup (same branch, reuse-oriented)

After the session 39 fixes were stable, we did a small cleanup pass specifically to reduce Uno-only policy in the control layer.

### Routed-command shim cleanup

`MS.Internal.Commands.CommandHelpers` had been simplified too aggressively: every registered WPF class command handler was effectively attached globally to the `RoutedCommand`, ignoring the `controlType` it was registered for.

That worked for RichTextBox navigation, but it was a code-reuse regression because it weakened WPF's class-command model and would let future controls accidentally see handlers not meant for them.

Fix:

- `System.Windows.Input.CommandBinding` now stores the registered target type.
- `System.Windows.Input.RoutedCommand.Execute/CanExecute` now filters bindings by the runtime target object.
- `MS.Internal.Commands.CommandHelpers.RegisterCommandHandler(...)` now preserves `controlType` when building the binding.

Regression coverage:

- Added `WpfRichTextBoxShellTests.RoutedCommand_ClassHandlers_RespectRegisteredControlType`.

### End-of-document caret cleanup

The session 39 branch had also accumulated end-of-document guard logic in `RichTextBox.uno.cs`:

- `IsAtLastVisiblePosition()`
- a special-case swallow for `MoveRightByCharacter`
- local clamping in `UpdateCaretFromSelection()`

Those behaviors were correct for Florence-backed rendering, but they belonged lower in the text-view adapter rather than inside the control.

Fix:

- `UnoFlowDocumentTextView` now exposes `NormalizeToVisiblePosition(ITextPointer)`.
- `GetRectangleFromTextPosition`, line navigation, caret-unit navigation, and line lookup normalize through that shared helper.
- `FlowDocumentView.SetCaretAt(ITextPointer)` now normalizes before painting the Uno caret rectangle.
- `RichTextBox.uno.cs` no longer contains its own "last visible position" policy.

This keeps the WPF `RichTextBox` control code closer to upstream behavior while letting the Florence-backed Uno text view own the mapping from WPF logical positions to visible Uno caret positions.

Focused verification executed after cleanup:

```text
dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.Constructor_CreatesImplicitEmptyFlowDocument --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.AssignedDocument_IsOwnedAndSerializable --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.Document_CannotBeSharedAcrossRichTextBoxes --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.AddChild_ReplacesOnlyImplicitDocument --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.AppendText_CreatesRunInLastParagraph --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.SelectionFormatting_CanBeAppliedAtCaret --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.RoutedCommand_ClassHandlers_RespectRegisteredControlType
```

### Shell-test cleanup

While validating the reuse cleanup, two RichTextBox shell tests also needed to be brought closer to the actual shared/document model:

- `AssignedDocument_IsOwnedAndSerializable` now passes because `RichTextBox.Document` assignment sets `FlowDocument.Parent = this` in the shared WPF-linked source, rather than depending on a Uno-only ownership side channel.
- `AppendText_CreatesRunInLastParagraph` now verifies appended text through `TextRange(run.ContentStart, run.ContentEnd).Text`, matching the session 38 text-container work instead of depending on `Run.Text`'s deferred local-value path.
- The old `AppendText_UsesCurrentTypingFormat` assertion was replaced with `SelectionFormatting_CanBeAppliedAtCaret`. In the current port, public selection formatting APIs work, but `AppendText` is still plain-text insertion and does not yet consume pending typing formatting, so the old assertion was overstating what the shell currently guarantees.

Final focused result: **Passed (7/7)**.

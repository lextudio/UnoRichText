# Session 38 - IMECharCount, Text Tree Ownership, and Core Text Integration

## Trigger

After the session 37 adapter cleanup, the visible caret can still stick near column 2 on line 1. The observed log pattern is:

```text
raw=<large Florence offset> IMECharCount=2 clamped=2
```

`UnoFlowDocumentTextView.GetTextPositionFromPoint(...)` correctly asks Florence for the hit-test offset, but then clamps that offset
to `FlowDocument.StructuralCache.TextContainer.IMECharCount`. Since `IMECharCount` is only `2`, every click beyond the first couple
of characters collapses to offset 2.

## Important Terminology Correction

`IMECharCount` is not "IME character width".

In WPF, `IMECharCount` is the count of IME-visible Unicode characters in the `TextContainer` splay tree. It is used by:

- `TextContainer.CreatePointerAtCharOffset(...)`;
- WPF `TextStore`;
- IME/composition code;
- selection/caret movement;
- text services property ranges.

It is a logical text count, not a pixel measurement. Pixel width belongs to layout (`ITextView.GetRectangleFromTextPosition`,
Florence run measurement, text shaping, candidate window bounds).

So the bug is not that our IME integration cannot measure character width. The bug is that the WPF `TextContainer` does not contain
the visible FlowDocument text that Florence is rendering.

## What The Current Code Does

### WPF `FlowDocument`

The linked WPF source creates a real text tree:

```csharp
private void Initialize(TextContainer textContainer)
{
    if (textContainer == null)
    {
        textContainer = new TextContainer(this, false /* plainTextOnly */);
    }
    _structuralCache = new StructuralCache(this, textContainer);
}
```

`RichTextBox.Document` then wires the editor to that tree:

```csharp
_document.TextContainer.CollectTextChanges = true;
this.InitializeTextContainer(_document.TextContainer);
```

That means the ported WPF `TextEditor` is looking at `FlowDocument.TextContainer`, not Florence directly.

### Florence

Florence currently renders by walking the object model:

```text
FlowDocument.Blocks -> Paragraph.Inlines -> Run.Text
```

This explains why text can appear even when the WPF text tree reports almost no characters. Rendering and editing are reading two
different representations:

```text
Florence visible text offsets:   actual paragraph/run text
WPF TextContainer IME offsets:   only root/document edge symbols, currently count 2
```

### `UnoFlowDocumentTextView`

The adapter is WPF-shaped, but the offset bridge is broken:

```csharp
int raw = HitTestCharOffset(hit, point.X);
int charOffset = Math.Clamp(raw, 0, tc.IMECharCount);
return tc.CreatePointerAtCharOffset(charOffset, LogicalDirection.Forward);
```

When `tc.IMECharCount == 2`, this destroys the Florence hit-test result.

## Why Core Text Does Not Fix This By Itself

`TextCore.Uno` / `LeXtudio.UI.Text.Core` solves platform IME plumbing:

- attaching to the native window;
- receiving platform composition updates;
- answering text requests;
- answering selection requests;
- answering layout/caret-rectangle requests;
- notifying platform IMEs when focus, selection, or layout changes.

The existing `LeXtudio.TextBox` integration shows the shape:

```text
CoreTextEditContext
  TextRequested       -> return current text slice
  TextUpdating        -> replace selected/current composition range
  SelectionRequested  -> return logical selection
  SelectionUpdating   -> update logical selection
  LayoutRequested     -> return caret/control bounds
```

That is the right model for WPF `RichTextBox` too, but it depends on a correct logical text model underneath. Core Text can ask for
text and selection; it cannot make WPF `TextContainer.IMECharCount` correct if the `TextContainer` was never populated with the
document text.

Put differently:

```text
Core Text solves: platform IME events <-> editor logical text API
This bug is:      editor logical text API <-> FlowDocument text tree
```

## Likely Root Cause

The most likely root cause is that FlowDocument content is present in Uno logical-child collections but is not fully represented in
the WPF text tree.

Evidence:

- `TextElementCollection.Add(...)` still calls upstream `item.RepositionWithContent(this.ContentEnd)`.
- `TextElement.EnsureTextContainer()` can create and insert a `TextElement` into a `TextContainer`.
- `FlowDocument.Initialize(...)` creates `new TextContainer(this, false)`.
- Yet runtime logs show `FlowDocument.StructuralCache.TextContainer.IMECharCount == 2` for a document with much more visible text.

The value `2` strongly suggests only structural edge symbols are present. The object model is enough for Florence rendering, but the
text tree lacks actual run text nodes.

## What Not To Do

Do not "fix" the cursor by clamping to Florence document length instead of `TextContainer.IMECharCount` while still returning
`tc.CreatePointerAtCharOffset(...)`.

That would just move the crash/incorrectness. `CreatePointerAtCharOffset` cannot return a valid WPF pointer for an offset that the
text tree does not contain. Selection, typing, deletion, drag/drop, and IME replacement would still be detached from WPF.

Do not put Core Text directly on `FlowDocumentView` offsets either. That would create a second editor model:

```text
platform IME -> Florence offsets -> direct document mutation
```

It would bypass WPF `TextEditor`, undo, `TextSelection`, command routing, and rich text mutation rules.

## Correct Direction

The required order is:

1. Make `FlowDocument.TextContainer.IMECharCount` match the document's visible text.
2. Make `CreatePointerAtCharOffset(...)` return positions whose `CharOffset` maps to Florence line/run offsets.
3. Let `UnoFlowDocumentTextView` convert between WPF text pointers and Florence rectangles.
4. Only then attach Core Text as the platform IME bridge.

Target ownership:

```text
TextContainer / TextSelection
  owns logical text, selection, insertion, deletion, undo-visible changes

UnoFlowDocumentTextView + Florence
  owns hit testing and layout rectangles for WPF logical positions

CoreTextEditContext
  owns native IME session and platform text-service requests
```

## Immediate Diagnostics To Add

Add a diagnostic after `RichTextBox.Document` is assigned and after layout:

```text
FlowDocument.Blocks visible length
FlowDocument.TextContainer.SymbolCount
FlowDocument.TextContainer.IMECharCount
FlowDocument.ContentStart/ContentEnd offsets
First paragraph IsInTree / TextElementNode present
First run IsInTree / TextElementNode present
TextRange(ContentStart, ContentEnd).Text
Florence total visible length
```

Expected failing shape today:

```text
visible length:        > 2
Florence length:       > 2
TextContainer count:   2
TextRange text:        empty or structurally incomplete
```

Expected passing shape:

```text
visible length == Florence visible length == TextContainer.IMECharCount
```

Paragraph breaks need an explicit decision. WPF `TextRange.Text` commonly exposes paragraph separation as CR/LF-style text, while
Florence currently lays paragraph text as visible runs and spacing. We need one mapping contract and tests for it.

## Candidate Implementation Paths

### Option A - Repair WPF TextTree Population

This is the preferred long-term path.

Investigate why `FlowDocument.Blocks.Add(...)`, `Paragraph.Inlines.Add(...)`, and `Run.Text` are not producing text-tree nodes with
the expected `IMECharCount`.

Likely places:

- `TextElementCollection.Add(...)` and `RepositionWithContent(...)`;
- `TextElement.Append(string)` / implicit `Run` paths;
- `Run.Text` property change path;
- Uno-specific logical child additions in `TextElement.uno.cs`;
- any `#if HAS_UNO` gates around `TextElementNode` or inheritance-context behavior.

Success criteria:

- existing WPF `TextContainer.CreatePointerAtCharOffset(...)` works without a special Florence pointer;
- WPF `TextSelection` and `TextRange` become useful;
- Core Text can later call into WPF selection/text APIs.

### Option B - Florence Text Container Adapter

If repairing WPF TextTree is too expensive short-term, build a real `ITextContainer` adapter whose offsets are Florence offsets.

This is riskier because WPF `TextEditor` expects more than `ITextContainer`; it expects text mutation, text changes, undo integration,
and pointer semantics. A shallow adapter would repeat the current problem under another name.

If chosen, it must be a proper compatibility layer:

- stable `ITextPointer` offsets;
- text run context;
- insertion/deletion;
- change events with correct symbol and IME char counts;
- text selection storage;
- `TextRange` compatibility.

### Option C - Temporary Visual-Only Cursor Fallback

Keep the current visual caret fallback for demos only. This is acceptable as a temporary rendering aid, but it must not become the
selection/IME implementation path.

## How To Bring In Core Text Later

Once the text-tree offset issue is fixed, add a WPF RichTextBox Core Text bridge in `WindowsShims`, probably as a Uno partial:

```text
RichTextBox.uno.cs
  OnGotFocus / OnLostFocus -> CoreTextEditContext.NotifyFocusEnter/Leave
  OnKeyDown                -> ProcessKeyEvent before WPF key handling
  SelectionChanged/layout  -> NotifySelectionChanged / NotifyCaretRectChanged

CoreText handlers
  TextRequested       -> TextRange over TextContainer char offsets
  TextUpdating        -> TextSelection.Select(range); TextEditorTyping/TextRange replacement
  SelectionRequested  -> current TextSelection start/end CharOffset
  SelectionUpdating   -> TextSelection.Select(CreatePointerAtCharOffset(...))
  LayoutRequested     -> ITextView.GetRectangleFromTextPosition(selection.Start)
  CompositionStarted  -> raise WPF/RichTextBox composition events if needed
  CompositionCompleted-> complete WPF composition state
```

But this must come after `IMECharCount` is meaningful. Otherwise every Core Text range from the platform will also collapse into the
same broken two-character coordinate space.

## Next Coding Step

Add a narrow diagnostic/test first. The best next session should not start by wiring Core Text. It should prove the text tree mismatch
and then repair population.

Recommended first test:

```text
Create FlowDocument
Add Paragraph with Run("abcdef") and Bold(Run("ghi"))
Assert FlowDocument.TextContainer.IMECharCount >= 9
Assert CreatePointerAtCharOffset(7).CharOffset == 7
Assert Florence hit-test offset 7 can round-trip through ITextView to a WPF pointer and rectangle
```

Only after this passes should selection drag and IME composition be implemented on top.

## Coding Update - Object Initializer Runs Were The Live Sample Gap

The first diagnostic using `new Paragraph(new Run("abcdef"))` passed: the WPF `TextContainer` did receive visible text and
`CreatePointerAtCharOffset(...)` worked. The live sample still showed:

```text
hit.Start=87 raw=36 IMECharCount=2 clamped=2
```

The difference was construction style. The sample builds runs with object initializers:

```csharp
new Run { Text = "This sample hosts a " }
```

while the passing test used the `Run(string)` constructor. In this port, the object-initializer path can set `Run.Text` before the run
is attached to the final document tree. The object model still renders because Florence walked `Run.Text`, but the shared WPF
`TextContainer` did not contain the text, so editor offsets collapsed.

Fixes made:

- added `LeXtudio.RichText.Tests` to `InternalsVisibleTo`;
- added a no-gate partial hook in linked `TextElementCollection.Add(...)`: `OnUnoItemAdded(item)`;
- implemented the hook in `TextElementCollection.uno.cs` to hydrate empty `Run` text into the real TextTree after insertion;
- changed Florence run collection to read `new TextRange(run.ContentStart, run.ContentEnd).Text` instead of `Run.Text`, so layout now
  follows the shared WPF text tree;
- fixed Florence paragraph/run offsets so wrapped runs use document-wide offsets instead of restarting from a paragraph-local zero.

Tests added:

- `TextContainer_IMECharCountIncludesRunText`;
- `TextContainer_CanCreatePointersAtVisibleCharOffsets`;
- `TextContainer_IMECharCountIncludesObjectInitializerRunText`;
- `FlorenceLayout_UsesDocumentWideRunOffsets`.

Focused verification:

```text
dotnet run --project src\LeXtudio.RichText.Tests\LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.TextContainer_IMECharCountIncludesObjectInitializerRunText --test=LeXtudio.RichText.Tests.Documents.FlowDocumentTests.FlorenceLayout_UsesDocumentWideRunOffsets
```

Result: both focused tests passed.

`WindowsShims` desktop build also passed:

```text
dotnet build src\LeXtudio.Windows\LeXtudio.Windows.csproj --no-restore -f net10.0-desktop
```

Remaining work: re-run the live sample and inspect `rtb-template.log`. The expected improvement is that `IMECharCount` is no longer
`2` for the sample document and `raw` offsets are in the same document-wide space as the hit line.

## Coding Update - Intermittent Caret Was A Second Florence Offset Bug

The newer sample log proved the first repair worked because `IMECharCount` jumped from `2` to real document values such as `234`.
But the same log still showed intermittent bad hit-testing:

```text
hit.Y=37.8 hit.Start=276 hit.End=357 raw=292 IMECharCount=234 clamped=234
```

That means the remaining failure was no longer "text tree is empty". Florence itself was sometimes producing line/run offsets larger
than the WPF `TextContainer` coordinate space.

Two bugs were found in `FlorenceLayoutEngine.FormatParagraph(...)`:

- later spans in the same paragraph reused `globalOffset + span.GlobalOffset`, which double-counted paragraph-local offsets after the
  first inline;
- when wrapping within a later span, the code advanced `globalOffset` by `lineText.Length`, which re-counted earlier inline text that
  was already present on the same visual line.

The fix keeps a stable `paragraphStartOffset` and advances wrapped-line starts from the actual consumed span offset instead of the
accumulated visible line text.

Added regression coverage:

- `FlorenceLayout_DoesNotDoubleCountParagraphInlineOffsets`

This test mirrors the live sample structure: one paragraph, many object-initializer runs, and it asserts Florence line/run offsets stay
within `TextContainer.IMECharCount`.

Interpretation after the fix:

- `TextRange(document.ContentStart, document.ContentEnd).Text` still includes paragraph terminators such as `\r\n`;
- `IMECharCount` tracks visible text coordinates for the shared WPF text tree;
- Florence offsets should match the latter, not the former.

So the right invariant for caret work is:

```text
Florence line/run offsets <= TextContainer.IMECharCount
```

not strict equality with `TextRange(...).Text.Length`.

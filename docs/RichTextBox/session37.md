# Session 37 - Reuse Audit After WPF RichTextBox Cursor Bring-up

## Trigger

From `WindowsShims` commit `fb2ce553746d4e3eaa850d43608721ba0a59192b` (`Add Florence engine to replace PTS`) to current `HEAD`
(`085a88e Place cursor at right place`), the RichTextBox effort made real progress:

- the ported WPF `RichTextBox` now instantiates under Uno;
- a `FlowDocumentView` render scope shows formatted text;
- pointer click can focus the control and show a caret at the clicked visual location.

The concern is valid: the recent path got there by adding Uno-only side paths (`RichTextBox.uno.cs`, `FlowDocumentView.uno.cs`,
`UnoFlowDocumentTextView.cs`) and by letting caret placement bypass the WPF `TextEditor`/`TextSelection`/`TextContainer` model.
That fixes the first visible demo, but it is the wrong direction for selection, keyboard navigation, drag/drop, IME, undo, and command routing.

## Diff Shape Since `fb2ce55`

`WindowsShims` changed by roughly 1.2k lines. Most new code is additive Uno infrastructure:

| File | Shape | Reuse assessment |
|---|---:|---|
| `MS.Internal.Documents/FlowDocumentView.uno.cs` | +215 | Uno render host, caret overlay, Florence layout call |
| `MS.Internal.Documents/UnoFlowDocumentTextView.cs` | +270 | WPF `ITextView` adapter backed by Florence geometry |
| `System.Windows.Controls/RichTextBox.uno.cs` | +147 | Uno input forwarding into WPF `TextEditorTyping`/`TextEditorMouse` |
| `MS.Internal/Florence/FlorenceEngine.cs` | +264 | Good shared-shim direction; now does text measurement and line breaking |
| `System.Windows.Documents/TextEditorDragDropUno.cs` | +173 | Uno-native drag/drop adapter; currently renderer/offset centric |
| `Themes/Generic.xaml` | +30 | Uno template for WPF `RichTextBox` |

Important: linked WPF source files did not explode with new gates in this range. Existing gates are still present in
`FlowDocument.cs`, `TextElement.cs`, and `TextEditorDragDrop.cs`, but the newest cursor work mostly added parallel Uno files rather
than modifying the linked WPF sources. So the immediate problem is not "too many new `#if HAS_UNO` lines in WPF files"; it is that
the new Uno files are starting to become a second editor.

## Current Architectural Break

The biggest break is documented by session 36:

> Florence assigns global visible-text offsets, but WPF `TextContainer.IMECharCount` reports stub values in the current Uno build.

Because of that, `RichTextBox.uno.cs` calls `TextEditorMouse.SetCaretPositionOnMouseEvent(...)`, but then immediately displays
the caret via `FlowDocumentView.SetCaretAt(...)`, which uses Florence geometry only. That means the visible caret can be correct
while WPF `TextEditor.Selection`, `CaretPosition`, keyboard movement, IME composition range, drag/drop deletion, and command logic
remain incorrect or inert.

This is the central reuse blocker. WPF editing code is already linked. It expects:

- `ITextView.GetTextPositionFromPoint` returns pointers in the same coordinate system as the real `TextContainer`;
- `ITextView.GetRectangleFromTextPosition` can map those pointers back to Florence geometry;
- `TextSelection` change notifications drive caret and selection rendering;
- text mutations happen through WPF `TextContainer`/`TextRangeEdit` paths, not a separate renderer offset model.

## What To Maximize

Keep the Uno-specific boundary at platform IO and rendering:

- Uno pointer/key/text/drag/IME events can be adapted at the edge.
- Florence can replace PTS for layout.
- The rest should look like WPF: `TextEditor`, `TextSelection`, `TextPointer`, `TextContainer`, `ITextView`, and command handlers.

That means new work should preferentially make Florence and the text tree compatible with the WPF contracts instead of adding more
features into `FlowDocumentView.uno.cs` or renderer-specific helpers.

## Priority Refactors

### 1. Make `TextContainer` offsets match Florence offsets

This is the highest-leverage task. `UnoFlowDocumentTextView` already has the right role, but it cannot succeed while
`tc.IMECharCount` and `tc.CreatePointerAtCharOffset(...)` use a stub/empty text tree offset space.

Target state:

- `FlowDocument.Initialize(...)` creates a real `TextContainer` on Uno.
- `FlowDocument.Blocks` content is inserted into that text tree, or the text tree can enumerate the same symbols that Florence lays out.
- `TextContainer.IMECharCount` equals the visible text length used by Florence, including agreed paragraph break semantics.
- `ITextPointer.CharOffset` maps one-to-one with `FlorenceLine.StartOffset` / `FlorenceRun.StartOffset`.

Payoff:

- remove the direct `FlowDocumentView.SetCaretAt(clickPoint)` bypass;
- let `TextEditorMouse.SetCaretPositionOnMouseEvent` update the real selection;
- make keyboard navigation, typing, selection, and IME use the linked WPF code path.

### 2. Move caret rendering behind `ITextSelection`/`ITextView`

`FlowDocumentView` can own the visual caret rectangle, but it should not own the logical caret. Today `SetCaretAt(Point)` makes the
view the source of truth.

Target state:

- `RichTextBox.uno.cs` forwards pointer input only.
- WPF `TextEditorMouse` updates `TextSelection`.
- `FlowDocumentView` subscribes to selection/caret changes and asks `ITextView.GetRectangleFromTextPosition(selection.Start)` for
geometry.
- `SetCaretAt(Point)` is deleted or reduced to a private debug fallback.

Payoff:

- selection and caret stay in the same logical model;
- selection rendering can be added from `TextSelection` ranges;
- IME composition rectangles can reuse the caret rectangle path.

### 3. Turn `UnoFlowDocumentTextView` into a true Florence-backed WPF adapter

This file is conceptually correct, but several methods are currently stubs:

- `TextSegments` returns `TextSegment.Null`;
- `GetTightBoundingGeometryFromTextPositions` returns `Geometry.Empty`;
- `GetGlyphRuns` returns an empty collection;
- page navigation is no-op;
- line/caret methods clamp to `tc.IMECharCount`, which is currently wrong.

Target state:

- keep this file, but treat it as the main compatibility shim;
- implement selection geometry from Florence lines/runs;
- return meaningful `TextSegment` values for the document/page;
- use shared Florence offset helpers for hit testing and x-position mapping.

Payoff:

- WPF `TextEditorSelection`, `TextEditorMouse`, and drag/drop can ask normal WPF questions and get Uno/Firenze answers.

### 4. Shrink `RichTextBox.uno.cs` to platform event adaptation

Current `RichTextBox.uno.cs` does three things:

- default style/logging;
- pointer/key/character event forwarding;
- visual caret correction.

Only the first two belong here. Once the offset model is fixed, delete the visual caret correction. Keep key mapping minimal and
prefer WPF command routing where possible.

Target state:

- no direct renderer-specific casts except template hookup;
- no direct caret positioning;
- no text mutation;
- no selection policy.

### 5. Revisit drag/drop as a WPF-shaped adapter, not renderer logic

`TextEditorDragDropUno.cs` is better than scattering drag logic through controls, but it currently depends on `IRichTextDragDropHost`
with renderer offsets and direct `InsertTextAt`.

Target state:

- Uno drag events adapt to WPF `DragEventArgs`-like data where possible;
- hit testing calls `ITextView.GetTextPositionFromPoint`;
- insertion/deletion goes through `TextEditorDragDrop`/`TextRangeEdit`/`TextSelection`;
- renderer offset host methods disappear or become thin wrappers around `ITextPointer`.

Payoff:

- move/copy semantics, selection deletion, undo grouping, and clipboard formats can reuse WPF code.

### 6. Treat IME as a `TextEditor` service bridge

Do not add IME insertion directly into the renderer. The existing RichEditBox IME lessons are useful, but for WPF RichTextBox the
bridge should feed composition text and selection requests into the WPF text editor model.

Target state:

- CoreText/TextInput events map to WPF `TextCompositionEventArgs` and selection/layout queries;
- layout queries use `ITextView.GetRectangleFromTextPosition`;
- composition replacement uses `TextSelection`/`TextRangeEdit`.

Payoff:

- IME, selection, undo, and command behavior do not diverge from WPF.

## Minimize Gated Lines In Linked WPF Sources

Preferred order when something does not compile or does not work on Uno:

1. Add/complete a shim type with the same namespace/name/contract as WPF expects.
2. Add a partial `.uno.cs` only for platform event hookup or platform service calls.
3. Add a small adapter implementing an existing WPF interface (`ITextView`, `IServiceProvider`, `ITextContainer`).
4. Only then add a narrow `#if HAS_UNO` inside linked WPF source, with a comment naming the missing platform service.

Current linked-source gates worth shrinking later:

- `FlowDocument.cs`: remaining PTS/paginator/automation gates are acceptable until Florence has pagination and automation peers.
- `TextElement.cs`: logical tree / inheritance-context gates should be revisited after the text tree is real.
- `TextEditorDragDrop.cs`: WPF OLE drag/drop calls need a platform adapter; avoid growing gates here.
- `RichTextBox.cs`: keep gates minimal; the actual Uno surface should live in partials and shims.

## Recommended Next Session Plan

1. Audit `FlowDocument.Initialize`, `TextElementCollection`, and `TextContainer` population on Uno.
2. Make a failing test or diagnostic proving `FlowDocument.ContentStart`, `ContentEnd`, `TextContainer.IMECharCount`, and Florence
   visible text length agree for a paragraph with multiple runs.
3. Fix the text-tree population or offset mapping until that diagnostic passes.
4. Change `UnoFlowDocumentTextView.GetTextPositionFromPoint` to return unclamped Florence/text-tree offsets.
5. Remove `RichTextBox.uno.cs` direct `FlowDocumentView.SetCaretAt(...)` call and verify the caret follows `TextSelection`.
6. Only after that, implement drag selection rendering using `ITextView.GetTightBoundingGeometryFromTextPositions`.

## Coding Step 1 - Move Geometry Back Toward `ITextView`

Implemented in `WindowsShims` after this audit:

- `FlowDocumentView.SetCaretAt(Point)` no longer owns its own Florence hit-test math.
- The caret overlay now asks `UnoFlowDocumentTextView.GetTextPositionFromPoint(...)` for the logical position and
  `UnoFlowDocumentTextView.GetRectangleFromTextPosition(...)` for the visual rectangle.
- `UnoFlowDocumentTextView` now exposes those two internal wrapper methods, while still implementing the WPF `ITextView` interface.
- Duplicated `HitTestXToPixel` logic was removed from `FlowDocumentView.uno.cs`.
- `UnoFlowDocumentTextView.GetTightBoundingGeometryFromTextPositions(...)` now returns per-line rectangle geometry from Florence
  runs instead of always returning `Geometry.Empty`.

This is intentionally a small step. It does not yet fix the larger `TextContainer.IMECharCount` / Florence offset mismatch, and
`RichTextBox.uno.cs` still calls `FlowDocumentView.SetCaretAt(Point)` as a visual fallback. But the caret fallback now passes through
the same WPF-facing adapter that selection, drag/drop, and IME should use next.

Verification:

- `dotnet build src\LeXtudio.Windows\LeXtudio.Windows.csproj --no-restore -f net10.0-desktop` passes.
- Full `dotnet build` still fails on the WinUI target with Uno `UNOB0008`, which asks for `msbuild` when XAML files are present.

## Bottom Line

The right reuse boundary is:

```text
Uno platform events
    -> small adapters
        -> WPF TextEditor / TextSelection / TextContainer
            -> Florence-backed ITextView + layout
                -> Uno visual rendering
```

The current code is close enough to steer back, but the next work should stop adding feature behavior to `FlowDocumentView.uno.cs`.
The single most important repair is making WPF text-tree offsets and Florence layout offsets the same coordinate system.

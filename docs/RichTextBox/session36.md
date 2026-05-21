# Session 36 — Uno Input Forwarding and Florence-Based Caret Placement

## Goal

Wire Uno Platform pointer and keyboard events into the WPF `RichTextBox` shim so that
clicking places a blinking caret and keyboard input reaches the WPF `TextEditor`.

---

## Problem 1 — `MeasureOverride` CPU loop (carried over)

### Root cause
`FlowDocumentView.ArrangeOverride` rebuilt `Children` on every call via `Children.Clear()` + `Children.Add()`, dirtying the panel every frame.  The `ReferenceEquals(_page, _arrangedPage)` guard was ineffective because `MeasureOverride` always produced a new `FlorencePage` object even when the document and size were unchanged.

### Fix
Added `_lastMeasureWidth` / `_lastMeasureHeight` fields.  `MeasureOverride` now skips `FlorenceLayoutEngine.Format` when the available size has not changed by more than 0.5 px and `_page != null`.

```csharp
if (_page == null || Math.Abs(w - _lastMeasureWidth) > 0.5 || Math.Abs(h - _lastMeasureHeight) > 0.5)
{
    _page = FlorenceLayoutEngine.Format(_document, new Windows.Foundation.Size(w, h));
    _lastMeasureWidth = w;
    _lastMeasureHeight = h;
    _textView?.OnLayoutUpdated();
}
```

---

## Problem 2 — Clicks not registering

### Root cause
`FlowDocumentView` (a `Panel`) had no Uno input handling. The WPF `TextEditor` receives input through WPF-routed events that never fire on Uno.

### Fix: `RichTextBox.uno.cs` — Uno input forwarding

Three overrides were added to the `RichTextBox` partial class:

| Override | Forwarded to |
|----------|-------------|
| `OnPointerPressed` | `TextEditorMouse.SetCaretPositionOnMouseEvent` |
| `OnKeyDown` | `TextEditorTyping.OnKeyDown` via `KeyEventArgs` bridge |
| `OnCharacterReceived` | `TextEditorTyping.OnTextInput` via `TextCompositionEventArgs` |

`MapVirtualKey` converts `Windows.System.VirtualKey` → `System.Windows.Input.Key` for the arrow, navigation, and editing keys.

`Key.PageUp` / `Key.PageDown` / `Key.Prior` / `Key.Next` were added to the `Key.cs` shim (values 33/34).

---

## Problem 3 — `SetCaretPositionOnMouseEvent` NullReferenceException in SplayTree

### Root cause
`UnoFlowDocumentTextView.GetTextPositionFromPoint` called `tc.CreatePointerAtCharOffset(raw, ...)` where `raw` came from `HitTestX`.  When `raw` exceeded `tc.IMECharCount` the splay-tree traversal walked off a null `RightChildNode`.

### Fix
Clamped the offset before calling `CreatePointerAtCharOffset`:

```csharp
int charOffset = Math.Clamp(HitTestX(hit, point.X), 0, tc.IMECharCount);
```

Same clamp applied to `GetPositionAtNextLine`.

---

## Problem 4 — Caret always at wrong position (charOffset always 2)

### Root cause (fundamental)
`FlowDocument.StructuralCache.TextContainer.IMECharCount` returns **2** in the Uno shim environment — the WPF TextContainer does not receive the FlowDocument's paragraph content in this build.  Florence assigns global character offsets starting from 0 counting visible text; the WPF TextContainer's offset space is completely different.  Clamping to `IMECharCount=2` forced every click to resolve to offset 2, which mapped back to a fixed pixel position near the top-left corner.

### Diagnosis
Log output:
```
[HitTest] point=(261.0,88.0) hit.Y=75.6 hit.H=16.8 hit.Start=87 hit.End=145 raw=40 IMECharCount=2 clamped=2
```
Florence correctly identified the line and character (raw=40 on a line starting at offset 87), but `IMECharCount=2` collapsed every click to offset 2.

### Fix — decouple caret display from WPF TextContainer entirely

The WPF TextEditor's `ITextSelection` / `ITextPointer` coordinate system cannot be used for caret rendering when the TextContainer is an empty stub.  The caret is now driven **exclusively by Florence hit-test geometry**, with no involvement of the WPF TextContainer.

**`FlowDocumentView.SetCaretAt(Windows.Foundation.Point clickPoint)`** was added:
1. Finds the Florence line whose Y band contains the click.
2. Within that line, proportionally maps the click X to the nearest character boundary using actual run widths.
3. Stores the resulting pixel rect in `_caretRect` and calls `InvalidateArrange()`.

`ArrangeOverride` positions the `_caret` Rectangle using `_caretRect`.

The `AttachSelection` / `OnSelectionChanged` / `UpdateCaretPosition` approach was removed — it depended on `ITextSelection.Changed` which never fired, and on `selection.Start.CharOffset` which returned stub values.

**`RichTextBox.OnPointerPressed`** now calls `fdv.SetCaretAt(unoPoint)` directly after `SetCaretPositionOnMouseEvent`.

---

## Problem 5 — Florence character width estimation inaccurate

### Root cause
`CharWidthRatio = 0.50` (estimated `width = fontSize * 0.5`) produced widths that did not match the actual `TextBlock` rendering, causing hit-test X positions to diverge from visual character positions.

### Fix — `TextMeasurer` using actual `TextBlock.Measure`

A `TextMeasurer` static class was added to `FlorenceEngine.cs`:

```csharp
internal static class TextMeasurer
{
    [ThreadStatic] private static TextBlock? _probe;

    internal static double MeasureWidth(string text, double fontSize, bool bold, bool italic)
    {
        _probe ??= new TextBlock();
        _probe.Text = text; _probe.FontSize = fontSize; /* bold/italic */
        _probe.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return _probe.DesiredSize.Width;
    }
}
```

`FlorenceLayoutEngine` now calls `TextMeasurer.MeasureWidth(...)` for all run widths and uses a binary-search `FindFitChars` for line-breaking, replacing the `CharWidthRatio` constant entirely.

---

## Files changed

| File | Change |
|------|--------|
| `WindowsShims/…/FlowDocumentView.uno.cs` | Caret overlay via `SetCaretAt`; `_lastMeasureWidth/Height` caching; caret `Rectangle` child at index 0; `ArrangeOverride` skips caret child when rebuilding lines |
| `WindowsShims/…/RichTextBox.uno.cs` | `OnPointerPressed`, `OnKeyDown`, `OnCharacterReceived`, `MapVirtualKey`; calls `fdv.SetCaretAt` |
| `WindowsShims/…/UnoFlowDocumentTextView.cs` | `Math.Clamp` on char offsets before `CreatePointerAtCharOffset` |
| `WindowsShims/…/FlorenceEngine.cs` | `TextMeasurer` class; `FindFitChars` binary search; removed `CharWidthRatio` |
| `WindowsShims/…/System.Windows/Input/Key.cs` | Added `PageUp`, `PageDown`, `Prior`, `Next` (values 33/34) |

---

## Remaining limitations

- **Text editing not yet wired**: `OnKeyDown` / `OnCharacterReceived` reach `TextEditorTyping` but the WPF `TextEditor`'s TextContainer has no document content, so typed characters have no effect.  The keyboard handler infrastructure is in place for the next session.
- **`_caretCharOffset` not tracked**: The Florence caret only knows its pixel position, not a character offset.  Connecting the pixel caret to a logical document position (for selection, keyboard navigation, and editing) requires `FlorenceTextPointer` to carry a proper offset.
- **Selection rendering not implemented**: click-drag to select text is not yet handled.

# Session 35 — Drag-and-Drop for RichTextBox

## Goal
Wire Uno Platform's cross-platform drag-and-drop API into the `RichTextBox` port so that
text can be dragged out of (or into) the control on all Uno Desktop targets without any
Win32 / WPF-only call sites.

## Architecture

### Uno drag-drop model
| API | Purpose |
|-----|---------|
| `UIElement.CanDrag = true` | Enables platform-initiated drag gesture (pointer-press + move) |
| `UIElement.AllowDrop = true` | Allows the element to receive drop events |
| `DragStarting` event | Fired by the platform when a drag gesture starts; populate `DataPackage` here |
| `DragEnter` / `DragOver` / `DragLeave` / `Drop` | Standard routed drag events; `DragEventArgs.GetPosition(UIElement)` returns the hit-test point |
| `DragEventArgs.DataView` | `DataPackageView` — async API to read the dropped content |
| `DragEventArgs.AcceptedOperation` | Set to `DataPackageOperation.Copy` to accept the drop |
| `DragStartingEventArgs.DragUI.SetContentFromDataPackage()` | Shows the default drag ghost |

There is **no** `UIElement.DoDragDrop()` static call in WinUI/Uno. The platform initiates the drag from `CanDrag` gestures. WPF's `DragDrop.DoDragDrop` call site in `TextEditorDragDrop.SourceDoDragDrop` is already gated by `#if HAS_UNO return;` (line 214) and remains so.

### WPF upstream
`TextEditorDragDrop.cs` already contained:
- `#if HAS_UNO return;` guard in `SourceDoDragDrop` (no change needed)
- `AllowDragDrop` already has a simplified HAS_UNO path (no change needed)
- `#if !HAS_UNO` gate around `AdornerLayer.Remove` calls in `DeleteCaret` (no change needed)

## Changes made

### `RichTextBlock.cs`

**New field:**
```csharp
private int _dropCaretOffset = -1;
```
Holds the character offset for the orange drop-insertion caret; `-1` means no active drag.

**Constructor additions:**
```csharp
AllowDrop = true;
CanDrag = true;
DragStarting += OnDragStarting;
DragEnter += OnDragEnter;
DragOver += OnDragOver;
DragLeave += OnDragLeave;
Drop += OnDrop;
```

**`TextRunItem` record extended:**
```csharp
private record TextRunItem(string Text, InheritedProperties Props, Hyperlink? Hyperlink = null, Run? SourceRun = null)
    : FlatItem(Props);
```
`SourceRun` tracks the originating WPF `Run` element so `InsertTextAt` can mutate the document model.
Passed as `run` in the `FlattenInlines` case for `Run` inlines; `null` for synthetic newline/break items.

**Drop caret rendering** (inside `RenderContent` after the keyboard caret block):
```csharp
if (_dropCaretOffset >= 0 && TryGetCaretX(lineFragments, _dropCaretOffset, out var dropCaretX))
{
    var dropRect = new Rectangle { Width = 2, Height = Math.Max(1, lineHeight),
        Fill = new SolidColorBrush(Color.FromArgb(255, 255, 140, 0)) };
    Canvas.SetLeft(dropRect, dropCaretX);
    Canvas.SetTop(dropRect, visualY);
    target.Children.Add(dropRect);
}
```
Orange (255, 140, 0) 2px caret appears at the hover position during drag.

**Event handlers:**

`OnDragStarting` — cancels if no selection; otherwise puts `GetTextRange(selMin, selMax)` into `DataPackage`:
```csharp
private void OnDragStarting(UIElement sender, DragStartingEventArgs e)
{
    ...
    e.Data.SetText(text);
    e.DragUI.SetContentFromDataPackage();
}
```

`OnDragEnter` / `OnDragOver` — accept text drops, update `_dropCaretOffset` via `HitTest`:
```csharp
e.AcceptedOperation = DataPackageOperation.Copy;
```

`OnDragLeave` — clears `_dropCaretOffset` and re-renders.

`OnDrop` (async) — reads text from `DataPackageView.GetTextAsync()`, inserts at drop position via `InsertTextAt`:
```csharp
private async void OnDrop(object sender, DragEventArgs e)
{
    var text = await e.DataView.GetTextAsync();
    InsertTextAt(insertAt, text);
}
```

**`GetTextRange(int start, int end)`** — uses existing `GetFullText()` and a substring.

**`InsertTextAt(int offset, string text)`** — walks `_flatItems`/`_flatItemCharOffsets` to find the
`TextRunItem` whose range contains the offset, then mutates `SourceRun.Text` directly. Falls back
to appending to the last run for end-of-content drops.

### `RichTextBox.cs`
No changes needed — `RichTextBox` wraps `RichTextBlock` and drag-drop is handled entirely at the
`RichTextBlock` layer. The control's `AllowDrop`/`CanDrag` are set on the renderer itself.

## Behaviour summary
| Gesture | Effect |
|---------|--------|
| Press + drag on selected text | Platform shows drag ghost; `DataPackage` contains selected text |
| Drag foreign text onto control | Orange caret tracks cursor position |
| Drop text | Inserted at caret position into the source `Run` in the `FlowDocument` |
| Drag leaves control | Orange caret cleared |

## Limitations
- Copy-only semantics: the source text is not deleted after drop (Move requires tracking source offset and deleting after drop — deferred).
- Single-`Run` insertion only: if the drop offset falls between two runs or on a paragraph boundary, the text is inserted into the nearest run. Cross-paragraph drops are not yet handled.
- External apps can drag text in (DataPackage.Text is accepted); dragging out to external apps depends on platform support (works on Linux/Skia Desktop).

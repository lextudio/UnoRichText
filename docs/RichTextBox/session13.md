# Session 13 — Promoting Upstream `TextElementCollection.cs`

Status: done.

## Goal

Enable upstream WPF `TextElementCollection<TextElementType>` and retire the
107-line local shim. This is Family A from the Session 12 wrap-up — the
collection that owns `BlockCollection`/`InlineCollection`/`ListItemCollection`
mutation and therefore controls whether children are reparented into their
new parent's `TextContainer` when added.

The cross-container blocker surfaced in Session 9 / re-confirmed in Session
11 (`paragraph.ContentStart` and `run.ContentStart` reporting
`NotInAssociatedTree`) lives in this file's `Add`/`Insert`/`Remove` paths.

## What this gets us

The previously-empty-text `TextRange(document.ContentStart,
document.ContentEnd).Text` now returns `"Alpha\r\nBeta"` for a two-paragraph
FlowDocument. The cross-paragraph acceptance test that was deferred from
Session 9/11 passes — Phase 1's `Select(start, end)` story is now reachable
via real WPF pointer semantics.

Three new tests added to [`TextRangeTests.cs`](../../../UnoRichText/src/LeXtudio.RichText.Tests/Documents/TextRangeTests.cs):

- `Text_ReturnsPlainTextAcrossParagraphs` — whole-document range, asserts
  both runs + `\r\n` separator.
- `Text_ReturnsSlicedPlainTextAcrossParagraphs` — partial range crossing
  paragraph boundary returns `"ha\r\nBe"`.
- (existing `Text_ReturnsRunPlainText`, etc., still green.)

## Changes

### Local shim removed

`WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextElementCollection.cs`
(107 lines, `ObservableCollection<T>`-backed) deleted.

### Upstream file un-excluded

[LeXtudio.Windows.csproj](../../../WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj):
flipped from removing upstream to removing the (now-deleted) local file
path.

### Upstream `TextElementCollection.cs` patched

Minimal `#if HAS_UNO` / partial-method additions:

- Class marked `partial`. No behavior change for WPF builds; allows
  `TextElementCollection.uno.cs` to add an `INotifyCollectionChanged`
  implementation as a side-channel.
- Added `using System.Collections.Specialized;`.
- Declared `partial void OnUnoCollectionChanged(NotifyCollectionChangedEventArgs args);`
  at the bottom of the class. With no implementation it's a zero-cost no-op
  on WPF.
- Inserted invocations of `OnUnoCollectionChanged(...)` at the end of
  `Add`, `Clear`, `Remove`, `InsertAfter`, `InsertBefore`. Arguments use
  the appropriate `NotifyCollectionChangedAction` (`Add` / `Remove` /
  `Reset`).
- `Invariant.Assert(owner is TextElement || owner is FlowDocument || owner
  is TextBlock)` — `TextBlock` removed under `HAS_UNO` (WinUI's
  `Microsoft.UI.Xaml.Controls.TextBlock` is not a text-tree owner in the
  shim).
- `_owner is TextBlock` branch in the `ContentStart`/`ContentEnd` accessor
  wrapped with `#if !HAS_UNO`.

### New Uno partial

[`TextElementCollection.uno.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextElementCollection.uno.cs):

```csharp
public partial class TextElementCollection<TextElementType> : INotifyCollectionChanged
    where TextElementType : TextElement
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    partial void OnUnoCollectionChanged(NotifyCollectionChangedEventArgs args)
        => CollectionChanged?.Invoke(this, args);
}
```

This preserves the `Blocks.CollectionChanged` / `Inlines.CollectionChanged`
contract that `RichTextBlock` already subscribes to — the renderer needs
zero changes for the event surface itself.

### Renderer indexer fix

`RichTextBlock.CollectFlatItems` used `blocks[i]` against the strongly-typed
indexer that the old `ObservableCollection<Block>`-based shim provided.
Upstream only exposes `IList.this[int]` (object-typed), so the loop is
rewritten to `foreach (var block in blocks)` with an index counter. Local
change in [`RichTextBlock.cs`](../../../UnoRichText/src/LeXtudio.RichText/Controls/RichTextBlock.cs).

### `List.GetListItemIndex`

Upstream `List.cs` had a `HAS_UNO` branch that called
`ListItems.IndexOf(item)` against the old `ObservableCollection`-based
shim. Updated to `((IList)ListItems).IndexOf(item)` since upstream
`TextElementCollection` only exposes the non-generic `IList.IndexOf`.

### SR strings

Added 7 new entries in [`SR.cs`](../../../WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/SR.cs)
that upstream `TextElementCollection` formats into exception messages:
`TextElementCollection_{CannotCopyToArrayNotSufficientMemory, IndexOutOfRange,
ItemHasUnexpectedType, NextSiblingDoesNotBelongToThisCollection, NoEnumerator,
PreviousSiblingDoesNotBelongToThisCollection, TextElementTypeExpected}`.

## Verification

```
Test Count: 62, Passed: 62, Failed: 0, Skipped: 0
```

(60 baseline + 2 new cross-paragraph TextRange tests.)

The Session 9/11 cross-container limitation is resolved: real
`TextElementCollection.Add` now routes through upstream
`TextContainer.InsertElementInternal`, which migrates the child element's
text tree into the parent's container. Pointers obtained from any element
inside the document now share the document's `TextContainer`, so
`TextRange.Text` walks correctly across element boundaries.

## What this unblocks for next sessions

- `RichTextBox.Select(start, end)` can now route `SelectedText` through
  `new TextRange(start, end).Text` (real WPF semantics) instead of the
  renderer's flat plain-text model.
- `TextEditor*` files (still `<Compile Remove>`'d) can be promoted next —
  they rely on `Inlines.Add` reparenting, validated above, plus
  `TextRange.Text`, validated in Session 11.
- The natural Session 14 candidate is `TextRangeEdit.cs` (range mutation
  primitives — Bold/Italic apply across ranges) or `TextEditorCharacters.cs`
  (character-formatting commands once `TextEditor.cs` is promoted).

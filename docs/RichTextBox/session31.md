# Session 31 — FlorenceTextContainer and FlorenceTextPointer

## Goal
Implement `FlorenceTextContainer` (implements `ITextContainer`) and `FlorenceTextPointer`
(implements `ITextPointer`) so that `StructuralCache` always holds a live text container on
HAS_UNO, and downstream code can hook `Changing`/`Change`/`Changed` events without crashing.

## Architecture change

```
StructuralCache
  ├── TextContainer   (System.Windows.Documents.TextContainer — null on HAS_UNO)
  ├── ITextContainer  (→ real TextContainer cast, or FlorenceTextContainer)   ← NEW
  └── FlorenceContainer (FlorenceTextContainer — always non-null)              ← NEW
```

## New types in `FlorenceEngine.cs`

### `MS.Internal.Florence.FlorenceTextPointer`

Implements `ITextPointer` modeled on `NullTextPointer`.  All positions in an empty container
are equivalent, so every distance is 0 and `MoveToNextContextPosition` always returns `false`.

Key decisions:
- `GetLocalValueEnumerator` delegates to `FormattingDependencyObject` (HAS_UNO shim for WPF `DependencyObject`)
- `ParentType` returns `typeof(object)` (no `FixedDocument` on HAS_UNO)
- `HasValidLayout` / `ValidateLayout` return false — layout is managed by Florence

### `MS.Internal.Florence.FlorenceTextContainer`

Implements `ITextContainer` modeled on `NullTextContainer`.

| Difference from NullTextContainer | Reason |
|-----------------------------------|--------|
| `IsReadOnly = false` | Future editing support |
| Real `Changing`, `Change`, `Changed` event fields | `InitializeForFirstFormatting` hooks these |
| Constructor takes `FlowDocument owner` | `Parent` property returns the owner |
| `BumpGeneration()` / `RaiseChanging()` / `RaiseChange()` / `RaiseChanged()` | Helpers for future mutation events |
| `ITextContainer.Highlights` returns `null` | Highlights subsystem not yet ported |
| `UndoManager` returns `null` | Undo not yet wired |

### `StructuralCache` additions

```csharp
// Constructor now always creates a FlorenceTextContainer.
_florenceContainer = new FlorenceTextContainer(owner);

// New properties:
internal ITextContainer ITextContainer
    => (ITextContainer?)TextContainer ?? _florenceContainer;

internal FlorenceTextContainer FlorenceContainer => _florenceContainer;
```

## Usings added to `FlorenceEngine.cs`

| Using | Provides |
|-------|---------|
| `System.Windows` | `LocalValueEnumerator`, `DependencyProperty`, `Rect` |
| `MS.Internal` | (reserved for future MS.Internal types) |
| `MS.Internal.Documents` | `UndoManager` (referenced by `ITextContainer`) |

## Still gated on HAS_UNO

- `FlowDocument.ContentStart` / `ContentEnd` — return WPF `TextPointer`, incompatible with `ITextPointer`
- `FlowDocument.InitializeForFirstFormatting` — still hooks `TextContainer.Highlights.Changed`
  (Florence has no `Highlights` implementation yet)
- `RichTextBox.InitializeTextContainer` — needs a real `TextContainer` for `TextEditor` wiring

## Future work (Session 32+)

1. **Un-gate `InitializeForFirstFormatting`** — implement a `FlorenceHighlights` stub so the
   `Highlights.Changed` subscription compiles and no-ops on HAS_UNO.
2. **Un-gate `ContentStart`/`ContentEnd`** — either shim `TextPointer` to wrap `ITextPointer`,
   or change the public API to return `ITextPointer` on HAS_UNO.
3. **Wire Preedit** into `FlowDocumentFormatter.OnContentInvalidated` to populate
   `FlorenceDocument.Pages`.

## Result
Both `net10.0-desktop` and `net10.0-windows10.0.19041.0` build with 0 errors, 0 warnings.
`StructuralCache` now exposes a live `FlorenceTextContainer` on HAS_UNO via `ITextContainer`
and `FlorenceContainer` properties.

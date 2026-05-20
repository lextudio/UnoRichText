# Session 32 — Un-gate InitializeForFirstFormatting and Uninitialize

## Goal
Un-gate `FlowDocument.InitializeForFirstFormatting` and `FlowDocument.Uninitialize` so that
text-container event subscriptions wire up on HAS_UNO via `FlorenceTextContainer`.

## Changes

### `FlorenceEngine.cs` — `FlorenceTextContainer`

Added a real `Highlights` instance (the upstream `System.Windows.Documents.Highlights` class,
already compiled in the project):

```csharp
private readonly Highlights _highlights;

internal FlorenceTextContainer(FlowDocument owner)
{
    ...
    _highlights = new Highlights(this);
}

Highlights ITextContainer.Highlights => _highlights;  // was: => null
```

This gives `InitializeForFirstFormatting` a non-null `Highlights.Changed` event to subscribe to.

### `FlowDocument.cs` — `InitializeForFirstFormatting`

Replaced `#if !HAS_UNO` (entire body gated) with a `#if HAS_UNO / #else` split that selects
the appropriate container:

```csharp
internal void InitializeForFirstFormatting()
{
#if HAS_UNO
    System.Windows.Documents.ITextContainer tc = _structuralCache.ITextContainer;
    tc.Changing += new EventHandler(OnTextContainerChanging);
    tc.Change += new TextContainerChangeEventHandler(OnTextContainerChange);
    tc.Highlights.Changed += new HighlightChangedEventHandler(OnHighlightChanged);
#else
    _structuralCache.TextContainer.Changing += ...;
    _structuralCache.TextContainer.Change += ...;
    _structuralCache.TextContainer.Highlights.Changed += ...;
#endif
}
```

The three handler methods (`OnTextContainerChanging`, `OnTextContainerChange`,
`OnHighlightChanged`) already existed on HAS_UNO with empty bodies — only their
bodies are gated, so the event subscriptions compile and no-op safely.

### `FlowDocument.cs` — `Uninitialize`

Same `#if HAS_UNO / #else` pattern. `_structuralCache.IsFormattedOnce = false` was moved
outside the gate (it is valid on both platforms):

```csharp
internal void Uninitialize()
{
#if HAS_UNO
    System.Windows.Documents.ITextContainer tc = _structuralCache.ITextContainer;
    tc.Changing -= ...;
    tc.Change -= ...;
    tc.Highlights.Changed -= ...;
#else
    _structuralCache.TextContainer.Changing -= ...;
    ...
#endif
    _structuralCache.IsFormattedOnce = false;  // un-gated — safe on both platforms
}
```

## Gates removed in this session

| Item | Status before | Status after |
|------|--------------|--------------|
| `InitializeForFirstFormatting` body | `#if !HAS_UNO` (blank on HAS_UNO) | HAS_UNO path uses `ITextContainer` |
| `Uninitialize` body | `#if !HAS_UNO` (blank on HAS_UNO) | HAS_UNO path uses `ITextContainer` |
| `_structuralCache.IsFormattedOnce = false` | inside `#if !HAS_UNO` | un-gated |
| `FlorenceTextContainer.Highlights` | returns `null` | returns real `Highlights` instance |

## Still gated on HAS_UNO

- `FlowDocument.ContentStart` / `ContentEnd` — return WPF `TextPointer` (incompatible with `ITextPointer`)
- `RichTextBox.Document` setter — `InitializeTextContainer(_document.TextContainer)` requires
  concrete `TextContainer`
- `OnTextContainerChanging` body — guard check uses `sender == _structuralCache.TextContainer`
  (always false on HAS_UNO since `TextContainer` is null; safe to leave gated)
- `OnTextContainerChange` / `OnHighlightChanged` bodies — reference PTS types, gated

## Result
Both `net10.0-desktop` and `net10.0-windows10.0.19041.0` build with 0 errors, 0 warnings.
`InitializeForFirstFormatting` and `Uninitialize` now execute real event hookup/teardown
on HAS_UNO via `FlorenceTextContainer`.

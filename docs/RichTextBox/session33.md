# Session 33 — TextContainer Unified: Eliminate Major HAS_UNO Gates

## Goal
The user observed that Florence should provide a seamless PTS API interface to eliminate
conditional compilation.  The key insight: `TextContainer.cs` is already `partial`, has no
`#if HAS_UNO` gates, and its entire TextTree infrastructure (`TextTreeRootNode`, `TextTreeText`,
etc.) is already compiled in the project.  The only barrier was a single gate in
`FlowDocument.Initialize` preventing `new TextContainer(this, false)` from running on HAS_UNO.
Removing that one gate unlocks most of the remaining `FlowDocument.cs` conditionals.

## Architecture clarification

The user's three-file model is already in place:
```
TextContainer.cs     (upstream WPF, partial, compiles on both — the unified implementation)
TextContainer.wpf.cs (future: PTS-heavy methods can move here as needed)
TextContainer.uno.cs (future: Florence-specific extensions, if any)
```
No new files were needed for this session.  `TextContainer`, `TextPointer`, and the full
`TextTree*` family all compile on HAS_UNO as-is.

## Gates removed

### `FlowDocument.cs — Initialize`
```csharp
// BEFORE
#if !HAS_UNO
    if (textContainer == null)
        textContainer = new TextContainer(this, false);
#endif

// AFTER — unconditional
if (textContainer == null)
    textContainer = new TextContainer(this, false);
```
This makes `_structuralCache.TextContainer` non-null on HAS_UNO for the first time.

### `FlowDocument.cs — ContentStart / ContentEnd`
```csharp
// BEFORE: returned null on HAS_UNO
// AFTER: unconditional
public TextPointer ContentStart { get { return _structuralCache.TextContainer.Start; } }
public TextPointer ContentEnd   { get { return _structuralCache.TextContainer.End;   } }
```

### `FlowDocument.cs — LogicalChildren`
```csharp
// BEFORE: returned EmptyEnumerator.Instance on HAS_UNO
// AFTER: unconditional
return new RangeContentEnumerator(_structuralCache.TextContainer.Start,
                                   _structuralCache.TextContainer.End);
```

### `FlowDocument.cs — IAddChild.AddChild`
```csharp
// BEFORE: Block.RepositionWithContent gated
// AFTER: unconditional — TextContainer.End is now valid
TextContainer textContainer = _structuralCache.TextContainer;
((Block)value).RepositionWithContent(textContainer.End);
```

### `FlowDocument.cs — IServiceProvider.GetService`
```csharp
// BEFORE: entire body gated
// AFTER: unconditional — returns ITextContainer or TextContainer service
if (serviceType == typeof(ITextContainer))   return _structuralCache.TextContainer;
else if (serviceType == typeof(TextContainer)) return _structuralCache.TextContainer as TextContainer;
```

### `RichTextBox.cs — Document setter`
```csharp
// BEFORE: two gated blocks
// AFTER: unconditional
_document.TextContainer.CollectTextChanges = false;  // on detach
...
_document.TextContainer.CollectTextChanges = true;   // on attach
this.InitializeTextContainer(_document.TextContainer);
```

## Remaining gates in `FlowDocument.cs`

| Lines | Reason to stay gated |
|-------|----------------------|
| 89 | Static constructor: `OverrideMetadata`, `TelemetryControls` — WPF framework infra |
| 746/780 | `SetDpi` / `OnPropertyChanged` — `FrameworkPropertyMetadata`, `e.IsAValueChange` — WPF property system |
| 864 | `IsEnabledCore` — `RichTextBox.IsDocumentEnabled` property (WPF RichTextBox) |
| 931/960 | `GetObjectPosition`/`OnChildDesiredSizeChanged` — `TextContainerHelper`, PTS-specific helpers |
| 1080 | `OnNewParent` — `CoerceValue(IsEnabledProperty)` — WPF property coercion |
| 1203 | `PixelsPerDip` — `FontCache.Util.PixelsPerDip` — WPF font subsystem |
| 1431/1490/1514 | Handler bodies (`OnHighlightChanged`, `OnTextContainerChanging`, `OnTextContainerChange`) — DirtyTextRange, PTS layout invalidation |

## Result
Both `net10.0-desktop` and `net10.0-windows10.0.19041.0` build with 0 errors, 0 warnings.
`ContentStart`, `ContentEnd`, `LogicalChildren`, `IAddChild.AddChild`, `IServiceProvider.GetService`,
and the `RichTextBox.Document` setter are all unconditional.
`new TextContainer(this, false)` now creates a real, traversable text tree on HAS_UNO.

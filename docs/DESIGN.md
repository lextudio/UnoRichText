# LeXtudio.RichText — Design Document

## Problem

Uno Platform does not properly implement the WinUI document model needed for rich inline text layout:

| WinUI Type | Uno Status |
|---|---|
| `RichTextBlock` | Only `Blocks` implemented; no styling properties (FontFamily, Foreground, TextWrapping, etc.) |
| `InlineUIContainer` | Not implemented at all |
| `TextBlock.Inlines` | Functional for Run/Bold/Italic/Span/Hyperlink/LineBreak |

The result: markdown rendered natively on Uno is missing inline code boxes, images, and all paragraph text inside nested containers (lists, tables, blockquotes, code blocks).

## Goal

Provide a drop-in rich text library for Uno that:

1. Mirrors the WinUI/WPF `Microsoft.UI.Xaml.Documents` and `Microsoft.UI.Xaml.Controls` API surface exactly (same type names, same property names).
2. Implements `InlineUIContainer` — the key missing piece — so UIElements (styled code boxes, images) can be embedded in inline text flows.
3. Implements `RichTextBlock` backed by **PretextSharp** for proper inline layout with text measurement.
4. Targets `net10.0-desktop` (Uno Skia). Windows (`net10.0-windows`) consumers continue using native WinUI types via `#if WINDOWS_APP_SDK`.

## Namespace Alignment

```
LeXtudio.UI.Xaml.Documents   ↔   Microsoft.UI.Xaml.Documents
LeXtudio.UI.Xaml.Controls    ↔   Microsoft.UI.Xaml.Controls
```

Type names are identical to their WinUI counterparts so consumers can switch with a using alias:

```csharp
#if WINDOWS_APP_SDK
using Inlines = Microsoft.UI.Xaml.Documents.InlineCollection;
// ...
#else
using Inlines = LeXtudio.UI.Xaml.Documents.InlineCollection;
// ...
#endif
```

## Project Structure

```
LeXtudio.RichText/
  ext/
    PretextSharp/               ← git submodule
  src/
    LeXtudio.RichText/
      LeXtudio.RichText.csproj  ← net10.0-desktop, refs Pretext.Uno
      Documents/
        Inline.cs               ← abstract base; common formatting properties
        InlineCollection.cs     ← ObservableCollection<Inline>
        Run.cs                  ← leaf text node
        Span.cs                 ← container for child inlines
        Bold.cs                 ← Span, FontWeight = Bold
        Italic.cs               ← Span, FontStyle = Italic
        LineBreak.cs            ← hard line break
        InlineUIContainer.cs    ← embeds any UIElement in the flow
        Hyperlink.cs            ← Span + Click event
      Controls/
        RichTextBlock.cs        ← Panel-derived control; PretextSharp layout
        InheritedProperties.cs  ← internal helper for property cascading
  docs/
    DESIGN.md                   ← this file
```

## Document Model

### `Inline` (abstract)

Common formatting properties. All are nullable/NaN so that child values inherit from parent `Span` or from the owning `RichTextBlock`:

```csharp
public abstract class Inline
{
    public FontFamily? FontFamily { get; set; }
    public double FontSize { get; set; } = double.NaN;      // NaN = inherit
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public Brush? Foreground { get; set; }                   // null = inherit
    public TextDecorations TextDecorations { get; set; } = TextDecorations.None;
}
```

### `Run : Inline`

```csharp
public class Run : Inline
{
    public string Text { get; set; } = string.Empty;
}
```

### `Span : Inline`

```csharp
public class Span : Inline
{
    public InlineCollection Inlines { get; } = new();
}
```

### `Bold : Span`

Constructor sets `FontWeight = FontWeights.Bold`.

### `Italic : Span`

Constructor sets `FontStyle = FontStyle.Italic`.

### `LineBreak : Inline`

No extra members. Signals a hard line break in the flow.

### `InlineUIContainer : Inline`

```csharp
public class InlineUIContainer : Inline
{
    public UIElement? Child { get; set; }
}
```

The `Child` is measured before PretextSharp layout runs. Its `DesiredSize.Width` becomes the `ExtraWidth` on the corresponding `RichInlineItem`.

### `Hyperlink : Span`

```csharp
public class Hyperlink : Span
{
    public Brush? Foreground { get; set; }
    public event EventHandler? Click;
    internal void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
}
```

Rendered as underlined text in `Foreground` (or inherited color). Fragment `TextBlock` gets a `Tapped` handler that calls `RaiseClick()`.

### `InlineCollection`

```csharp
public class InlineCollection : ObservableCollection<Inline> { }
```

Changes raise `CollectionChanged`, which `RichTextBlock` listens to in order to call `InvalidateMeasure()`.

## `RichTextBlock` Control

### Base class

Extends `Panel`. A single `Canvas _canvas` lives in `Panel.Children`; all fragment `TextBlock`s and `UIElement`s are added to `_canvas.Children`. This avoids circular measure invalidation: changes to `_canvas.Children` do not propagate `InvalidateMeasure()` to the parent `Panel`.

### Public API (mirrors WinUI `RichTextBlock`)

```csharp
public class RichTextBlock : Panel
{
    public InlineCollection Inlines { get; }
    public FontFamily FontFamily { get; set; }
    public double FontSize { get; set; }
    public FontWeight FontWeight { get; set; }
    public Brush Foreground { get; set; }
    public TextWrapping TextWrapping { get; set; }
    public double LineHeight { get; set; }   // 0 = auto (FontSize * 1.4)
    public event EventHandler<HyperlinkClickEventArgs>? LinkClicked;
}
```

### Layout Pipeline

```
Inlines changed
  └── InvalidateMeasure()

MeasureOverride(availableSize)
  1. FlattenInlines()           → List<FlatItem>
  2. Measure UIElement children (InlineUIContainer.Child) directly (no parent needed)
  3. BuildRichInlineItems()     → RichInlineItem[] for PretextSharp
       • Text run       → RichInlineItem(text, fontString, Break=Normal, ExtraWidth=0)
       • UI container   → RichInlineItem("", fontString, Break=Never, ExtraWidth=child.DesiredSize.Width)
       • LineBreak      → RichInlineItem("\n", fontString)
  4. PretextLayout.PrepareRichInline(items) → _prepared
  5. PretextLayout.MeasureRichInlineStats(_prepared, maxWidth) → stats
  Return Size(stats.MaxLineWidth, stats.LineCount * ResolvedLineHeight)

ArrangeOverride(finalSize)
  1. _canvas.Children.Clear()
  2. PretextLayout.WalkRichInlineLineRanges(_prepared, finalSize.Width, range =>
       line = MaterializeRichInlineLineRange(prepared, range)
       x = 0; y = lineIndex * ResolvedLineHeight
       for each fragment in line.Fragments:
         x += fragment.GapBefore
         if flatItems[fragment.ItemIndex] is UIContainerItem → position child UIElement
         else → create TextBlock with fragment.Text and inherited props
         Canvas.SetLeft(el, x); Canvas.SetTop(el, y)
         _canvas.Children.Add(el)
         x += fragment.OccupiedWidth
       lineIndex++
     )
  3. _canvas.Width = finalSize.Width; _canvas.Height = lineCount * ResolvedLineHeight
  4. _canvas.Arrange(new Rect(0, 0, finalSize.Width, totalHeight))
  Return Size(finalSize.Width, totalHeight)
```

### Property Inheritance During Flatten

`FlattenInlines` carries an `InheritedProperties` context through the inline tree:

```csharp
internal record InheritedProperties(
    FontFamily FontFamily, double FontSize,
    FontWeight FontWeight, FontStyle FontStyle,
    Brush Foreground, TextDecorations TextDecorations)
{
    internal InheritedProperties Merge(Inline inline) => this with
    {
        FontFamily      = inline.FontFamily ?? FontFamily,
        FontSize        = double.IsNaN(inline.FontSize) ? FontSize : inline.FontSize,
        FontWeight      = inline.FontWeight,
        FontStyle       = inline.FontStyle,
        Foreground      = inline.Foreground ?? Foreground,
        TextDecorations = inline.TextDecorations | TextDecorations,
    };
}
```

### PretextSharp Font String

Converts `InheritedProperties` → CSS font shorthand:

```
"[italic ][<weight> ]<size>px <family>"

Examples:
  "14px Segoe UI"
  "bold 14px Segoe UI"
  "italic 500 14px Courier New"
```

Weight mapping: 700+ → "bold", other non-400 → numeric string, 400 → omitted.

### `ResolvedLineHeight`

```csharp
private double ResolvedLineHeight =>
    LineHeight > 0 ? LineHeight : FontSize * 1.4;
```

## Integration with WinUI.Markdown

`NativeMarkdownVisitor` uses compile-time aliases at the top of the file:

```csharp
#if WINDOWS_APP_SDK
using RichTextBlock     = Microsoft.UI.Xaml.Controls.RichTextBlock;
using InlineCollection  = Microsoft.UI.Xaml.Documents.InlineCollection;
using Run               = Microsoft.UI.Xaml.Documents.Run;
using Bold              = Microsoft.UI.Xaml.Documents.Bold;
using Italic            = Microsoft.UI.Xaml.Documents.Italic;
using Span              = Microsoft.UI.Xaml.Documents.Span;
using LineBreak         = Microsoft.UI.Xaml.Documents.LineBreak;
using InlineUIContainer = Microsoft.UI.Xaml.Documents.InlineUIContainer;
using Hyperlink         = Microsoft.UI.Xaml.Documents.Hyperlink;
#else
using RichTextBlock     = LeXtudio.UI.Xaml.Controls.RichTextBlock;
using InlineCollection  = LeXtudio.UI.Xaml.Documents.InlineCollection;
using Run               = LeXtudio.UI.Xaml.Documents.Run;
using Bold              = LeXtudio.UI.Xaml.Documents.Bold;
using Italic            = LeXtudio.UI.Xaml.Documents.Italic;
using Span              = LeXtudio.UI.Xaml.Documents.Span;
using LineBreak         = LeXtudio.UI.Xaml.Documents.LineBreak;
using InlineUIContainer = LeXtudio.UI.Xaml.Documents.InlineUIContainer;
using Hyperlink         = LeXtudio.UI.Xaml.Documents.Hyperlink;
#endif
```

All method bodies use `Xaml*` aliases — no scattered `#if` inside logic.

`WinUI.Markdown.csproj` adds the project reference conditionally:

```xml
<ItemGroup Condition="'$(TargetFramework)' != 'net10.0-windows10.0.19041.0'">
  <ProjectReference Include="..\..\..\..\LeXtudio.RichText\src\LeXtudio.RichText\LeXtudio.RichText.csproj" />
</ItemGroup>
```

## Open Items / Future Work

- [x] **Text selection**: `IsTextSelectionEnabled`, `SelectedText`, `SelectAll()`, `ClearSelection()`. Pointer drag-select, Ctrl+A, Ctrl+C implemented.
- [ ] **Performance**: Cache `PreparedRichInline` across layout passes when content hasn't changed (use a dirty flag set by `InlineCollection.CollectionChanged`).

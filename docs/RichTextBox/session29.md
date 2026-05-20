# Session 29 — Enable Upstream FlowDocument.cs

## Goal
Replace the local minimal FlowDocument shim (~45 lines) with the full upstream WPF `FlowDocument.cs`
(1,727 lines), compiled via `#if !HAS_UNO` guards for the PTS layout engine and WPF-only APIs.

## Changes

### WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for upstream `FlowDocument.cs` — it now compiles.
- Added `<Compile Remove="System.Windows\Documents\FlowDocument.cs" />` to exclude the local shim.
- Comment: `<!-- FlowDocument.cs: WPF enabled (Session 29). Local shim removed. -->`

### New shim files

**System.Windows/Documents/DocumentPaginatorShims.cs**
- `IDocumentPaginatorSource` interface stub.
- `DocumentPaginator` abstract class stub (WPF printing stack, not needed on HAS_UNO).

### Modified shim files

**System.Windows/Markup/Modifiability.cs**
- Added `Inherit` value to `Modifiability` enum.
- Added `Readability` enum (`Unreadable`, `Readable`, `Inherit`).

**System.Windows/Markup/LocalizabilityAttribute.cs**
- Added `public Readability Readability { get; set; }` property.
- Removed inline `Readability`/`Modifiability` enum definitions (they now live in Modifiability.cs).

**GlobalUsings.cs**
- Added `global using Readability = System.Windows.Markup.Readability;`
  so upstream files can use `Readability.Unreadable` in `[Localizability]` attributes.

**System.Windows/FrameworkElement.cs (FrameworkContentElement)**
- Added `protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e) { }` —
  upstream FlowDocument seals and overrides this.
- Added `protected virtual bool IsEnabledCore => true;` — FlowDocument overrides it to propagate
  RichTextBox read-only state.

### Upstream patches (ext/wpf/.../FlowDocument.cs)

All gates use `#if !HAS_UNO` (WPF-only code) or `#if HAS_UNO … #else … #endif` (alternate impl).

| Lines | Change |
|-------|--------|
| 89–92 | Gate `DefaultStyleKeyProperty.OverrideMetadata`, `FocusableProperty.OverrideMetadata`, `TelemetryControls.FlowDocument` in static constructor |
| 174–175 | `ContentStart`: return `null` on HAS_UNO (no TextContainer) |
| 188–190 | `ContentEnd`: return `null` on HAS_UNO |
| 750–759 | `SetDpi` body gated — references `_pixelsPerDip`, `StructuralCache`, `_formatter` |
| 779–809 | `OnPropertyChanged` body gated — references `_structuralCache` cache invalidation |
| 815–822 | `OnCreateAutomationPeer` — return `null` on HAS_UNO (no `DocumentAutomationPeer`) |
| 848–861 | `IsEnabledCore` — return `true` on HAS_UNO (no `RichTextBox.IsDocumentEnabled` lookup) |
| 843–845 | `LogicalChildren` — use `MS.Internal.Controls.EmptyEnumerator.Instance` on HAS_UNO |
| 938–952 | `GetObjectPosition` — gate `TextContainerHelper`, `_structuralCache.TextContainer`, `ContentPosition.Missing` |
| 948–979 | `OnChildDesiredSizeChanged` body gated |
| 986–988 | `InitializeForFirstFormatting` body gated |
| 996–999 | `Uninitialize` body gated |
| 1029 | `ComputePageMargin`: use `lineHeight = 0` on HAS_UNO |
| 1071 | `OnNewParent`: gate `CoerceValue(IsEnabledProperty)` |
| 1067–1091 | `BottomlessFormatter` property gated (entire property) |
| 1087–1092 | `StructuralCache` property gated |
| 1130–1136 | `IFlowDocumentFormatter Formatter` property gated |
| 1142–1176 | `IsLayoutDataValid`, `TextContainer`, `PixelsPerDip` properties gated |
| 1226–1239 | `Initialize(TextContainer)` body gated |
| 1248–1263 | `OnPageMetricsChanged`: gate `_structuralCache`/`_formatter` block; fire `PageSizeChanged` on HAS_UNO |
| 1387–1438 | `OnHighlightChanged` body gated |
| 1444–1458 | `OnTextContainerChanging` body gated |
| 1466–1553 | `OnTextContainerChange` body gated |
| 1598–1602 | Private fields: `_structuralCache`, `_formatter`, `_pixelsPerDip` gated |
| 1689–1690 | `IAddChild.AddChild`: gate `TextContainer` usage |
| 1735–1741 | `IServiceProvider.GetService`: gate `_structuralCache.TextContainer` returns |
| 1707–1721 | `IDocumentPaginatorSource.DocumentPaginator`: return `null` on HAS_UNO |

### Upstream patches (ext/wpf/.../RichTextBox.cs)

| Lines | Change |
|-------|--------|
| 399 | Gate `_document.TextContainer.CollectTextChanges = false` |
| 424–427 | Gate `CollectTextChanges = true` and `InitializeTextContainer(_document.TextContainer)` |

### Upstream patches (ext/wpf/.../TextElementCollection.cs)

| Lines | Change |
|-------|--------|
| 715–719 | `FlowDocument` branch: assign `textContainer = null` on HAS_UNO |

## Result
Both `net10.0-desktop` and `net10.0-windows10.0.19041.0` targets build with 0 errors, 0 warnings.

The local FlowDocument shim is fully replaced by the upstream file.
PTS layout engine code (`StructuralCache`, `FlowDocumentFormatter`, `IFlowDocumentFormatter`,
`FlowDocumentPaginator`) is gated out on HAS_UNO — these Windows-only types are not available
in the cross-platform build.

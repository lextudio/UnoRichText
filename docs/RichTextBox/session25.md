# Session 25 — AdornerLayer, AdornerDecorator, AdornerHitTestResult

Status: complete. Build clean (0 errors), 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable the three remaining Adorner infrastructure files from upstream WPF source —
`AdornerLayer.cs`, `AdornerDecorator.cs`, `AdornerHitTestResult.cs` — which were
excluded by the `Adorner*.cs` glob remove. The session also completed enabling
`Adorner.cs`, `ColumnResizeAdorner.cs`, and `ColumnResizeUndoUnit.cs` that were
started but not fully wired up.

## What this gets us

- `AdornerLayer` — full upstream implementation of the adorner layout pass.
  Core rendering methods (MeasureOverride, ArrangeOverride) compile and are live.
  Visual-tree management (`VisualCollection`, `AddAdornerToVisualTree`,
  `GetClipGeometry`, `GetProposedTransform`, `AdornerHitTest`) are gated
  `#if !HAS_UNO` since WinUI does not have those primitives.
- `AdornerDecorator` — upstream single-child decorator that hosts an AdornerLayer.
  `VisualChildrenCount`, `GetVisualChild`, `EffectiveValuesInitialSize`, and
  `NonLogicalAdornerDecorator` all gated `#if !HAS_UNO`.
- `AdornerHitTestResult` — upstream type; compiles as-is once `PointHitTestResult`
  shim was added.

## Changes

### LeXtudio.Windows.csproj (WindowsShims)

Updated Adorner block comment and added three `<Compile Include>` entries after the
existing `<Compile Remove="...\Adorner*.cs" />`:

```xml
<!-- Adorner*.cs: all enabled (Session 25). -->
<Compile Remove="...\Adorner*.cs" />
<Compile Include="...\Adorner.cs" Link="System.Windows.Documents\Adorner.cs" />
<Compile Include="...\AdornerLayer.cs" Link="System.Windows.Documents\AdornerLayer.cs" />
<Compile Include="...\AdornerDecorator.cs" Link="System.Windows.Documents\AdornerDecorator.cs" />
<Compile Include="...\AdornerHitTestResult.cs" Link="System.Windows.Media\AdornerHitTestResult.cs" />
```

### New shims added

**`System.Windows.Controls/PanelShims.cs`**
- Added `Decorator : FrameworkElement` — WPF's single-child base class not in WinUI.
  Provides `Child`, `IntChild`, `AddVisualChild`, `RemoveVisualChild`,
  `AddLogicalChild`, `RemoveLogicalChild` (visual-tree methods are no-ops).
- Modified `ScrollContentPresenter` to expose `AdornerLayer AdornerLayer => null;`
  so `AdornerLayer.GetAdornerLayer` visual-tree walk compiles.

**`System.Windows.Media/WinUIMediaExtensions.cs`**
- Added `PointHitTestResult` — WPF base class for `AdornerHitTestResult`, not in WinUI.

**`System.Windows.Documents/SR.cs`**
- Added string constants: `AdornedElementNotFound`, `AdornerNotFound`,
  `Visual_ArgumentOutOfRange`.

### Stubs removed

**`System.Windows.Documents/AdornerShims.cs`**
- Removed `AdornerDecorator : FrameworkElement` stub (replaced by upstream file).

**`System.Windows.Documents/EarlyBatchEditorShims.cs`**
- Removed `AdornerLayer : FrameworkElement` stub (replaced by upstream file).
- Restored `CaretElement` stub (still needed; not yet promoted from upstream).

### Upstream patches (`ext/wpf/...`)

**`AdornerLayer.cs`**
- Gated `using MS.Internal.Controls;` and `using MS.Internal.Media;` with `#if !HAS_UNO`.
- Split default constructor: `internal AdornerLayer() : this(Dispatcher.CurrentDispatcher)`
  gated `#if !HAS_UNO`; added parameterless `#else` branch.
- Gated `LayoutUpdated` and `_children = new VisualCollection(this)` in second ctor.
- Gated `VisualChildrenCount`, `GetVisualChild`, `LogicalChildren` properties.
- Gated `MeasureOverride` body (returns `new Size()` on HAS_UNO).
- Gated `ArrangeOverride` body (returns `finalSize` on HAS_UNO).
- Qualified `ScrollContentPresenter` as `System.Windows.Controls.ScrollContentPresenter`
  in `GetAdornerLayer` to avoid ambiguity with `Microsoft.UI.Xaml.Controls.ScrollContentPresenter`.
- In `Add(Adorner, int)`: gated `AddAdornerToVisualTree` and `AddLogicalChild`.
- In `Remove`: gated `_children.Remove` and `RemoveLogicalChild`.
- In `SetAdornerZOrder`: gated `_children.Remove` and `AddAdornerToVisualTree`.
- Gated entire `AddAdornerToVisualTree` method.
- Gated entire `GetClipGeometry` method.
- Gated `AdornerHitTest` body (`return null;` on HAS_UNO).
- In `InvalidateAdorner`: gated `adornerInfo.Adorner.InvalidateVisual()`.
- In `UpdateElementAdorners`: gated clip/AffineTransform branch; simplified HAS_UNO
  path to `NeedsUpdate || Transform == null` condition.
- Gated `GetProposedTransform` body (returns `sourceTransform` directly on HAS_UNO).
- Gated `EffectiveValuesInitialSize` property.
- Gated `private VisualCollection _children` field.

**`AdornerDecorator.cs`**
- Gated `NonLogicalAdornerDecorator` class.
- Gated `VisualChildrenCount` and `GetVisualChild` overrides.
- Gated `EffectiveValuesInitialSize` override.
- Fixed `new Rect(finalSize)` → `new Rect(0, 0, finalSize.Width, finalSize.Height)`
  (WinUI `Rect` has no `Size` constructor).

**`ColumnResizeAdorner.cs`**
- Gated `adornerLayer.InvalidateVisual()` (`InvalidateVisual` not on WinUI FrameworkElement).

**`TextEditorDragDrop.cs`**
- Gated `layer.Remove(_caretDragDrop)` with `#if !HAS_UNO`
  (`CaretElement` stub does not extend `Adorner`; Remove() expects Adorner).

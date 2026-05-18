# Session 10 — Enable WPF TextElement.cs; TextTree Infrastructure Complete

Status: done.

## Goals

- Remove the local `TextElement.cs` shim and replace it with WPF's upstream
  `TextElement.cs` from the ext source tree.
- Ensure all TextTree infrastructure (TextContainer, TextPointer, TextPointerBase,
  ITextPointer, all TextTree undo units) remains enabled and compiling.
- Keep all 51 tests passing with 0 failures.

## Work Done

### WPF TextElement.cs enabled

- Removed `<Compile Remove>` entry for
  `ext/wpf/.../System/Windows/Documents/TextElement.cs` from `LeXtudio.Windows.csproj`.
- Patched WPF `TextElement.cs` to be `partial` so Uno-specific additions can
  coexist: `public abstract partial class TextElement`.
- Added `#if !HAS_UNO` / `#if HAS_UNO` guards in WPF `TextElement.cs` for:
  - `ContainerTextElementField` static field (replaced by our standalone class).
  - `LogicalChildren` override (Uno uses `_children` list instead).
  - `OnPropertyChanged` override (WPF property system hooks not used in WinUI).
  - `IAddChild.AddChild` / `IAddChild.AddText` implementation (too many WPF-only
    type dependencies; Uno partial provides its own).
  - `DeepEndInit` body (uses `IsInitialized` + `LogicalChildren` — guarded away).
- Deleted local `WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/TextElement.cs`.

### TextElement.uno.cs rewritten

Previous version was a complete standalone shim; it is now a true partial that
extends WPF's `TextElement` with Uno-specific content only:

- **WinUI-only DependencyProperties** not registered by WPF `TextElement`:
  `DefaultStyleKey`, `Focusable`, `IsEnabled`, `AccessKey`, `CharacterSpacing`,
  `AllowFocusOnInteraction`, `ExitDisplayModeOnAccessKeyInvoked`,
  `HorizontalTextAlignment`, `IsAccessKeyScope`, `IsTextScaleFactorEnabled`,
  `KeyTipHorizontalOffset/PlacementMode/VerticalOffset`, `Language`.
- **Uno layout-host infrastructure** (not in WPF): `_children` list, `_layoutHost`
  field, `LayoutHost` property, `SetLayoutHostRecursive`, `AddLogicalChild`,
  `InsertLogicalChild`, `RemoveLogicalChild`, `ClearLogicalChildren`,
  `IndexOfLogicalChild`, `ChildObjects`.
- **`IInputElement`** interface implementation (WPF `TextElement` implements
  `IAddChild`; `IInputElement` is added here for WinUI compatibility).
- **`IAddChild`** Uno stub that routes `AddChild`/`AddText` to `AddLogicalChild`.
- **`LogicalChildren`** override using `_children` list.
- **Virtual input handlers** not on WPF `TextElement`: `OnMouseLeftButtonDown`,
  `OnMouseLeftButtonUp`, `OnKeyDown`.
- **`EffectiveValuesInitialSize`** / **`DTypeThemeStyleKey`** overrides (WPF
  internal virtuals expected by `Hyperlink`, `Run`, etc.).
- `Resources`, `Name`, `XamlRoot`, WinUI key-tip events, `FindName`.

### New shims added

| File | What was added |
|------|---------------|
| `System.Windows/SystemFonts.cs` | `MessageFontFamily`, `MessageFontStyle`, `MessageFontWeight`, `MessageFontSize`, `ThemeMessageFontSize` — defaults used by WPF `TextElement` font DP registrations. |
| `System.Windows/FrameworkElement.cs` | `EffectiveValuesInitialSize`, `DTypeThemeStyleKey` virtuals; `IsInitialized`, `LogicalChildren`, `VerifyAccess()`. |
| `System.Windows/PropertySystem.cs` | `FrameworkPropertyMetadataOptions.AffectsArrange`, `AffectsParentArrange`, `SubPropertiesDoNotAffectRender`. |
| `System.Windows/Media/TextEffectCollection.cs` | `static readonly Empty` field. |
| `System.Windows/Documents/SR.cs` | `TextElement_UnmatchedEndPointer`, `TextSchema_ThisInlineUIContainerHasAChildUIElementAlready`, `TextSchema_ThisBlockUIContainerHasAChildUIElementAlready`, `TextSchema_TextIsNotAllowed`. |

### Misc fixes

- `Run.uno.cs`: `IsEmpty` shadowing WPF's `TextElement.IsEmpty` — added `new` keyword.
- `TextElementCollection.cs` (WPF ext): kept blocked via `<Compile Remove>`; local
  shim stays active (different type-parameter name `T` vs WPF's `TextElementType`).

## Architecture

WPF `TextElement.cs` now provides the full text-tree integration:
`ContentStart`, `ContentEnd`, `ElementStart`, `ElementEnd`, `TextElementNode`,
`IsEmpty`, `IsInTree`, `NextElement`, `PreviousElement`, `GetCommonAncestor`,
`EnsureTextContainer`, `Reposition`, `RepositionWithContent`, `DeepEndInit` —
all from upstream WPF source with no local replication.

Uno-specific tree management (`_children`, `LayoutHost`) lives purely in
`TextElement.uno.cs` as a clean partial extension.

## Test Result

```
Passed!  - Failed: 0, Passed: 51, Skipped: 6, Total: 57
```

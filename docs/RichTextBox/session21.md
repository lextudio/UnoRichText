# Session 21 — FrameworkTextComposition + FrameworkRichTextComposition + TextEffectResolver + TextSelectionHighlightLayer

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable four upstream WPF files: the text-composition pair used by the IME/input
pipeline, the text-effect resolver, and the selection highlight layer.

## What this gets us

- `FrameworkTextComposition` and `FrameworkRichTextComposition` supply the
  composition objects passed through `TextEditor` and `TextEditorTyping` during
  IME input; the composition positions (`ResultStart`/`ResultEnd`,
  `CompositionStart`/`CompositionEnd`) are now backed by real WPF logic.
- `TextEffectResolver` provides public `Resolve` + `TextEffectTarget.Enable/Disable`
  APIs for applying animated text effects to document ranges.
- `TextSelectionHighlightLayer` provides the real WPF selection highlight layer
  rather than the three-line stub.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for all four upstream files.

### Deleted stub classes
- `TextSelectionHighlightLayer` stub removed from `TableAndHighlightShims.cs`.
- `FrameworkTextComposition` stub removed from `TextCompositionEventArgs.cs`.

### System.Windows/Input/TextCompositionEventArgs.cs
- Added `TextCompositionAutoComplete` enum (Off/On) — used in the
  `FrameworkTextComposition` constructor chain.
- Added `InputManager` stub class with `Current => null` — required by the
  `FrameworkTextComposition(InputManager, IInputElement, object)` constructor.
- Added internal `TextComposition(InputManager, IInputElement, string, TextCompositionAutoComplete)`
  constructor — matches the base-class ctor signature called by the upstream file.
- Added `CompositionText` property to `TextComposition` — set during IME composition
  updates; `FrameworkTextComposition.SetCompositionPositions` assigns it.
- Added `virtual Complete()` to `TextComposition` — `FrameworkTextComposition`
  overrides this to set `_pendingComplete = true`.

### System.Windows/Media/TextEffect.cs
- Added `PositionStart` and `PositionCount` int properties.
- Added `Clone()` method (MemberwiseClone) — `TextEffectResolver.Resolve` clones
  the effect template for each resolved text segment.

### MS.Internal/Text/TextDpi.cs
- Added `DynamicPropertyReader` stub in `MS.Internal.Text` namespace (matches the
  `using MS.Internal.Text;` import in `TextEffectResolver.cs`).
- `GetTextEffects(DependencyObject)` reads `TextElement.TextEffectsProperty`
  from the element; used by `TextEffectTarget.Enable/Disable/IsEnabled`.

### Upstream patches (ext/wpf)

**FrameworkTextComposition.cs**
- Gated `CompleteCurrentComposition(ITfDocumentMgr)` (lines 149–167) with
  `#if !HAS_UNO ... #endif` — TSF COM call, Windows-only.
- Gated `GetCurrentCompositionView(ITfDocumentMgr)` (lines 169–181) with
  `#if !HAS_UNO ... #endif` — TSF COM call, Windows-only.
- Gated `GetComposition(ITfContext)` (private, lines 313–330) with
  `#if !HAS_UNO ... #endif` — TSF COM call, Windows-only.

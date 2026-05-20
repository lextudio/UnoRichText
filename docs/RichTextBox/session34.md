# Session 34 — Spell Checking: Full Pipeline for RichTextBox

## Goal
Integrate Uno Platform's cross-platform spell checking (PR #22937) into the `RichTextBox` port,
drawing red wavy underlines on misspelled words and wiring the WPF `SpellCheck.IsEnabled` API.

## Architecture

### Uno's spell checking model (6.7+)
- `ISpellCheckingService` — public interface in `Microsoft.UI.Xaml.Documents` (Uno.UI.dll)
- `SpellCheckingService` — implementation in `Uno.WinUI.SpellChecking.dll` (separate package)
- Enabled by adding `SpellChecking` to `<UnoFeatures>` in the project file
- Uses WeCantSpell.Hunspell with a bundled en_US dictionary
- `SpellCheck(List<int> wordBoundaries, string text)` → `List<(int correctionStart, int correctionEnd)?>`,
  one entry per word-boundary segment; `null` = correct, non-null = misspelled (offsets relative to segment start)

### Our RichTextBox render model
Rendering is **XAML Canvas-based** (not direct Skia). Each text fragment is a `TextBlock` child of `Canvas`.
Decorations (underlines, strikethrough, selection highlights) are `Rectangle` or `Polyline` children.
There is no direct `SKCanvas` access in this layer.

### Version situation
| Uno.Sdk | Uno.WinUI | `ISpellCheckingService` | `SpellChecking` UnoFeature |
|---------|-----------|------------------------|---------------------------|
| 6.5.31  | 6.5.237   | NOT present             | NOT recognized             |
| 6.7.0-dev.52 | 6.7.x | **public** in Uno.UI   | **recognized**, deploys `Uno.WinUI.SpellChecking.dll` |

## Changes made

### `RichTextBlock.cs`
New additions:

**`IsSpellCheckEnabled` DP** (default `false`):
```csharp
public static DependencyProperty IsSpellCheckEnabledProperty { get; } =
    DependencyProperty.Register(nameof(IsSpellCheckEnabled), typeof(bool), typeof(RichTextBlock),
        new PropertyMetadata(false, OnLayoutPropertyChanged));
```

**Reflection-based service loader** (forward-compatible with any Uno version):
```csharp
private static readonly Lazy<ISpellCheckBridge?> _spellCheckingService = new(TryLoadSpellCheckService);

private static ISpellCheckBridge? TryLoadSpellCheckService()
{
    var svcType = Type.GetType(
        "Uno.WinUI.SpellChecking.SpellCheckingService, Uno.WinUI.SpellChecking",
        throwOnError: false);
    if (svcType == null) return null;
    var instance = Activator.CreateInstance(svcType, typeof(RichTextBlock));
    var spellCheckMethod = svcType.GetMethod("SpellCheck")!;
    return new ReflectionSpellCheckBridge(instance!, spellCheckMethod);
}
```
Gracefully returns `null` when `Uno.WinUI.SpellChecking.dll` is not present (Uno 6.5.x).

**Word boundary computation** (simple letter/digit transitions, no ICU dependency):
```csharp
private static List<int> GetWordBoundaries(string text)
{
    var boundaries = new List<int>();
    int i = 0;
    while (i < text.Length)
    {
        bool isWord = char.IsLetterOrDigit(text[i]) || text[i] == '\'';
        while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '\'') == isWord)
            i++;
        boundaries.Add(i);
    }
    return boundaries;
}
```

**`UpdateSpellCorrections()`** — called at end of `MeasureOverride` after `_preparedSegments` is built:
- Reconstructs full plain text from `_flatItems` (TextRunItem.Text + `￼` for UI containers)
- Calls `SpellCheck(wordBoundaries, text)`
- Caches `_spellWordBoundaries` and `_spellCorrections`

**`DrawSpellUnderlineForFragment()`** — called from `RenderContent` after `DrawDecorations` for each `TextRunItem`:
- Maps absolute char range of fragment against correction segments
- Calls `DrawSpellUnderline()` for any overlapping misspelling

**`DrawSpellUnderline()`** — draws a `Polyline` with zigzag pattern:
```csharp
// step=3.5, amplitude=1.5px, positioned at y + lineHeight * 0.92
// stroke: red #DC0000, thickness 1.5
```

**Local bridge types** (inner types):
```csharp
private interface ISpellCheckBridge
{
    List<(int correctionStart, int correctionEnd)?> SpellCheck(List<int> wordBoundaries, string text);
}

private sealed class ReflectionSpellCheckBridge(object service, MethodInfo spellCheckMethod) : ISpellCheckBridge { ... }
```

### `RichTextBox.cs`
```csharp
public static DependencyProperty IsSpellCheckEnabledProperty { get; } = ...
public bool IsSpellCheckEnabled { get; set; }  // forwards to _renderer.IsSpellCheckEnabled

public SpellCheck SpellCheck =>
    _spellCheck ??= new SpellCheck(v => IsSpellCheckEnabled = v);
```

### `TextBoxBaseShims.cs`
`SpellCheck` now accepts an `Action<bool>` callback so `IsEnabled` changes propagate:
```csharp
public SpellCheck(Action<bool> isEnabledChanged) { _isEnabledChanged = isEnabledChanged; }

public bool IsEnabled
{
    get => _isEnabled;
    set { _isEnabled = value; _isEnabledChanged?.Invoke(value); }
}
```
Legacy `SpellCheck(object owner)` overload kept for WPF `TextBoxBase.SpellCheck` call site.

### `LeXtudio.RichText.Sample.csproj`
```xml
<UnoFeatures>SkiaRenderer;SpellChecking;</UnoFeatures>
```
Silently ignored by Uno.Sdk 6.5.31 (with a warning), will activate automatically on 6.7+.

## Also fixed in this session
`TextElementCollection.TextContainer` was returning `null` for `FlowDocument` under `HAS_UNO`,
causing a crash in `Add()`. The leftover `#else null` branch was removed — `FlowDocument`
now unconditionally returns `_structuralCache.TextContainer` (consistent with session 33 work).

## Activation path
1. Upgrade `global.json` to `Uno.Sdk: 6.7.0-dev.52` (or first stable 6.7 release)
2. No code changes needed — `SpellChecking` UnoFeature is already in the sample csproj
3. `Uno.WinUI.SpellChecking.dll` is deployed to output automatically
4. Reflection loader finds `SpellCheckingService`, spell checking activates
5. Set `IsSpellCheckEnabled = true` (or WPF: `SpellCheck.IsEnabled = true`) on any `RichTextBox`

## Verified in 6.7.0-dev.52 test build
- `ISpellCheckingService` is **public** in `Microsoft.UI.Xaml.Documents` (Uno.UI.dll) ✅
- `Uno.WinUI.SpellChecking.dll` is emitted when `SpellChecking` UnoFeature is enabled ✅
- `SpellCheckingService.SpellCheck(List<int>, string)` signature matches bridge exactly ✅
- Full build: 0 errors against current Uno.Sdk 6.5.31 ✅

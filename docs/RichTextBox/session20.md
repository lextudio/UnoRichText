# Session 20 — TextRangeSerialization + TextEditorCharacters

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable `TextRangeSerialization.cs` and `TextEditorCharacters.cs` from upstream WPF.

## What this gets us

XAML/fragment serialization types compile from upstream source. The serialization
write path (WriteXaml) and the character-formatting command handlers (_OnApplyProperty,
ToggleUnderline) are now backed by upstream WPF logic rather than the previous
two-line stubs.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for TextRangeSerialization.cs and TextEditorCharacters.cs.

### Deleted stub files
- `System.Windows/Documents/TextEditorCharacters.cs` — replaced by upstream.
- `System.Windows/Documents/ValidationAndSerializationShims.cs` — replaced by upstream.

### TextEditorSystemShims.cs
- Added `XamlTypeMapper` stub and `XmlParserDefaults.DefaultMapper` to
  `System.Windows.Markup` namespace (used by TextRangeSerialization's serialization
  context but never called on HAS_UNO).

### System.Windows/Documents/SR.cs
- Added 27 TextEditorCharacters display string constants (KeyResetFormat,
  KeyToggleBold, KeyToggleItalic, KeyToggleUnderline, KeyToggleSubscript,
  KeyToggleSuperscript, KeyIncreaseFontSize, KeyDecreaseFontSize, KeySetFontSize,
  KeySetFontFamily, KeySetForeground, KeySetBackground, KeySpellCheck, and
  display-string counterparts).

### WinUIDependencyPropertyExtensions.cs
- Added `OwnerType` instance extension property (returns null; serialization-only path).
- Added `FromName(string, Type)` static extension method (returns null; serialization-only).

### FrameworkElement.cs (FrameworkContentElement)
- Added `LanguageProperty` static DependencyProperty (used by TextRangeSerialization
  to detect the xml:lang attribute special case).

### System.Windows/Media/TextDecorationCollection.cs
- Added `TryRemove(TextDecorationCollection, out TextDecorationCollection)` method —
  WPF extension that removes matching decorations and returns whether any were removed.
  Used by TextEditorCharacters.ToggleUnderline.

### System.Windows/Media/Imaging/ImagingShims.cs
- Added `BitmapImage` shim class with `UriSourceProperty` and `CacheOptionProperty`
  DependencyProperty stubs (used by TextRangeSerialization WriteEmbeddedObject; the
  entire image-package path is gated `#if !HAS_UNO`).

### System.Windows/Documents/XamlRtfShims.cs
- Added `WpfPayload.AddImage(object)` no-op stub (serialization path, gated `#if !HAS_UNO`).

### Upstream patches (ext/wpf)

**TextRangeSerialization.cs**
- Replaced `new DependencyObject()` (2x, property-bag usage) with
  `#if HAS_UNO new FormattingDependencyObject() #else new DependencyObject() #endif`.
- Qualified `Image` type checks (lines 548–549 and 1900) with
  `global::System.Windows.Controls.Image` to resolve ambiguity with
  `Microsoft.UI.Xaml.Controls.Image`.
- Gated `WriteEmbeddedObject` image-package block (Image/WpfPayload.AddImage/BitmapImage
  references) with `#if !HAS_UNO ... #endif`.

**TextEditorCharacters.cs**
- Patched `ToggleUnderline`'s `toggledTextDecorations.Add(TextDecorations.Underline)`
  (called when TryRemove returns false) with:
  `#if HAS_UNO foreach (var dec in TextDecorations.Underline) toggledTextDecorations.Add(dec);`
  `#else toggledTextDecorations.Add(TextDecorations.Underline); #endif`
  (WPF's overload accepts a collection; WinUI's Add takes a single TextDecoration.)

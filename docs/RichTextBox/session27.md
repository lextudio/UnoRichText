# Session 27 — net10.0-only TFM Migration

Status: complete. Build clean (0 errors on net10.0-desktop), 62-test baseline confirmed (all passing).

## Goal

Remove all net9.0 target frameworks from the dependency chain and fix the cascading
build errors that resulted from upgrading UnoRichText to net10.0 while dependencies
remained on net9.0.

## Changes

### WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj

- TFMs: `net9.0-desktop;net10.0-desktop` → `net10.0-desktop` (Windows adds `net10.0-windows10.0.19041.0`)
- Windows PropertyGroup condition simplified from covering both `net9.0-windows` and `net10.0-windows` to `net10.0-windows10.0.19041.0` only.

### TextCore.Uno/src/LeXtudio.UI.Text.Core/LeXtudio.UI.Text.Core.csproj

- TFM: `net9.0-desktop;net10.0-desktop` → `net10.0-desktop`

### TextCore.Uno/src/LeXtudio.TextBox/LeXtudio.TextBox.csproj

- TFMs: `net9.0-desktop;net9.0-windows10.0.19041.0;net10.0-desktop;net10.0-windows10.0.19041.0` → `net10.0-desktop;net10.0-windows10.0.19041.0`
- All PropertyGroup/ItemGroup conditions simplified to reference `net10.0-windows10.0.19041.0` only.

### UnoRichText/src/LeXtudio.RichText/Controls/RichTextBlock.cs

- Added `#if !WINDOWS_APP_SDK` guard around `using Pretext;` — Pretext.Uno is
  `net10.0-desktop` only and is not available in Windows-targeted builds.

## Notes

- The `LeXtudio.RichText` test project targets `net10.0-desktop` only — Windows build
  of the library is not exercised by tests.
- The `LeXtudio.RichText.csproj` still has a dead `net9.0-windows10.0.19041.0`
  PropertyGroup condition (pre-existing, not introduced here). The Windows TFM
  (`net10.0-windows10.0.19041.0`) compiles the non-WINDOWS_APP_SDK (Panel-based)
  code path since that condition never fires. Fixing the Windows pass-through path
  (which inherits from the sealed `Microsoft.UI.Xaml.Controls.RichTextBlock`) is
  deferred as future work.

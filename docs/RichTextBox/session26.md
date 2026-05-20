# Session 26 — TextEditorSpelling

Status: complete. Build clean (0 errors), 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable `TextEditorSpelling.cs` from upstream WPF source, replacing the local stub
in `EarlyBatchEditorShims.cs`. The stub covered the spelling command handlers
(CorrectSpellingError / IgnoreSpellingError) but had no implementations.

## What this gets us

- `TextEditorSpelling` upstream — full spelling command handler logic: correct, ignore,
  query-enabled. On HAS_UNO the Speller is always null so all handlers short-circuit
  cleanly; no `#if !HAS_UNO` guards required in the upstream file.
- `XmlLanguage.GetSpecificCulture()` — added to XmlLanguage shim so that
  `SetSelectedText(correctedText, language.GetSpecificCulture())` compiles.
  Returns `CultureInfo.GetCultureInfo(IetfLanguageTag)` with InvariantCulture fallback.

## Changes

### LeXtudio.Windows.csproj (WindowsShims)

- Replaced `<Compile Remove="...\TextEditorSpelling.cs" />` with a comment:
  `<!-- TextEditorSpelling.cs: WPF enabled (Session 26). -->`

### EarlyBatchEditorShims.cs

- Removed `TextEditorSpelling` stub class (4 methods); replaced by upstream.
- Extended `SpellingError` stub with `IgnoreAll()` (no-op).
- Extended `Speller` stub with `GetError(...)` (returns null) and
  `GetNextSpellingErrorPosition(...)` (returns null) so the stub continues to
  satisfy any remaining callers while Speller files remain excluded.

### TextEditorSystemShims.cs

- Added `XmlLanguage.GetSpecificCulture()` method to the XmlLanguage shim.

### Upstream patches

None required — `TextEditorSpelling.cs` compiles as-is on HAS_UNO because all
Speller interactions are null-conditional (`?.`) and return `SpellingError`
(the stub type) which is always null at runtime.

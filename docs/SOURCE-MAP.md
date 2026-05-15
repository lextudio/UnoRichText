# UnoRichText Source Map — Archived

> **This document is no longer maintained.** It tracked a WPF source-first port of `System.Windows.Documents` that has been abandoned. See [DESIGN.md](DESIGN.md) for the current scope: a WinUI `RichTextBlock`-compatible control for Uno, nothing more.

## Why archived

The previous plan mapped UnoRichText document types to upstream WPF sources with a migration ladder (`Direct-share` / `Share-with-partials` / `Uno-only`). Under the narrower current goal, that mapping is no longer relevant:

- UnoRichText document types now mirror **WinUI** (`Microsoft.UI.Xaml.Documents.*`), not WPF.
- Document-model shimming is gap-fill only, driven by what `RichTextBlock` actually needs to render.
- Most WPF document types have no WinUI rendering path (see `ext/shims/docs/ARCHITECTURE.md` → "Do Not Port").

## If you need to track a new type

Add it to [DESIGN.md](DESIGN.md) under the relevant architecture layer, with a one-line justification of which `RichTextBlock` behavior requires it. Do not resurrect the migration ladder.

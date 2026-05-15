# UnoRichText Provenance Log — Archived

> **This document is no longer maintained.** It tracked upstream WPF commit hashes for files imported under a source-first port plan that has been abandoned. See [DESIGN.md](DESIGN.md) for the current scope.

## Why archived

UnoRichText no longer derives its document model from WPF source. The control mirrors WinUI semantics; document-model gaps are filled with minimal local shims. There is no "import the upstream file then track its revision" workflow anymore.

## If a shim is derived from external source

Note the source as a comment at the top of the shim file. There is no central manifest.

## Historical note

The `LeXtudio.Windows` shim library (separate repo) still vendors and links some upstream WPF files. Provenance for that library is tracked there (`ext/shims/docs/PROVENANCE.md`), not here.

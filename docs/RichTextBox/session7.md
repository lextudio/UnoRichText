# Session 7 — Pre-Layout Selection Snapshot Fix

Status: done.

Goal:

- Fix the behavior gap from Session 6 where `SelectAll()` after
  post-assignment `Document.Blocks` mutation could leave `SelectedText` empty.

Changes:

- Updated `RichTextBlock` selection path:
  - Added `EnsureFlatItemsSnapshot()`.
  - Called it from `SelectAll()` and `BuildSelectedText()`.
- This ensures selection APIs have up-to-date character offsets even before
  the next layout pass.
- Restored stronger runtime assertions in `RichTextBoxTests` to require
  selected text contains both paragraph payloads (`Alpha`, `Beta`).

Runtime command:

- `dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -p:EnableRichTextBox=true -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.RichTextBoxTests`

Verification:

- Runtime mode: 4 passed, 0 failed, 0 skipped.
- Plain VSTest mode: skipped by design via runtime-host guard.

Next recommended slice:

- Add dedicated tests for `Select(start,end)` across paragraph boundaries to
  validate offset slicing and newline behavior consistently.

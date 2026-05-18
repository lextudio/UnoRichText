# Session 5 — Document Replacement Host Wiring

Status: done.

Goal:

- Protect the Phase 0 `RichTextBox` host behavior when replacing `Document`.
- Verify old `FlowDocument` detaches from renderer and new document attaches.

Changes:

- Added `ReplacingDocument_ReattachesTextLayoutHost` to
  `Controls/RichTextBoxTests.cs`.
- Test asserts:
  - Initial `control.Document.TextLayoutHost` is assigned.
  - New external document starts with `TextLayoutHost == null`.
  - After replacement, old document host is null and new document host is set.

Runtime command:

- `dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -p:EnableRichTextBox=true -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.RichTextBoxTests`

Verification:

- Runtime mode: 3 passed, 0 failed, 0 skipped.
- Plain VSTest mode (non-runtime host): skipped by design due to
  `RequireUnoRuntimeHost` one-time setup.

Next recommended slice:

- Add one UI-level test that mutates `Document.Blocks` after assignment and
  verifies control-facing state remains consistent for selection APIs.

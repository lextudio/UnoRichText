# Session 6 — Post-Assignment Mutation Coherence

Status: done.

Goal:

- Verify `RichTextBox` remains coherent when the assigned `FlowDocument` is
  mutated after binding (`Document.Blocks.Add(...)`).
- Protect selection-facing API behavior in this scenario.

Changes:

- Added `MutatingAssignedDocumentBlocks_KeepsSelectionApisCoherent` in
  `Controls/RichTextBoxTests.cs`.
- Test flow:
  - Assign `FlowDocument` with one paragraph (`Alpha`).
  - Mutate document by appending second paragraph (`Beta`).
  - Assert block count updates through `control.Document`.
  - Call `SelectAll()`.
  - Assert selection offsets are ordered and `SelectedText` API remains
    non-null/coherent.

Runtime command:

- `dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -p:EnableRichTextBox=true -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.RichTextBoxTests`

Verification:

- Runtime mode: 4 passed, 0 failed, 0 skipped.
- Plain VSTest mode: skipped by design via runtime-host guard.

Observed gap:

- After post-assignment block mutation, `SelectAll()` currently does not
  guarantee populated `SelectedText` content in this Phase 0 host path.
  Covered as a known behavior gap for future selection/render synchronization
  work.

Next recommended slice:

- Add selection-change event tests (`SelectionChanged`) for keyboard/pointer
  driven selection once synthetic input hooks are in place for the runtime test
  host.

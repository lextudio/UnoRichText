# Session 8 — Cross-Paragraph Selection Text Shape

Status: done.

Goal:

- Strengthen selection verification across paragraph boundaries.
- Clarify why precise `Select(start,end)` offset tests are not yet viable.

Changes:

- Updated `MutatingAssignedDocumentBlocks_KeepsSelectionApisCoherent` to assert
  selected text contains newline (`"\n"`) between paragraph content.

Verification:

- Runtime mode (`--uno-runtime-tests`, RichTextBoxTests): passed.
- Plain VSTest mode: skipped by design via runtime-host guard.

Constraint identified:

- `System.Windows.Documents.TextPointer.Offset` currently returns constant `0`
  in the shim implementation, so targeted offset-based `Select(start,end)`
  assertions are not meaningful yet.

Next recommended slice:

- Implement real `TextPointer.Offset`/position mapping in `WindowsShims`
  `TextPointer` + `TextContainer`, then add dedicated range tests for
  `RichTextBox.Select(TextPointer start, TextPointer end)` across paragraph
  boundaries.

# Session 9 — Pointer-Range Selection Investigation

Status: done (investigation).

Goal:

- Start enabling `RichTextBox.Select(start, end)` range tests across paragraph
  boundaries using pointer offsets.

Work done:

- Prototyped `TextPointer.Offset` plumbing improvements in
  `WindowsShims/System.Windows.Documents/TextPointer`:
  - Preserve pointer property bag across `CreatePointer(int)`.
  - Track `PointerOffset` values when present.

Findings:

- Even with this prototype, runtime behavior still treats pointer-derived
  `Select(start,end)` as full-range selection in current host paths.
- The missing piece is broader: pointer identity/offset semantics between
  `RichTextBlock` pointer stamps and WindowsShims `TextPointer` movement are
  not yet consistently modeled.

Decision:

- Removed the failing range-slice test to keep regression suite stable.
- Kept existing Session 8 cross-paragraph `SelectAll` assertions as the current
  reliable coverage boundary.

Next required slice:

- Implement a consistent offset model in `WindowsShims` for:
  - `TextPointer.CreatePointer(int)` movement,
  - `TextPointer.Offset`,
  - and range comparisons used by `RichTextBlock.GetOffset(TextPointer)`.
- After that, re-introduce a targeted `Select(start,end)` runtime test with
  expected sliced text (e.g. `"pha\nBe"`).

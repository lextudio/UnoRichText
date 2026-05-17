# RichEditBox Backup Plan

This document keeps the `RichEditBox` workstream alive as a fallback while the main effort shifts to the WPF-style `RichTextBox` port.

The goal here is not to compete with the `RichTextBox` port. It is to preserve the useful work already done, record what still remains, and make it easy to resume if we need a pragmatic WinUI-shaped editor sooner.

## Current state

`LeXtudio.UI.Xaml.Controls.RichEditBox` already has a meaningful scaffold:

- WinUI-shaped dependency properties and events exist.
- A real editing host exists via `LeXtudio.UI.Controls.TextBox`.
- A `LeXtudio.UI.Text.RichEditTextDocument` model is attached.
- Document-to-host and host-to-document sync exists in basic form.
- Formatting accelerators were partially wired.
- Collapsed-selection typing-format state exists in the document layer.
- A `RichTextBlock` overlay path exists for richer rendering experiments.

This means the control is no longer just an API stub. It is a functioning prototype with enough structure to continue incrementally.

## What worked before

These milestones were reached during the earlier track and are worth keeping:

- `Ctrl+B` no longer deleted the selection and replaced it with a space.
- Selection/caret interaction became stable enough that bolding a selected word could visually work.
- Logging and diagnostics were added and proved useful for debugging event order.
- The diagnostics path was made optional rather than always-on.

Those fixes matter because they narrowed the problem from "keyboard shortcut is broken" to "editing/rendering model is still too plain-text and transient."

## Core limitation

The current `RichEditBox` is still fundamentally a plain-text editor with rich-text aspirations:

- the editing surface is `LeXtudio.UI.Controls.TextBox`
- the backing model is not yet the source of truth for rich inline formatting
- the render overlay can show formatting, but typed input tends to fall back to plain host behavior

So the main unsolved problem is not just keyboard shortcuts. It is the split-brain architecture:

1. plain-text editing host
2. rich-text document model
3. optional rich renderer overlay

Until those three are unified better, formatting can flash, disappear, or fail to persist across typing.

## Recommended scope if we resume

If we come back to `RichEditBox`, we should treat it as a staged compatibility editor, not a full WinUI clone in one jump.

Recommended order:

1. Stabilize document editing semantics.
2. Persist inline formatting across subsequent typing.
3. Make selection/caret/document updates share one source of truth.
4. Only then broaden WinUI API coverage.

## Remaining milestones

### Checkout status

The current balanced checkout for this fallback track is unit-test oriented:

- `RichEditTextDocumentTests.CollapsedSelectionToggle_AppliesToTypedText`
- `RichEditTextDocumentTests.CaretInputFormat_PrefersCaretOverDefault`
- `RichEditTextDocumentTests.CaretInputFormat_ClearsWhenCaretMoves`
- `RichEditTextDocumentTests.Typing_BoldThenItalicOnly_CreatesExpectedRuns`

Those tests cover the document-layer typing-format behavior for collapsed selections.

Latest local checkout attempt:

- command: `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj`
- result: passed, 36 tests
- unrelated warning: `Tmds.DBus.Protocol` 0.21.2 has a known high severity advisory

### Phase 1: Make the document authoritative

The biggest remaining task is to stop letting `_editorHost.Text` be the effective truth for rich content.

Needed work:

- route insert/replace/delete operations through `Document.Selection`
- make selection replacement mutate document text/runs, not just raw plain text
- keep `_editorHost` as caret/input infrastructure, but not as the sole content owner
- define one canonical conversion path between document state and visible editor text

Exit criteria:

- typing text after bold/italic keeps the expected formatting state
- replacing selected text updates document structure predictably
- document programmatic changes no longer fight the host text

### Phase 2: Persist typing attributes

Status: document-layer behavior is implemented for character-format typing state. Control-level visual confirmation is still needed because the host/overlay architecture can still hide or blur correct document state.

We already learned that formatting accelerators alone are not enough. The editor needs a typing-format state.

Needed work:

- track current typing attributes on collapsed selection: done in `RichEditTextDocument`
- when selection is non-empty, apply formatting to selected content: done for character runs
- when selection collapses after a format command, preserve the springloaded typing state: done through caret input format preservation
- apply the typing state to newly inserted text: done for document `TypeText` and host text-diff insertion paths

Important attributes:

- bold: covered by tests
- italic: covered by tests
- underline: implemented through the same format path, but should get a dedicated test before this phase is fully closed
- hyperlink intent
- paragraph alignment

Exit criteria:

- `Ctrl+B`, `Ctrl+I`, `Ctrl+U` affect future typing when the selection is collapsed
- selected text keeps formatting after the selection is collapsed
- subsequent input does not silently revert to plain text

Remaining checkout:

- add/confirm underline-specific unit coverage
- visually verify `Ctrl+B`, `Ctrl+I`, and `Ctrl+U` in the sample because renderer/host sync is still a separate risk

### Phase 3: Replace markdown-style transforms with document edits

Status: started. The live `RichEditBox` sample toolbar now uses document-level formatting for bold, italic, and underline instead of markdown-style text wrapping. Formatting accelerators also use document formatting, with underline routed through an explicit command helper so repeated `Ctrl+U` behaves as a toggle.

Some earlier behavior leaned toward wrapping plain text, which is useful for debugging but not a real rich text editor.

Needed work:

- move toolbar/accelerator actions to document-level formatting operations: done for the sample toolbar and built-in formatting accelerators
- avoid `WrapSelection(...)` as the long-term implementation path: done for the live sample toolbar; the helper remains as a temporary public fallback
- represent formatting as runs/spans in the document model
- keep plain-text transforms only as temporary fallback helpers

Exit criteria:

- bold/italic/underline change formatting, not source text syntax
- copied plain text remains plain text
- rich operations survive renderer refresh

Current checkout:

- unit: `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj` passes with 35 tests
- visual: still needed in the sample for selected text, collapsed-selection typing, and repeated `Ctrl+B` / `Ctrl+I` / `Ctrl+U`

### Phase 4: Unify host and renderer

Status: started. The live sample now includes a compact document snapshot beside the `RichEditBox`, showing selection range, active character format, plain text, and document format runs. This gives visual checkout a direct view of the document model while the overlay and host are being compared.

Right now there is still a tension between the editor host and the render overlay.

Possible approaches:

1. Keep `TextBox` as the input host and make overlay rendering authoritative for presentation.
2. Replace the host gradually with richer document-aware editing primitives.

If we continue this track, option 1 is the faster backup plan.

Needed work:

- keep caret/selection geometry from host
- map host selection to document selection reliably: in progress; the sample snapshot exposes mismatches immediately
- repaint overlay from document after every edit transaction
- avoid flicker or temporary mismatch between overlay and host

Exit criteria:

- no more "flash of bold, then plain text"
- selection/caret remain visible and correctly positioned
- overlay and typed content stay in sync

Current checkout:

- build: `dotnet build UnoRichText/src/LeXtudio.RichText.Sample/LeXtudio.RichText.Sample.csproj` passes
- unit: `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj` passes with 35 tests
- visual: use the live snapshot while testing selection collapse, typing after format changes, and host/overlay repaint timing

### Phase 5: Clipboard and common editing commands

Status: undo/redo are landed and stable. `Ctrl+Z` / `Ctrl+Y` flow through the `TextBox` host as document-owner editing commands; `Document.Undo()` / `Document.Redo()` replay text, formatting runs, and selection together. Formatting toggles record their own undo entries, snapshots carry selection bounds, and a delta-merge formatting path preserves per-run attributes (color, font, etc.) when toggling bold/italic over a selection. A scoped retro-apply window lets a host-driven `TextChanged` handler color the just-typed characters (WinUI parity). `RichEditBox.TextChanged` now also surfaces document-driven updates so external toolbars refresh after undo/redo.

The public WinUI control will feel incomplete until normal editor actions behave well.

Needed work:

- copy/cut/paste from `Document.Selection`
- delete/backspace behavior against selections and collapsed carets
- enter/new paragraph behavior
- select all
- undo/redo transaction boundaries: started for keyboard command routing

Exit criteria:

- basic editing behaves like a normal editor
- clipboard round-trips plain text reliably
- rich clipboard can be deferred if needed, but plain text must be solid

Current checkout:

- unit: `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj` passes with 36 tests
- visual: type formatted text in the RichEditBox Gallery editor, then verify `Ctrl+Z` and `Ctrl+Y` undo/redo both text and visible formatting

### Phase 6: WinUI API parity polish

Status: started. Read-only behavior is wired: `IsReadOnly` now propagates to the editor host (gating typing, paste, delete), and the formatting accelerator and editing-command handlers swallow `Ctrl+B/I/U`, `Ctrl+Z`, `Ctrl+Y` when the control is read-only — matching how WinUI's `RichEditBox` ignores those gestures on a non-editable surface.

Remaining work in this phase:

- `ITextDocument` / `ITextRange` behavior gaps
- proofing-related events and flyouts
- candidate window events
- placeholder/header/description polish

Current checkout:

- unit: `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj` passes with 49 tests
- visual: toggle `IsReadOnly` in the sample and verify that typing, paste, `Ctrl+B`, `Ctrl+Z` are all suppressed while selection/caret remain usable.

## Debugging notes

If this backup plan resumes, keep these debugging habits:

- keep diagnostics disabled by default
- re-enable only around event-order or selection bugs
- log:
  - key gesture received
  - selection start/length before command
  - document text before/after command
  - host text before/after sync
  - whether update came from host or document

That logging was high leverage earlier and will still help.

## Suggested next fallback milestone

If we need to resume `RichEditBox`, the best next milestone is:

`Replace the toolbar and accelerator markdown-style commands with document-level formatting operations, then visually verify that Ctrl+B / Ctrl+I / Ctrl+U persist through the RichEditBox host and overlay.`

That is the smallest milestone that meaningfully changes the user experience instead of just adding more API surface.

## Relationship to RichTextBox plan

Current recommendation:

- primary path: keep pushing the WPF-style `RichTextBox` port
- fallback path: keep `RichEditBox` documented and resumable

Why:

- `RichTextBox` has a more natural source-reuse story from WPF
- `RichEditBox` can still become a practical WinUI-shaped editor, but it needs more architecture work
- the work already done on `RichEditBox` is still valuable as a backup and as a source of editing-host lessons

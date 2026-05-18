# WPF RichTextBox vs UnoRichText RichEditBox Architecture

This note compares WPF's `System.Windows.Controls.RichTextBox` with the current
`LeXtudio.UI.Xaml.Controls.RichEditBox` implementation, using:

- WPF reference:
  `WindowsShims/ext/wpf/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/RichTextBox.cs`
- UnoRichText implementation:
  `UnoRichText/src/LeXtudio.RichText/Controls/RichEditBox.cs`
- UnoRichText document layer:
  `UnoRichText/src/LeXtudio.RichText/TextDocument/RichEditTextDocument.cs`
  and related range/selection types.

The goal is not to copy WPF's public API. `RichEditBox` is a WinUI-shaped
control. The useful comparison is architectural: where does document authority
live, who owns editing transactions, and how rendering, selection, undo, IME, and
formatting stay coherent.

## Executive Summary

WPF `RichTextBox` is a thin policy and hosting layer over a single integrated
editing stack:

```text
RichTextBox
  -> TextBoxBase
     -> TextEditor
        -> FlowDocument.TextContainer
        -> TextSelection / TextPointer
        -> FlowDocumentView render scope
```

The WPF control's most important job is attaching exactly one `FlowDocument` to
the inherited `TextBoxBase` editing pipeline. Once attached, document mutation,
selection, undo, command routing, hit testing, and rendering all operate on the
same `TextContainer`.

The current UnoRichText `RichEditBox` is a pragmatic three-layer editor:

```text
RichEditBox
  -> LeXtudio.UI.Controls.TextBox          plain-text input host
  -> LeXtudio.UI.Text.RichEditTextDocument rich text model and undo/runs
  -> RichTextBlock overlay                 formatted presentation
```

This is a valid staged implementation, but it is not yet the same architecture
as WPF. The highest-risk area is the split authority between `_editorHost.Text`,
`Document.Buffer`, `Document.Selection`, and the overlay. To be on the right
track, the next RichEditBox work should deliberately make the document layer the
transaction owner and demote the plain `TextBox` host to input/IME/caret plumbing,
or else replace the host with a document-aware editor surface.

## What WPF Gets Right Architecturally

### One Document Owns Text, Formatting, Selection, and Undo

WPF `RichTextBox.Document` is a `FlowDocument`. Assigning it:

- rejects a document already owned by another rich text editor,
- detaches the old document from logical tree and text-change collection,
- enables text-change collection on the new `TextContainer`,
- calls `InitializeTextContainer(_document.TextContainer)`,
- recreates the inherited `TextEditor`,
- reattaches the render scope, and
- raises a clear undo action/text-changed notification for non-initial swaps.

That means the editor never has to reconcile a plain-text control with a rich
document after the fact. The `TextContainer` is the mutation substrate.

### Rendering Is Attached to the Same Document

`CreateRenderScope()` creates a `FlowDocumentView` and sets its `Document` to the
same `FlowDocument` used by the editor. WPF does not render from a secondary
projection of the text; it renders the document being edited.

This is a major stability point. Selection geometry, caret placement, hit
testing, line layout, and formatted rendering are computed from one model.

### Selection Is Document-Native

WPF exposes `Selection` as `TextSelection` and `CaretPosition` as `TextPointer`.
Both are tied to the same document. `CaretPosition` validates that the pointer
belongs to the current document before moving the selection.

The important design rule is: selection is not an index range copied from a
separate host. It is a document object.

### Control Properties Are Transferred into the Document

WPF explicitly bridges inherited formatting and behavioral properties from the
control to its `FlowDocument`. It only transfers formatting inheritance to the
implicit document, but behavioral properties are transferred to any document.

That mechanism keeps a document stable when moved, serialized, printed, or used
outside the original UI tree. It also prevents a hidden mismatch between the
control chrome and the document defaults.

### RichTextBox Itself Does Not Implement Editing Semantics

The WPF file is not where typing, delete/backspace, paragraph editing, selection
navigation, clipboard, and command behavior live. It registers rich command
handlers through `TextEditor.RegisterCommandHandlers(...)` and delegates to the
shared WPF text editing engine.

This matters for our planning: source-porting the whole `RichTextBox.cs` file
alone would not give us a rich editor. The value is in the surrounding
`TextEditor*`, `TextContainer`, `TextPointer`, `TextRange`, and serialization
infrastructure.

## Current UnoRichText RichEditBox Shape

### Public Surface

`RichEditBox` exposes WinUI-shaped dependency properties and events:
`AcceptsReturn`, `IsReadOnly`, `SelectionFlyout`, `TextChanged`,
`SelectionChanged`, composition events, paste/copy/cut events, and related
properties.

The `Document` property currently returns `LeXtudio.UI.Text.RichEditTextDocument`
instead of `Microsoft.UI.Text.RichEditTextDocument`, because Uno's concrete type
has an internal constructor and many non-virtual throw-stubs. This divergence is
intentional and already documented in `DESIGN.md`.

### Input Host

The real keyboard/caret/IME host is `_editorHost`, a
`LeXtudio.UI.Controls.TextBox`. It supplies:

- platform text input,
- selection indices,
- caret behavior,
- basic command routing,
- context-menu entry points, and
- read-only enforcement through the underlying text box.

The host is currently a plain-text control. It cannot itself represent rich
inline state.

### Document Model

`RichEditTextDocument` owns:

- the canonical text buffer,
- character format runs,
- selection object,
- caret input format,
- undo/redo snapshots,
- format mutation,
- text insertion/deletion, and
- document change/formatting notifications.

Recent work has moved important semantics into this layer: collapsed-selection
formatting, formatting of future typed input, undo/redo of formatting runs and
selection, paragraph-boundary normalization, and a scoped retro-apply behavior
for `TextChanged` handlers.

This is the right place for editing semantics to accumulate.

### Rendering

Formatted rendering is done by `_renderOverlay`, a `RichTextBlock` placed above
the plain text host. `RefreshRichRenderOverlay()` rebuilds WPF-shaped inline
content from `Document.GetCharacterFormatRuns()`. When visible formatting exists,
the editor host's foreground is made transparent and the overlay renders the
formatted text and an external caret.

This makes formatting visible without replacing the text input stack, but it
creates a second rendered representation that must stay pixel-aligned with the
host.

## Direct Comparison

| Concern | WPF RichTextBox | Current RichEditBox | Assessment |
|---|---|---|---|
| Document authority | `FlowDocument.TextContainer` is authoritative | `RichEditTextDocument.Buffer` is intended authority, but `_editorHost.Text` still drives input diffs | Direction is good, but not finished |
| Rendering source | Same `FlowDocument` being edited | Projection from document runs into `RichTextBlock` overlay | Acceptable short-term, risky long-term |
| Selection model | `TextSelection` over document positions | Host selection indices synchronized into document selection | Works for simple text, fragile for rich structure |
| Caret model | Document/layout-native caret | Host caret when plain, overlay caret when formatted | Split model; needs more visual testing |
| Undo/redo | Integrated `TextEditor` undo | Document snapshot stacks | Good direction; transaction grouping still incomplete |
| Formatting | Native `TextElement` properties/ranges | Character format runs over string offsets | Good for WinUI `ITextRange`, less rich than WPF blocks/inlines |
| Paragraphs | First-class blocks/paragraphs/lists/tables | Text buffer with paragraph ranges inferred from newlines | Fine for staged WinUI parity, not WPF-equivalent |
| Clipboard/RTF | Rich pipeline in WPF text stack | Plain text mostly; RTF TODO | Known gap |
| IME | Text services integrated with text container/layout | Delegated to TextBox/TextCore host | Practical short-term; composition-to-document ownership must be verified |
| Property inheritance | Explicit transfer to `FlowDocument` | Partial property forwarding to host/overlay | Needs a document-default policy |
| Hit testing | `FlowDocumentView`/text view | Host hit testing plus overlay render | Overlay and host can diverge |

## Are We on the Right Track?

Yes, if the target is a WinUI-compatible rich editor delivered incrementally.
The current approach has produced real behavior and tests without needing to
port WPF's full editing engine up front. The document layer is gaining the right
responsibilities: it owns text, runs, selection state, typing format, and undo.

But we are only on the right track if we now stop treating `_editorHost.Text` as
the effective source of truth. WPF's main lesson is that rich editing becomes
stable when one model owns all mutations and rendering is a view of that model.
Every feature added while authority remains split will multiply sync bugs.

The architecture should be described as a staged bridge, not as the final shape.

## Recommended Architectural Direction

### 1. Make `RichEditTextDocument` the Transaction Owner

All editing commands should become document operations:

- typing,
- replacement,
- delete/backspace,
- paste,
- format toggles,
- undo/redo,
- paragraph insert/split,
- select all and selection movement where feasible.

The host may still receive raw text input, but the mutation should resolve into
`Document.Selection.TypeText`, `Document.DeleteRange`, or equivalent document
transactions. `_editorHost.Text` should be synchronized from the document after
transactions, not treated as a peer model.

### 2. Keep the TextBox Host as a Temporary Input Adapter

The plain text host remains valuable because it already solves platform input,
focus, IME, and baseline caret behavior in Uno. Use it as infrastructure, but
keep its responsibilities narrow:

- collect platform input,
- expose caret/selection gestures while the custom editor surface matures,
- route accelerators and context menu gestures,
- provide text-services integration.

Avoid adding rich semantics to the host itself.

### 3. Make Overlay Rendering a Document View

The overlay should be treated as the visual view of `RichEditTextDocument`, not
as an optional decoration. That means:

- every document transaction invalidates the overlay,
- overlay caret/selection geometry is tested against host geometry,
- plain host foreground hiding is isolated to presentation,
- no feature should depend on parsing visible overlay content back into the
  document.

This is still less ideal than WPF's single `FlowDocumentView`, but it is a
coherent staged model.

### 4. Define Document Defaults and Property Transfer

WPF's inherited-property transfer has an equivalent need here. `RichEditBox`
properties such as font family, font size, foreground, text alignment, wrapping,
reading order, and paragraph defaults should have a clear transfer policy:

- control visual properties configure document defaults,
- explicit document/range formatting overrides defaults,
- renderer and host both consume the same effective defaults.

Without this, formatted overlay text and plain host text can drift.

### 5. Defer Full WPF-Style Rich Structure Until the WinUI Text Model Needs It

The current run-based model is enough for many WinUI `RichEditBox` scenarios:
plain text plus character/paragraph formatting. Full WPF block/inline editing
should be introduced only when needed for:

- RTF/XAML round-trip fidelity,
- lists,
- tables,
- inline UI containers,
- complex paragraph editing,
- rich clipboard.

When those features are needed, port WPF infrastructure into WindowsShims rather
than inventing new behavior.

## Concrete Next Milestones

1. Route direct typing and selection replacement through `Document.Selection`
   as the canonical operation.
2. Add control-level tests that simulate host text changes and verify document
   text, format runs, selection, and undo state after each edit.
3. Add visual/sample checks for formatted typing, selection, caret visibility,
   undo/redo, and overlay alignment.
4. Implement document-owned delete/backspace and paste semantics.
5. Define and test default formatting propagation from `RichEditBox` properties
   to document defaults and overlay rendering.
6. Treat RTF load/save and rich clipboard as a later WPF-shim porting task.

## Decision Point

There are two viable paths:

### Path A: Continue the Staged WinUI RichEditBox

Keep the current architecture, but harden it around a document-owned transaction
model. This is the fastest route to a usable WinUI-compatible editor.

Use this path if the near-term goal is practical RichEditBox behavior across Uno
platforms.

### Path B: Build a WPF-Style Editor Core

Port more of WPF's `TextEditor`, `TextContainer`, `TextPointer`, and
`FlowDocumentView`-adjacent infrastructure into WindowsShims and host that from
the WinUI control.

Use this path if the near-term goal is high-fidelity WPF rich document semantics,
RTF/XAML fidelity, tables/lists, and complex text navigation.

## Recommendation

For finishing `RichEditBox`, choose Path A now, with one hard rule:

> `RichEditTextDocument` must become the only owner of text, formatting,
> selection state, and undo transactions. The `TextBox` host is an input adapter,
> not a second editor model.

This follows the central WPF architecture lesson without requiring a full WPF
text engine port before the control is useful. It also leaves a clean migration
path: once the document-owned transaction boundary is solid, the host/overlay
pieces can be replaced incrementally by a richer document-aware surface.

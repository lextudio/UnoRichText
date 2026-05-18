# RichTextBox Porting Bounds and Tracking

This document defines how we track the source-first `RichTextBox` port and its
supporting `System.Windows.Documents` types.

## Current Decision

The active path is to align the new Uno/WinUI `RichTextBox` work with WPF
`RichTextBox`, not to keep filling gaps in the earlier `RichEditBox`-style
implementation.

That changes the old boundary:

- `UnoRichText/docs/SOURCE-MAP.md`, `UnoRichText/docs/PROVENANCE.md`, and
  `UnoRichText/docs/NAMESPACE-INVENTORY.md` are archived for the old
  `RichTextBlock`/`RichEditBox` direction.
- `WindowsShims/docs/ARCHITECTURE.md` contains useful shim design rules, but
  its older "do not port full RichTextBox" guidance is superseded for this
  effort by `docs/RichTextBox/design.md`.
- `WindowsShims` is now the canonical home for WPF-compatible document and
  editor source.
- `UnoRichText` is the host/rendering integration layer.

## Source Boundaries

### WindowsShims owns WPF compatibility

Track these in `WindowsShims`:

- upstream WPF files linked from `WindowsShims/ext/wpf`
- local WindowsShims replacements for WPF files that cannot compile directly
- `.uno.cs` partials that isolate Uno-specific behavior
- compatibility frontiers such as dependency property, dispatcher, input,
  undo, selection, and text-container bridges
- build status for the `LeXtudio.Windows` baseline

### UnoRichText owns the Uno host

Track these in `UnoRichText/docs/RichTextBox`:

- runtime host status for `LeXtudio.UI.Xaml.Controls.RichTextBox`
- feature switches such as `EnableRichTextBox`
- renderer integration through `RichTextBlock`
- sample enablement
- test-harness blockers discovered from Uno-side verification

## Tracking Files

Use these files together:

- `WindowsShims/docs/PLAN.md`: narrative migration log and proven
  compile-frontier workflow.
- `WindowsShims/docs/RICHTEXTBOX-PORT-CATALOG.md`: file/status catalog for the
  RichTextBox and Documents port.
- `UnoRichText/docs/RichTextBox/design.md`: current source-first architecture
  and session index.
- `UnoRichText/docs/RichTextBox/session*.md`: session-level work records.

`WindowsShims/docs/PLAN.md` is not a catalog. It records what happened and why.
The catalog records what exists, how each item is sourced, what blocks it, and
what should be enabled next.

## Catalog Rules

Each tracked source item should record:

- WPF source file or source family
- local WindowsShims file or linked project item
- status
- Uno-specific partial or bridge file, if any
- current blocker, if not fully usable
- last verification command or session reference

Allowed status values:

- `linked-upstream`: upstream WPF source compiles directly or with narrow
  bridges.
- `local-shell`: local implementation exists and intentionally approximates the
  WPF surface.
- `local-bridge`: compatibility layer used by linked upstream files.
- `partial-uno`: split implementation with Uno-specific partial behavior.
- `blocked`: known required item that cannot be enabled yet.
- `deferred`: known WPF source that is outside the current milestone.

## Decision Rules

1. Prefer upstream WPF source for complex editing semantics.
2. Keep imported WPF source read-only except for provenance-preserving fixes.
3. Put platform adaptation in bridges or `.uno.cs` partials.
4. Do not add broad local rewrites for editor behavior unless the upstream
   source cannot be isolated behind a reasonable frontier.
5. Keep `LeXtudio.Windows` green after each migration unit.
6. If a type compiles but has no Uno-side consumer yet, mark it as compiled but
   blocked or deferred rather than calling it complete.

## Type Reuse Rules

For foundational WinUI types that already cover WPF concepts (for example
`Color`, `Brush`, and related media primitives), do not port duplicate WPF
implementations.

Use this order:

1. Reuse the existing WinUI type directly (or existing alias/global using in
   `WindowsShims`) when the semantic match is sufficient.
2. If the WinUI type is missing a small WPF API surface, add narrow shim
   members (for example extension members in C#) instead of creating a new type.
3. If source needs to compile for both WPF and WinUI with minor signature
   differences, use conditional compilation to adapt the call sites or partial
   implementations.

This keeps the compatibility layer thin and avoids maintaining parallel
implementations of the same primitive type family.

## Immediate Tracking Focus

The next catalog pass should focus on the minimum RichTextBox editing spine:

- `System.Windows.Controls.RichTextBox`
- `System.Windows.Controls.Primitives.TextBoxBase`
- `System.Windows.Documents.FlowDocument`
- `System.Windows.Documents.TextElement` and inline/block model
- `System.Windows.Documents.TextPointer`
- `System.Windows.Documents.TextContainer`
- `System.Windows.Documents.TextRange`
- `System.Windows.Documents.TextSelection`
- `System.Windows.Documents.TextEditor` and adjacent `TextEditor*` helpers
- serialization and copy/paste helpers needed by `RichTextBox`

Table, fixed-document, paginator, adorner, and advanced formatting families
should remain tracked but not treated as milestone blockers unless the WPF
editor spine actually requires them.

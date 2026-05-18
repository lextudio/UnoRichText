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

## WPF Source Triage Workflow

When evaluating a WPF source file for inclusion in `WindowsShims`, apply the
following four-step triage. Use `TextPointer` as the reference example of a
file that cannot be linked and must follow steps 3–4.

### Step 1 — Classify each member as WPF-tight or platform-neutral

Read through the WPF source file and mark each member:

**WPF-tight** — depends on internal WPF infrastructure that does not exist and
cannot be shimmed cheaply:

- `TextTree`, `TextTreeNode`, `TextTreeTextNode`, `SplayTreeNode`
- `TextContainer` (WPF internal version, not the shim)
- `Win32` / `HwndSource` / `PresentationSource` dependencies
- Dispatcher-thread checks using WPF-specific threading primitives
- `Adorner`, `AdornerLayer`, `Visual`, `DrawingContext` render internals

These members **must** be extracted into a `.wpf.cs` companion file so they
stay with the WPF ext tree and do not compile in the shim project.

**Platform-neutral** — depends only on public `System.Windows.*` APIs and
interfaces (`ITextPointer`, `LogicalDirection`, `DependencyProperty`, etc.) or
pure C# logic with no platform rendering coupling. These can remain in the
original `.cs` file in the WPF ext tree and compile directly in `WindowsShims`
if all their dependencies are satisfied.

### Step 2 — Decide whether the file can be linked

A file is **directly linkable** (status `linked-upstream`) when:

- All its dependencies are already present in the shim project, AND
- The WPF-tight surface (if any) is small enough to stub with a `#if` guard or
  a thin bridge rather than a full separate file.

A file is **not linkable** when:

- It is fundamentally coupled to `TextTree`/`TextContainer` internals (like
  `TextPointer.cs` — 4 376 lines, 5 critical internal type dependencies), OR
- Satisfying its dependencies would require porting hundreds of additional
  lines of tree-management infrastructure.

For non-linkable files, proceed to steps 3 and 4.

### Step 3 — Create the `.uno.cs` companion in `WindowsShims`

In `WindowsShims/src/LeXtudio.Windows/` at the matching path, create
`<TypeName>.uno.cs` as a `partial class`. This file:

- Provides only the members that are **Uno/WinUI-specific** or that replace
  WPF-tight members stripped from the main class.
- Registers Uno/WinUI `DependencyProperty` objects (using
  `Microsoft.UI.Xaml.DependencyProperty.Register`).
- Implements WinUI interfaces such as `Microsoft.UI.Xaml.Input.IInputElement`
  where the WPF version implements the WPF equivalent.
- Bridges renderer callbacks (`TextLayoutHost`, `RichTextBlock`) that have no
  WPF equivalent.
- **Does not** duplicate logic that belongs in the neutral `.cs` file.

Pattern already established:

- `TextElement.cs` (WPF ext, neutral logic) + `TextElement.uno.cs` (Uno DPs,
  WinUI interface impl)
- `Run.cs` (WPF ext) + `Run.uno.cs` (implicit-run helpers, shim-specific
  members)
- `Paragraph.cs` (WPF ext) + `Paragraph.uno.cs`

### Step 4 — Write the neutral `.cs` in `WindowsShims/src`

For types that cannot link the WPF source at all (status `local-shell`), write
a standalone `.cs` in `WindowsShims/src/LeXtudio.Windows/` that:

- Implements the **public and internal API surface** callers actually need.
- Uses the same namespace and type name as the WPF original.
- Stores state explicitly (e.g. `_explicitOffset` in `TextPointer`) instead of
  computing it from a tree walk.
- Is kept as a `sealed` or `partial` class matching the WPF declaration.
- Avoids duplicating members that belong in `.uno.cs`.

Then compile **both** `<TypeName>.cs` and `<TypeName>.uno.cs` together in the
`LeXtudio.Windows` project. The result is a single compiled type whose neutral
logic lives in the `.cs` file and whose Uno-specific surface lives in
`.uno.cs`.

### Reference: TextPointer triage outcome

| Criterion | Outcome |
| --------- | ------- |
| WPF source linkable? | No — depends on `TextTree`/`SplayTreeNode` chain (~6 400 lines) |
| WPF-tight members moved to `.wpf.cs`? | Not applicable (file not linked) |
| Neutral `.cs` written in `WindowsShims`? | Yes — `TextPointer.cs` (~415 lines), explicit-offset model |
| Uno companion `.uno.cs` needed? | No — no Uno DPs or WinUI interfaces on `TextPointer` itself |
| Status | `local-shell` |

`GetPositionAtOffset`, `GetOffsetToPosition`, and `GetOffset` fallback logic
were added to the shim to match WPF semantics without the tree infrastructure.

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

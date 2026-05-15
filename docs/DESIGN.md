# LeXtudio.RichText — Design Document

## Goal

Provide a **fully implemented `RichTextBlock` for Uno with 100% API parity to WinUI 3**.

The two halves of the goal are equally load-bearing:

- **API parity with WinUI 3** (measured): every public member of `Microsoft.UI.Xaml.Controls.RichTextBlock` and its supporting document types — as declared by **the Windows App SDK** — exists on the LeXtudio side with the same signature. Tracked by [`tools/RichTextBlockCompat`](../tools/RichTextBlockCompat/README.md) against [`COMPAT-REPORT.md`](COMPAT-REPORT.md). Target: 100%.
- **Fully implemented** (means it actually works): members do real work, not `NotImplementedException`. We accept stubs only for members WinUI 3 itself rarely uses meaningfully (see Phase 4 of [`COMPAT-PLAN.md`](COMPAT-PLAN.md)).

### The Three Categories

To keep the picture clear, three different things share similar names:

1. **WinUI 3** — `Microsoft.UI.Xaml.*` as declared by the Windows App SDK. **This is the reference authority.**
2. *(out of scope)* — Uno Platform's own reimplementation of `Microsoft.UI.Xaml.*`. We do not measure against it, do not depend on its implementation status, and do not inherit from its types in our public surface.
3. **UnoRichText** — `LeXtudio.UI.Xaml.Controls.RichTextBlock` and the `System.Windows.Documents.*` types we ship. **This is the subject under test.**

The goal is parity between (1) and (3). Anything about (2) is informational at most.

## Why

Uno's built-in `RichTextBlock` has rendering, layout, and interaction gaps that block native markdown rendering and other rich-text scenarios — especially around `InlineUIContainer`, hyperlink hit-testing, and text selection consistency. The pragmatic fix is a drop-in replacement control, not a full document-model rewrite.

## What "Compatible" Means

`LeXtudio.UI.Xaml.Controls.RichTextBlock` is API-compatible with `Microsoft.UI.Xaml.Controls.RichTextBlock` for the surface that real consumers actually use:

1. **Block content**: `Blocks` collection accepts `Paragraph`, with `Inlines` of `Run`, `Span`, `Bold`, `Italic`, `Underline`, `Hyperlink`, `LineBreak`, `InlineUIContainer`.
2. **Visual fidelity**: text layout, line breaking, inline UI, and selection rendering match WinUI behavior closely enough that callers cannot tell the difference at the API or visual level.
3. **Interaction**: pointer-based text selection, copy, hyperlink hit-testing, and cursor changes behave as WinUI users expect.
4. **Properties**: `FontSize`, `FontFamily`, `FontWeight`, `Foreground`, `TextWrapping`, `IsTextSelectionEnabled`, and other commonly-set properties have the WinUI semantics.

Anything beyond this — `TextPointer`, `TextRange`, editing, `FlowDocument`-level features — is **not a goal** and not part of compatibility.

## Document Model Source

The document types (`Run`, `Span`, `Paragraph`, `Hyperlink`, `Inline`, …) reflect the **WinUI** document model in `Microsoft.UI.Xaml.Documents`. Where Uno already provides the type, use it directly. Where Uno's version is incomplete, fill the gap with the minimum local shim needed to make `RichTextBlock` render and interact correctly.

WPF source is a **reference only** — consulted when WinUI's semantics are underspecified — never the design authority for this control.

## Non-Goals

These are explicitly out of scope. Do not invest engineering time here:

- WPF `FlowDocument` pipeline (`FlowDocumentScrollViewer`, `DocumentPaginator`, pagination).
- `Table`/`TableRow`/`TableCell` rendering (WinUI doesn't render tables in `RichTextBlock`; we won't either).
- `TextPointer` / `TextRange` / `TextSelection` editing API.
- `RichEditBox`-style editing (`RichTextBlock` is read-only in WinUI; ours is too).
- Full-namespace port of `System.Windows.Documents` (the previous "223 files" plan is abandoned).
- Source-parity tests against WPF.

If a future scenario genuinely needs one of these, it gets its own scoped design — not bundled into `RichTextBlock`.

## Architecture

### Layer 1 — `LeXtudio.UI.Xaml.Controls.RichTextBlock` (the control)

The drop-in replacement. Lives in `src/LeXtudio.RichText/Controls/RichTextBlock.cs`. Responsibilities:

- Hosts a flat-item layout pipeline backed by PretextSharp for text measurement.
- Walks `Blocks` / `Inlines` and produces visual fragments on a `Canvas`.
- Owns pointer handling, selection rendering, hyperlink hit-testing, and clipboard copy.

This is the only file consumers care about. Keep its public surface aligned with `Microsoft.UI.Xaml.Controls.RichTextBlock`.

### Layer 2 — Document model gap fills

Where Uno's `Microsoft.UI.Xaml.Documents.*` is incomplete for our needs, fill the gap with the smallest shim that makes Layer 1 work. Each shim must be justified by a concrete `RichTextBlock` rendering or interaction requirement.

### Layer 3 — `LeXtudio.Windows` shim (separate repo)

Exists for WPF→WinUI source compatibility in a broader sense (used by UnoEdit too). Its evolution is **not driven** by `RichTextBlock` needs — `RichTextBlock` only consumes what's already there.

## Build and File Layout

- `src/LeXtudio.RichText/Controls/` — the control.
- `src/LeXtudio.RichText/Documents/` — gap-fill shims, if any.
- `src/LeXtudio.RichText.Sample/` — visual smoke test, also runs `--diag` for CI.
- `src/LeXtudio.RichText.Tests/` — behavior tests for the control.

Naming: plain `.cs` only. No `.wpf.cs` / `.uno.cs` splits — this project targets Uno; there is no other platform.

## Testing

The bar is **behavioral equivalence with WinUI `RichTextBlock`**, not parity with WPF source:

1. `LeXtudio.RichText.Tests` covers the control's observable behavior — flat-item layout, selection geometry, inline-code rendering, hyperlink hit-testing.
2. The sample's `--diag` mode is the integration smoke gate.
3. When a regression is reported, the test added should pin down the WinUI-equivalent behavior, not a WPF reference behavior.

## Measuring Compatibility

API-surface coverage is tracked by [`tools/RichTextBlockCompat`](../tools/RichTextBlockCompat/README.md). It reflects over the WinUI types and our local types, compares public members, and emits a percentage plus a gap report at [`docs/COMPAT-REPORT.md`](COMPAT-REPORT.md).

- Run: `dotnet run --project UnoRichText/tools/RichTextBlockCompat`
- Gate: `dotnet run --project UnoRichText/tools/RichTextBlockCompat -- --min 50` (exit 1 if overall coverage drops below 50%).

The tool measures *static* API shape only — runtime behavior and visual fidelity are out of its scope by design. Use the regular tests for those.

## Decision History

- **Previously**: WPF-source-first port of `System.Windows.Documents` (223 files) with `.wpf.cs` / `.uno.cs` partials, linked upstream files, provenance manifests, dependency-ring migrations. **Abandoned** — the cost was disproportionate to the actual `RichTextBlock` consumer need, and most WPF document infrastructure has no WinUI rendering path anyway (see `ext/shims/docs/ARCHITECTURE.md` — "Do Not Port").
- **Now**: WinUI-compatible drop-in control, minimal document-model shimming, no full namespace ambition.

## Risks

1. **Uno feature drift**: Uno's `Microsoft.UI.Xaml.Documents` types evolve. Mitigation: keep our consumption surface narrow; add a smoke test per Uno bump.
2. **WinUI semantic ambiguity**: where WinUI's behavior is underspecified (e.g. exact selection geometry), make a pragmatic choice and pin it with a test.
3. **Scope creep**: pressure to add `TextPointer`, editing, tables, etc. Mitigation: this document is the answer — those are non-goals.

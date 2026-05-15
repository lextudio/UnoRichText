# LeXtudio.RichText — Design Document

## Goal

Provide a fully implemented `RichTextBlock` for Uno with 100% API parity to WinUI 3.

The goal has two equally important halves:

- API parity with WinUI 3, measured against the Windows App SDK metadata through [`tools/RichTextBlockCompat`](../tools/RichTextBlockCompat/README.md) and tracked in [`COMPAT-REPORT.md`](COMPAT-REPORT.md).
- Real behavior, meaning the members that matter to rendering and interaction actually work.

## The Three Categories

Three different things are easy to conflate, so we keep them separate:

1. **WinUI 3** — `Microsoft.UI.Xaml.*` as declared by Microsoft in the Windows App SDK. This is the reference authority.
2. **Other reimplementations of `Microsoft.UI.Xaml.*`** — informational only. We do not measure parity against them and do not inherit from them in our public surface.
3. **UnoRichText** — `LeXtudio.UI.Xaml.Controls.RichTextBlock` plus the `System.Windows.Documents.*` bridge types we ship. This is the subject under test.

Parity is measured between (1) and (3).

## Architecture Direction

We are taking a WinUI-parity-first approach while maximizing source sharing with WPF where it is reasonable.

### Control layer

`LeXtudio.UI.Xaml.Controls.RichTextBlock` is ours. We own its files and add members directly there.

### Document layer

`System.Windows.Documents.*` types are brought over from upstream WPF source where practical. The working model is:

| File | Ownership | Purpose |
|---|---|---|
| `Type.cs` | upstream WPF, read-only | linked source-of-truth implementation |
| `Type.wpf.cs` | WPF-target glue, read-only | upstream or WPF-side adapter code |
| `Type.uno.cs` | editable | Uno and WinUI bridging, parity fills, local behavior |

For document types, new parity members go into `Type.uno.cs` partials. If a type has no `.uno.cs` yet, create one. Do not edit upstream `Type.cs` or `Type.wpf.cs` unless there is explicit approval for that exact change.

### Shim layer

`ext/shims/src/LeXtudio.Windows` exists to make WPF-oriented source compile and behave sensibly on WinUI and Uno. The default rule is:

- use WinUI types directly when there is a clean equivalent
- keep a local shim only when WinUI has no equivalent or the WPF contract is materially different

That is why aliases to WinUI types are preferred for things like `Brush`, `Color`, `Point`, `Rect`, `Size`, `FlowDirection`, and `TextBlock`.

## Parity Mechanisms

When a member is missing on UnoRichText, add it in one of two ways:

| Mechanism | Default use |
|---|---|
| Real member on the type | preferred |
| C# 14 extension member | fallback when the type cannot otherwise be reached cleanly |

Real members are preferred because they show up naturally in reflection, match the WinUI shape better, and do not require special imports from consumers.

Extension members are acceptable for shimmed WinUI-owned types, but they should be the exception, not the routine path.

## Stub vs Implement

Not every parity member deserves full behavior on day one.

- **Implement** members that matter to layout, rendering, selection, hit-testing, text styling, and realistic app usage.
- **Stub** members that are compile blockers in non-goal areas such as deep text-pointer editing or keytip/access-key infrastructure.

Stubs must not throw `NotImplementedException`. They should return a sensible default, no-op, or dead event.

## Scope

Current parity work is focused on:

- `LeXtudio.UI.Xaml.Controls.RichTextBlock`
- `System.Windows.Documents.TextElement` and the WinUI-facing document hierarchy around it
- the API surface that WinUI 3 exposes for `Block`, `Paragraph`, `Inline`, `Run`, `Span`, `Bold`, `Italic`, `Underline`, `Hyperlink`, `LineBreak`, and `InlineUIContainer`

This does not mean we avoid bringing over more of `System.Windows.Documents`. It means additions should be driven by WinUI parity and coherent bridge architecture, not by bulk import alone.

## Non-goals

These remain outside the current effort unless explicitly re-scoped:

- full WPF pagination pipeline
- full editing stack comparable to `RichEditBox`
- complete `TextRange` and `TextSelection` behavior parity
- blind source-porting of upstream internals that have no WinUI-facing value

## Testing and Measurement

We measure shape and behavior separately.

1. `tools/RichTextBlockCompat` measures public API parity against real Windows App SDK metadata.
2. Unit and sample tests validate behavior, rendering, selection, and interaction.

Run the compatibility tool with:

```powershell
dotnet run --project UnoRichText/tools/RichTextBlockCompat
```

The generated report lives at [`COMPAT-REPORT.md`](COMPAT-REPORT.md).

## Working Rules

- Prefer WinUI types when there is a clean one-to-one mapping.
- Prefer linked WPF source for document types where that improves long-term maintainability.
- Put Uno-specific parity members in `.uno.cs` partials.
- Keep upstream WPF-linked files read-only unless explicit approval is given.
- Re-run the compat tool after each meaningful parity batch.

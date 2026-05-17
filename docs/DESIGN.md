# LeXtudio.RichText — Design Document

## Goal

Provide fully implemented WinUI 3 rich-text controls for Uno with 100% API parity to WinUI 3:

1. `RichTextBlock` — read-only multi-paragraph rich text display.
2. `RichTextBlockOverflow` — overflow container for paginating `RichTextBlock` content across regions.
3. `RichEditBox` — editable rich text input with full document model, selection, IME, and clipboard support.

The goal has two equally important halves for every control:

- API parity with WinUI 3, measured against the Windows App SDK metadata through [`tools/RichTextBlockCompat`](../tools/RichTextBlockCompat/README.md) and tracked in [`COMPAT-REPORT.md`](COMPAT-REPORT.md).
- Real behavior, meaning the members that matter to rendering, editing, and interaction actually work.

## The Three Categories

Three different things are easy to conflate, so we keep them separate:

1. **WinUI 3** — `Microsoft.UI.Xaml.*` as declared by Microsoft in the Windows App SDK. This is the reference authority.
2. **Other reimplementations of `Microsoft.UI.Xaml.*`** — informational only. We do not measure parity against them and do not inherit from them in our public surface.
3. **UnoRichText** — `LeXtudio.UI.Xaml.Controls.{RichTextBlock, RichTextBlockOverflow, RichEditBox}` plus the `System.Windows.Documents.*` and `Microsoft.UI.Text.*` bridge types we ship. This is the subject under test.

Parity is measured between (1) and (3).

## Architecture Direction

We take a WinUI-parity-first approach while maximizing source sharing with WPF where it is reasonable.

### Control layer

- `LeXtudio.UI.Xaml.Controls.RichTextBlock` — read-only display surface, panel-based.
- `LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow` — overflow target that paints leftover content from a linked `RichTextBlock` or another `RichTextBlockOverflow`, forming a single overflow chain.
- `LeXtudio.UI.Xaml.Controls.RichEditBox` — editable rich text control. Backed by an internal document model, caret/selection engine, and text-services integration.

All three are ours. We own their files and add members directly there.

### Document layer

`System.Windows.Documents.*` types are shared between `RichTextBlock`, `RichTextBlockOverflow`, and `RichEditBox`. The file conventions are:

| File | Ownership | Purpose |
|---|---|---|
| `Type.cs` | upstream WPF, read-only | linked source-of-truth implementation |
| `Type.wpf.cs` | WPF-target glue, read-only | upstream or WPF-side adapter code |
| `Type.uno.cs` | editable | Uno and WinUI bridging, parity fills, local behavior |

For document types, new parity members go into `Type.uno.cs` partials. If a type has no `.uno.cs` yet, create one. Do not edit upstream `Type.cs` or `Type.wpf.cs` unless there is explicit approval for that exact change.

### Document model for `RichEditBox`

`RichEditBox` exposes its content through `Microsoft.UI.Text.RichEditTextDocument` (the `Document` property) — distinct from the inline `System.Windows.Documents.*` model used by `RichTextBlock`. The bridge types we own:

- `Microsoft.UI.Text.RichEditTextDocument` (and its base `Microsoft.UI.Text.ITextDocument`)
- `Microsoft.UI.Text.ITextRange`, `ITextSelection`, `ITextCharacterFormat`, `ITextParagraphFormat`
- enum/struct support types in `Microsoft.UI.Text.*`

These do not inherit from the WinUI types; they are our own declarations that match the WinUI shapes member-for-member.

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

- **Implement** members that matter to layout, rendering, selection, hit-testing, text styling, editing, IME, clipboard, undo/redo, and realistic app usage.
- **Stub** members that are compile blockers in non-goal areas such as accessibility/keytip infrastructure or rarely-used proofing surfaces.

Stubs must not throw `NotImplementedException`. They should return a sensible default, no-op, or dead event.

### What "Implement" means for each control

`RichTextBlock`:

- Inline/block layout with paragraph and inline styling
- Selection, copy-to-clipboard, hyperlink invoke
- Overflow detection (`HasOverflowContent`) and chained painting through `OverflowContentTarget`

`RichTextBlockOverflow`:

- Hosts the overflow visual produced by its source's layout engine
- Forwards selection so that the overflow region participates in the source's selection
- Chains its own `OverflowContentTarget` to the next region

`RichEditBox`:

- Caret rendering and blink
- Selection (mouse, keyboard, touch, shift-arrow)
- Text editing (insert, delete, backspace, word navigation)
- IME composition
- Undo/redo stack
- Cut/copy/paste of plain text and RTF (where available)
- Character and paragraph formatting through `ITextSelection.CharacterFormat` / `ParagraphFormat`
- `SelectionFlyout`, context menu, header, placeholder text
- `Document.SetText` / `GetText` round-trip for plain text, RTF, and OLE-compatible streams (where the platform supports it)
- ReadOnly / disabled visual states
- Spell-check is **stub** initially (no underlining) but the API surface is present

## Scope

Current parity work is focused on:

- `LeXtudio.UI.Xaml.Controls.RichTextBlock`
- `LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow`
- `LeXtudio.UI.Xaml.Controls.RichEditBox`
- `System.Windows.Documents.TextElement` and the WinUI-facing document hierarchy around it (`Block`, `Paragraph`, `Inline`, `Run`, `Span`, `Bold`, `Italic`, `Underline`, `Hyperlink`, `LineBreak`, `InlineUIContainer`)
- `Microsoft.UI.Text.RichEditTextDocument` and its supporting `ITextRange` / `ITextSelection` / `ITextCharacterFormat` / `ITextParagraphFormat` surfaces

## Non-goals

These remain outside the current effort unless explicitly re-scoped:

- full WPF pagination pipeline
- complete `TextRange` and `TextSelection` behavior parity beyond what `RichEditBox` needs
- blind source-porting of upstream internals that have no WinUI-facing value
- Win32 RichEdit DLL host (`Riched20`) compatibility — we ship our own engine
- Ink, handwriting, dictation, voice input
- Spell-check word lists / proofing dictionaries (the API is exposed, behavior is stub)

## Testing and Measurement

We measure shape and behavior separately.

1. `tools/RichTextBlockCompat` measures public API parity against real Windows App SDK metadata. The tool's `TypePairs` list covers `RichTextBlock`, `RichTextBlockOverflow`, `RichEditBox`, and their document/text-document model types.
2. Unit and sample tests validate behavior, rendering, editing, selection, and interaction.

Run the compatibility tool with:

```powershell
dotnet run --project UnoRichText/tools/RichTextBlockCompat
```

The generated report lives at [`COMPAT-REPORT.md`](COMPAT-REPORT.md).

## Source Reuse Across Sibling Repos

We do **not** rewrite text editing infrastructure from scratch. Several sibling repos in this workspace already implement the hard parts and should be the first place to look when starting a new feature for `RichEditBox`:

| Sibling | Useful for | Notes |
|---|---|---|
| `TextCore.Uno/src/LeXtudio.UI.Text.Core` | IME / text-services integration | Ships `CoreTextEditContext`, `CoreTextRange`, `CoreTextLayoutRequest`, and platform input adapters for Win32, macOS, X11/IBus, and a null fallback. Use this as the IME backend for `RichEditBox`. |
| `TextCore.Uno/src/LeXtudio.TextBox` | Plain-text `TextBox` patterns | Reference for control-template wiring (`ContentElement`, `PlaceholderTextContentPresenter`, `HeaderContentPresenter`), visual states, and event flow. |
| `UnoEdit/src/UnoEdit/Editing` | Caret, selection, command handlers, input handlers | `Caret`, `Selection`, `CaretNavigationCommandHandler`, `EditingCommandHandler`, `InputHandlers`, `SelectionMouseHandler` — these encode the keyboard / pointer / accelerator behavior we need for `RichEditBox`. Treat them as a behavior reference and pull what is applicable into `RichEditBox` (they are MIT-licensed from AvalonEdit / SharpDevelop). |
| `UnoEdit/src/UnoEdit` (rest) | Document model, line layout, text rendering | While AvalonEdit's document model is line-oriented and not directly the WinUI document model, its rendering and layout pipelines, hit testing, virtual lines, and folding are reusable patterns. Useful for the `RichEditBox` rendering engine. |
| `WindowsShims/src/LeXtudio.Windows` | WPF-shape compatibility shims **plus a deep set of ported WPF types directly useful for `RichEditBox`** | `System.Windows.Documents.*` (`TextElement`, `Run`, `Span`, `Paragraph`, `Hyperlink`, `List`, `ListItem`, `Section`, `TextPointer`, `TextRangeEdit`, `TextRangeEditTables`, `TextSchema`, `ITextPointer`, `ITextLayoutHost`, `AnchoredBlock`, `FlowDocument`). `System.Windows.Input.*` (`ApplicationCommands`, `EditingCommands`, `RoutedCommand`, `CommandBinding`, `KeyGesture`, `Key`, `ModifierKeys`, `TextCompositionEventArgs`). `MS.Internal` text-range and pointer helpers. **Use these wherever WPF semantics are already encoded — they will save weeks of work on the editing engine, text pointer arithmetic, and command surface.** |
| External: `ext/PretextSharp` (consumed via `Pretext.Uno`) | Inline layout / text shaping for `RichTextBlock` | Already used for `RichTextBlock`'s inline layout. The same engine should drive `RichEditBox`'s read-side rendering for consistency. |

### Porting more from WPF on demand

When `RichEditBox` needs a non-trivial piece of behavior that already exists in upstream WPF (e.g. RTF parsing / writing, table editing, list renumbering, complex caret navigation across bidi runs, drag-and-drop of formatted content), the preferred path is to **port the WPF source into `WindowsShims/src/LeXtudio.Windows`** following the existing `.cs` / `.wpf.cs` / `.uno.cs` convention there, rather than reinventing it.

Concrete examples we expect to need:

- `System.Windows.Documents.RtfToXamlReader` and `XamlToRtfWriter` (and the `RtfControlWordInfo`, `RtfFormatStack`, `FormatState` supporting types) — drives `RichEditTextDocument.LoadFromStream` / `SaveToStream` for RTF.
- `System.Windows.Documents.TextStore` and the `MS.Internal.Documents` text-pointer arithmetic family — already partially mirrored in `WindowsShims`, can be extended.
- `System.Windows.Documents.TextEditor*` (`TextEditorCharacters`, `TextEditorParagraphs`, `TextEditorLists`, `TextEditorCopyPaste`, `TextEditorTyping`, `TextEditorSelection`) — these encode the editing semantics that `ITextSelection.TypeText`, `Cut/Copy/Paste`, `ChangeCase`, and the format-toggle commands need.

Porting rule: keep upstream files read-only (`Type.cs`, `Type.wpf.cs`), add Uno/WinUI glue in `Type.uno.cs` partials, log provenance with a one-line header comment.

Working rule: **search siblings first, then port or link, then write new**.

Where source is linked or ported, mark provenance at the top of the file with a one-line comment naming the origin path.

## Working Rules

- Prefer WinUI types when there is a clean one-to-one mapping.
- Prefer linked WPF source for document types where that improves long-term maintainability.
- Prefer reusing sibling-repo code (see "Source Reuse" above) over writing new editor infrastructure.
- Put Uno-specific parity members in `.uno.cs` partials.
- Keep upstream WPF-linked files read-only unless explicit approval is given.
- Re-run the compat tool after each meaningful parity batch.
- For `RichEditBox`: treat `Microsoft.UI.Text.*` document-model types as ours; they are not shims over WinUI's implementation.

# Plan: 100% Rich Text Control API Parity with WinUI 3

Target: **100%** API parity measured by [`tools/RichTextBlockCompat`](../tools/RichTextBlockCompat/README.md) against [`COMPAT-REPORT.md`](COMPAT-REPORT.md) for **all three controls**:

- `RichTextBlock`
- `RichTextBlockOverflow`
- `RichEditBox`

…and their associated document-model types.

## Scope Clarification (Three Categories)

1. **WinUI 3 (Windows App SDK)** — `Microsoft.UI.Xaml.*` and `Microsoft.UI.Text.*` as declared by Microsoft. **This is the reference.**
2. *(out of scope)* — Any other reimplementation of `Microsoft.UI.Xaml.*`. We do not compare against it, depend on its implementation status, or inherit from its types in our public surface.
3. **UnoRichText (ours)** — the three controls + `System.Windows.Documents.*` document model + `Microsoft.UI.Text.*` text-document model (`RichEditTextDocument`, `ITextRange`, `ITextSelection`, `ITextCharacterFormat`, `ITextParagraphFormat`, supporting enums and structs). **This is the subject.**

Parity is measured between (1) and (3). The tool must load its reference metadata directly from the Windows App SDK package — not from the runtime our code happens to compile against.

## Implementation Strategy

Members missing on (3) are added in one of two ways:

| Way | When | Trade-off |
|---|---|---|
| **Real member on the type** | Most cases | Shows up in reflection, matches WinUI's own pattern, no consumer ceremony |
| **C# 14 extension member** | When adding to a type we can't otherwise reach | Requires the consumer to import the extension namespace; **also requires the compat tool to scan source for `extension(T)` blocks** to count it as covered |

Neither strategy involves inheriting from any reimplementation of WinUI in (2) — we always own the declaration.

### Where Members May Be Added (File Ownership Rules)

These constraints apply to all parity work. Violating them requires explicit approval per change.

**`LeXtudio.UI.Xaml.Controls.RichTextBlock`** — fully owned by us. Add members directly.

**`LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow`** — fully owned by us. Add members directly. New control; no upstream WPF analogue (the WPF analogue is `FlowDocumentScrollViewer` / `FlowDocumentPageViewer`, which we are not porting).

**`LeXtudio.UI.Xaml.Controls.RichEditBox`** — fully owned by us. Add members directly. No upstream WPF source is being linked for this control's body; `RichEditBox` has no clean WPF analogue (WPF uses `RichTextBox`, which has a different document model and is not being ported).

**`System.Windows.Documents.*` types** (`Run`, `Span`, `Paragraph`, `Block`, `Inline`, `Hyperlink`, `Bold`, `Italic`, `Underline`, `LineBreak`, `InlineUIContainer`, `TextElement`, …) — these are ported from upstream WPF source. The file conventions are:

| File | Status | What it contains |
|---|---|---|
| `Type.cs` | **Read-only** (upstream WPF) | Original WPF source; only sync-from-upstream changes |
| `Type.wpf.cs` | **Read-only** (WPF target adapter) | WPF-target-specific glue |
| `Type.uno.cs` | **Editable** | Uno/WinUI bridging — this is where parity-fill members live |

All new members for parity go into `Type.uno.cs` as a partial-class extension. If a document type doesn't yet have a `.uno.cs` partial, create one. If a member must be added to the upstream `.cs` or `.wpf.cs`, pause and seek explicit approval.

**`Microsoft.UI.Text.*` text-document types** (`RichEditTextDocument`, `ITextRange`, `ITextSelection`, `ITextCharacterFormat`, `ITextParagraphFormat`, plus enum/struct types). These are entirely new files we author. Place them under `LeXtudio.RichText/TextDocument/`. They are fully editable; no upstream linkage.

### Stub vs Implement

For every member we add:

| Class | Behavior | When |
|---|---|---|
| **Implement** | Real working code | The member is used by the rendering or editing path or by realistic consumers |
| **Stub** | Sensible default (`0` / `null` / no-op event), never throws | The member is in a non-goal area (KeyTip, AccessKey, proofing word lists) but its absence breaks compile |

**Stubs do not throw `NotImplementedException`.** Throwing breaks "drop-in compile" just as badly as a missing member.

For `RichEditBox` specifically, the "implement" line is drawn around editing, selection, IME, undo/redo, clipboard, character/paragraph formatting, and read-only state. Spell-check is API-present, behavior-stub.

## Phases

### Phase 1 — Tool: extend `TypePairs` and refresh baseline

**Gate before any new code on the three controls.**

- [ ] Extend `tools/RichTextBlockCompat/TypePairs.cs` to include:
  - `Microsoft.UI.Xaml.Controls.RichTextBlockOverflow` ↔ `LeXtudio.UI.Xaml.Controls.RichTextBlockOverflow`
  - `Microsoft.UI.Xaml.Controls.RichEditBox` ↔ `LeXtudio.UI.Xaml.Controls.RichEditBox`
  - `Microsoft.UI.Text.RichEditTextDocument` ↔ `Microsoft.UI.Text.RichEditTextDocument` (ours, same namespace)
  - `Microsoft.UI.Text.ITextDocument` ↔ `Microsoft.UI.Text.ITextDocument`
  - `Microsoft.UI.Text.ITextRange` ↔ `Microsoft.UI.Text.ITextRange`
  - `Microsoft.UI.Text.ITextSelection` ↔ `Microsoft.UI.Text.ITextSelection`
  - `Microsoft.UI.Text.ITextCharacterFormat` ↔ `Microsoft.UI.Text.ITextCharacterFormat`
  - `Microsoft.UI.Text.ITextParagraphFormat` ↔ `Microsoft.UI.Text.ITextParagraphFormat`
- [ ] Confirm metadata loader picks up `LeXtudio.RichText.dll` for the new types (and `LeXtudio.Windows.dll` for any document types we still resolve through the shim).
- [ ] Regenerate `COMPAT-REPORT.md` and capture the **new baseline**. `RichTextBlock` should stay near its current coverage; `RichTextBlockOverflow`, `RichEditBox`, and the `Microsoft.UI.Text.*` rows will start at 0% — that is the input to the rest of the plan.

### Phase 2 — `RichTextBlockOverflow`

`RichTextBlockOverflow` is smaller than `RichEditBox` and exercises chained-layout plumbing we already need for `RichTextBlock`'s `HasOverflowContent` / `OverflowContentTarget`. Closing it first validates that plumbing.

Implementation order:

- [ ] Declare the type as a `Control` (matching WinUI base) under `LeXtudio.UI.Xaml.Controls`.
- [ ] DPs: `ContentSource`, `OverflowContentTarget`, `HasOverflowContent`, `IsTextTrimmed`, `MaxLines`, `Padding`.
- [ ] Layout: in `MeasureOverride` / `ArrangeOverride`, ask the upstream source (a `RichTextBlock` or another `RichTextBlockOverflow`) for the next slice of its rendered content and paint it into our `Canvas`.
- [ ] Forward selection events to the source so cross-region selection works.
- [ ] Events: `IsTextTrimmedChanged`, `SelectionChanged` (proxied to source).
- [ ] Re-run compat tool; iterate until `RichTextBlockOverflow` ≥ 95%.

### Phase 3 — Text-document model (`Microsoft.UI.Text.*`)

Before `RichEditBox` can be implemented, its `Document` surface must exist.

Implementation order:

- [ ] `Microsoft.UI.Text` enums and supporting types — `TextGetOptions`, `TextSetOptions`, `TextRangeUnit`, `TextScript`, `TextConstants`, `RichEditMathMode`, etc. Match WinUI names and member values.
- [ ] `ITextRange`, `ITextSelection`, `ITextCharacterFormat`, `ITextParagraphFormat` interfaces — declare with WinUI shapes.
- [ ] Concrete implementations: `TextRange`, `TextSelection`, `TextCharacterFormat`, `TextParagraphFormat` (internal classes; public API is the interfaces).
- [ ] `RichEditTextDocument : ITextDocument` — `GetText`, `SetText`, `GetRange`, `Selection`, `LoadFromStream`, `SaveToStream` (RTF and plain text). Streams use `Windows.Storage.Streams.IRandomAccessStream`.
- [ ] Round-trip tests for plain text and RTF.

### Phase 4 — `RichEditBox`

**Source reuse map** — before writing new code, pull from these siblings:

| Concern | Reuse from |
|---|---|
| IME / text-services | `TextCore.Uno/src/LeXtudio.UI.Text.Core` (`CoreTextEditContext`, `Win32TextInputAdapter`, `MacOSTextInputAdapter`, `LinuxIbusTextInputAdapter`, `X11KeyHelper`) |
| Control-template scaffolding | `TextCore.Uno/src/LeXtudio.TextBox/Controls/TextBox.cs` |
| WPF-ported document model (Run, Span, Paragraph, Hyperlink, List, ListItem, Section, TextPointer, FlowDocument) | `WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/*` |
| WPF-ported editing commands & input plumbing (`EditingCommands`, `ApplicationCommands`, `RoutedCommand`, `CommandBinding`, `KeyGesture`, `TextCompositionEventArgs`) | `WindowsShims/src/LeXtudio.Windows/System.Windows/Input/*` |
| WPF-ported text-pointer / range editing helpers (`TextRangeEdit`, `TextRangeEditTables`, `TextSchema`, `ITextPointer`, `ITextLayoutHost`) | `WindowsShims/src/LeXtudio.Windows/System.Windows/Documents/*` and `WindowsShims/src/LeXtudio.Windows/MS.Internal/*` |
| Caret + blink + setter callbacks | `UnoEdit/src/UnoEdit/Editing/Caret.uno.cs` |
| Selection model + clipboard HTML/RTF fragment | `UnoEdit/src/UnoEdit/Editing/Selection.cs` |
| Caret navigation accelerators | `UnoEdit/src/UnoEdit/Editing/CaretNavigationCommandHandler.uno.cs` |
| Editing commands (Ctrl+B/I/U, Cut/Copy/Paste, Undo/Redo) | `UnoEdit/src/UnoEdit/Editing/EditingCommandHandler.uno.cs` |
| Pointer / mouse selection | `UnoEdit/src/UnoEdit/Editing/SelectionMouseHandler.uno.cs` |
| Input handler stack | `UnoEdit/src/UnoEdit/Editing/InputHandlers.cs` |
| Inline text layout / rendering | `Pretext.Uno` (already a dependency of `LeXtudio.RichText`) |

Implementation order:

- [ ] Declare as `Control` with template parts `ContentElement` (scroll container), `PlaceholderTextContentPresenter`, `HeaderContentPresenter`.
- [ ] DPs covering: `Header`, `HeaderTemplate`, `PlaceholderText`, `IsReadOnly`, `IsSpellCheckEnabled`, `IsTextPredictionEnabled`, `MaxLength`, `AcceptsReturn`, `TextAlignment`, `TextWrapping`, `Document`, `SelectionHighlightColor`, `SelectionHighlightColorWhenNotFocused`, `SelectionFlyout`, `ContextFlyout`, `DesiredCandidateWindowAlignment`, `InputScope`, `TextReadingOrder`, `PreventKeyboardDisplayOnProgrammaticFocus`, `ClipboardCopyFormat`, `CharacterCasing`, `HorizontalTextAlignment`, `CornerRadius`, `Description`, `DisabledFormattingAccelerators`, `ProofingMenuFlyout`, etc.
- [ ] Caret + blink, drawn into the same `Canvas`/`Path` model used by `RichTextBlock`.
- [ ] Mouse / touch / pen pointer input → selection.
- [ ] Keyboard input — character input, deletion, navigation, accelerators (Ctrl+B, Ctrl+I, Ctrl+U, Ctrl+C/X/V, Ctrl+Z/Y, Ctrl+A, Home/End, Page Up/Down, Shift+nav, Ctrl+Shift+nav).
- [ ] IME composition (via `CoreTextEditContext` on platforms that support it; soft IME bridging through `InputPane`).
- [ ] Undo/redo stack.
- [ ] Clipboard cut/copy/paste (plain text, RTF, and OLE on Windows).
- [ ] Visual states: `Normal`, `Disabled`, `PointerOver`, `Focused`, `ReadOnly`, `FocusedDisabled`.
- [ ] Events: `TextChanging`, `TextChanged`, `SelectionChanged`, `SelectionChanging`, `BeforeTextChanging`, `ContextMenuOpening`, `Paste`, `CopyingToClipboard`, `CuttingToClipboard`, `CandidateWindowBoundsChanged`, `TextCompositionStarted` / `Changed` / `Ended`.
- [ ] Methods: `Focus(FocusState)`, `Undo`, `Redo`, `Copy`, `Cut`, `Paste`, `SelectAll`.

### Phase 5 — Sync `RichTextBlock` + Overflow chain

- [ ] Implement `RichTextBlock.HasOverflowContent` and `OverflowContentTarget` to actually chain into `RichTextBlockOverflow`.
- [ ] Verify cross-region selection round-trips.
- [ ] Re-run compat; `RichTextBlock` coverage should hold.

### Phase 6 — Resolve Signature Mismatches

Any `:warning: Mismatch` rows in the report are stronger violations than missing members — same name + same kind but wrong shape. For each: change the local signature to match WinUI 3.

### Phase 7 — Long Tail and Gate

- [ ] Anything still missing: investigate, implement or stub.
- [ ] When overall hits 100%, add the CI gate:

```powershell
dotnet run --project UnoRichText/tools/RichTextBlockCompat -- --min 100
```

## Tracking

After each step, regenerate `COMPAT-REPORT.md`:

```powershell
dotnet run --project UnoRichText/tools/RichTextBlockCompat
```

The report has per-type coverage, gap-by-kind, and a "Gaps" table per type — work from the top of those tables down.

## Sequencing Note

The phases are sequential because each phase produces APIs the next phase needs:

```
Phase 1 (tool) → Phase 2 (Overflow) → Phase 3 (text-document model) → Phase 4 (RichEditBox) → Phase 5 (chain) → Phase 6 (mismatch fixes) → Phase 7 (gate)
```

Phases 2 and 3 are independent of each other and can run in parallel if multiple contributors are available.

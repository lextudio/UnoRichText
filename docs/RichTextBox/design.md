# RichTextBox Source-First Port Plan

This document describes the exact source-first port strategy for bringing WPF `RichTextBox` and its supporting WPF document editing engine into Uno using `WindowsShims` as the WPF source compatibility layer.

## Objective

- Port the upstream WPF `RichTextBox` engine and the WPF `System.Windows.Documents`/`System.Windows.Input` dependencies.
- Keep imported WPF files read-only and preserve provenance.
- Maximize shared source between WPF and Uno using the `.cs` / `.wpf.cs` / `.uno.cs` partial pattern.
- Bridge WPF and Uno by mapping basic WPF types onto WinUI equivalents in `LeXtudio.Windows` and implementing only the minimal shims needed for the editor.
- Place platform-specific WPF runtime code in `.wpf.cs` and Uno-specific integration in `.uno.cs` partial files.
- Use `WindowsShims` to host the WPF-compatible types and `UnoRichText`/`UnoEdit`/`CoreText.Uno` for the runtime editing, rendering, and input plumbing.

## What this plan is

- A source-first port, not a rewrite.
- A WPF-centric compatibility project, not a WinUI-only `RichEditBox` reimplementation.
- A plan to port WPF source where it is the most maintainable long-term solution for complex editing semantics.
- A strategy that maximizes shared code through `.cs` / `.wpf.cs` / `.uno.cs` partials.

## What this plan is not

- Not a new editor engine built from scratch.
- Not a minimal benchmark against WinUI API parity only.
- Not a path that abandons WPF source provenance or the `.cs`/`.wpf.cs`/`.uno.cs` convention.

## Scope

### In scope

- `System.Windows.Controls.RichTextBox` port from upstream WPF.
- `System.Windows.Documents` document model and editing helpers used by `RichTextBox`.
- `System.Windows.Input` editing commands and text composition event types.
- Source import into `WindowsShims/src/LeXtudio.Windows/*`.
- Uno-specific runtime integration in `UnoRichText` and sibling repos.
- WPF source provenance tracking and read-only imported files.

### Out of scope

- A separate `Microsoft.UI.Xaml.Controls.RichEditBox`-only implementation.
- Non-WPF editor controls such as `RichTextBlock`/`RichTextBlockOverflow` unless they are required for the WPF `RichTextBox` port.
- Any rewrite that replaces imported WPF source with a brand-new custom implementation unless there is no viable upstream equivalent.

## Source-first strategy

### 1. Port WPF source into WindowsShims

- Identify the upstream WPF source files needed for `RichTextBox` and `FlowDocument` editing.
- Import them into `WindowsShims/src/LeXtudio.Windows` using the repository's existing source-first pattern.
- Preserve file ownership and maximize shared behavior with:
  - `Type.cs` — read-only WPF shared source
  - `Type.wpf.cs` — WPF-specific runtime adapter code
  - `Type.uno.cs` — Uno-specific runtime wiring and integration
- Add a one-line provenance header comment to each imported WPF file.

### 2. Keep the WPF semantic layer in WindowsShims

- Use `WindowsShims` for:
  - `System.Windows.Documents.*` types such as `TextElement`, `Run`, `Span`, `Paragraph`, `Hyperlink`, `List`, `ListItem`, `Section`, `TextPointer`, `TextRange`, `TextRangeEdit`, `TextSchema`, `FlowDocument`.
  - `System.Windows.Input.*` types such as `EditingCommands`, `ApplicationCommands`, `RoutedCommand`, `KeyGesture`, `TextCompositionEventArgs`.
  - `MS.Internal` editing infrastructure that `RichTextBox` semantics require.
- Do not reimplement these behaviors in `UnoRichText` if the WPF source already contains the correct logic.

### 3. Integrate with Uno runtime hosts

- Use `UnoRichText` as the Uno-side rendering and host surface for text layout.
- Use `UnoEdit` and `CoreText.Uno` for caret, selection, input handlers, and IME integration.
- Where WPF source computes document/selection state, bridge it into the Uno rendering host rather than replace it.

### 4. Maintain provenance and compatibility

- `WindowsShims` is the canonical home for ported WPF source and shared WPF/Uno logic.
- `UnoRichText` is the canonical home for Uno runtime wiring.
- In the meantime, bridge WPF to Uno exactly as `LeXtudio.Windows` already does: map basic WPF types like brushes, colors, and geometry to WinUI types, and keep additional shim surface minimal.
- Keep the upstream WPF source portion read-only and documented, while using `.wpf.cs` and `.uno.cs` partials for platform-specific implementations.
- Prefer importing upstream WPF source for hard editing behaviors instead of rewriting.

## Implementation phases

### Phase 0 — Compilable host shell

This is the first implementation checkpoint. It is deliberately smaller than the
full WPF source port so that the sample can move from `RichEditBox` experiments
to a real `RichTextBox` control without waiting for the entire WPF text editor
dependency graph to compile.

- Extend the existing `System.Windows.Controls.RichTextBox` compatibility stub
  in `WindowsShims` with a WPF-shaped `FlowDocument Document` property.
- Add `LeXtudio.UI.Xaml.Controls.RichTextBox` in `UnoRichText` as the Uno runtime
  host.
- Render the `FlowDocument` through the existing `RichTextBlock` renderer.
- Treat the document model as source of truth. Do not route this milestone
  through `Microsoft.UI.Text.RichEditTextDocument`.
- Support document replacement and document content invalidation well enough for
  samples and tests.
- Keep editing, caret, selection, IME, clipboard, undo/redo, and upstream WPF
  `TextEditor` behavior out of this phase.

Acceptance:

- `new RichTextBox().Document` returns a non-null `FlowDocument`.
- Assigning a `FlowDocument` renders its `Blocks`.
- The sample can host a `RichTextBox` with paragraphs, bold, italic, underline,
  hyperlinks, and inline UI using the same document types as `RichTextBlock`.

### Phase 1 — Audit and import

- Audit the upstream WPF `RichTextBox` implementation and its dependency graph.
- Pin the exact source files in `dotnet/wpf` needed for the port.
- Import or link them into `WindowsShims/src/LeXtudio.Windows` with provenance metadata.
- Keep uncompiled upstream candidates as `<None>` until their dependency slice is ready.
- Promote files from `<None>` to `<Compile>` only when the slice compiles in
  the WindowsShims environment.

Current concrete slice:

- `System.Windows.Controls.Primitives.TextBoxBase`
- `System.Windows.Controls.RichTextBox`
- `System.Windows.Documents.TextSelection`
- `System.Windows.Documents.TextEditor`
- `System.Windows.Documents.TextRange`
- `System.Windows.Documents.TextContainer`

Current file-shape rule for this slice:

- Prefer upstream WPF files tracked as `UpstreamWpf/*`.
- If a slice is not yet compilable, place temporary bridge code in local `*.uno.cs`.
- Do not grow temporary bridge files into long-term semantic owners; replace them by promoting the real upstream file when the dependency slice is ready.

Porting rule for this slice:

- Prefer matching WPF public and internal shape in `WindowsShims` first.
- Keep Uno behavior in `UnoRichText` as temporary glue only.
- Avoid adding new RichTextBlock-specific navigation or editing behavior unless it is required to host the WPF slice.

### Phase 2 — Source-first typing

- Ensure `WindowsShims` exposes the WPF type surface required by `RichTextBox` consumers.
- Add missing WPF compatibility shim types as thin wrappers or adapter classes.
- Use the `.uno.cs` partials to expose Uno-friendly constructors, adapters, and platform-specific helpers.

### Phase 3 — Bridge to UnoRichText runtime

- Implement the runtime integration layer in `UnoRichText`.
- Use WPF source types for document editing semantics and command handling.
- Map WPF document state into Uno rendering inputs.
- Implement any missing runtime plumbing in `UnoRichText` using sibling repo behavior rather than rewriting.
- Replace the Phase 0 render-only host internals as upstream editing pieces
  become available, preserving the public `Document` surface.

### Phase 4 — Behavior completeness

- Add editor interaction features from WPF source: selection, formatting commands, clipboard, undo/redo, IME, tables/lists if required.
- Port or reuse WPF RTF support via `RtfToXamlReader` / `XamlToRtfWriter` if the imported `RichTextBox` requires it.
- Preserve WPF editing semantics for `TextPointer` movement, `TextRange` editing, and rich-content manipulation.

### Phase 5 — Validation and gate

- Validate against real WPF expectations using unit tests and manual scenarios.
- Use compatibility tooling where applicable to ensure WPF-facing surface parity.
- Keep imported upstream files intact and use `.uno.cs` partials for all Uno-specific divergence.

## Sibling reuse map

### WindowsShims

- `WindowsShims` is the source-first port container for WPF compatibility.
- It already contains many ported WPF document and input types.
- Add new WPF imports and maintain them here.

### UnoRichText

- Use `UnoRichText` for the actual Uno runtime host and rendering path.
- It should consume WPF document state from `WindowsShims` and render it across Uno platforms.

### UnoEdit

- Use `UnoEdit` for caret, selection, pointer/mouse handling, and command execution patterns.
- Reuse its existing editing helpers instead of writing new low-level text navigation.

### CoreText.Uno

- Use `CoreText.Uno` for cross-platform IME and text input integration.
- Bridge `TextCompositionEventArgs` and composition state through this layer.

## Provenance rules

- Every file imported from `dotnet/wpf` must start with a provenance header comment.
- Do not edit imported `Type.cs` or `Type.wpf.cs` files except to preserve upstream compatibility.
- Place Uno-specific platform glue in `Type.uno.cs` partials.
- Document any port decision that diverges from upstream source in `WindowsShims/docs/PROVENANCE.md`.

## Success criteria

- The port is based on upstream WPF source for the `RichTextBox` editing engine.
- `WindowsShims` contains the WPF compatibility layer and imported source with provenance.
- `UnoRichText` contains only the Uno-side integration and rendering wiring.
- The result preserves WPF editing semantics rather than reinventing them.
- The system remains maintainable with future upstream WPF source updates.

## Work Session Log

This section and session*.md files are used to record implementation sessions for the Uno-side
`RichTextBox` port. Keep each entry focused on what changed, what was verified,
what remains blocked, and the next recommended slice.

- [Session 1](session1.md): Phase 0 feature switch and baseline validation.
- [Session 2](session2.md): WindowsShims shell test inventory; blocked on a
  dispatcher-backed test host.
- [Session 3](session3.md): Uno runtime test harness; WindowsShims shell tests
  pass inside the Uno app process.
- [Session 4](session4.md): `LeXtudio.UI.Xaml.Controls.RichTextBox` tests moved
  to runtime harness with skip-safe plain `dotnet test` behavior.
- [Session 5](session5.md): Added document replacement test for
  `TextLayoutHost` detach/attach behavior in Phase 0 host.
- [Session 6](session6.md): Added mutation-after-assignment selection coherence
  runtime test for `RichTextBox`.
- [Session 7](session7.md): Fixed pre-layout selection snapshot so `SelectAll()`
  after document mutation returns expected selected text.
- [Session 8](session8.md): Added newline-preserving cross-paragraph selection
  assertion and documented `TextPointer.Offset` blocker for range tests.
- [Session 9](session9.md): Investigated pointer-range selection; documented
  current `Select(start,end)` blocker and required next dependency slice.
- [Session 11](session11.md): Brought upstream `TextRange` online — fixed the
  infinite-recursion trap in the `TextRangeBase` shim, populated `_TextSegments`
  in the constructor, and implemented WPF-style plain-text walking in
  `TextRangeBase.GetText`.
- [Session 12](session12.md): Promoted upstream `TextSchema.cs` — replaced the
  66-line return-true stub with real WPF schema validation; small
  `#if !HAS_UNO` patch covers WPF-only DependencyProperty references; `Image`
  resolves to the WinUI control, not a shim.
- [Session 13](session13.md): Promoted upstream `TextElementCollection<T>` —
  resolved the cross-container reparenting blocker; cross-paragraph
  `TextRange.Text` now works end-to-end; an Uno partial preserves the
  `INotifyCollectionChanged` surface the renderer relies on.
- [Session 14](session14.md): Promoted upstream `TextRangeBase.cs` —
  retired the Session 11 hand-rolled stub; real WPF change-block and
  text-walker semantics back `ITextRange`; clipboard/serialization paths
  gated behind `#if !HAS_UNO` pending future sessions.
- [Session 15](session15.md): Promoted upstream `TextRangeEdit.cs` (2,382
  lines) — real range-mutation primitives back Bold/Italic/SetInlineProperty;
  added `GetValueSource` / `GetDefaultValue` extension members and minimal
  `TextRangeEditLists` / `TextEditorCharacters` stubs.
- [Session 16](session16.md): Promoted upstream `TextEditor.cs` (WPF editing
  controller) and `TextRangeEditLists.cs` — built out command-handler, dispatcher,
  cursor, IME, and system-metrics shim infrastructure; spell-check gated
  `#if !HAS_UNO`; 62-test baseline unchanged.
- [Session 17](session17.md): Promoted TextEditorParagraphs, TextEditorLists,
  TextEditorTables, TextEditorContextMenu, TextEditorMouse — added ContextMenu/MenuItem
  shims, Visual3D partial stub, mouse-event shims, `GetPosition(object?)` adapters,
  TSF reconversion gated `#if HAS_UNO`; 62-test baseline unchanged.
- [Session 18](session18.md): Promoted TextEditorSelection and TextEditorTyping — added
  KeyboardDevice, Key enum extensions, TextComposition stubs, InputLanguageManager event,
  ScrollBar/IScrollInfo/TextBoxView shims, RoutedEventArgs.Handled, RoutedCommand 2-arg
  overloads; RichTextBox.TextEditor returns null-Documents.TextEditor; 62-test baseline unchanged.
- [Session 19](session19.md): Promoted TextEditorCopyPaste and TextEditorDragDrop — added
  IDataObject/Clipboard/DataObject* event args, DragDrop statics, DragDropEffects/KeyStates enums,
  WindowInteropHelper, RoutedEvent constructor, extended IScrollInfo with scroll methods, AdornerLayer
  stub; gated WpfPayload rich-content paths and Win32 drag-drop with `#if HAS_UNO`; 62-test baseline
  unchanged.
- [Session 20](session20.md): Promoted TextRangeSerialization and TextEditorCharacters —
  added TryRemove to TextDecorationCollection, DependencyProperty.OwnerType/FromName extensions,
  FrameworkContentElement.LanguageProperty, BitmapImage shim, WpfPayload.AddImage stub;
  gated image-package path with `#if !HAS_UNO`; patched ToggleUnderline Add call; 62-test baseline unchanged.
- [Session 21](session21.md): Promoted FrameworkTextComposition, FrameworkRichTextComposition,
  TextEffectResolver, TextSelectionHighlightLayer — added InputManager/TextCompositionAutoComplete/
  CompositionText shims, TextEffect.Clone/PositionStart/PositionCount, DynamicPropertyReader;
  gated three TSF COM methods with `#if !HAS_UNO`; 62-test baseline unchanged.
- [Session 22](session22.md): Promoted NullTextContainer, NullTextNavigator, TextTreeDumper —
  added DependencyProperty.DefaultMetadata extension; patched new DependencyObject() and
  FixedDocument reference in NullTextNavigator; 62-test baseline unchanged.
- [Session 23](session23.md): Promoted Table, TableRow, TableCell, TableRowGroup, and three
  collection files — removed all Table* stubs from TableAndHighlightShims.cs; added
  OnNewParent virtual to FrameworkContentElement; fixed InsertTable read-only property
  assignments; no upstream patches needed; 62-test baseline unchanged.
- [Session 24](session24.md): Promoted TextMapOffsetErrorLogger, TextRangeEditTables
  (upstream), TextFindEngine, ColumnResizeUndoUnit — added TextDocumentView/CellInfo/
  ColumnResizeAdorner stubs; one #if !HAS_UNO guard in TableBorderHitTest private body;
  Fixed layout branch gated in TextFindEngine; deleted TextRangeEditTables local stub;
  62-test baseline unchanged.

## Session 40 — RichTextBox selection porting investigation

Caret placement and caret-key navigation were already in a good place coming into this session. The remaining breakage turned out to be selection-specific, with two separate issues:

1. Mouse drag selection was never fully ported on Uno.
2. Copy could execute logically, but the WPF clipboard shim did not persist any text.

### Root cause 1: drag selection gesture was only half-wired

`System.Windows.Controls.RichTextBox.uno.cs` forwarded pointer press into WPF `TextEditorMouse.SetCaretPositionOnMouseEvent(...)`, which is enough to place a caret, but it never forwarded the rest of the mouse selection lifecycle.

That meant:

- pointer down established an anchor/caret;
- pointer move never called `ITextSelection.ExtendSelectionByMouse(...)`;
- pointer up never finalized anything;
- the user could click to place the caret, but could not drag to grow the selection.

### Root cause 2: selection visuals were not being painted by the Uno view

`TextSelection` state could still change logically. This is why `Select All` updated the sample's `Selection:` label correctly.

But on Uno, the visible selection highlight was not being rendered by the old WPF adorner/caret path in a useful way. We already own the Uno caret rectangle in `FlowDocumentView`, so selection highlight needed the same treatment: render it directly in the Florence-backed Uno view layer.

### Root cause 3: clipboard shim was a no-op

`System.Windows.Clipboard` in the Uno shim returned empty/default values and ignored `SetDataObject(...)` / `SetText(...)`.

So even if `ApplicationCommands.Copy` ran and `TextEditorCopyPaste` created a `DataObject`, nothing was written to any clipboard store.

## Fixes implemented

### 1. Full pointer drag selection in `RichTextBox.uno.cs`

File: `WindowsShims/src/LeXtudio.Windows/System.Windows/Controls/RichTextBox.uno.cs`

- Added Uno `OnPointerMoved(...)` support:
  - when a left-button drag is active, hit-test the current Florence position;
  - call `ITextSelection.ExtendSelectionByMouse(...)`;
  - refresh caret and selection visuals.
- Added Uno `OnPointerReleased(...)` support:
  - end the drag gesture;
  - release pointer capture;
  - refresh visible selection state.
- Pointer press now also starts a local selection gesture and refreshes selection visuals immediately.

This keeps selection growth in the normal WPF `TextSelection` model while only porting the missing Uno input plumbing.

### 2. Selection rendering moved into `FlowDocumentView`

File: `WindowsShims/src/LeXtudio.Windows/MS.Internal.Documents/FlowDocumentView.uno.cs`

- Added `RefreshSelection()`.
- It reads the current `TextSelection` from the document `TextContainer`.
- For each rendered Florence line, it computes the overlap between:
  - logical selection range, and
  - that visual line's offsets.
- It then paints translucent Uno rectangles behind the text and keeps the caret on top.

This mirrors the earlier caret strategy:

- WPF owns logical selection/caret state,
- Florence/Uno view owns visible geometry.

### 3. Shared hook for selection-change repaint

Files:

- `WindowsShims/ext/wpf/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/Primitives/TextBoxBase.cs`
- `WindowsShims/src/LeXtudio.Windows/System.Windows/Controls/Primitives/TextBoxBase.uno.cs`

Changes:

- `TextBoxBase` is now declared `partial`.
- Added Uno-side `NotifySelectionChanged()` hook.
- That hook:
  - refreshes `FlowDocumentView` selection visuals when the render scope is Florence-backed;
  - raises the normal `SelectionChanged` routed event.

This keeps the selection-visual refresh attached to shared control lifecycle rather than sprinkling ad hoc refreshes across unrelated code.

### 4. Clipboard shim now stores and publishes text

File: `WindowsShims/src/LeXtudio.Windows/System.Windows/System.Windows.cs`

- Implemented backing storage for `Clipboard.SetDataObject(...)`, `GetDataObject()`, `SetText(...)`, and `GetText()`.
- When text formats are present, the shim now also pushes text into WinUI clipboard APIs:
  - `Windows.ApplicationModel.DataTransfer.DataPackage`
  - `Clipboard.SetContent(...)`
  - `Clipboard.Flush()`

This is enough for WPF RichTextBox text copy scenarios on Uno.

## Why this keeps reuse in a good place

This session deliberately avoided forking more WPF selection logic.

We did **not** reimplement selection semantics in an Uno-only document model. Instead:

- WPF `TextSelection` still owns anchor/moving positions and selection heuristics;
- Florence still provides layout/hit-testing;
- Uno-specific code only fills in the missing platform adapters:
  - pointer gesture forwarding,
  - visual highlight painting,
  - clipboard bridge.

That is the same reuse pattern that worked well for the caret fixes in sessions 38–39.

## Verification

Build:

```text
dotnet build WindowsShims/src/LeXtudio.Windows/LeXtudio.Windows.csproj --no-restore -f net10.0-desktop
```

Focused RichTextBox shell verification:

```text
dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.Constructor_CreatesImplicitEmptyFlowDocument --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.AssignedDocument_IsOwnedAndSerializable --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.Document_CannotBeSharedAcrossRichTextBoxes --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.AddChild_ReplacesOnlyImplicitDocument --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.AppendText_CreatesRunInLastParagraph --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.SelectionFormatting_CanBeAppliedAtCaret --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests.RoutedCommand_ClassHandlers_RespectRegisteredControlType
```

Result: **Passed (7/7)**.

## Next practical validation

Run the sample tab again and verify:

1. click, drag, and release selects visible text on the WPF RichTextBox sample;
2. `Select all` shows a visible highlight, not only a logical label change;
3. `Copy Selection` places the selected plain text onto the clipboard.

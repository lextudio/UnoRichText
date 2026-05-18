# Session 2 — WindowsShims RichTextBox Shell Test Inventory

Status: superseded by Session 3 runtime harness.

Goal:

- Add focused regression coverage for the WPF-shaped
  `System.Windows.Controls.RichTextBox` compatibility shell in WindowsShims.
- Exercise Phase 0 document ownership and append behavior before wiring the
  Uno runtime host to a WPF-style text editor pipeline.

Changes:

- Added `Controls/WpfRichTextBoxShellTests.cs`.
- Included the test file in `LeXtudio.RichText.Tests.csproj`.
- Marked the fixture ignored because construction currently requires an active
  Uno dispatcher host.

Covered behavior:

- implicit `FlowDocument` creation
- implicit empty document serialization suppression
- explicit document ownership and serialization
- rejecting a `FlowDocument` already owned by another rich text box
- `IAddChild` replacement of the implicit document
- `AppendText` creating a run in the last paragraph
- `AppendText` honoring current typing font weight/style through the shell's
  temporary typing-property bridge

Verification:

- `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -p:UseNuGetPackage=false --filter WpfRichTextBoxShellTests`
- Result before ignoring the fixture: all six tests failed during construction
  because `FlowDocument`/`RichTextBox` construction enters Uno dependency-object
  dispatcher initialization without an active dispatcher.
- Result after ignoring the fixture: command exits successfully; 0 failed, 0
  passed, 6 skipped.
- Session 3 added a Uno runtime test mode and enabled this fixture there.

Notes:

- These tests intentionally target `System.Windows.Controls.RichTextBox`, not
  `LeXtudio.UI.Xaml.Controls.RichTextBox`. The Uno host still requires a UI
  runtime; this session is about hardening the WPF compatibility shell that the
  host will eventually consume.
- The original assumption that these could be pure non-UI tests was wrong in
  the current port shape. `FlowDocument` derives through WindowsShims document
  types that still depend on Uno dependency-object initialization, and the
  control shell derives from `TextBoxBase`.

Next recommended slice:

- Use the Session 3 runtime harness for dispatcher-bound document/control tests.
- Keep the shell tests narrow: document ownership, serialization decisions,
  child-content replacement, append behavior, and typing properties.

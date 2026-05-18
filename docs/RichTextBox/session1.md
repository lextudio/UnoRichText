# Session 1 — Phase 0 feature switch and baseline validation

Status: done.

Goal:

- Make the existing `LeXtudio.UI.Xaml.Controls.RichTextBox` host compile on
  demand without making it the default package surface yet.
- Keep the default build stable while allowing targeted Phase 0 validation with
  a single MSBuild property.

Changes:

- Added `EnableRichTextBox` to `src/Directory.Build.props`.
- When `EnableRichTextBox=true`, `RICHTEXTBOX` is added to `DefineConstants`.
- Default remains `EnableRichTextBox=false`, so the experimental host is not
  exposed accidentally.
- Updated the Phase 0 Uno host to stop directly constructing WPF
  `TextSelection`. The active WindowsShims frontier now compiles upstream
  `TextSelection`, whose constructor is internal as in WPF. The render host keeps
  `SelectionStart`, `SelectionEnd`, and `SelectedText` active; the `Selection`
  property is deferred until the WPF `TextEditor` bridge owns selection.

Verification:

- `dotnet build UnoRichText/src/LeXtudio.RichText/LeXtudio.RichText.csproj`
  succeeded with warnings only.
- `dotnet build UnoRichText/src/LeXtudio.RichText/LeXtudio.RichText.csproj -p:EnableRichTextBox=true -p:UseNuGetPackage=false`
  succeeded with warnings only.

Notes:

- The sample XAML still comments out the live `<rt:RichTextBox>` instance. That
  is intentional for this slice because XAML cannot be gated by the C# symbol in
  the same way. The sample should be switched in a later dedicated session.
- `EnableRichTextBox=true` currently requires `UseNuGetPackage=false` so the
  build consumes the active local `WindowsShims` source. The published
  `LeXtudio.Windows` package does not yet expose the same upstream
  `TextSelection` frontier.
- The Phase 0 host's `Selection` property intentionally throws
  `NotSupportedException` for now. This is preferable to constructing
  upstream-WPF `TextSelection` outside its real owner (`TextEditor`).

Next recommended slice:

- Add non-UI tests around the WindowsShims `System.Windows.Controls.RichTextBox`
  compatibility shell: implicit document creation, document ownership transfer,
  `IAddChild`, `ShouldSerializeDocument`, and `AppendText` typing format.

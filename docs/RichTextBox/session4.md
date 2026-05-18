# Session 4 — RichTextBox Runtime Test Migration

Status: done.

Goal:

- Move `LeXtudio.UI.Xaml.Controls.RichTextBox` tests off `[Ignore]` and onto
  the Uno runtime harness from Session 3.
- Keep plain `dotnet test` stable by skipping these tests when a runtime UI
  host is not available.

Changes:

- Updated `Controls/RichTextBoxTests.cs` to use
  `UnoRuntimeTestHost.RunOnUIThreadAsync(...)`.
- Added a `[OneTimeSetUp] RequireUnoRuntimeHost()` guard mirroring
  `WpfRichTextBoxShellTests`, so non-runtime execution gets `Assert.Ignore`
  instead of hard failures.
- Removed old ignore-only behavior and converted tests to async runtime tests.

Runtime command:

- `dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -p:EnableRichTextBox=true -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.RichTextBoxTests`

Verification:

- Runtime mode: 2 passed, 0 failed, 0 skipped.
- Plain VSTest command:
  `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -p:UseNuGetPackage=false -p:EnableRichTextBox=true --filter LeXtudio.RichText.Tests.Controls.RichTextBoxTests`
- Plain VSTest result after guard: tests are skipped (host unavailable), no
  failures.

Notes:

- `RichTextBoxTests` are behind `#if RICHTEXTBOX`; use
  `-p:EnableRichTextBox=true` when running them.
- This closes the immediate harness gap and keeps CI/dev local loops predictable.

Next recommended slice:

- Add one integration test around document replacement + visual host invalidation
  in `LeXtudio.UI.Xaml.Controls.RichTextBox` to protect Phase 0 behavior.

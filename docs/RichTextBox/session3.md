# Session 3 — Uno Runtime Test Harness

Status: done.

Goal:

- Make dispatcher-bound `FlowDocument`/`RichTextBox` tests runnable inside a
  real Uno desktop app process.
- Keep plain `dotnet test` safe by skipping dispatcher-bound tests with a clear
  message.

Changes:

- Added `Runtime/RuntimeTestApp.cs`.
- Added `Runtime/UnoRuntimeTestHost.cs`.
- Added a `--uno-runtime-tests` mode to `LeXtudio.RichText.Tests.Program`.
- Moved `WpfRichTextBoxShellTests` from skipped inventory to active runtime
  tests when launched through the Uno runtime app.

Runtime command:

- `dotnet run --project UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -f net10.0-desktop -p:UseNuGetPackage=false -- --uno-runtime-tests --test=LeXtudio.RichText.Tests.Controls.WpfRichTextBoxShellTests`

Verification:

- Runtime command result: 6 passed, 0 failed, 0 skipped.
- `dotnet test UnoRichText/src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -p:UseNuGetPackage=false --filter WpfRichTextBoxShellTests`
- Plain VSTest result: 0 failed, 0 passed, 6 skipped. This is intentional;
  dispatcher-bound tests must use `--uno-runtime-tests`.

Notes:

- Starting a `UnoPlatformHostBuilder` from inside VSTest did not reliably
  launch the app and initially hung. The stable shape is an executable runtime
  test mode where the Uno app owns the UI loop and NUnitLite runs after launch.
- The helper exposes `RunOnUIThreadAsync`, so future dispatcher-bound tests can
  stay focused on assertions rather than host setup.

Next recommended slice:

- Move the existing `LeXtudio.UI.Xaml.Controls.RichTextBox` ignored tests onto
  the runtime harness.
- Add a narrow sample-host test once the live `<rt:RichTextBox>` is enabled.

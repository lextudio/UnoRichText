# RichTextBlockCompat

Measures API parity between **WinUI 3** (Windows App SDK) and `LeXtudio.UI.Xaml.Controls.RichTextBlock` + related document types.

Goal: **100%**.

## Reference Authority

The only reference is **WinUI 3 as declared by the Windows App SDK** — `Microsoft.WinUI.dll` from the `Microsoft.WindowsAppSDK` NuGet package. The tool loads that DLL via `System.Reflection.MetadataLoadContext` so the comparison is against Microsoft's authoritative metadata, independent of any runtime the host project happens to compile against.

Other reimplementations of `Microsoft.UI.Xaml.*` (e.g. Uno Platform's) are out of scope. The tool does not load them, does not measure against them, does not flag members based on their implementation status.

## What's measured

Each `TypePair` in `TypePairs.cs` defines a WinUI 3 reference type and the local type that's supposed to mirror its API. For each pair:

- Public properties, methods, events, and static `DependencyProperty` getters are extracted from both sides (including inherited members, stopping at `System.Object`).
- Property/event accessors, obsolete members, and `object`-defined members are filtered out.
- For each WinUI 3 member, the local type is checked for an equivalent member with the same name, kind, and signature.

Each WinUI 3 member maps to one of:

- **Match** — local type has a member with the same name, kind, and signature.
- **Signature mismatch** — same name + kind exists locally, but the signature differs.
- **Missing** — no member with the same name + kind exists locally.

The percentage is `matched / total WinUI 3 members`.

The check is **unidirectional**. Extra members on the local side are not violations.

## Run

```powershell
dotnet run --project UnoRichText/tools/RichTextBlockCompat
```

Outputs:
- A markdown report at `UnoRichText/docs/COMPAT-REPORT.md`.
- A console summary with per-type and overall percentages.

## CI gating

```powershell
dotnet run --project UnoRichText/tools/RichTextBlockCompat -- --min 100
```

Exit code `1` if overall coverage falls below the threshold. The target value is `100`.

## Adding a new type pair

Edit `TypePairs.cs`. Each pair is a (WinUI 3 type full name, local type full name) string pair — the metadata context resolves them.

## What this tool does NOT check

- Runtime behavior (does setting `FontSize=20` actually render larger text?).
- Visual fidelity (do equivalent inputs produce equivalent rendered output?).
- Markup compatibility (XAML namespace, content-property attribute).

Those live in `LeXtudio.RichText.Tests` or future visual-diff tooling — they are intentionally out of scope here. The bar this tool sets is the *static* shape of the API: if it passes, callers can compile against `LeXtudio.UI.Xaml.Controls.RichTextBlock` interchangeably with WinUI 3's `Microsoft.UI.Xaml.Controls.RichTextBlock`.

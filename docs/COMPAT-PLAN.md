# Plan: 100% RichTextBlock API Parity with WinUI 3

Target: **100%** API parity measured by [`tools/RichTextBlockCompat`](../tools/RichTextBlockCompat/README.md) against [`COMPAT-REPORT.md`](COMPAT-REPORT.md).

## Scope Clarification (Three Categories)

1. **WinUI 3 (Windows App SDK)** — `Microsoft.UI.Xaml.*` as declared by Microsoft. **This is the reference.**
2. *(out of scope)* — Any other reimplementation of `Microsoft.UI.Xaml.*`. We do not compare against it, depend on its implementation status, or inherit from its types in our public surface.
3. **UnoRichText (ours)** — `LeXtudio.UI.Xaml.Controls.RichTextBlock` + `System.Windows.Documents.*` shim types. **This is the subject.**

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

**`LeXtudio.UI.Xaml.Controls.RichTextBlock`** — fully owned by us. Add members directly to the type's existing source files. Any organization is fine.

**`System.Windows.Documents.*` types** (`Run`, `Span`, `Paragraph`, `Block`, `Inline`, `Hyperlink`, `Bold`, `Italic`, `Underline`, `LineBreak`, `InlineUIContainer`, `TextElement`, …) — these are ported from upstream WPF source. The file conventions are:

| File | Status | What it contains |
|---|---|---|
| `Type.cs` | **Read-only** (upstream WPF) | Original WPF source; only sync-from-upstream changes |
| `Type.wpf.cs` | **Read-only** (WPF target adapter) | WPF-target-specific glue |
| `Type.uno.cs` | **Editable** | Uno/WinUI bridging — this is where parity-fill members live |

All new members for parity go into `Type.uno.cs` as a partial-class extension of the document type. If a document type doesn't yet have a `.uno.cs` partial, create one. If a member must be added to the upstream `.cs` or `.wpf.cs` (e.g. because partial-class semantics can't express it cleanly), pause and seek explicit approval before making the edit.

This also implies all document types must be declared `partial` in their upstream `.cs` files. They generally already are; if one isn't, that single keyword change is the kind that needs approval.

### Stub vs Implement

For every member we add:

| Class | Behavior | When |
|---|---|---|
| **Implement** | Real working code | The member is used by the `RichTextBlock` render/interaction path or by realistic consumers |
| **Stub** | Sensible default (`0` / `null` / no-op event), never throws | The member is in a non-goal area (TextPointer, AccessKey, KeyTip) but its absence breaks compile |

**Stubs do not throw `NotImplementedException`.** Throwing breaks "drop-in compile" just as badly as a missing member.

## Phase 1 — Make the Tool Authoritative

**Gate before any code change to (3).** Until the tool's reference is real WinUI 3 metadata, every parity number is suspect.

- [ ] Refactor `tools/RichTextBlockCompat` to use `System.Reflection.MetadataLoadContext`.
- [ ] Resolve reference metadata from the **Microsoft.WindowsAppSDK** NuGet package directly (`Microsoft.WinUI.dll`).
- [ ] Resolve subject metadata from `LeXtudio.RichText.dll` + `LeXtudio.Windows.dll` build outputs.
- [ ] Remove any "Uno-stubbed" annotation paths — they're (2)-related and out of scope.
- [ ] Regenerate `COMPAT-REPORT.md`; capture the **true baseline** percentage. The current 53.2% number is against (2); the WinUI 3 number may be different (likely lower, possibly different shape).

## Phase 2 — Decide the Mechanism (and possibly extend the tool)

Before mass member additions, pick the mechanism explicitly.

- **Default:** real members on our types. Simpler, no tool change required, no consumer-import friction.
- **Only if we are forced to use C# 14 extension members:** extend `tools/RichTextBlockCompat` to additionally scan source files for `extension(LocalType)` blocks via Roslyn and merge those members into the local surface before comparison.

Document the choice in `DESIGN.md` so future contributors don't drift.

## Phase 3 — Diagnose the Real Gap

Once Phase 1 lands a real baseline:

- [ ] Recompute the "Gap by Member Kind" table. The earlier diagnosis (Properties dominate) was correct at the level of *what* is missing but was measured against the wrong reference. Re-confirm.
- [ ] Cluster the gap. Likely clusters based on WinUI 3's shape:
  - `TextElement`-level surface (AccessKey, BaseLineAlignment, CharacterSpacing, FontStretch, Language, TextDecorations, KeyTip*) → 30–40 members repeated across every document type
  - `Hyperlink`-specific (NavigateUri, UnderlineStyle, Click, RequestNavigate) → ~30 members
  - `RichTextBlock`-specific (MaxLines, OverflowContentTarget, IsColorFontEnabled, SelectionChanged, etc.) → ~50 members
  - TextPointer-dependent members (`ContentStart`, `ContentEnd`, `BaselineOffset`, `GetPositionFromPoint`) → stub category

## Phase 4 — Close the Document-Type Surface

For every member on WinUI 3's `TextElement`, `Block`, `Inline`, `Paragraph`, `Run`, `Span`, `Bold`, `Italic`, `Underline`, `Hyperlink`, `LineBreak`, `InlineUIContainer`:

- [ ] **All edits land in `Type.uno.cs`** (the Uno partial). The upstream `Type.cs` and `Type.wpf.cs` stay read-only unless explicitly approved. Create `Type.uno.cs` if it doesn't exist.
- [ ] Declare the member on the local partial class (or on a shared base if the same member appears across many types).
- [ ] If it's a `DependencyProperty` getter (e.g. `static DependencyProperty FontSizeProperty { get; }`), register a real DP on our type with the correct name/type/metadata.
- [ ] If it's a regular property with `get`/`set`, wire it to a DP, or to plain field storage if it's not bindable in WinUI 3.
- [ ] If it's an event, store handlers in `ConditionalWeakTable` keyed by instance; raise from the relevant interaction path if `Implement`, else leave dead if `Stub`.
- [ ] If it's a method, implement or stub per the table above.

Re-run the compat tool after each cluster (TextElement surface, then Inline surface, etc.).

## Phase 5 — Close the RichTextBlock-specific Surface

Same mechanics, applied to members declared on `Microsoft.UI.Xaml.Controls.RichTextBlock` directly. The control already inherits the `FrameworkElement` surface, so the gap is the RichTextBlock-specific properties/events/methods.

## Phase 6 — Resolve Signature Mismatches

Any `:warning: Mismatch` rows in the report are stronger violations than missing members — same name + same kind but wrong shape. For each: change the local signature to match WinUI 3. Divergence here is almost always a bug we'd want to fix anyway.

## Phase 7 — Long Tail and Gate

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

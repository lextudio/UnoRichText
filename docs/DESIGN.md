# LeXtudio.RichText — Design Document

## Problem

Uno Platform does not fully implement the WinUI/WPF document stack needed by native markdown rendering, especially around `RichTextBlock` and embedded inline elements. This causes fidelity and interaction gaps (layout, inline UI, text selection consistency).

## Strategic Direction (New)

LeXtudio.RichText should be **WPF-source-first**:

1. Reuse Microsoft open-source WPF document model code from  
   `C:\Users\lextudio\source\repos\uno-tools\wpf\src\Microsoft.DotNet.Wpf\src\PresentationFramework\System\Windows\Documents`.
2. Keep Microsoft logic in shared `.cs` files whenever possible.
3. Isolate platform-specific parts into partial files:
   - `*.wpf.cs` for WPF-specific glue.
   - `*.uno.cs` for Uno-specific glue.
4. Avoid re-inventing document semantics when upstream Microsoft behavior already exists.

This is the default approach unless a type is tightly coupled to WPF internals that cannot be ported safely.

## Why This Approach Is Right

1. Behavior parity: We align with mature Microsoft semantics for document object model behavior.
2. Lower drift: We reduce long-term divergence between our model and upstream expectations.
3. Faster maintenance: Bug fixes become targeted adaptation fixes instead of full reimplementation.
4. Better confidence: More predictable interoperability with markdown pipelines that assume WinUI/WPF-like behavior.

## Scope Split

### Reuse-first layer (`Documents`)

`Inline`, `Run`, `Span`, `Bold`, `Italic`, `LineBreak`, `Hyperlink`, collections, and related document tree behavior should prefer shared Microsoft-derived logic.

### Uno-only layer (`Controls`)

`RichTextBlock` remains Uno-specific for rendering and interaction because it depends on:
- PretextSharp line layout and measurement.
- Uno visual tree and pointer behavior.
- Existing LeXtudio selection rendering pipeline.

The control can still consume reused document-model types.

## File Strategy

For each candidate type:

1. Create a shared core file, example: `Span.cs`.
2. Move platform-dependent code to partials:
   - `Span.wpf.cs`
   - `Span.uno.cs`
3. Keep shared file free of WPF-only APIs where possible.
4. Keep Uno-specific API bridging in `.uno.cs` only.

### Naming Rules

- Shared logic file: `TypeName.cs`
- WPF adaptation: `TypeName.wpf.cs`
- Uno adaptation: `TypeName.uno.cs`
- Internal helper split uses same suffix rule.

## Project Configuration Rules

1. UnoRichText project compiles:
   - shared files (`*.cs`)
   - Uno partials (`*.uno.cs`)
2. WPF partials (`*.wpf.cs`) stay excluded from Uno build.
3. WPF upstream files are added as **linked non-compiling artifacts** under project logical path `UpstreamWpf/Documents/*`.
4. Linked upstream files are source-of-truth for audits and diffs; Uno compile path remains local shared + `.uno.cs` partials.
5. If we vendor Microsoft source files directly, keep original headers and clear provenance comments.
6. Prefer linked/shared files over copy-paste to reduce drift.

## Linked-File Policy

1. For each mapped type, maintain a linked upstream file in project view (`UpstreamWpf/Documents/*`).
2. Do not edit linked upstream files in UnoRichText.
3. Local shared file (`Type.cs`) should remain minimal and platform-neutral.
4. Uno behavior/adaptation must be implemented in `Type.uno.cs`.
5. When upstream changes, update the `wpf` submodule/revision and review diffs against linked files before adapting local partials.

## Provenance and Compliance

Each Microsoft-derived shared file should include:

- Source path in WPF repo.
- Upstream commit hash used for import.
- Notes on local edits (if any).

Maintain a small manifest (future `docs/SOURCE-MAP.md`) listing:
- local file
- upstream file
- import revision
- adaptation status

## Adaptation Guidelines

1. Do not alter upstream logic in shared files unless required.
2. If behavior must differ for Uno, implement it in `.uno.cs`.
3. Keep adaptation seams narrow and explicit.
4. Prefer composition/partial overrides over invasive rewrites.

## Selection and Interaction Model

Current selection behavior in `LeXtudio.UI.Xaml.Controls.RichTextBlock` remains the interaction authority for Uno.

Design constraints:

1. Document model parity comes from reused Microsoft code.
2. Rendering and pointer handling remain Uno control responsibilities.
3. Hyperlink hit testing/cursor/selection behavior should be implemented in Uno control layer, not forced into shared document core.

## Migration Plan

### Phase 1: Baseline inventory

1. Map current `LeXtudio.RichText/Documents/*` files to WPF counterparts.
2. Classify each type:
   - `Direct-share`
   - `Share-with-partials`
   - `Uno-only`

### Phase 2: Shared core extraction

1. Introduce shared core files for highest-value types first:
   - `Inline`
   - `Run`
   - `Span`
   - `Hyperlink`
   - inline collections
2. Add `.uno.cs` shims for namespace/type-system adaptation.

### Phase 3: Behavior validation

1. Expand tests to verify parity-critical semantics.
2. Keep existing selection diagnostics and markdown rendering checks.
3. Add regressions for hyperlink and list-item interaction.

### Phase 4: Ongoing sync

1. Periodically sync upstream WPF changes for mapped files.
2. Update provenance manifest.
3. Keep local divergence documented and minimal.

### Phase 5: Full Namespace Onboarding (223 files)

This is the execution path to bring the entire `System.Windows.Documents` namespace under the same linked/split policy without destabilizing Uno builds.

1. Bootstrap metadata and links:
   - Run `tools/bootstrap-documents-migration.ps1` to:
     - append placeholder backlog rows for all missing document files in `SOURCE-MAP.md`.
     - append placeholder provenance rows in `PROVENANCE.md`.
     - add all missing `UpstreamWpf/Documents/*.cs` links to `LeXtudio.RichText.csproj` as non-compiling `None`.
2. Keep compile scope narrow:
   - Do **not** compile newly linked WPF files directly in Uno.
   - Continue compiling only local shared files + `*.uno.cs` partials.
3. Migrate by dependency rings:
   - Ring A: core DOM/value types (`TextElement`, `Section`, `List`, `ListItem`, `Table*` public shells).
   - Ring B: collections and editing-neutral helpers.
   - Ring C: text editing engine (`TextEditor*`, `TextRange*`, tree internals) only when Uno feature scope needs them.
4. For each migrated type:
   - create `Type.cs` shared shell (Microsoft-aligned).
   - move Uno behavior to `Type.uno.cs`.
   - update row status in `SOURCE-MAP.md` and `PROVENANCE.md`.
5. Gate every slice:
   - `dotnet build src/LeXtudio.RichText/LeXtudio.RichText.csproj -c Debug`
   - `dotnet test src/LeXtudio.RichText.Tests/LeXtudio.RichText.Tests.csproj -c Debug`
   - `pwsh tools/verify-documents-parity.ps1 -Mode core` (must pass)
   - `pwsh tools/verify-documents-parity.ps1 -Mode full` (progressively trends to pass)

## Testing Expectations

1. Keep `LeXtudio.RichText.Tests` green.
2. Add source-parity tests where behavior can be asserted without WPF runtime.
3. Keep sample diagnostic mode (`--diag`) as quick integration gate.
4. Add tests for:
   - link cursor/hit behavior
   - list item selection routing
   - code block selection behavior

## Risks and Mitigations

1. Risk: Hidden WPF dependencies in reused files.
   Mitigation: split with partials early; avoid deep transitive imports.
2. Risk: Over-coupling to WPF naming internals.
   Mitigation: adapt only at boundary files and retain clear Uno facades.
3. Risk: Upstream drift.
   Mitigation: provenance manifest + scheduled sync checks.

## Decision

Yes, your proposed approach is the right one for this repo:

- Reuse original Microsoft files where feasible.
- Split into shared `.cs` plus platform partials (`.wpf.cs`, `.uno.cs`) when needed.
- Keep Uno-specific rendering/interaction in Uno control files.

This gives us better parity and better long-term maintainability than continued full custom reimplementation.

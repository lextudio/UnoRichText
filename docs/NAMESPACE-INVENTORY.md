# System.Windows.Documents Inventory — Archived

> **This document is no longer maintained.** It enumerated all ~223 files in the upstream WPF `System.Windows.Documents` namespace as input to a full-namespace port plan that has been abandoned. See [DESIGN.md](DESIGN.md) for the current scope.

## Why archived

The full-namespace port is not happening. UnoRichText is scoped to a single goal: a WinUI `RichTextBlock`-compatible control for Uno. Most WPF document types — `TextPointer`, `TextRange`, `FlowDocument` infrastructure, editing engine, table rendering — have no WinUI counterpart and are not needed for that goal.

For the WPF→WinUI gap analysis methodology (so we don't repeat the mistake), see [ext/shims/docs/ARCHITECTURE.md](../../ext/shims/docs/ARCHITECTURE.md) → "Pre-Porting Checklist" and "Known WPF→WinUI Gaps".

# Session 22 — NullTextContainer + NullTextNavigator + TextTreeDumper

Status: complete. Build clean, 62-test baseline intact (51 passed, 11 skipped).

## Goal

Enable three self-contained utility files: the null text container/pointer
implementation and the debug tree dumper.

## What this gets us

- `NullTextContainer` and the `NullTextPointer` class inside `NullTextNavigator.cs`
  provide the "empty container" implementation used when no document is loaded —
  these are the fallback pointers returned by TextEditor in a null-document state.
- `TextTreeDumper` provides debug/diagnostics support for the text tree structure.

## Changes

### LeXtudio.Windows.csproj
- Removed `<Compile Remove>` for `NullTextNavigator.cs`, `NullTextContainer.cs`,
  and `TextTreeDumper.cs`.

### WinUIDependencyPropertyExtensions.cs
- Added `DefaultMetadata` extension property on `DependencyProperty` — returns
  `GetMetadata(typeof(object))` or a new `PropertyMetadata(null)` fallback.
  Used by `NullTextPointer.GetValue(DependencyProperty)` to retrieve the default
  value for a property in the null-document context.

### Upstream patches (ext/wpf)

**NullTextNavigator.cs**
- `GetLocalValueEnumerator()` (line 153): replaced `new DependencyObject()` with
  `#if HAS_UNO new FormattingDependencyObject() #else new DependencyObject() #endif`
  — `DependencyObject` is abstract in Uno; `FormattingDependencyObject` is the
  concrete shim subclass used throughout the port.
- `ParentType` property (line 412): gated `typeof(FixedDocument)` with
  `#if HAS_UNO typeof(object) #else typeof(FixedDocument) #endif`
  — `FixedDocument` is not available on HAS_UNO (Fixed-document layout excluded).

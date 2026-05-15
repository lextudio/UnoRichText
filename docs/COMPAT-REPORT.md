# RichTextBlock API Parity with WinUI 3

**Coverage: 84.5%** (990 / 1171 WinUI 3 members)

**Goal:** 100% API parity with WinUI 3.

- **Reference:** Microsoft.WinUI from the Windows App SDK (3.0.0.0+f6b3531dce4818fb5bf5130a6cd96bca2d3de762).
- **Subject:** the LeXtudio types listed below — the union of `LeXtudio.UI.Xaml.Controls.RichTextBlock` and the `System.Windows.Documents.*` types in `LeXtudio.Windows`.

## Per-Type Summary

| Type | Coverage | Matched | Mismatched | Missing |
|---|---:|---:|---:|---:|
| `RichTextBlock` | 95.7% | 356 | 3 | 13 |
| `Block` | 82.7% | 62 | 2 | 11 |
| `Paragraph` | 79.0% | 64 | 4 | 13 |
| `Inline` | 87.5% | 56 | 3 | 5 |
| `Run` | 82.9% | 58 | 4 | 8 |
| `Span` | 83.6% | 56 | 5 | 6 |
| `Bold` | 81.2% | 56 | 6 | 7 |
| `Italic` | 81.2% | 56 | 6 | 7 |
| `Underline` | 81.2% | 56 | 6 | 7 |
| `Hyperlink` | 55.9% | 57 | 8 | 37 |
| `LineBreak` | 84.8% | 56 | 4 | 6 |
| `InlineUIContainer` | 85.1% | 57 | 4 | 6 |

## Gap by Member Kind

Where the work is concentrated. Each cell counts WinUI 3 members of that kind that are Missing or Mismatched on the subject side.

| Type | Property | Method | Event | DP Field |
|---|---:|---:|---:|---:|
| `RichTextBlock` | 1 | 15 | 0 | 0 |
| `Block` | 6 | 7 | 0 | 0 |
| `Paragraph` | 7 | 10 | 0 | 0 |
| `Inline` | 2 | 6 | 0 | 0 |
| `Run` | 3 | 9 | 0 | 0 |
| `Span` | 3 | 8 | 0 | 0 |
| `Bold` | 3 | 10 | 0 | 0 |
| `Italic` | 3 | 10 | 0 | 0 |
| `Underline` | 3 | 10 | 0 | 0 |
| `Hyperlink` | 30 | 12 | 3 | 0 |
| `LineBreak` | 2 | 8 | 0 | 0 |
| `InlineUIContainer` | 2 | 8 | 0 | 0 |

## RichTextBlock

- **WinUI 3:** `Microsoft.UI.Xaml.Controls.RichTextBlock`
- **Subject:** `LeXtudio.UI.Xaml.Controls.RichTextBlock`
- **Coverage:** 95.7% (356/372 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FocusState FocusState { get; }` | `FocusState FocusState { get; set; }` |
| :warning: Mismatch | Method | `BindingExpression GetBindingExpression(DependencyProperty dp)` | `BindingExpression GetBindingExpression(DependencyProperty dependencyProperty)` |
| :warning: Mismatch | Method | `Void SetBinding(DependencyProperty dp, BindingBase binding)` | `Void SetBinding(Object target, String dependencyProperty, BindingBase binding)` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(RichTextBlock other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(FrameworkElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(UIElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `RichTextBlock FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `FrameworkElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `UIElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |

## Block

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Block`
- **Subject:** `System.Windows.Documents.Block`
- **Coverage:** 82.7% (62/75 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Method | `Boolean Equals(Block other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty LineHeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineStackingStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty MarginProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `TextDecorations TextDecorations { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Block FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Paragraph

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Paragraph`
- **Subject:** `System.Windows.Documents.Paragraph`
- **Coverage:** 79.0% (64/81 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Paragraph other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Block other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty LineHeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineStackingStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty MarginProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextIndentProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Paragraph FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Block FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Inline

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Inline`
- **Subject:** `System.Windows.Documents.Inline`
- **Coverage:** 87.5% (56/64 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Run

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Run`
- **Subject:** `System.Windows.Documents.Run`
- **Coverage:** 82.9% (58/70 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Run other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty FlowDirectionProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Run FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Span

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Span`
- **Subject:** `System.Windows.Documents.Span`
- **Coverage:** 83.6% (56/67 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Span other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Bold

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Bold`
- **Subject:** `System.Windows.Documents.Bold`
- **Coverage:** 81.2% (56/69 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Bold other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Span other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Bold FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Italic

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Italic`
- **Subject:** `System.Windows.Documents.Italic`
- **Coverage:** 81.2% (56/69 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Italic other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Span other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Italic FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Underline

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Underline`
- **Subject:** `System.Windows.Documents.Underline`
- **Coverage:** 81.2% (56/69 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Underline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Span other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Underline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## Hyperlink

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Hyperlink`
- **Subject:** `System.Windows.Documents.Hyperlink`
- **Coverage:** 55.9% (57/102 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(Hyperlink other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Span other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Focus(FocusState value)` | `Boolean Focus()` |
| :warning: Mismatch | Event | `event TypedEventHandler<Hyperlink, HyperlinkClickEventArgs> Click` | `event RoutedEventHandler Click` |
| :x: Missing | Property | `ElementSoundMode ElementSoundMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ElementSoundModeProperty { get; }` | `—` |
| :x: Missing | Property | `FocusState FocusState { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FocusStateProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTabStop { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTabStopProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty NavigateUriProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 TabIndex { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TabIndexProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `UnderlineStyle UnderlineStyle { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty UnderlineStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject XYFocusDown { get; set; }` | `—` |
| :x: Missing | Property | `XYFocusNavigationStrategy XYFocusDownNavigationStrategy { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusDownNavigationStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusDownProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject XYFocusLeft { get; set; }` | `—` |
| :x: Missing | Property | `XYFocusNavigationStrategy XYFocusLeftNavigationStrategy { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusLeftNavigationStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusLeftProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject XYFocusRight { get; set; }` | `—` |
| :x: Missing | Property | `XYFocusNavigationStrategy XYFocusRightNavigationStrategy { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusRightNavigationStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusRightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject XYFocusUp { get; set; }` | `—` |
| :x: Missing | Property | `XYFocusNavigationStrategy XYFocusUpNavigationStrategy { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusUpNavigationStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty XYFocusUpProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Hyperlink FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Event | `event RoutedEventHandler GotFocus` | `—` |
| :x: Missing | Event | `event RoutedEventHandler LostFocus` | `—` |

## LineBreak

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.LineBreak`
- **Subject:** `System.Windows.Documents.LineBreak`
- **Coverage:** 84.8% (56/66 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(LineBreak other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `LineBreak FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

## InlineUIContainer

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.InlineUIContainer`
- **Subject:** `System.Windows.Documents.InlineUIContainer`
- **Coverage:** 85.1% (57/67 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Equals(InlineUIContainer other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(Inline other)` | `Boolean Equals(TextElement other)` |
| :warning: Mismatch | Method | `Boolean Equals(DependencyObject other)` | `Boolean Equals(TextElement other)` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `InlineUIContainer FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |

---
_Generated by `tools/RichTextBlockCompat`. Reference is real WinUI 3 metadata (Microsoft.WinUI.dll from Windows App SDK), loaded via System.Reflection.MetadataLoadContext. The check is unidirectional (WinUI → Subject); extra members on the subject side do not count as incompatibilities._

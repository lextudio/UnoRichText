# RichTextBlock API Parity with WinUI 3

**Coverage: 39.7%** (465 / 1171 WinUI 3 members)

**Goal:** 100% API parity with WinUI 3.

- **Reference:** Microsoft.WinUI from the Windows App SDK (3.0.0.0+f6b3531dce4818fb5bf5130a6cd96bca2d3de762).
- **Subject:** the LeXtudio types listed below — the union of `LeXtudio.UI.Xaml.Controls.RichTextBlock` and the `System.Windows.Documents.*` types in `LeXtudio.Windows`.

## Per-Type Summary

| Type | Coverage | Matched | Mismatched | Missing |
|---|---:|---:|---:|---:|
| `RichTextBlock` | 79.3% | 295 | 3 | 74 |
| `Block` | 25.3% | 19 | 0 | 56 |
| `Paragraph` | 25.9% | 21 | 1 | 59 |
| `Inline` | 21.9% | 14 | 3 | 47 |
| `Run` | 22.9% | 16 | 3 | 51 |
| `Span` | 20.9% | 14 | 4 | 49 |
| `Bold` | 20.3% | 14 | 4 | 51 |
| `Italic` | 20.3% | 14 | 4 | 51 |
| `Underline` | 20.3% | 14 | 4 | 51 |
| `Hyperlink` | 14.7% | 15 | 6 | 81 |
| `LineBreak` | 21.2% | 14 | 3 | 49 |
| `InlineUIContainer` | 22.4% | 15 | 3 | 49 |

## Gap by Member Kind

Where the work is concentrated. Each cell counts WinUI 3 members of that kind that are Missing or Mismatched on the subject side.

| Type | Property | Method | Event | DP Field |
|---|---:|---:|---:|---:|
| `RichTextBlock` | 56 | 18 | 3 | 0 |
| `Block` | 42 | 11 | 3 | 0 |
| `Paragraph` | 43 | 14 | 3 | 0 |
| `Inline` | 37 | 10 | 3 | 0 |
| `Run` | 38 | 13 | 3 | 0 |
| `Span` | 38 | 12 | 3 | 0 |
| `Bold` | 38 | 14 | 3 | 0 |
| `Italic` | 38 | 14 | 3 | 0 |
| `Underline` | 38 | 14 | 3 | 0 |
| `Hyperlink` | 65 | 16 | 6 | 0 |
| `LineBreak` | 37 | 12 | 3 | 0 |
| `InlineUIContainer` | 37 | 12 | 3 | 0 |

## RichTextBlock

- **WinUI 3:** `Microsoft.UI.Xaml.Controls.RichTextBlock`
- **Subject:** `LeXtudio.UI.Xaml.Controls.RichTextBlock`
- **Coverage:** 79.3% (295/372 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FocusState FocusState { get; }` | `FocusState FocusState { get; set; }` |
| :warning: Mismatch | Method | `BindingExpression GetBindingExpression(DependencyProperty dp)` | `BindingExpression GetBindingExpression(DependencyProperty dependencyProperty)` |
| :warning: Mismatch | Method | `Void SetBinding(DependencyProperty dp, BindingBase binding)` | `Void SetBinding(Object target, String dependencyProperty, BindingBase binding)` |
| :x: Missing | Property | `Double BaselineOffset { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ContentEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ContentStart { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean HasOverflowContent { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty HasOverflowContentProperty { get; }` | `—` |
| :x: Missing | Property | `TextAlignment HorizontalTextAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty HorizontalTextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsColorFontEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsColorFontEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextSelectionEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextTrimmed { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextTrimmedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineHeightProperty { get; }` | `—` |
| :x: Missing | Property | `LineStackingStrategy LineStackingStrategy { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineStackingStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 MaxLines { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty MaxLinesProperty { get; }` | `—` |
| :x: Missing | Property | `OpticalMarginAlignment OpticalMarginAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty OpticalMarginAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `RichTextBlockOverflow OverflowContentTarget { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty OverflowContentTargetProperty { get; }` | `—` |
| :x: Missing | Property | `Thickness Padding { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty PaddingProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty SelectedTextProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer SelectionEnd { get; }` | `—` |
| :x: Missing | Property | `FlyoutBase SelectionFlyout { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty SelectionFlyoutProperty { get; }` | `—` |
| :x: Missing | Property | `SolidColorBrush SelectionHighlightColor { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty SelectionHighlightColorProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer SelectionStart { get; }` | `—` |
| :x: Missing | Property | `TextAlignment TextAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `TextDecorations TextDecorations { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `IList<TextHighlighter> TextHighlighters { get; }` | `—` |
| :x: Missing | Property | `Double TextIndent { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextIndentProperty { get; }` | `—` |
| :x: Missing | Property | `TextLineBounds TextLineBounds { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextLineBoundsProperty { get; }` | `—` |
| :x: Missing | Property | `TextReadingOrder TextReadingOrder { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextReadingOrderProperty { get; }` | `—` |
| :x: Missing | Property | `TextTrimming TextTrimming { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextTrimmingProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextWrappingProperty { get; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Void CopySelectionToClipboard()` | `—` |
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
| :x: Missing | Method | `TextPointer GetPositionFromPoint(Point point)` | `—` |
| :x: Missing | Method | `Void Select(TextPointer start, TextPointer end)` | `—` |
| :x: Missing | Event | `event ContextMenuOpeningEventHandler ContextMenuOpening` | `—` |
| :x: Missing | Event | `event TypedEventHandler<RichTextBlock, IsTextTrimmedChangedEventArgs> IsTextTrimmedChanged` | `—` |
| :x: Missing | Event | `event RoutedEventHandler SelectionChanged` | `—` |

## Block

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Block`
- **Subject:** `System.Windows.Documents.Block`
- **Coverage:** 25.3% (19/75 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `FontStyle FontStyle { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `TextAlignment HorizontalTextAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty HorizontalTextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineHeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineStackingStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty MarginProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `TextDecorations TextDecorations { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Block other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Block FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Paragraph

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Paragraph`
- **Subject:** `System.Windows.Documents.Paragraph`
- **Coverage:** 25.9% (21/81 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `FontStyle FontStyle { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `TextAlignment HorizontalTextAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty HorizontalTextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineHeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty LineStackingStrategyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty MarginProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextIndentProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Paragraph other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Block other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Paragraph FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Block FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Inline

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Inline`
- **Subject:** `System.Windows.Documents.Inline`
- **Coverage:** 21.9% (14/64 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Run

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Run`
- **Subject:** `System.Windows.Documents.Run`
- **Coverage:** 22.9% (16/70 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FlowDirectionProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Run other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Run FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Span

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Span`
- **Subject:** `System.Windows.Documents.Span`
- **Coverage:** 20.9% (14/67 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Span other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Bold

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Bold`
- **Subject:** `System.Windows.Documents.Bold`
- **Coverage:** 20.3% (14/69 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Bold other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Span other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Bold FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Italic

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Italic`
- **Subject:** `System.Windows.Documents.Italic`
- **Coverage:** 20.3% (14/69 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Italic other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Span other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Italic FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Underline

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Underline`
- **Subject:** `System.Windows.Documents.Underline`
- **Coverage:** 20.3% (14/69 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(Underline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Span other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Underline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Hyperlink

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.Hyperlink`
- **Subject:** `System.Windows.Documents.Hyperlink`
- **Coverage:** 14.7% (15/102 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `InlineCollection Inlines { get; set; }` | `InlineCollection Inlines { get; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :warning: Mismatch | Method | `Boolean Focus(FocusState value)` | `Boolean Focus()` |
| :warning: Mismatch | Event | `event TypedEventHandler<Hyperlink, HyperlinkClickEventArgs> Click` | `event RoutedEventHandler Click` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `ElementSoundMode ElementSoundMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ElementSoundModeProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `FocusState FocusState { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FocusStateProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTabStop { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTabStopProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty NavigateUriProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 TabIndex { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TabIndexProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `UnderlineStyle UnderlineStyle { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty UnderlineStyleProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
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
| :x: Missing | Method | `Boolean Equals(Hyperlink other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Span other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Hyperlink FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Span FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |
| :x: Missing | Event | `event RoutedEventHandler GotFocus` | `—` |
| :x: Missing | Event | `event RoutedEventHandler LostFocus` | `—` |

## LineBreak

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.LineBreak`
- **Subject:** `System.Windows.Documents.LineBreak`
- **Coverage:** 21.2% (14/66 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(LineBreak other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `LineBreak FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## InlineUIContainer

- **WinUI 3:** `Microsoft.UI.Xaml.Documents.InlineUIContainer`
- **Subject:** `System.Windows.Documents.InlineUIContainer`
- **Coverage:** 22.4% (15/67 WinUI 3 members)

### Gaps

| Status | Kind | WinUI 3 signature | Subject signature |
|---|---|---|---|
| :warning: Mismatch | Property | `FontStyle FontStyle { get; set; }` | `Nullable<FontStyle> FontStyle { get; set; }` |
| :warning: Mismatch | Property | `FontWeight FontWeight { get; set; }` | `Nullable<FontWeight> FontWeight { get; set; }` |
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontFamilyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontSizeProperty { get; }` | `—` |
| :x: Missing | Property | `FontStretch FontStretch { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStretchProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontStyleProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty FontWeightProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty ForegroundProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsAccessKeyScope { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsAccessKeyScopeProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean IsTextScaleFactorEnabled { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty IsTextScaleFactorEnabledProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipHorizontalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipHorizontalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `KeyTipPlacementMode KeyTipPlacementMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipPlacementModeProperty { get; }` | `—` |
| :x: Missing | Property | `Double KeyTipVerticalOffset { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty KeyTipVerticalOffsetProperty { get; }` | `—` |
| :x: Missing | Property | `String Language { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty LanguageProperty { get; }` | `—` |
| :x: Missing | Property | `String Name { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `I As()` | `—` |
| :x: Missing | Method | `Boolean Equals(InlineUIContainer other)` | `—` |
| :x: Missing | Method | `Boolean Equals(Object obj)` | `—` |
| :x: Missing | Method | `Boolean Equals(Inline other)` | `—` |
| :x: Missing | Method | `Boolean Equals(TextElement other)` | `—` |
| :x: Missing | Method | `Boolean Equals(DependencyObject other)` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `InlineUIContainer FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Inline FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `TextElement FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `DependencyObject FromAbi(IntPtr thisPtr)` | `—` |
| :x: Missing | Method | `Int32 GetHashCode()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

---
_Generated by `tools/RichTextBlockCompat`. Reference is real WinUI 3 metadata (Microsoft.WinUI.dll from Windows App SDK), loaded via System.Reflection.MetadataLoadContext. The check is unidirectional (WinUI → Subject); extra members on the subject side do not count as incompatibilities._

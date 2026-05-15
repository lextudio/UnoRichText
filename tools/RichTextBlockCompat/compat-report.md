# RichTextBlock API Surface Compatibility

**Overall: 53.2%** (674 / 1268 WinUI members covered)

## Per-Type Summary

| Type | Coverage | Matched | Mismatched | Missing |
|---|---:|---:|---:|---:|
| `RichTextBlock` | 84.3% | 328 | 0 | 61 |
| `Block` | 41.7% | 35 | 0 | 49 |
| `Paragraph` | 42.5% | 37 | 1 | 49 |
| `Inline` | 40.5% | 30 | 3 | 41 |
| `Run` | 40.5% | 32 | 3 | 44 |
| `Span` | 40.0% | 30 | 4 | 41 |
| `Bold` | 40.0% | 30 | 4 | 41 |
| `Italic` | 40.0% | 30 | 4 | 41 |
| `Underline` | 40.0% | 30 | 4 | 41 |
| `Hyperlink` | 29.2% | 31 | 6 | 69 |
| `LineBreak` | 40.5% | 30 | 3 | 41 |
| `InlineUIContainer` | 41.3% | 31 | 3 | 41 |

## RichTextBlock

- **WinUI**: `Microsoft.UI.Xaml.Controls.RichTextBlock`
- **Local**: `LeXtudio.UI.Xaml.Controls.RichTextBlock`
- **Coverage**: 84.3% (328/389)

### Gaps

| Status | Kind | WinUI signature | Local signature |
|---|---|---|---|
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
| :x: Missing | Method | `Void CopySelectionToClipboard()` | `—` |
| :x: Missing | Method | `TextPointer GetPositionFromPoint(Point point)` | `—` |
| :x: Missing | Method | `Void Select(TextPointer start, TextPointer end)` | `—` |
| :x: Missing | Event | `event ContextMenuOpeningEventHandler ContextMenuOpening` | `—` |
| :x: Missing | Event | `event TypedEventHandler<RichTextBlock, IsTextTrimmedChangedEventArgs> IsTextTrimmedChanged` | `—` |
| :x: Missing | Event | `event RoutedEventHandler SelectionChanged` | `—` |

## Block

- **WinUI**: `Microsoft.UI.Xaml.Documents.Block`
- **Local**: `System.Windows.Documents.Block`
- **Coverage**: 41.7% (35/84)

### Gaps

| Status | Kind | WinUI signature | Local signature |
|---|---|---|---|
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `TextDecorations TextDecorations { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Paragraph

- **WinUI**: `Microsoft.UI.Xaml.Documents.Paragraph`
- **Local**: `System.Windows.Documents.Paragraph`
- **Coverage**: 42.5% (37/87)

### Gaps

| Status | Kind | WinUI signature | Local signature |
|---|---|---|---|
| :warning: Mismatch | Property | `TextDecorations TextDecorations { get; set; }` | `TextDecorationCollection TextDecorations { get; set; }` |
| :x: Missing | Property | `String AccessKey { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyObject AccessKeyScopeOwner { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AccessKeyScopeOwnerProperty { get; }` | `—` |
| :x: Missing | Property | `Boolean AllowFocusOnInteraction { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty AllowFocusOnInteractionProperty { get; }` | `—` |
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextIndentProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Inline

- **WinUI**: `Microsoft.UI.Xaml.Documents.Inline`
- **Local**: `System.Windows.Documents.Inline`
- **Coverage**: 40.5% (30/74)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Run

- **WinUI**: `Microsoft.UI.Xaml.Documents.Run`
- **Local**: `System.Windows.Documents.Run`
- **Coverage**: 40.5% (32/79)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnTextChanged()` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Span

- **WinUI**: `Microsoft.UI.Xaml.Documents.Span`
- **Local**: `System.Windows.Documents.Span`
- **Coverage**: 40.0% (30/75)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Bold

- **WinUI**: `Microsoft.UI.Xaml.Documents.Bold`
- **Local**: `System.Windows.Documents.Bold`
- **Coverage**: 40.0% (30/75)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Italic

- **WinUI**: `Microsoft.UI.Xaml.Documents.Italic`
- **Local**: `System.Windows.Documents.Italic`
- **Coverage**: 40.0% (30/75)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Underline

- **WinUI**: `Microsoft.UI.Xaml.Documents.Underline`
- **Local**: `System.Windows.Documents.Underline`
- **Coverage**: 40.0% (30/75)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## Hyperlink

- **WinUI**: `Microsoft.UI.Xaml.Documents.Hyperlink`
- **Local**: `System.Windows.Documents.Hyperlink`
- **Coverage**: 29.2% (31/106)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 CharacterSpacing { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty CharacterSpacingProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementEnd { get; }` | `—` |
| :x: Missing | Property | `ElementSoundMode ElementSoundMode { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ElementSoundModeProperty { get; }` | `—` |
| :x: Missing | Property | `TextPointer ElementStart { get; }` | `—` |
| :x: Missing | Property | `Boolean ExitDisplayModeOnAccessKeyInvoked { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty ExitDisplayModeOnAccessKeyInvokedProperty { get; }` | `—` |
| :x: Missing | Property | `FocusState FocusState { get; set; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty NavigateUriProperty { get; }` | `—` |
| :x: Missing | Property | `Int32 TabIndex { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TabIndexProperty { get; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `UnderlineStyle UnderlineStyle { get; set; }` | `—` |
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
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |
| :x: Missing | Event | `event RoutedEventHandler GotFocus` | `—` |
| :x: Missing | Event | `event RoutedEventHandler LostFocus` | `—` |

## LineBreak

- **WinUI**: `Microsoft.UI.Xaml.Documents.LineBreak`
- **Local**: `System.Windows.Documents.LineBreak`
- **Coverage**: 40.5% (30/74)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

## InlineUIContainer

- **WinUI**: `Microsoft.UI.Xaml.Documents.InlineUIContainer`
- **Local**: `System.Windows.Documents.InlineUIContainer`
- **Coverage**: 41.3% (31/75)

### Gaps

| Status | Kind | WinUI signature | Local signature |
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
| :x: Missing | Property | `BaseLineAlignment BaseLineAlignment { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty BaseLineAlignmentProperty { get; }` | `—` |
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
| :x: Missing | Property | `String Name { get; set; }` | `—` |
| :x: Missing | Property | `DependencyProperty TextDecorationsProperty { get; }` | `—` |
| :x: Missing | Property | `XamlRoot XamlRoot { get; set; }` | `—` |
| :x: Missing | Method | `Object FindName(String name)` | `—` |
| :x: Missing | Method | `Void OnThemeChanged()` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayDismissedEventArgs> AccessKeyDisplayDismissed` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyDisplayRequestedEventArgs> AccessKeyDisplayRequested` | `—` |
| :x: Missing | Event | `event TypedEventHandler<TextElement, AccessKeyInvokedEventArgs> AccessKeyInvoked` | `—` |

---
_Generated by `tools/RichTextBlockCompat`. The check is unidirectional (WinUI → Local); extra members on the local side do not count as incompatibilities._

# UnoRichText Provenance Log

This document tracks provenance for Microsoft-derived or Microsoft-aligned sources in UnoRichText.

## Upstream Reference

- Upstream repository: `wpf`
- Upstream root: `src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Documents`
- Local checkout: `C:\Users\lextudio\source\repos\uno-tools\wpf`

## Status Keys

- `Planned`: mapped but not yet imported/synced from upstream.
- `Captured`: upstream file path and commit hash captured.
- `Imported`: shared core file content imported/adapted from upstream.
- `Diverged`: local changes differ materially from upstream and are documented.

## Entries

| Local Type | Local Files | Upstream File | Upstream Commit | Status | Notes |
|---|---|---|---|---|---|
| `Inline` | `src/LeXtudio.RichText/Documents/Inline.cs`, `Inline.uno.cs` | `System/Windows/Documents/Inline.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `Run` | `src/LeXtudio.RichText/Documents/Run.cs`, `Run.uno.cs` | `System/Windows/Documents/Run.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `Span` | `src/LeXtudio.RichText/Documents/Span.cs`, `Span.uno.cs` | `System/Windows/Documents/Span.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Pilot split complete; next import candidate. |
| `Bold` | `src/LeXtudio.RichText/Documents/Bold.cs`, `Bold.uno.cs` | `System/Windows/Documents/Bold.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `Italic` | `src/LeXtudio.RichText/Documents/Italic.cs`, `Italic.uno.cs` | `System/Windows/Documents/Italic.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `LineBreak` | `src/LeXtudio.RichText/Documents/LineBreak.cs`, `LineBreak.uno.cs` | `System/Windows/Documents/LineBreak.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `Hyperlink` | `src/LeXtudio.RichText/Documents/Hyperlink.cs`, `Hyperlink.uno.cs` | `System/Windows/Documents/Hyperlink.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `InlineCollection` | `src/LeXtudio.RichText/Documents/InlineCollection.cs`, `InlineCollection.uno.cs` | `System/Windows/Documents/InlineCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `InlineUIContainer` | `src/LeXtudio.RichText/Documents/InlineUIContainer.cs`, `InlineUIContainer.uno.cs` | `System/Windows/Documents/InlineUIContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `Block` | `src/LeXtudio.RichText/Documents/Block.cs`, `Block.uno.cs` | `System/Windows/Documents/Block.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `Paragraph` | `src/LeXtudio.RichText/Documents/Paragraph.cs`, `Paragraph.uno.cs` | `System/Windows/Documents/Paragraph.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |
| `BlockCollection` | `src/LeXtudio.RichText/Documents/BlockCollection.cs`, `BlockCollection.uno.cs` | `System/Windows/Documents/BlockCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Captured | Upstream file presence verified in local `wpf` checkout. |

## Capture Procedure

1. Locate upstream source file in local `wpf` checkout.
2. Record current commit hash from upstream checkout.
3. Update row status to `Captured`.
4. If shared core content is imported from upstream, update status to `Imported` and note adaptation details.

## Backlog (Auto-generated)
| `Adorner` | _TBD_ | `System/Windows/Documents/Adorner.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `AdornerDecorator` | _TBD_ | `System/Windows/Documents/AdornerDecorator.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `AdornerHitTestResult` | _TBD_ | `System/Windows/Documents/AdornerHitTestResult.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `AdornerLayer` | _TBD_ | `System/Windows/Documents/AdornerLayer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `AnchoredBlock` | _TBD_ | `System/Windows/Documents/AnchoredBlock.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `BlockUIContainer` | _TBD_ | `System/Windows/Documents/BlockUIContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `CaretElement` | _TBD_ | `System/Windows/Documents/CaretElement.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `CaretScrollMethod` | _TBD_ | `System/Windows/Documents/CaretScrollMethod.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ChangeBlockUndoRecord` | _TBD_ | `System/Windows/Documents/ChangeBlockUndoRecord.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ChildDocumentBlock` | _TBD_ | `System/Windows/Documents/ChildDocumentBlock.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ColumnResizeAdorner` | _TBD_ | `System/Windows/Documents/ColumnResizeAdorner.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ColumnResizeUndoUnit` | _TBD_ | `System/Windows/Documents/ColumnResizeUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `CompositionAdorner` | _TBD_ | `System/Windows/Documents/CompositionAdorner.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DisplayAttributeHighlightLayer` | _TBD_ | `System/Windows/Documents/DisplayAttributeHighlightLayer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentReference` | _TBD_ | `System/Windows/Documents/DocumentReference.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentReferenceCollection` | _TBD_ | `System/Windows/Documents/DocumentReferenceCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentSequence` | _TBD_ | `System/Windows/Documents/DocumentSequence.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentSequenceHighlightLayer` | _TBD_ | `System/Windows/Documents/DocumentSequenceHighlightLayer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentSequenceTextContainer` | _TBD_ | `System/Windows/Documents/DocumentSequenceTextContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentSequenceTextPointer` | _TBD_ | `System/Windows/Documents/DocumentSequenceTextPointer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DocumentSequenceTextView` | _TBD_ | `System/Windows/Documents/DocumentSequenceTextView.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `DPTypeDescriptorContext` | _TBD_ | `System/Windows/Documents/DPTypeDescriptorContext.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `EditingCommands` | _TBD_ | `System/Windows/Documents/EditingCommands.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ElementEdge` | _TBD_ | `System/Windows/Documents/ElementEdge.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Figure` | _TBD_ | `System/Windows/Documents/Figure.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedDocument` | _TBD_ | `System/Windows/Documents/FixedDocument.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedDSBuilder` | _TBD_ | `System/Windows/Documents/FixedDSBuilder.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedElement` | _TBD_ | `System/Windows/Documents/FixedElement.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedFindEngine` | _TBD_ | `System/Windows/Documents/FixedFindEngine.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedFlowMap` | _TBD_ | `System/Windows/Documents/FixedFlowMap.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedHighlight` | _TBD_ | `System/Windows/Documents/FixedHighlight.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedHyperlink` | _TBD_ | `System/Windows/Documents/FixedHyperlink.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedLineResult` | _TBD_ | `System/Windows/Documents/FixedLineResult.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedNode` | _TBD_ | `System/Windows/Documents/FixedNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedPage` | _TBD_ | `System/Windows/Documents/FixedPage.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedPageStructure` | _TBD_ | `System/Windows/Documents/FixedPageStructure.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedPosition` | _TBD_ | `System/Windows/Documents/FixedPosition.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSchema` | _TBD_ | `System/Windows/Documents/FixedSchema.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMContainer` | _TBD_ | `System/Windows/Documents/FixedSOMContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMElement` | _TBD_ | `System/Windows/Documents/FixedSOMElement.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMFixedBlock` | _TBD_ | `System/Windows/Documents/FixedSOMFixedBlock.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMGroup` | _TBD_ | `System/Windows/Documents/FixedSOMGroup.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMImage` | _TBD_ | `System/Windows/Documents/FixedSOMImage.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMLineCollection` | _TBD_ | `System/Windows/Documents/FixedSOMLineCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMLineRanges` | _TBD_ | `System/Windows/Documents/FixedSOMLineRanges.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMPage` | _TBD_ | `System/Windows/Documents/FixedSOMPage.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMPageConstructor` | _TBD_ | `System/Windows/Documents/FixedSOMPageConstructor.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMPageElement` | _TBD_ | `System/Windows/Documents/FixedSOMPageElement.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMSemanticBox` | _TBD_ | `System/Windows/Documents/FixedSOMSemanticBox.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMTable` | _TBD_ | `System/Windows/Documents/FixedSOMTable.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMTableCell` | _TBD_ | `System/Windows/Documents/FixedSOMTableCell.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMTableRow` | _TBD_ | `System/Windows/Documents/FixedSOMTableRow.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedSOMTextRun` | _TBD_ | `System/Windows/Documents/FixedSOMTextRun.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedTextBuilder` | _TBD_ | `System/Windows/Documents/FixedTextBuilder.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedTextContainer` | _TBD_ | `System/Windows/Documents/FixedTextContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedTextPointer` | _TBD_ | `System/Windows/Documents/FixedTextPointer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FixedTextView` | _TBD_ | `System/Windows/Documents/FixedTextView.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Floater` | _TBD_ | `System/Windows/Documents/Floater.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FlowDocument` | _TBD_ | `System/Windows/Documents/FlowDocument.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FlowNode` | _TBD_ | `System/Windows/Documents/FlowNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FlowPosition` | _TBD_ | `System/Windows/Documents/FlowPosition.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FrameworkRichTextComposition` | _TBD_ | `System/Windows/Documents/FrameworkRichTextComposition.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `FrameworkTextComposition` | _TBD_ | `System/Windows/Documents/FrameworkTextComposition.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Glyphs` | _TBD_ | `System/Windows/Documents/Glyphs.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `HighlightChangedEventArgs` | _TBD_ | `System/Windows/Documents/HighlightChangedEventArgs.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `HighlightChangedEventHandler` | _TBD_ | `System/Windows/Documents/HighlightChangedEventHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `HighlightLayer` | _TBD_ | `System/Windows/Documents/HighlightLayer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Highlights` | _TBD_ | `System/Windows/Documents/Highlights.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `HighlightVisual` | _TBD_ | `System/Windows/Documents/HighlightVisual.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ImmComposition` | _TBD_ | `System/Windows/Documents/ImmComposition.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `InkInteropObject` | _TBD_ | `System/Windows/Documents/InkInteropObject.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `InputScopeAttribute` | _TBD_ | `System/Windows/Documents/InputScopeAttribute.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ITextContainer` | _TBD_ | `System/Windows/Documents/ITextContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ITextPointer` | _TBD_ | `System/Windows/Documents/ITextPointer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ITextRange` | _TBD_ | `System/Windows/Documents/ITextRange.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ITextSelection` | _TBD_ | `System/Windows/Documents/ITextSelection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ITextView` | _TBD_ | `System/Windows/Documents/ITextView.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `IXamlAttributes` | _TBD_ | `System/Windows/Documents/IXamlAttributes.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `IXamlContentHandler` | _TBD_ | `System/Windows/Documents/IXamlContentHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `IXamlErrorHandler` | _TBD_ | `System/Windows/Documents/IXamlErrorHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `LinkTarget` | _TBD_ | `System/Windows/Documents/LinkTarget.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `List` | _TBD_ | `System/Windows/Documents/List.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ListItem` | _TBD_ | `System/Windows/Documents/ListItem.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ListItemCollection` | _TBD_ | `System/Windows/Documents/ListItemCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `LogicalDirection` | _TBD_ | `System/Windows/Documents/LogicalDirection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `MoveSizeWinEventHandler` | _TBD_ | `System/Windows/Documents/MoveSizeWinEventHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `NaturalLanguageHyphenator` | _TBD_ | `System/Windows/Documents/NaturalLanguageHyphenator.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `NLGSpellerInterop` | _TBD_ | `System/Windows/Documents/NLGSpellerInterop.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `NullTextContainer` | _TBD_ | `System/Windows/Documents/NullTextContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `NullTextNavigator` | _TBD_ | `System/Windows/Documents/NullTextNavigator.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `PageContent` | _TBD_ | `System/Windows/Documents/PageContent.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `PageContentAsyncResult` | _TBD_ | `System/Windows/Documents/PageContentAsyncResult.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `PageContentCollection` | _TBD_ | `System/Windows/Documents/PageContentCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `PrecursorTextChangeType` | _TBD_ | `System/Windows/Documents/PrecursorTextChangeType.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `PropertyRecord` | _TBD_ | `System/Windows/Documents/PropertyRecord.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `PropertyValueAction` | _TBD_ | `System/Windows/Documents/PropertyValueAction.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RangeContentEnumerator` | _TBD_ | `System/Windows/Documents/RangeContentEnumerator.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfControls` | _TBD_ | `System/Windows/Documents/RtfControls.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfControlWord` | _TBD_ | `System/Windows/Documents/RtfControlWord.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfControlWordInfo` | _TBD_ | `System/Windows/Documents/RtfControlWordInfo.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfDestination` | _TBD_ | `System/Windows/Documents/RtfDestination.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfFormatStack` | _TBD_ | `System/Windows/Documents/RtfFormatStack.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfImageFormat` | _TBD_ | `System/Windows/Documents/RtfImageFormat.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfSuperSubscript` | _TBD_ | `System/Windows/Documents/RtfSuperSubscript.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfToken` | _TBD_ | `System/Windows/Documents/RtfToken.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfTokenType` | _TBD_ | `System/Windows/Documents/RtfTokenType.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfToXamlError` | _TBD_ | `System/Windows/Documents/RtfToXamlError.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfToXamlLexer` | _TBD_ | `System/Windows/Documents/RtfToXamlLexer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RtfToXamlReader` | _TBD_ | `System/Windows/Documents/RtfToXamlReader.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `RubberbandSelector` | _TBD_ | `System/Windows/Documents/RubberbandSelector.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Section` | _TBD_ | `System/Windows/Documents/Section.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SelectionHighlightInfo` | _TBD_ | `System/Windows/Documents/SelectionHighlightInfo.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SelectionWordBreaker` | _TBD_ | `System/Windows/Documents/SelectionWordBreaker.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Speller` | _TBD_ | `System/Windows/Documents/Speller.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SpellerError` | _TBD_ | `System/Windows/Documents/SpellerError.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SpellerHighlightLayer` | _TBD_ | `System/Windows/Documents/SpellerHighlightLayer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SpellerInteropBase` | _TBD_ | `System/Windows/Documents/SpellerInteropBase.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SpellerStatusTable` | _TBD_ | `System/Windows/Documents/SpellerStatusTable.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SplayTreeNode` | _TBD_ | `System/Windows/Documents/SplayTreeNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `SplayTreeNodeRole` | _TBD_ | `System/Windows/Documents/SplayTreeNodeRole.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `StaticTextPointer` | _TBD_ | `System/Windows/Documents/StaticTextPointer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Table` | _TBD_ | `System/Windows/Documents/Table.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableCell` | _TBD_ | `System/Windows/Documents/TableCell.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableCellCollection` | _TBD_ | `System/Windows/Documents/TableCellCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableColumn` | _TBD_ | `System/Windows/Documents/TableColumn.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableColumnCollection` | _TBD_ | `System/Windows/Documents/TableColumnCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableRow` | _TBD_ | `System/Windows/Documents/TableRow.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableRowCollection` | _TBD_ | `System/Windows/Documents/TableRowCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableRowGroup` | _TBD_ | `System/Windows/Documents/TableRowGroup.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TableRowGroupCollection` | _TBD_ | `System/Windows/Documents/TableRowGroupCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextChangeType` | _TBD_ | `System/Windows/Documents/TextChangeType.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextContainer` | _TBD_ | `System/Windows/Documents/TextContainer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextContainerChangedEventArgs` | _TBD_ | `System/Windows/Documents/TextContainerChangedEventArgs.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextContainerChangedEventHandler` | _TBD_ | `System/Windows/Documents/TextContainerChangedEventHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextContainerChangeEventArgs` | _TBD_ | `System/Windows/Documents/TextContainerChangeEventArgs.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextContainerChangeEventHandler` | _TBD_ | `System/Windows/Documents/TextContainerChangeEventHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditor` | _TBD_ | `System/Windows/Documents/TextEditor.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorCharacters` | _TBD_ | `System/Windows/Documents/TextEditorCharacters.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorContextMenu` | _TBD_ | `System/Windows/Documents/TextEditorContextMenu.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorCopyPaste` | _TBD_ | `System/Windows/Documents/TextEditorCopyPaste.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorDragDrop` | _TBD_ | `System/Windows/Documents/TextEditorDragDrop.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorLists` | _TBD_ | `System/Windows/Documents/TextEditorLists.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorMouse` | _TBD_ | `System/Windows/Documents/TextEditorMouse.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorParagraphs` | _TBD_ | `System/Windows/Documents/TextEditorParagraphs.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorSelection` | _TBD_ | `System/Windows/Documents/TextEditorSelection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorSpelling` | _TBD_ | `System/Windows/Documents/TextEditorSpelling.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorTables` | _TBD_ | `System/Windows/Documents/TextEditorTables.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorThreadLocalStore` | _TBD_ | `System/Windows/Documents/TextEditorThreadLocalStore.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEditorTyping` | _TBD_ | `System/Windows/Documents/TextEditorTyping.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextEffectResolver` | _TBD_ | `System/Windows/Documents/TextEffectResolver.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextElement` | _TBD_ | `System/Windows/Documents/TextElement.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextElementCollection` | _TBD_ | `System/Windows/Documents/TextElementCollection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextElementCollectionHelper` | _TBD_ | `System/Windows/Documents/TextElementCollectionHelper.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextElementEditingBehaviorAttribute` | _TBD_ | `System/Windows/Documents/TextElementEditingBehaviorAttribute.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextElementEnumerator` | _TBD_ | `System/Windows/Documents/TextElementEnumerator.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextFindEngine` | _TBD_ | `System/Windows/Documents/TextFindEngine.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextFlow` | _TBD_ | `System/Windows/Documents/TextFlow.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextMapOffsetErrorLogger` | _TBD_ | `System/Windows/Documents/TextMapOffsetErrorLogger.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextParentUndoUnit` | _TBD_ | `System/Windows/Documents/TextParentUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextPointer` | _TBD_ | `System/Windows/Documents/TextPointer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextPointerBase` | _TBD_ | `System/Windows/Documents/TextPointerBase.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextPointerContext` | _TBD_ | `System/Windows/Documents/TextPointerContext.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextRange` | _TBD_ | `System/Windows/Documents/TextRange.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextRangeBase` | _TBD_ | `System/Windows/Documents/TextRangeBase.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextRangeEdit` | _TBD_ | `System/Windows/Documents/TextRangeEdit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextRangeEditLists` | _TBD_ | `System/Windows/Documents/TextRangeEditLists.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextRangeEditTables` | _TBD_ | `System/Windows/Documents/TextRangeEditTables.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextRangeSerialization` | _TBD_ | `System/Windows/Documents/TextRangeSerialization.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextSchema` | _TBD_ | `System/Windows/Documents/TextSchema.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextSegment` | _TBD_ | `System/Windows/Documents/TextSegment.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextSelection` | _TBD_ | `System/Windows/Documents/TextSelection.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextSelectionHighlightLayer` | _TBD_ | `System/Windows/Documents/TextSelectionHighlightLayer.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextServicesDisplayAttribute` | _TBD_ | `System/Windows/Documents/TextServicesDisplayAttribute.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextServicesDisplayAttributePropertyRanges` | _TBD_ | `System/Windows/Documents/TextServicesDisplayAttributePropertyRanges.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextServicesHost` | _TBD_ | `System/Windows/Documents/TextServicesHost.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextServicesProperty` | _TBD_ | `System/Windows/Documents/TextServicesProperty.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextServicesPropertyRanges` | _TBD_ | `System/Windows/Documents/TextServicesPropertyRanges.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextStore` | _TBD_ | `System/Windows/Documents/TextStore.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeDeleteContentUndoUnit` | _TBD_ | `System/Windows/Documents/TextTreeDeleteContentUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeDumper` | _TBD_ | `System/Windows/Documents/TextTreeDumper.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeExtractElementUndoUnit` | _TBD_ | `System/Windows/Documents/TextTreeExtractElementUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeFixupNode` | _TBD_ | `System/Windows/Documents/TextTreeFixupNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeInsertElementUndoUnit` | _TBD_ | `System/Windows/Documents/TextTreeInsertElementUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeInsertUndoUnit` | _TBD_ | `System/Windows/Documents/TextTreeInsertUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeNode` | _TBD_ | `System/Windows/Documents/TextTreeNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeObjectNode` | _TBD_ | `System/Windows/Documents/TextTreeObjectNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreePropertyUndoUnit` | _TBD_ | `System/Windows/Documents/TextTreePropertyUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeRootNode` | _TBD_ | `System/Windows/Documents/TextTreeRootNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeRootTextBlock` | _TBD_ | `System/Windows/Documents/TextTreeRootTextBlock.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeText` | _TBD_ | `System/Windows/Documents/TextTreeText.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeTextBlock` | _TBD_ | `System/Windows/Documents/TextTreeTextBlock.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeTextElementNode` | _TBD_ | `System/Windows/Documents/TextTreeTextElementNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeTextNode` | _TBD_ | `System/Windows/Documents/TextTreeTextNode.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeUndo` | _TBD_ | `System/Windows/Documents/TextTreeUndo.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `TextTreeUndoUnit` | _TBD_ | `System/Windows/Documents/TextTreeUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Typography` | _TBD_ | `System/Windows/Documents/Typography.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `UIElementPropertyUndoUnit` | _TBD_ | `System/Windows/Documents/UIElementPropertyUndoUnit.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `Underline` | _TBD_ | `System/Windows/Documents/Underline.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ValidationHelper` | _TBD_ | `System/Windows/Documents/ValidationHelper.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `WinEventHandler` | _TBD_ | `System/Windows/Documents/WinEventHandler.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `WinRTSpellerInterop` | _TBD_ | `System/Windows/Documents/WinRTSpellerInterop.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `WinRTSpellerInteropExtensions` | _TBD_ | `System/Windows/Documents/WinRTSpellerInteropExtensions.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `WpfPayload` | _TBD_ | `System/Windows/Documents/WpfPayload.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XamlAttribute` | _TBD_ | `System/Windows/Documents/XamlAttribute.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XamlRtfConverter` | _TBD_ | `System/Windows/Documents/XamlRtfConverter.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XamlTokenType` | _TBD_ | `System/Windows/Documents/XamlTokenType.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XamlToRtfError` | _TBD_ | `System/Windows/Documents/XamlToRtfError.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XamlToRtfParser` | _TBD_ | `System/Windows/Documents/XamlToRtfParser.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XamlToRtfWriter` | _TBD_ | `System/Windows/Documents/XamlToRtfWriter.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `XPSS0ValidatingLoader` | _TBD_ | `System/Windows/Documents/XPSS0ValidatingLoader.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |
| `ZoomPercentageConverter` | _TBD_ | `System/Windows/Documents/ZoomPercentageConverter.cs` | `59b315842480f5d36134bb6348f7e537c08bdc0b` | Planned | Generated placeholder entry. |

# UnoRichText Source Map (WPF Reuse Plan)

This file tracks mapping between UnoRichText document-model sources and upstream WPF sources, plus migration status.

For the full namespace inventory, see [NAMESPACE-INVENTORY.md](C:/Users/lextudio/source/repos/uno-tools/UnoRichText/docs/NAMESPACE-INVENTORY.md).

## Legend

- `Direct-share`: import/keep Microsoft logic mostly unchanged in shared `.cs`.
- `Share-with-partials`: shared `.cs` + platform partials (`.wpf.cs` / `.uno.cs`).
- `Uno-only`: stays local implementation (no practical upstream reuse).
- `Status`: `Not started`, `In progress`, `Migrated`.

## Upstream Base

- Repo: `wpf`
- Root: `src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Documents`
- Local checkout: `C:\Users\lextudio\source\repos\uno-tools\wpf`

## Document Types

| UnoRichText Type | Local File | Expected Upstream Source | Strategy | Status | Notes |
|---|---|---|---|---|---|
| `Inline` | `src/LeXtudio.RichText/Documents/Inline.cs` + `Inline.uno.cs` | `Inline.cs` | Share-with-partials | In progress | Split landed; shared core is now partial with Uno properties in adaptation file. |
| `Run` | `src/LeXtudio.RichText/Documents/Run.cs` + `Run.uno.cs` | `Run.cs` | Share-with-partials | Migrated | Uno text property moved to adaptation partial; shared core in place. |
| `Span` | `src/LeXtudio.RichText/Documents/Span.cs` + `Span.uno.cs` | `Span.cs` | Share-with-partials | In progress | Pilot split started: shared core + Uno partial landed. |
| `Bold` | `src/LeXtudio.RichText/Documents/Bold.cs` + `Bold.uno.cs` | `Bold.cs` | Share-with-partials | Migrated | Constructor behavior moved to Uno partial; shared core in place. |
| `Italic` | `src/LeXtudio.RichText/Documents/Italic.cs` + `Italic.uno.cs` | `Italic.cs` | Share-with-partials | Migrated | Constructor behavior moved to Uno partial; shared core in place. |
| `LineBreak` | `src/LeXtudio.RichText/Documents/LineBreak.cs` + `LineBreak.uno.cs` | `LineBreak.cs` | Share-with-partials | Migrated | Split completed; trivial shared core with Uno partial companion. |
| `Hyperlink` | `src/LeXtudio.RichText/Documents/Hyperlink.cs` + `Hyperlink.uno.cs` | `Hyperlink.cs` | Share-with-partials | Migrated | Event/click behavior moved to Uno partial; shared core in place. |
| `InlineCollection` | `src/LeXtudio.RichText/Documents/InlineCollection.cs` + `InlineCollection.uno.cs` | `InlineCollection.cs` | Share-with-partials | Migrated | Collection base moved to Uno partial; shared core in place. |
| `InlineUIContainer` | `src/LeXtudio.RichText/Documents/InlineUIContainer.cs` + `InlineUIContainer.uno.cs` | `InlineUIContainer.cs` | Share-with-partials | Migrated | Uno `UIElement` child binding moved to adaptation partial. |

## Block Types

| UnoRichText Type | Local File | Expected Upstream Source | Strategy | Status | Notes |
|---|---|---|---|---|---|
| `Block` | `src/LeXtudio.RichText/Documents/Block.cs` + `Block.uno.cs` | `Block.cs` | Share-with-partials | Migrated | Common shell + Uno property set split completed. |
| `Paragraph` | `src/LeXtudio.RichText/Documents/Paragraph.cs` + `Paragraph.uno.cs` | `Paragraph.cs` | Share-with-partials | Migrated | `Inlines` ownership moved to Uno partial. |
| `BlockCollection` | `src/LeXtudio.RichText/Documents/BlockCollection.cs` + `BlockCollection.uno.cs` | `BlockCollection.cs` | Share-with-partials | Migrated | Collection base moved to Uno partial; shared core in place. |

## Controls

| UnoRichText Type | Local File | Expected Upstream Source | Strategy | Status | Notes |
|---|---|---|---|---|---|
| `RichTextBlock` | `src/LeXtudio.RichText/Controls/RichTextBlock.cs` | `RichTextBlock` family | Uno-only | Not started | Keep Uno/Pretext rendering and selection model local. Reuse document model only. |

## Next Slice (Recommended)

1. Expand this map to all files in `NAMESPACE-INVENTORY.md` with initial Tier A/B/C classification.
2. Use `tools/verify-documents-parity.ps1` as CI/local guard for source-map/provenance/project links.
3. Add focused tests for migrated split behavior across `Inline`/`Run`/`Span`/`Hyperlink`.

## Backlog (Auto-generated)
| `Adorner` | _TBD_ | `Adorner.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `AdornerDecorator` | _TBD_ | `AdornerDecorator.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `AdornerHitTestResult` | _TBD_ | `AdornerHitTestResult.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `AdornerLayer` | _TBD_ | `AdornerLayer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `AnchoredBlock` | _TBD_ | `AnchoredBlock.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `BlockUIContainer` | _TBD_ | `BlockUIContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `CaretElement` | _TBD_ | `CaretElement.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `CaretScrollMethod` | _TBD_ | `CaretScrollMethod.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ChangeBlockUndoRecord` | _TBD_ | `ChangeBlockUndoRecord.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ChildDocumentBlock` | _TBD_ | `ChildDocumentBlock.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ColumnResizeAdorner` | _TBD_ | `ColumnResizeAdorner.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ColumnResizeUndoUnit` | _TBD_ | `ColumnResizeUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `CompositionAdorner` | _TBD_ | `CompositionAdorner.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DisplayAttributeHighlightLayer` | _TBD_ | `DisplayAttributeHighlightLayer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentReference` | _TBD_ | `DocumentReference.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentReferenceCollection` | _TBD_ | `DocumentReferenceCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentSequence` | _TBD_ | `DocumentSequence.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentSequenceHighlightLayer` | _TBD_ | `DocumentSequenceHighlightLayer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentSequenceTextContainer` | _TBD_ | `DocumentSequenceTextContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentSequenceTextPointer` | _TBD_ | `DocumentSequenceTextPointer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DocumentSequenceTextView` | _TBD_ | `DocumentSequenceTextView.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `DPTypeDescriptorContext` | _TBD_ | `DPTypeDescriptorContext.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `EditingCommands` | _TBD_ | `EditingCommands.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ElementEdge` | _TBD_ | `ElementEdge.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Figure` | _TBD_ | `Figure.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedDocument` | _TBD_ | `FixedDocument.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedDSBuilder` | _TBD_ | `FixedDSBuilder.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedElement` | _TBD_ | `FixedElement.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedFindEngine` | _TBD_ | `FixedFindEngine.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedFlowMap` | _TBD_ | `FixedFlowMap.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedHighlight` | _TBD_ | `FixedHighlight.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedHyperlink` | _TBD_ | `FixedHyperlink.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedLineResult` | _TBD_ | `FixedLineResult.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedNode` | _TBD_ | `FixedNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedPage` | _TBD_ | `FixedPage.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedPageStructure` | _TBD_ | `FixedPageStructure.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedPosition` | _TBD_ | `FixedPosition.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSchema` | _TBD_ | `FixedSchema.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMContainer` | _TBD_ | `FixedSOMContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMElement` | _TBD_ | `FixedSOMElement.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMFixedBlock` | _TBD_ | `FixedSOMFixedBlock.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMGroup` | _TBD_ | `FixedSOMGroup.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMImage` | _TBD_ | `FixedSOMImage.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMLineCollection` | _TBD_ | `FixedSOMLineCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMLineRanges` | _TBD_ | `FixedSOMLineRanges.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMPage` | _TBD_ | `FixedSOMPage.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMPageConstructor` | _TBD_ | `FixedSOMPageConstructor.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMPageElement` | _TBD_ | `FixedSOMPageElement.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMSemanticBox` | _TBD_ | `FixedSOMSemanticBox.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMTable` | _TBD_ | `FixedSOMTable.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMTableCell` | _TBD_ | `FixedSOMTableCell.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMTableRow` | _TBD_ | `FixedSOMTableRow.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedSOMTextRun` | _TBD_ | `FixedSOMTextRun.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedTextBuilder` | _TBD_ | `FixedTextBuilder.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedTextContainer` | _TBD_ | `FixedTextContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedTextPointer` | _TBD_ | `FixedTextPointer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FixedTextView` | _TBD_ | `FixedTextView.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Floater` | _TBD_ | `Floater.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FlowDocument` | _TBD_ | `FlowDocument.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FlowNode` | _TBD_ | `FlowNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FlowPosition` | _TBD_ | `FlowPosition.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FrameworkRichTextComposition` | _TBD_ | `FrameworkRichTextComposition.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `FrameworkTextComposition` | _TBD_ | `FrameworkTextComposition.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Glyphs` | _TBD_ | `Glyphs.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `HighlightChangedEventArgs` | _TBD_ | `HighlightChangedEventArgs.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `HighlightChangedEventHandler` | _TBD_ | `HighlightChangedEventHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `HighlightLayer` | _TBD_ | `HighlightLayer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Highlights` | _TBD_ | `Highlights.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `HighlightVisual` | _TBD_ | `HighlightVisual.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ImmComposition` | _TBD_ | `ImmComposition.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `InkInteropObject` | _TBD_ | `InkInteropObject.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `InputScopeAttribute` | _TBD_ | `InputScopeAttribute.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ITextContainer` | _TBD_ | `ITextContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ITextPointer` | _TBD_ | `ITextPointer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ITextRange` | _TBD_ | `ITextRange.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ITextSelection` | _TBD_ | `ITextSelection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ITextView` | _TBD_ | `ITextView.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `IXamlAttributes` | _TBD_ | `IXamlAttributes.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `IXamlContentHandler` | _TBD_ | `IXamlContentHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `IXamlErrorHandler` | _TBD_ | `IXamlErrorHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `LinkTarget` | _TBD_ | `LinkTarget.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `List` | _TBD_ | `List.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ListItem` | _TBD_ | `ListItem.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ListItemCollection` | _TBD_ | `ListItemCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `LogicalDirection` | _TBD_ | `LogicalDirection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `MoveSizeWinEventHandler` | _TBD_ | `MoveSizeWinEventHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `NaturalLanguageHyphenator` | _TBD_ | `NaturalLanguageHyphenator.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `NLGSpellerInterop` | _TBD_ | `NLGSpellerInterop.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `NullTextContainer` | _TBD_ | `NullTextContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `NullTextNavigator` | _TBD_ | `NullTextNavigator.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `PageContent` | _TBD_ | `PageContent.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `PageContentAsyncResult` | _TBD_ | `PageContentAsyncResult.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `PageContentCollection` | _TBD_ | `PageContentCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `PrecursorTextChangeType` | _TBD_ | `PrecursorTextChangeType.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `PropertyRecord` | _TBD_ | `PropertyRecord.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `PropertyValueAction` | _TBD_ | `PropertyValueAction.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RangeContentEnumerator` | _TBD_ | `RangeContentEnumerator.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfControls` | _TBD_ | `RtfControls.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfControlWord` | _TBD_ | `RtfControlWord.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfControlWordInfo` | _TBD_ | `RtfControlWordInfo.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfDestination` | _TBD_ | `RtfDestination.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfFormatStack` | _TBD_ | `RtfFormatStack.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfImageFormat` | _TBD_ | `RtfImageFormat.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfSuperSubscript` | _TBD_ | `RtfSuperSubscript.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfToken` | _TBD_ | `RtfToken.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfTokenType` | _TBD_ | `RtfTokenType.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfToXamlError` | _TBD_ | `RtfToXamlError.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfToXamlLexer` | _TBD_ | `RtfToXamlLexer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RtfToXamlReader` | _TBD_ | `RtfToXamlReader.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `RubberbandSelector` | _TBD_ | `RubberbandSelector.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Section` | _TBD_ | `Section.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SelectionHighlightInfo` | _TBD_ | `SelectionHighlightInfo.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SelectionWordBreaker` | _TBD_ | `SelectionWordBreaker.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Speller` | _TBD_ | `Speller.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SpellerError` | _TBD_ | `SpellerError.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SpellerHighlightLayer` | _TBD_ | `SpellerHighlightLayer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SpellerInteropBase` | _TBD_ | `SpellerInteropBase.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SpellerStatusTable` | _TBD_ | `SpellerStatusTable.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SplayTreeNode` | _TBD_ | `SplayTreeNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `SplayTreeNodeRole` | _TBD_ | `SplayTreeNodeRole.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `StaticTextPointer` | _TBD_ | `StaticTextPointer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Table` | _TBD_ | `Table.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableCell` | _TBD_ | `TableCell.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableCellCollection` | _TBD_ | `TableCellCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableColumn` | _TBD_ | `TableColumn.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableColumnCollection` | _TBD_ | `TableColumnCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableRow` | _TBD_ | `TableRow.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableRowCollection` | _TBD_ | `TableRowCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableRowGroup` | _TBD_ | `TableRowGroup.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TableRowGroupCollection` | _TBD_ | `TableRowGroupCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextChangeType` | _TBD_ | `TextChangeType.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextContainer` | _TBD_ | `TextContainer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextContainerChangedEventArgs` | _TBD_ | `TextContainerChangedEventArgs.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextContainerChangedEventHandler` | _TBD_ | `TextContainerChangedEventHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextContainerChangeEventArgs` | _TBD_ | `TextContainerChangeEventArgs.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextContainerChangeEventHandler` | _TBD_ | `TextContainerChangeEventHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditor` | _TBD_ | `TextEditor.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorCharacters` | _TBD_ | `TextEditorCharacters.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorContextMenu` | _TBD_ | `TextEditorContextMenu.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorCopyPaste` | _TBD_ | `TextEditorCopyPaste.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorDragDrop` | _TBD_ | `TextEditorDragDrop.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorLists` | _TBD_ | `TextEditorLists.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorMouse` | _TBD_ | `TextEditorMouse.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorParagraphs` | _TBD_ | `TextEditorParagraphs.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorSelection` | _TBD_ | `TextEditorSelection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorSpelling` | _TBD_ | `TextEditorSpelling.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorTables` | _TBD_ | `TextEditorTables.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorThreadLocalStore` | _TBD_ | `TextEditorThreadLocalStore.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEditorTyping` | _TBD_ | `TextEditorTyping.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextEffectResolver` | _TBD_ | `TextEffectResolver.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextElement` | _TBD_ | `TextElement.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextElementCollection` | _TBD_ | `TextElementCollection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextElementCollectionHelper` | _TBD_ | `TextElementCollectionHelper.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextElementEditingBehaviorAttribute` | _TBD_ | `TextElementEditingBehaviorAttribute.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextElementEnumerator` | _TBD_ | `TextElementEnumerator.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextFindEngine` | _TBD_ | `TextFindEngine.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextFlow` | _TBD_ | `TextFlow.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextMapOffsetErrorLogger` | _TBD_ | `TextMapOffsetErrorLogger.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextParentUndoUnit` | _TBD_ | `TextParentUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextPointer` | _TBD_ | `TextPointer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextPointerBase` | _TBD_ | `TextPointerBase.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextPointerContext` | _TBD_ | `TextPointerContext.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextRange` | _TBD_ | `TextRange.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextRangeBase` | _TBD_ | `TextRangeBase.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextRangeEdit` | _TBD_ | `TextRangeEdit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextRangeEditLists` | _TBD_ | `TextRangeEditLists.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextRangeEditTables` | _TBD_ | `TextRangeEditTables.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextRangeSerialization` | _TBD_ | `TextRangeSerialization.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextSchema` | _TBD_ | `TextSchema.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextSegment` | _TBD_ | `TextSegment.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextSelection` | _TBD_ | `TextSelection.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextSelectionHighlightLayer` | _TBD_ | `TextSelectionHighlightLayer.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextServicesDisplayAttribute` | _TBD_ | `TextServicesDisplayAttribute.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextServicesDisplayAttributePropertyRanges` | _TBD_ | `TextServicesDisplayAttributePropertyRanges.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextServicesHost` | _TBD_ | `TextServicesHost.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextServicesProperty` | _TBD_ | `TextServicesProperty.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextServicesPropertyRanges` | _TBD_ | `TextServicesPropertyRanges.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextStore` | _TBD_ | `TextStore.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeDeleteContentUndoUnit` | _TBD_ | `TextTreeDeleteContentUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeDumper` | _TBD_ | `TextTreeDumper.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeExtractElementUndoUnit` | _TBD_ | `TextTreeExtractElementUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeFixupNode` | _TBD_ | `TextTreeFixupNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeInsertElementUndoUnit` | _TBD_ | `TextTreeInsertElementUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeInsertUndoUnit` | _TBD_ | `TextTreeInsertUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeNode` | _TBD_ | `TextTreeNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeObjectNode` | _TBD_ | `TextTreeObjectNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreePropertyUndoUnit` | _TBD_ | `TextTreePropertyUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeRootNode` | _TBD_ | `TextTreeRootNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeRootTextBlock` | _TBD_ | `TextTreeRootTextBlock.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeText` | _TBD_ | `TextTreeText.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeTextBlock` | _TBD_ | `TextTreeTextBlock.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeTextElementNode` | _TBD_ | `TextTreeTextElementNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeTextNode` | _TBD_ | `TextTreeTextNode.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeUndo` | _TBD_ | `TextTreeUndo.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `TextTreeUndoUnit` | _TBD_ | `TextTreeUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Typography` | _TBD_ | `Typography.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `UIElementPropertyUndoUnit` | _TBD_ | `UIElementPropertyUndoUnit.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `Underline` | _TBD_ | `Underline.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ValidationHelper` | _TBD_ | `ValidationHelper.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `WinEventHandler` | _TBD_ | `WinEventHandler.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `WinRTSpellerInterop` | _TBD_ | `WinRTSpellerInterop.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `WinRTSpellerInteropExtensions` | _TBD_ | `WinRTSpellerInteropExtensions.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `WpfPayload` | _TBD_ | `WpfPayload.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XamlAttribute` | _TBD_ | `XamlAttribute.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XamlRtfConverter` | _TBD_ | `XamlRtfConverter.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XamlTokenType` | _TBD_ | `XamlTokenType.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XamlToRtfError` | _TBD_ | `XamlToRtfError.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XamlToRtfParser` | _TBD_ | `XamlToRtfParser.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XamlToRtfWriter` | _TBD_ | `XamlToRtfWriter.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `XPSS0ValidatingLoader` | _TBD_ | `XPSS0ValidatingLoader.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |
| `ZoomPercentageConverter` | _TBD_ | `ZoomPercentageConverter.cs` | Share-with-partials | Not started | Generated placeholder; classify/refine during migration. |

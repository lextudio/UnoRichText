using NUnit.Framework;
using System.Linq;
using MS.Internal.Florence;
using FlowDocument = System.Windows.Documents.FlowDocument;
using LogicalDirection = System.Windows.Documents.LogicalDirection;
using Paragraph = System.Windows.Documents.Paragraph;
using Run = System.Windows.Documents.Run;
using TextPointer = System.Windows.Documents.TextPointer;
using TextRange = System.Windows.Documents.TextRange;

namespace LeXtudio.RichText.Tests.Documents;

[TestFixture]
public sealed class FlowDocumentTests
{
    [Test]
    public void FlowDocument_OwnsBlockCollection()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public void FlowDocument_CanAttachRichTextBlockLayoutHost()
    {
        SampleDiagnostics.AssertPassMarker("DOCUMENTS: PASS");
    }

    [Test]
    public async Task TextContainer_IMECharCountIncludesRunText()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("abcdef")));

            Assert.That(document.TextContainer.IMECharCount, Is.GreaterThanOrEqualTo(6));
            Assert.That(new TextRange(document.ContentStart, document.ContentEnd).Text, Does.Contain("abcdef"));
        });
    }

    [Test]
    public async Task TextContainer_CanCreatePointersAtVisibleCharOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run("abcdef")));

            TextPointer? pointer = document.TextContainer.CreatePointerAtCharOffset(4, LogicalDirection.Forward);

            Assert.That(pointer, Is.Not.Null);
            Assert.That(pointer!.CharOffset, Is.EqualTo(4));
        });
    }

    [Test]
    public async Task TextContainer_IMECharCountIncludesObjectInitializerRunText()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = "abcdef" });
            document.Blocks.Add(paragraph);

            Assert.That(document.TextContainer.IMECharCount, Is.GreaterThanOrEqualTo(6));
            Assert.That(new TextRange(document.ContentStart, document.ContentEnd).Text, Does.Contain("abcdef"));
        });
    }

    [Test]
    public async Task FlorenceLayout_UsesDocumentWideRunOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run { Text = "First paragraph with enough text to wrap at a narrow width." }));
            document.Blocks.Add(new Paragraph(new Run { Text = "Second paragraph should not restart run offsets at zero." }));

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(120, double.PositiveInfinity));

            foreach (var line in page.Lines)
            foreach (var run in line.Runs)
                Assert.That(run.StartOffset, Is.GreaterThanOrEqualTo(line.StartOffset));
        });
    }

    [Test]
    public async Task FlorenceLayout_DoesNotDoubleCountParagraphInlineOffsets()
    {
        await Runtime.UnoRuntimeTestHost.RunOnUIThreadAsync(() =>
        {
            var document = new FlowDocument();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run { Text = "This sample hosts a " });
            paragraph.Inlines.Add(new Run { Text = "FlowDocument" });
            paragraph.Inlines.Add(new Run { Text = " inside the ported " });
            paragraph.Inlines.Add(new Run { Text = "System.Windows.Controls.RichTextBox" });
            paragraph.Inlines.Add(new Run { Text = "." });
            document.Blocks.Add(paragraph);

            var page = FlorenceLayoutEngine.Format(document, new global::Windows.Foundation.Size(600, double.PositiveInfinity));
            var plainText = new TextRange(document.ContentStart, document.ContentEnd).Text;
            int visibleTextLength = paragraph.Inlines
                .OfType<Run>()
                .Sum(run => new TextRange(run.ContentStart, run.ContentEnd).Text.Length);

            Assert.That(page.Lines, Is.Not.Empty);
            Assert.That(page.Lines[^1].EndOffset, Is.LessThanOrEqualTo(document.TextContainer.IMECharCount));
            Assert.That(page.Lines[^1].EndOffset, Is.EqualTo(visibleTextLength));
            Assert.That(plainText.Length, Is.GreaterThanOrEqualTo(visibleTextLength));

            foreach (var line in page.Lines)
            foreach (var run in line.Runs)
            {
                Assert.That(run.StartOffset, Is.GreaterThanOrEqualTo(0));
                Assert.That(run.EndOffset, Is.LessThanOrEqualTo(document.TextContainer.IMECharCount));
            }
        });
    }
}
